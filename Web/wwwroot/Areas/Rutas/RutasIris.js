// Archivo global: routes.js

window.AppRoutes = Object.freeze({ 
    //Object.freeze() sirve para proteger el objeto contra modificaciones accidentales o maliciosas en tiempo de ejecución. el objeto AppRoutes queda inmutable:


    // Rutas para módulo ADMINISTRACIÓN   
    Administracion: {
        // Controlador Emppleados
        UrlGetFuncionarios: '/Empleados/F_GetFuncionarios',
        UrlGetEmpleadoIntel: '/Empleados/F_GetEmpleadoIntel',
        // Controlador Usuarios
        UrlInsUdpUsuarios: '/Admin/Usuarios/P_InsUdpUsuarios',
        UrlGetUserRoles: '/Admin/Usuarios/F_GetUserRoles',
        UrlInsRoles: '/Admin/Usuarios/P_InsRoles',
        UrlDelRoles: '/Admin/Usuarios/P_DelRoles',
        UrlGetListUsuarios: '/Admin/Usuarios/F_GetListUsuarios'
    },


    // Rutas para módulo IRISP1 / Registro
    RegistroIrisP1: {
        // Controlador Registros IRISP1
        UrlGetAniosIrisp1: '/Irisp1/RegistrosIrisp1/F_GetAniosIrisP1',
        UrlGetInfoGrillas: '/Irisp1/RegistrosIrisp1/F_GetInfoGrillas',
        UrlGetFuncionarios: '/Empleados/F_GetFuncionarios',
        UrlInsIntegrantesPreliminar: '/Irisp1/RegistrosIrisp1/P_InsIntegrantesPreliminar',
        UrlInsIntegrantes: '/Irisp1/RegistrosIrisp1/P_InsIntegrantes',
        UrlGetConsecutivoIris: '/Irisp1/RegistrosIrisp1/F_ConsultarSeqIris',
        UrlGetConsecutivoIntegrante: '/Irisp1/RegistrosIrisp1/F_ConsultarSeqIntegrante',
        UrlGetIntegrantes: '/Irisp1/RegistrosIrisp1/F_GetIntegrantes',
        UrlGetIntegrantesPreliminar: '/Irisp1/RegistrosIrisp1/F_GetIntegrantesPreliminar',
        UrlInsRegistroIrisP1: '/Irisp1/RegistrosIrisp1/P_InsRegistroIrisP1',
        UrlGetDelitosIris: '/Irisp1/RegistrosIrisp1/F_GetDelitosIris',
        UrlGetInfoAdicional: '/Irisp1/RegistrosIrisp1/F_GetInfoAdicional',
        UrlGetDocIris: '/Irisp1/RegistrosIrisp1/F_GetDocIris',
        UrlGetFotosIris: '/Irisp1/RegistrosIrisp1/f_GetFotosCriminalidad',
        UrlGetUbicacion: '/Irisp1/RegistrosIrisp1/F_GetUbicacionIris',
        UrlInsDelitos: '/Irisp1/RegistrosIrisp1/P_InsDelitosIris',
        UrlInsUbicacion: '/Irisp1/RegistrosIrisp1/P_InsUbicacionIris',
        UrlInsInfoAdicional: '/Irisp1/RegistrosIrisp1/P_InsInfoAdicionalIris',
        UrlUpdCriminalidad: '/Irisp1/RegistrosIrisp1/P_UpdCriminalidad',
        UrlUpdEstadoCriminalidad: '/Irisp1/RegistrosIrisp1/P_UpdEstadoCriminalidad',
        UrlUpdExistenciaCriminalidad: '/Irisp1/RegistrosIrisp1/P_UpdExistenciaCriminalidad',
        UrlDelIris: '/Irisp1/RegistrosIrisp1/P_DellIris',
        UrlDelIntegrante: '/Irisp1/RegistrosIrisp1/P_DelIntegranteIris',
        UrlDelDelitos: '/Irisp1/RegistrosIrisp1/P_DelDelitosIris',
        UrlDelInfoAdicionalIris: '/Irisp1/RegistrosIrisp1/P_DelDelInfoAdicionalIris',
        UrlDelUbiacionIris: '/Irisp1/RegistrosIrisp1/P_DelUbicacionIris',
        UrlDelDocumentoIris: '/Irisp1/RegistrosIrisp1/P_DelDocumentoIris'
    },

    // Rutas para módulo IRISP1 / Seguimiento
    Seguimiento: {
        // Controlador Seguimiento
        UrlGetInfoGrillas: '/Irisp1/Seguimiento/F_GetInfoGrillas',
        UrlGetAniosIrisp1: '/Irisp1/Seguimiento/F_GetAniosIrisP1',
        UrlConsultarAnioSeguimiento: '/Irisp1/Seguimiento/ConsultarAnioSeguimiento',
        UrlGetResponsable: '/Irisp1/Seguimiento/F_GetResponsables',
        UrlGetDependencias: '/Irisp1/Seguimiento/F_GetUnidadesPorSigla',
        UrlInsResponsable: '/Irisp1/Seguimiento/P_InsResponsable',
        UrlUpdResponsable: '/Irisp1/Seguimiento/P_UpdUnidadResponsable',
        UrlDelResponsable: '/Irisp1/Seguimiento/P_DelUnidadResponsable',
        UrlEvalTarea: '/Irisp1/Seguimiento/P_EvalTarea',
        UrlReasignarTarea: '/Irisp1/Seguimiento/P_ReasignarTarea',
        UrlFinalizarIris: '/Irisp1/Seguimiento/P_FinalizarIris',
        // Controlador Verificación
        UrlGetResponsablesTareasIris: '/Irisp1/Seguimiento/F_GetResponsablesTareasIris',
        UrlGetResultadosIris: '/Irisp1/Verificacion/F_GetResultados',
        // Controlador Empleados
        UrlGetFuncionarios: '/Empleados/F_GetFuncionarios',
        // Controlador Registros IRISP1
        UrlGetIntegrantes: '/Irisp1/RegistrosIrisp1/F_GetIntegrantes',
        UrlGetUbicacion: '/Irisp1/RegistrosIrisp1/F_GetUbicacionIris',
        UrlGetDelitosIris: '/Irisp1/RegistrosIrisp1/F_GetDelitosIris',
        UrlGetInfoAdicional: '/Irisp1/RegistrosIrisp1/F_GetInfoAdicional',
        UrlGetDocIris: '/Irisp1/RegistrosIrisp1/F_GetDocIris'
       
    },

    // Rutas para módulo Expendios / Registro
    RegistroExpendio: {
        // Controlador Seguimiento
        UrlGetInfoGrillas: '/Expendios/Registros/F_GetInfoGrillas',
        UrlGetIntegrantes: '/Expendios/Registros/F_GetIntegrantes',
        UrlGetDelitosIris: '/Expendios/Registros/F_GetDelitosIris',
        UrlGetBitacora: '/Expendios/Registros/F_GetBitacora',
        UrlGetResultados: '/Expendios/Registros/F_GetResultados',
        UrlGetIntegrantesAll: '/Expendios/Registros/F_GetIntegranteAll',
        UrlInsIntgrante: '/Expendios/Registros/P_InsIntegrante',
        UrlInsIntgrantePreliminar: '/Expendios/Registros/P_InsIntegrantePreliminar',
        UrlInsDelito: '/Expendios/Registros/P_InsDelito',
        UrlInsBitacora: '/Expendios/Registros/P_InsBitacora',
        UrlInsResultados: '/Expendios/Registros/P_InsResultados',
        UrlUpdExpendio: '/Expendios/Registros/P_UpdExpendio',
        UrlUpdIntegrante: '/Expendios/Registros/P_UpdIntegrante',
        UrlInsExpendio: '/Expendios/Registros/P_UpdIntegrante',
        UrlGetIntegrantesPreliminar: '/Expendios/Registros/F_GetIntegrantesPreliminar',
        UrlGetConsecutivoIris: '/Expendios/Registros/F_ConsultarSeqIris',
        UrlInsRegistroExpendio: '/Expendios/Registros/P_InsRegistroExpendio'
     
    },


    // Rutas para módulo Integrantes/ Registrar
    RegistroReincidentes: {
        // Controlador ReagistrarInteg
        UrlGetInfoGrila: '/Integrantes/RegistrarInteg/F_GetReincidentes',
        UrlGetReincidentes: '/Integrantes/RegistrarInteg/F_GetReincidentesPorId',
        UrlInsOrUpdReincidente: '/Integrantes/RegistrarInteg/P_InsOrUpdReincidente',
        UrlUpdReincidente: '/Integrantes/RegistrarInteg/P_UpdReincidente',
        UrlDellReincidente: '/Integrantes/RegistrarInteg/P_DellReincidente',
    },

       // Rutas para módulo Integrantes / Buscar
    BuscarIntegrantes: {
        // Controlador BuscarInteg
        UrlGetListaIris: '/Integrantes/BuscarInteg/F_GetListaIris',
        UrlGetintegrantesPorId: '/Integrantes/BuscarInteg/F_GetIntegrantesPorId',
       


    }

});



