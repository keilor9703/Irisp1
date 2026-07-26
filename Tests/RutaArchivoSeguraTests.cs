using Comun.General;
using Xunit;

namespace Tests
{
    public class RutaArchivoSeguraTests
    {
        // Base neutral respecto de plataforma (funciona en Windows y Linux).
        private static readonly string Base = Path.Combine(Path.GetTempPath(), "irisp_docs");

        [Fact]
        public void Resolver_RutaValidaDentroDeLaBase_DevuelveRutaConfinada()
        {
            var resultado = RutaArchivoSegura.Resolver(Base, "2026/05/documento.pdf");

            Assert.NotNull(resultado);
            var esperado = Path.GetFullPath(Path.Combine(Base, "2026", "05", "documento.pdf"));
            Assert.Equal(esperado, resultado);
        }

        [Theory]
        [InlineData("../../etc/passwd")]
        [InlineData("../../../windows/win.ini")]
        [InlineData("subcarpeta/../../../fuera.txt")]
        public void Resolver_IntentoDePathTraversal_DevuelveNull(string rutaMaliciosa)
        {
            var resultado = RutaArchivoSegura.Resolver(Base, rutaMaliciosa);
            Assert.Null(resultado);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Resolver_EntradaVacia_DevuelveNull(string? entrada)
        {
            Assert.Null(RutaArchivoSegura.Resolver(Base, entrada));
            Assert.Null(RutaArchivoSegura.Resolver(entrada, "archivo.pdf"));
        }

        [Fact]
        public void Resolver_RutaConLetraDeUnidad_DevuelveNull()
        {
            Assert.Null(RutaArchivoSegura.Resolver(Base, "C:/Windows/win.ini"));
        }

        [Fact]
        public void Resolver_PrefijoDeCarpetaHermana_NoSeConfundeConLaBase()
        {
            // "irisp_docs_otro" empieza por "irisp_docs" pero es otra carpeta: no debe pasar.
            var baseHermana = Base + "_otro";
            var resultado = RutaArchivoSegura.Resolver(Base, Path.Combine("..", Path.GetFileName(baseHermana), "x.pdf"));
            Assert.Null(resultado);
        }
    }
}
