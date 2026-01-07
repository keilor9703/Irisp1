using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun.Areas.Admin
{
    public interface IDbMfaIris
    {
        Task<MfaDbDto> GetMfaAsync(long idUsuario);

        Task P_Guardar_LlaveSecreta(long idUsuario, string secretEnc, string ip, long userAudit);

        Task P_Validacion_exitosa(long idUsuario, string ip, long userAudit);

        Task P_Intentos_Fallidos(long idUsuario, string ip, long userAudit, int bloqueoMinutos = 5, int maxIntentos = 5);

        Task<int> IsTrustedDeviceAsync(long idUsuario, string deviceHash);

        Task SaveTrustedDeviceAsync(long idUsuario, string deviceHash, int expiraDias, string ip, long userAudit);

        Task ResetMfaAsync(long idUsuario, string ip, long userAudit);



    }
}
