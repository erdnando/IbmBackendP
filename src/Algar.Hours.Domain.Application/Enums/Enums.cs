namespace Algar.Hours.Application.Enums
{
    public class Enums
    {
        public enum Aprobacion
        {

            Aprobado = 1,
            Rechazado = 2,
            Pendiente = 3
        }
        public enum AprobacionPortalDB
        {
            Pendiente = 0,
            AprobadoN1 = 1,
            AprobadoN2 = 2,
            Rechazado = 3,
            Cerrada = 4,
            AprobadoN0 = 5,

        }

        public enum Pais
        {
            Colombia = 613,
            Uruguay = 869,
            Venezuala = 871,
            Argentina = 613,
            Chile = 655,
            Peru = 815,
            Ecuador = 683,
            Mexico = 781
        }

        public enum CodigoCliente
        {
            CodColombia = 000648,
            CodArgentina = 994854,
            CodChile = 992049,
            CodEcuador = 999104,
            CodPeru = 998101,
            CodUruguay = 992200,
            CodVenezuela = 994020,
            CodMexico = 999909
        }
    }
}
