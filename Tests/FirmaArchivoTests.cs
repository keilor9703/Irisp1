using Comun.General;
using Xunit;

namespace Tests
{
    public class FirmaArchivoTests
    {
        private static readonly byte[] Pdf = { 0x25, 0x50, 0x44, 0x46, 0x2D, 0x31 };   // %PDF-1
        private static readonly byte[] Zip = { 0x50, 0x4B, 0x03, 0x04, 0x14, 0x00 };   // PK.. (docx/xlsx)
        private static readonly byte[] Ole = { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1 };   // OLE2 (doc/xls)
        private static readonly byte[] Mz  = { 0x4D, 0x5A, 0x90, 0x00 };               // "MZ" ejecutable

        [Fact]
        public void Pdf_ConFirmaPdf_EsValido() =>
            Assert.True(FirmaArchivo.CoincideConExtension(Pdf, ".pdf"));

        [Theory]
        [InlineData(".docx")]
        [InlineData(".xlsx")]
        public void Ooxml_ConFirmaZip_EsValido(string ext) =>
            Assert.True(FirmaArchivo.CoincideConExtension(Zip, ext));

        [Fact]
        public void Doc_ConFirmaOle_EsValido() =>
            Assert.True(FirmaArchivo.CoincideConExtension(Ole, ".doc"));

        [Fact]
        public void Ejecutable_RenombradoComoPdf_EsRechazado() =>
            Assert.False(FirmaArchivo.CoincideConExtension(Mz, ".pdf"));

        [Fact]
        public void Pdf_ConFirmaZip_EsRechazado() =>
            Assert.False(FirmaArchivo.CoincideConExtension(Zip, ".pdf"));

        [Fact]
        public void Txt_SeAceptaSinFirmaBinaria() =>
            Assert.True(FirmaArchivo.CoincideConExtension(new byte[] { 0x48, 0x6F, 0x6C, 0x61 }, ".txt"));

        [Fact]
        public void EncabezadoNulo_OExtensionVacia_EsRechazado()
        {
            Assert.False(FirmaArchivo.CoincideConExtension(null, ".pdf"));
            Assert.False(FirmaArchivo.CoincideConExtension(Pdf, ""));
        }
    }
}
