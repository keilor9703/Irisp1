using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negocio.Interfaz.Admin
{
    public interface IMfaTotpService
    {
        (string secretBase32, string qrPngBase64) GenerateEnrollmentQr(string issuer, string accountName);
        bool ValidateCode(string secretBase32, string code);
        string ProtectSecret(string secretBase32);
        string UnprotectSecret(string secretEnc);
        string HashDeviceId(string deviceId);
    }
}
