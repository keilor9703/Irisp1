// Archivo global: routes.js

window.AppRoutes = Object.freeze({ 
    //Object.freeze() sirve para proteger el objeto contra modificaciones accidentales o maliciosas en tiempo de ejecución. el objeto AppRoutes queda inmutable:

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
        UrlEvaltarea: '/Irisp1/Seguimiento/P_EvalTarea',

        // Controlador Verificación
        UrlGetResponsablesTareasIris: '/Irisp1/Verificacion/F_GetResponsablesTareasIris',
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
     
    }



});



