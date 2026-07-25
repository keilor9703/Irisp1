# Auditoría integral del sistema IRIS-P1

**Sistema:** IRIS-P1 — Plataforma de inteligencia policial (Policía Nacional de Colombia / JESEP-DISEC)
**Stack:** ASP.NET Core 8 MVC (arquitectura por Áreas) + Oracle PL/SQL (Dapper/Dapper.Oracle, sin ORM)
**Fecha de la auditoría:** 25/07/2026
**Alcance:** Repositorio `keilor9703/Irisp1` (rama `main`) — capas Web, Negocio, Comun, Servicios, Datos y scripts Oracle.

> **Nota de método:** este entorno no tiene compilador .NET ni conexión a Oracle. Los hallazgos de código se verificaron por lectura estática, búsqueda de patrones y análisis de la lógica; los de base de datos, sobre los scripts `.txt` de despliegue. Ninguna conclusión proviene de ejecución en vivo. Donde escribo "confirmado" me refiero a evidencia textual directa en el repositorio, con archivo y línea.

---

## 1. Propósito y objetivo del sistema (entendimiento)

IRIS-P1 es el sistema que soporta el ciclo de **registro, verificación, seguimiento y análisis de casos de criminalidad ("IRISP1")** para unidades de la Policía Nacional. A partir de la lectura del código y del PL/SQL, el flujo de negocio es:

1. **Registro (`Irisp1/RegistrosIrisp1`)** — se captura un caso de criminalidad (`IRISP_CRIMINALIDAD`), con integrantes, ubicaciones georreferenciadas (lat/lon), delitos, información asociada, documentos y fotos. Un caso pertenece a una **unidad** (`SIGLA_UNIDAD`) y a una **región** de policía.
2. **Verificación (`Irisp1/Verificacion`)** — una unidad responsable valida el caso; se generan **tareas** (`IRISP_TAREA`) con estados y evidencias.
3. **Seguimiento (`Irisp1/Seguimiento`)** — se hace trazabilidad de las tareas hasta su finalización (`ID_ESTADO = 5`) y se determina la **existencia** (SI/NO EXISTE) del hecho investigado.
4. **Integrantes / Expendios** — módulos de apoyo (personas vinculadas, expendios de droga).
5. **Reportes y Control de Gestión** — la capa analítica: reporte de verificación, tablero de tiempos/SLA, mapa de geolocalización y KPIs de efectividad por unidad (módulo desarrollado en esta sesión).

**Objetivo real del sistema:** convertir la operación de inteligencia (casos dispersos) en **información accionable y trazable** — quién hizo qué, cuándo, con qué resultado — respetando un modelo de **roles** (rol 1/2 = administración/visión total; rol 8 = unidad; rol 7 = reportes) y con **auditoría** de cada acción. La seguridad y la trazabilidad no son accesorias: son parte del propósito, porque es un sistema de datos sensibles de seguridad del Estado.

Esta lectura del propósito ordena las prioridades de la auditoría: **primero la confidencialidad/integridad de los datos y la trazabilidad, luego la corrección funcional, luego el rendimiento y la innovación.**

---

## 2. Resumen ejecutivo

El sistema es funcional y ha madurado (la capa analítica nueva es sólida), pero arrastra **deuda de seguridad de nivel crítico** impropia de un sistema de datos policiales, y deuda estructural que encarece cada cambio. Los tres focos que exigen acción inmediata:

| # | Severidad | Hallazgo | Impacto |
|---|-----------|----------|---------|
| C-1 | 🔴 Crítico | Credenciales de producción (Oracle, PIP, llaves de cifrado, secreto HMAC) **en texto plano y versionadas en git** | Compromiso total de la BD de inteligencia si el repo se filtra |
| C-2 | 🔴 Crítico | **Path traversal** en los endpoints de descarga de documentos | Lectura de archivos arbitrarios del servidor de archivos (UNC) |
| C-3 | 🔴 Crítico | Cifrado con **llave estática embebida** + **AES-ECB** usado como token de autorización | IDOR: acceso a casos ajenos manipulando el id "cifrado" |
| A-1 | 🟠 Alto | **Sin token anti-CSRF** en ~40 endpoints POST que modifican estado | Falsificación de peticiones (crear/editar/borrar casos) |
| A-2 | 🟠 Alto | **MFA deshabilitado** por configuración y `DominiosController` sin `[Authorize]` | Superficie de acceso ampliada |
| A-3 | 🟠 Alto | **HTML de presentación generado dentro del PL/SQL** e inyectado en el DOM | Riesgo de XSS almacenado + acoplamiento BD/UI |

El detalle y la remediación de cada uno están abajo, con severidad decreciente.

---

## 3. Hallazgos de seguridad

### 🔴 C-1 — Secretos de producción en texto plano y versionados en git
**Archivo:** `Web/appsettings.json` (líneas 2–7, 18, 25, 32–35, 56) · confirmado también en el historial (`git log -- Web/appsettings.json`).

`appsettings.json` está **rastreado por git** (`git ls-files` lo lista) y contiene, en claro:
- Cadena de conexión Oracle de **producción** DISEC: `User ID=USR_DISEC ; Password=d1s3c2018$$` y la de TELEPOL con `Password=PrivateGRUDE2021`.
- Credenciales del servicio PIP: `USRSW.JESEP-IRISP1` / `$BNS1mU%+%vd#XJi+@b!`.
- Llave de cifrado (`Encryption:Key`), secreto HMAC de MFA (`SECRET_HMAC_IRIS_P1`) y rutas internas de servidores de archivos.

Cualquiera con acceso al repositorio (o a un clon, un fork, un backup) obtiene acceso directo a la base de datos de inteligencia. `.gitignore` **no** excluye `appsettings.json`.

**Remediación (orden estricto):**
1. **Rotar YA** todas las contraseñas y llaves expuestas — deben considerarse comprometidas por el solo hecho de haber estado en el repo.
2. Mover los secretos fuera del código: *User Secrets* en desarrollo, **variables de entorno** o Azure Key Vault / un secret store en producción. .NET los sobreescribe sobre `appsettings.json` sin cambios de código.
3. Añadir `appsettings.json` y `appsettings.*.json` a `.gitignore`, dejar solo un `appsettings.example.json` con placeholders, y `git rm --cached` el actual.
4. Idealmente, **purgar el historial** (git filter-repo / BFG), aunque la rotación de credenciales del paso 1 es lo que de verdad cierra el riesgo.

---

### 🔴 C-2 — Path traversal en descarga de documentos
**Archivos:** `Web/Areas/Irisp1/Controllers/RegistrosIrisp1Controller.cs:363` (`DescargarArchivo`), `:911`; `Web/Areas/Irisp1/Controllers/VerificacionController.cs:213` (`descargar`), `:253` (`descargarTarea`).

El parámetro `ruta` viene del cliente y se combina directamente con la base UNC sin validación:

```csharp
string rutaCompleta = Path.Combine(uncBase, ruta.Replace("/", "\\"));
...
var bytes = System.IO.File.ReadAllBytes(rutaCompleta);
```

`Path.Combine(@"\\SRV\base", @"..\..\otra\cosa.pdf")` **escapa** de la carpeta base. Un usuario autenticado puede pedir `?ruta=..\..\..\Windows\win.ini` (o cualquier recurso legible por la cuenta de servicio) y descargarlo. Además, la URL de descarga se arma **dentro del PL/SQL** (`PK_VERIFICACION_IRIS.txt:404`) con la ruta cruda de BD, así que la superficie es amplia.

**Remediación:** canonicalizar y confinar la ruta antes de leer:
```csharp
var full = Path.GetFullPath(Path.Combine(uncBase, ruta.Replace("/", "\\")));
var baseFull = Path.GetFullPath(uncBase);
if (!full.StartsWith(baseFull, StringComparison.OrdinalIgnoreCase))
    return Forbid();
```
Mejor aún: no aceptar rutas del cliente. Guardar un **id de documento** y que el servidor resuelva la ruta desde BD, verificando además que el documento pertenece a un caso que el usuario tiene permitido ver (hoy no se valida propiedad).

---

### 🔴 C-3 — Cifrado con llave estática usado como control de acceso (IDOR)
**Archivo:** `Web/Models/ClsEncriptar.cs` (llave `"MAKV2SPBNI99212"` y salt embebidos, líneas 9–11, 34–36).

El `idCriminalidad` se pasa a las vistas "cifrado" con `ClsEncriptar.Encriptar` y se descifra en el servidor para operar sobre el caso. Problemas:
- La **llave y el salt están embebidos en el código** (y por tanto en git). El "cifrado" no aporta confidencialidad real.
- Se usa como **token de autorización implícito**: si conozco/genero el valor cifrado de otro caso, opero sobre él. No hay verificación de que el caso pertenezca a mi unidad. Es un patrón clásico de **IDOR**.
- `Desencriptar` ante un fallo **devuelve `"123"`** en vez de error (línea ~55), lo que puede degradar silenciosamente a operar sobre el caso id=123.

Adicional relacionado — **AES-ECB** en el login: `Negocio/Gestion/Admin/DbAdministracion.cs` (`Decript`, `CipherMode.ECB`). ECB no es semánticamente seguro (patrones del texto plano se filtran). El mensaje de login se descifra con ECB.

**Remediación:** dejar de usar cifrado reversible como identificador de acceso. Opciones: (a) usar el id real y **autorizar en cada operación** comprobando unidad/rol contra la BD; (b) si se quiere ocultar el id secuencial, usar identificadores opacos aleatorios (GUID) persistidos. Reemplazar ECB por **AES-GCM** (o al menos CBC con IV aleatorio por mensaje) y sacar las llaves a configuración segura. Eliminar el fallback a `"123"`.

---

### 🟠 A-1 — Ausencia de protección anti-CSRF en endpoints que modifican estado
**Evidencia (conteo POST vs `ValidateAntiForgeryToken`):**

| Controlador | POST | AntiForgery |
|---|---|---|
| RegistrosIrisp1 | 19 | 0 |
| Seguimiento | 6 | 0 |
| Verificacion | 4 | 0 |
| RegistrarInteg | 3 | 0 |
| ConfiguracionSla | 2 | 0 |
| Registros (Expendios) | 9 | 0 |
| **CuentaController** | 1 | **1** ✅ |

Solo el login está protegido. Todos los POST de negocio (crear/editar caso, guardar tarea, subir documento, CRUD de SLA) aceptan peticiones sin token anti-forgery. Con las cookies configuradas `SameSite=None` (`Program.cs:48,142`), el navegador **sí** envía la cookie en peticiones cross-site, lo que agrava el CSRF.

**Remediación:** aplicar `[AutoValidateAntiforgeryToken]` de forma global (un filtro en `Program.cs`) y emitir el token en las vistas/AJAX (header `RequestVerificationToken`). Revisar si `SameSite=None` es realmente necesario; si no hay federación cross-site que lo exija, `SameSite=Lax` reduce la superficie.

---

### 🟠 A-2 — MFA deshabilitado y endpoint sin autorización
- **MFA off:** `appsettings.json:28` → `"MfaCentral": { "Enabled": false }`. Toda la maquinaria MFA (bien construida en `CuentaController`) está inactiva. Para un sistema de inteligencia, MFA debería estar activo en producción.
- **`DominiosController` sin `[Authorize]`:** `Web/Controllers/DominiosController.cs:6` — la clase no tiene atributo de autorización (a diferencia de `EmpleadosController`, `HomeController`, etc.). Sus 4 endpoints (`F_GetDominios`, `F_GetDepartamentos`, `F_GetMunicipios`, `F_GetUnidadesPoliciales`) son **accesibles anónimamente**. Aunque son catálogos, exponen estructura organizacional (unidades policiales) sin autenticar.

**Remediación:** activar `Enabled: true` en producción (vía config segura); añadir `[Authorize]` a `DominiosController`.

---

### 🟠 A-3 — Lógica de presentación (HTML) generada dentro del PL/SQL
**Archivo:** `Oracle/PK_VERIFICACION_IRIS.txt:368–410+` (bloque `TO_CLOB('<ul ...>') || XMLAGG(...)`).

El PL/SQL construye HTML completo (—`<li>`, `<span style=...>`, enlaces `<a href="/Irisp1/Verificacion/descargar?ruta=...">`—) que luego el frontend inyecta en el DOM (los JS del proyecto usan `.html(...)`). Dos problemas:
1. **Riesgo de XSS almacenado:** si algún campo interpolado (observación, justificación, nombre de tarea) contiene `<script>` u otro payload, se ejecuta en el navegador del analista, porque se concatena sin escapar y se inserta como HTML.
2. **Acoplamiento BD↔UI:** los colores, estilos y rutas viven en la base de datos. Cambiar la UI obliga a recompilar paquetes Oracle; es la causa raíz de varios "estilos rotos" ya vistos en esta sesión.

**Remediación:** que las funciones devuelvan **datos** (estado, fechas, url, observación) y que el HTML se arme en el cliente con escape (textContent / plantillas que escapan). Es un refactor grande; mientras tanto, como mitigación mínima, escapar los campos de texto libre dentro del PL/SQL antes de concatenar.

---

### 🟡 Otros hallazgos de seguridad (medios)
- **Fuga de detalle de excepción al cliente:** múltiples endpoints devuelven `ex.Message` en la respuesta JSON/HTTP (`RegistrosIrisp1Controller.cs:405,669,1002,1025,...`; `SeguimientoController.cs:213`). Expone rutas, nombres de objetos Oracle y estructura interna. Devolver un mensaje genérico y loguear el detalle server-side (ya hay Serilog).
- **`[AllowAnonymous]` sobre un método privado** (`ProcesarYRegistrarDocumentoAsync`, `RegistrosIrisp1Controller.cs:451`): no tiene efecto (no es endpoint), pero es señal de confusión sobre el modelo de autorización; conviene quitarlo para no inducir a error.
- **Validación de subida de archivos incompleta:** se valida extensión y tamaño (bien), pero **no el content-type real ni el contenido** (magic bytes). Un `.pdf` puede ser cualquier cosa. Para archivos que se sirven luego, conviene validar el tipo real y almacenar con nombre no ejecutable.

---

## 4. Rendimiento

Buena parte de la deuda de rendimiento **ya se corrigió** en esta sesión (deduplicación de `VM_UNIDADES`, filtros no-sargables `EXTRACT(YEAR)`, `Task.WhenAll` en combos, acotar reportes por año). Lo que queda:

- **P-1 — `commandTimeout` de 120 s como norma.** Varios repositorios usan `commandTimeout: 120`. Un timeout de 2 minutos por consulta es un parche que oculta consultas lentas; en producción se traduce en peticiones colgadas. Medir las consultas realmente pesadas y optimizarlas (índices sobre `IRISP_CRIMINALIDAD.SIGLA_UNIDAD`, `FECHA_CREACION`, `VIGENTE`; sobre `IRISP_TAREA.FECHA_CREACION/FECHA_MODIFICA`), no subir el timeout.
- **P-2 — Agregación en memoria de datasets potencialmente grandes.** El tablero trae todas las filas y agrega en C# (`ArmarKpis*`). Es correcto para volúmenes moderados y evita segundos viajes a Oracle, pero si un rango de fechas amplio devuelve decenas de miles de casos, el payload JSON y el cómputo cliente crecen. Recomendación: para vistas puramente agregadas (conteos, promedios), ofrecer una función Oracle que devuelva **solo los agregados**, y reservar el dataset crudo para el drill-down bajo demanda.
- **P-3 — HTML voluminoso desde Oracle** (ver A-3): además del riesgo XSS, mover CLOBs de HTML por la red es más caro que mover datos estructurados. El refactor de A-3 mejora también el rendimiento.
- **P-4 — Falta de paginación server-side** en las grillas grandes (DataTables client-side). Con muchos casos, se transfiere todo el conjunto al navegador. Migrar a paginación/orden server-side las grillas de mayor volumen.

---

## 5. Calidad de código y mantenibilidad

- **Q-1 — `Web.csproj` vacío en la raíz** (0 bytes, `./Web.csproj`) conviviendo con el real `Web/Web.csproj`. Es ruido que confunde herramientas y personas; eliminarlo.
- **Q-2 — Sin pruebas ni CI.** No hay proyecto de tests ni workflow de GitHub Actions en el repo. Para un sistema de esta criticidad, al menos pruebas de los helpers puros (formateo de duración, `ArmarKpis*`, clasificación de existencia/estado) y un pipeline que compile en cada push. La lógica de KPIs se presta muy bien a tests unitarios deterministas.
- **Q-3 — Modelos de roles divergentes entre paquetes.** Ya documentado: `PK_CONSULTA_IRISP`, `PK_VERIFICACION_IRIS`, `PK_REPORTES_IRIS` implementan cada uno su propio filtro de roles con números distintos. Es una inconsistencia estructural que causa que "un caso aparezca en un módulo y no en otro". Converger a un único predicado de visibilidad por rol (una función/vista compartida).
- **Q-4 — Manejo de errores y comentarios inconsistentes.** Conviven `catch { }` silenciosos, `catch` que devuelven `ex.Message`, y bloques con emojis/comentarios de depuración (`❌ AQUÍ BORRAMOS...`, `✅ RESTAURAMOS...` en `Program.cs:61,69`). Unificar el patrón de manejo de errores y limpiar comentarios de bitácora.
- **Q-5 — Artefacto binario versionado:** `Ponal.Seguridad.MfaCliente.zip` (438 KB) está en el repo. Los binarios no deberían versionarse; publicar como paquete/artefacto.

---

## 6. Funcionalidades faltantes o incompletas

Ordenadas por valor para el propósito del sistema:

1. **Panel de administración de SLA sin visibilidad operativa cerrada.** El CRUD de SLA existe (`ConfiguracionSlaController`) pero no hay alertas proactivas: el sistema calcula "EN RIESGO"/"VENCIDO" pero **no notifica**. Falta un mecanismo de aviso (correo/tablero de alertas) al responsable cuando una tarea entra en riesgo o vence. Es el cierre natural del módulo de Control de Gestión que se está construyendo.
2. **Autorización a nivel de dato (row-level) incompleta.** Los endpoints de descarga/edición no verifican que el caso pertenezca a la unidad del usuario (ver C-2/C-3). Es funcionalidad de seguridad faltante, no solo un bug.
3. **Exportaciones parciales.** Existe export a PDF (verificación, y ahora tablero con marca de agua) y Excel (verificación), pero no en todos los módulos que lo ameritan (integrantes, seguimiento). Convendría un servicio de exportación transversal reutilizable en vez de repetir el patrón por controlador.
4. **Gestión de sesión y cierre por inactividad del lado servidor.** Hay expiración de cookie (30 min) pero el middleware de `Program.cs:169` solo redirige si falta el menú/IP en sesión; conviene un manejo explícito y mensajes claros de sesión expirada (hoy `SesionExpirada` redirige con `_mensaje=""`).
5. **Trazabilidad de cambios (quién modificó qué campo).** Hay auditoría de acciones (`P_InsAuditoria`) pero no un histórico de versiones de un caso. Para inteligencia, el *audit trail* a nivel de campo es muy valioso.

---

## 7. Innovación y oportunidades de alto valor

Apalancando lo ya construido (mapa de geolocalización, KPIs, drill-down):

- **I-1 — Alertas y "bandeja de gestión" proactiva.** Convertir el cálculo de SLA en una bandeja por usuario/unidad con lo que está por vencer, ordenado por urgencia, y notificación por correo. Es el mayor salto de "sistema que reporta" a "sistema que dirige la operación".
- **I-2 — Analítica geoespacial de patrones.** Ya hay lat/lon y modos punto/cluster/calor. Siguiente paso: **detección de zonas calientes** (densidad temporal), correlación de casos por proximidad geográfica y ventana temporal, para sugerir vínculos entre casos de la misma zona.
- **I-3 — Indicadores predictivos de carga.** Con el histórico de tiempos por tarea/unidad se puede estimar tiempo esperado de resolución de un caso nuevo y detectar unidades sobrecargadas antes de que incumplan.
- **I-4 — API de datos abierta interna** (como ya existe el concepto en el sistema hermano SISGE/OFTIC) para que otros sistemas de la Policía consuman indicadores agregados de IRIS-P1 de forma controlada.
- **I-5 — Búsqueda transversal** (un solo buscador sobre casos, integrantes, ubicaciones) con relevancia, en lugar de grillas por módulo.

---

## 8. Plan de remediación priorizado

| Prioridad | Acción | Esfuerzo | Riesgo si no se hace |
|-----------|--------|----------|----------------------|
| **P0 — inmediato** | C-1: rotar credenciales, sacar secretos de git, `.gitignore` | Bajo | Compromiso total de la BD |
| **P0 — inmediato** | C-2: confinar rutas de descarga (anti path-traversal) | Bajo | Lectura de archivos arbitrarios |
| **P0 — inmediato** | A-2: activar MFA en prod + `[Authorize]` en DominiosController | Bajo | Acceso no autenticado |
| **P1 — corto plazo** | A-1: anti-CSRF global + revisar SameSite | Medio | Falsificación de operaciones |
| **P1 — corto plazo** | C-3: autorización row-level + quitar cifrado como token + AES-GCM | Medio-Alto | IDOR sobre casos ajenos |
| **P1 — corto plazo** | Medios: no filtrar `ex.Message`, validar content-type de subidas | Bajo | Fuga de información |
| **P2 — medio plazo** | A-3/P-3: sacar el HTML del PL/SQL hacia el cliente | Alto | XSS + coste de mantenimiento |
| **P2 — medio plazo** | Q-2: proyecto de tests + CI que compile en cada push | Medio | Regresiones silenciosas |
| **P2 — medio plazo** | I-1: alertas proactivas de SLA (cierre del módulo de gestión) | Medio | Valor de negocio sin capturar |
| **P3 — continuo** | Q-1/Q-3/Q-4/Q-5, P-1/P-2/P-4, resto de innovación | Variable | Deuda acumulada |

---

## 9. Cierre

IRIS-P1 cumple su propósito operativo y la capa analítica reciente lo eleva de "captura de datos" hacia "dirección de la gestión". El obstáculo no es funcional sino de **seguridad y estructura**: para un sistema de datos de inteligencia, los tres hallazgos críticos (secretos versionados, path traversal, cifrado como control de acceso) deben cerrarse **antes** de cualquier nueva funcionalidad. Con esos tres resueltos y el anti-CSRF, el sistema queda en una base sólida para capitalizar las oportunidades de innovación (alertas de SLA, analítica geoespacial), que es donde está el mayor retorno.

*Auditoría generada como parte del ciclo de mejora continua del sistema IRIS-P1.*
