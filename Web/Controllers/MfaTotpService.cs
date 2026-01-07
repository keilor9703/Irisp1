using Microsoft.AspNetCore.DataProtection;
using Negocio.Interfaz.Admin;
using OtpNet;
using QRCoder;
using System.Security.Cryptography;
using System.Text;

namespace Negocio.Gestion.Admin
{
    /// <summary>
    /// Servicio encargado de toda la lógica de MFA basada en TOTP (Time-based One-Time Password).
    /// 
    /// Responsabilidades:
    ///  - Generar secretos TOTP seguros
    ///  - Construir códigos QR compatibles con Google/Microsoft/Authy
    ///  - Validar códigos TOTP ingresados por el usuario
    ///  - Proteger (cifrar) el secreto antes de almacenarlo en base de datos
    ///  - Hashear de forma segura los identificadores de dispositivos confiables
    /// 
    /// Esta clase NO maneja vistas ni sesiones, solo lógica criptográfica y de seguridad.
    /// </summary>
    public class MfaTotpService : IMfaTotpService
    {
        /// <summary>
        /// Protector de datos de ASP.NET Core.
        /// Se utiliza para cifrar y descifrar el secreto TOTP antes de persistirlo.
        /// </summary>
        private readonly IDataProtector _protector;

        /// <summary>
        /// Constructor del servicio MFA.
        /// 
        /// Se crea un IDataProtector con un "purpose" específico.
        /// Esto garantiza que los datos cifrados solo puedan ser descifrados
        /// por este mismo contexto lógico de la aplicación.
        /// </summary>
        public MfaTotpService(IDataProtectionProvider provider)
        {
            // El purpose identifica de forma única el uso del cifrado
            // y permite versionar el esquema si es necesario (v1, v2, etc.)
            _protector = provider.CreateProtector("IRIS-P1.MFA.TOTP.SECRET.v1");
        }

        /// <summary>
        /// Genera el secreto TOTP y el código QR para el enrolamiento MFA del usuario.
        /// 
        /// Este método se ejecuta cuando el usuario configura MFA por primera vez.
        /// </summary>
        /// <param name="issuer">
        /// Nombre del sistema (ej: IRIS-P1).
        /// Aparece en la app autenticadora.
        /// </param>
        /// <param name="accountName">
        /// Identificador del usuario (usuario empresarial o identificación).
        /// </param>
        /// <returns>
        /// - secretBase32: clave secreta en Base32 (para validación TOTP)
        /// - qrPngBase64: imagen PNG del QR en Base64 (para renderizar en HTML)
        /// </returns>
        public (string secretBase32, string qrPngBase64)
            GenerateEnrollmentQr(string issuer, string accountName)
        {
            // Genera una clave aleatoria segura de 20 bytes (160 bits),
            // tamaño recomendado por el estándar RFC 6238
            var keyBytes = KeyGeneration.GenerateRandomKey(20);

            // Convierte la clave a Base32, formato estándar para TOTP
            var secretBase32 = Base32Encoding.ToString(keyBytes);

            // Construcción del URI estándar "otpauth://"
            // Este formato es entendido por Google Authenticator, Microsoft Authenticator, Authy, etc.
            var otpauth = new OtpUri(
                OtpType.Totp,
                secretBase32,
                accountName,
                issuer
            ).ToString();

            // Generación del código QR a partir del URI TOTP
            using var qrGen = new QRCodeGenerator();

            // ECCLevel.Q = alta tolerancia a errores (mejor lectura del QR)
            using var qrData = qrGen.CreateQrCode(otpauth, QRCodeGenerator.ECCLevel.Q);

            // Genera el QR en formato PNG
            using var qr = new PngByteQRCode(qrData);

            // El tamaño gráfico (6) define la resolución del QR
            byte[] png = qr.GetGraphic(6);

            // Retorna:
            // - El secreto en Base32 (para validación posterior)
            // - El QR en Base64 (para mostrarlo directamente en la vista)
            return (secretBase32, Convert.ToBase64String(png));
        }

        /// <summary>
        /// Valida un código TOTP ingresado por el usuario.
        /// </summary>
        /// <param name="secretBase32">
        /// Secreto TOTP en Base32 (ya desencriptado).
        /// </param>
        /// <param name="code">
        /// Código de 6 dígitos ingresado por el usuario.
        /// </param>
        /// <returns>
        /// true si el código es válido dentro de la ventana permitida, false en caso contrario.
        /// </returns>
        public bool ValidateCode(string secretBase32, string code)
        {
            // Validación defensiva: el código no puede ser nulo ni vacío
            if (string.IsNullOrWhiteSpace(code))
                return false;

            // Se eliminan caracteres no numéricos (espacios, guiones, etc.)
            code = new string(code.Where(char.IsDigit).ToArray());

            // El código TOTP estándar es de 6 dígitos
            if (code.Length != 6)
                return false;

            // Convierte el secreto Base32 a bytes
            var keyBytes = Base32Encoding.ToBytes(secretBase32);

            // Se crea el generador TOTP:
            // - step: 30 segundos
            // - totpSize: 6 dígitos
            var totp = new Totp(
                keyBytes,
                step: 30,
                totpSize: 6
            );

            // Verificación del código con ventana de tolerancia:
            // - 1 intervalo anterior (30s)
            // - 1 intervalo futuro (30s)
            // Esto evita errores por desfase de reloj del celular
            return totp.VerifyTotp(
                code,
                out _,
                new VerificationWindow(previous: 1, future: 1)
            );
        }

        /// <summary>
        /// Cifra el secreto TOTP antes de almacenarlo en la base de datos.
        /// 
        /// Nunca se guarda el secreto en texto plano.
        /// </summary>
        public string ProtectSecret(string secretBase32)
            => _protector.Protect(secretBase32);

        /// <summary>
        /// Descifra el secreto TOTP almacenado en la base de datos.
        /// 
        /// Se utiliza únicamente en memoria para validar códigos.
        /// </summary>
        public string UnprotectSecret(string secretEnc)
            => _protector.Unprotect(secretEnc);

        /// <summary>
        /// Genera un hash SHA-256 del identificador de dispositivo.
        /// 
        /// Se usa para la funcionalidad "Recordar este equipo".
        /// Nunca se almacena el identificador real del dispositivo.
        /// </summary>
        /// <param name="deviceId">
        /// Identificador aleatorio generado para el dispositivo (cookie).
        /// </param>
        /// <returns>
        /// Hash SHA-256 en formato hexadecimal.
        /// </returns>
        public string HashDeviceId(string deviceId)
        {
            using var sha = SHA256.Create();

            // Calcula el hash SHA-256 del identificador
            var bytes = sha.ComputeHash(
                Encoding.UTF8.GetBytes(deviceId)
            );

            // Retorna el hash en formato hexadecimal
            return Convert.ToHexString(bytes);
        }
    }
}
