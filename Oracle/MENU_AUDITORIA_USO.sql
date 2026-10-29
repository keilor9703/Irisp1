--==================================================================================================
-- Alta de menú para el submódulo "Dashboard de Uso y Auditoría" dentro de ADMINISTRACIÓN.
-- Se deriva el nodo padre, el TIPO y los roles de la opción existente "Usuarios" (AREA=Admin,
-- CONTROLADOR=Usuarios) para quedar consistente con el resto del menú. Idempotente: no duplica.
-- Ejecutar en el esquema USR_DISEC. Ajustar la DESCRIPCION/ICONO si se desea.
--==================================================================================================
DECLARE
  v_idpadre   NUMBER;
  v_tipo      CTR_MENU_IRIS.TIPO%TYPE;
  v_posicion  NUMBER;
  v_id_menu   NUMBER;
  v_idmenurol NUMBER;
  v_existe    NUMBER;
BEGIN
  -- Padre (nodo ADMINISTRACIÓN) y TIPO, tomados de la opción "Usuarios".
  SELECT idpadre, tipo
    INTO v_idpadre, v_tipo
    FROM (SELECT idpadre, tipo
            FROM CTR_MENU_IRIS
           WHERE UPPER(controlador) = 'USUARIOS'
             AND UPPER(area) = 'ADMIN'
             AND vigente = 1
           ORDER BY id_menu)
   WHERE ROWNUM = 1;

  -- ¿Ya existe el submódulo? (clave única: DESCRIPCION + IDPADRE)
  SELECT COUNT(*) INTO v_existe
    FROM CTR_MENU_IRIS
   WHERE controlador = 'AuditoriaUso' AND UPPER(area) = 'ADMIN';

  IF v_existe = 0 THEN
    -- Siguiente posición bajo el mismo padre.
    SELECT NVL(MAX(posicion), 0) + 1 INTO v_posicion
      FROM CTR_MENU_IRIS WHERE idpadre = v_idpadre;

    -- ID del menú: siguiente valor libre de la tabla (sin depender de una secuencia,
    -- ya que CTR_MENU_IRIS_SEQ puede no existir o tener otro nombre en este esquema).
    SELECT NVL(MAX(id_menu), 0) + 1 INTO v_id_menu FROM CTR_MENU_IRIS;

    INSERT INTO CTR_MENU_IRIS
      (id_menu, descripcion, idpadre, posicion, tipo, area, controlador, vista, icono,
       vigente, detalle, usuario_creacion, fecha_creacion, maquina_creacion)
    VALUES
      (v_id_menu, 'Uso y Auditoría', v_idpadre, v_posicion, v_tipo, 'Admin', 'AuditoriaUso',
       'VwDashboardUso', 'fas fa-chart-line', 1, 'Dashboard de uso y auditoría del sistema',
       0, SYSDATE, 'DEPLOY');

    -- Roles que pueden ver la opción: los mismos que "Usuarios" (normalmente 1 y 2).
    -- El id_menurol también se calcula con MAX+1 (más ROWNUM para no repetir en el lote).
    SELECT NVL(MAX(id_menurol), 0) INTO v_idmenurol FROM CTR_MENU_ROLES_IRIS;

    INSERT INTO CTR_MENU_ROLES_IRIS
      (id_menurol, id_rol, id_menu, usuario_creacion, fecha_creacion, maquina_creacion)
    SELECT v_idmenurol + ROWNUM, mr.id_rol, v_id_menu, 0, SYSDATE, 'DEPLOY'
      FROM (SELECT DISTINCT mr.id_rol
              FROM CTR_MENU_ROLES_IRIS mr
              JOIN CTR_MENU_IRIS m ON m.id_menu = mr.id_menu
             WHERE UPPER(m.controlador) = 'USUARIOS' AND UPPER(m.area) = 'ADMIN') mr;

    COMMIT;
    DBMS_OUTPUT.PUT_LINE('Menú creado. ID_MENU=' || v_id_menu || ' bajo IDPADRE=' || v_idpadre);
  ELSE
    DBMS_OUTPUT.PUT_LINE('El submódulo AuditoriaUso ya existe; no se duplica.');
  END IF;
END;
/
