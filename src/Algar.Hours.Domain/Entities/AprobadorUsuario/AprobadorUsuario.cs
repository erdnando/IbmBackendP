using Algar.Hours.Domain.Entities.User;

namespace Algar.Hours.Domain.Entities.AprobadorUsuario
{
    public class AprobadorUsuario
    {
        public Guid IdAprobadorUsuario { get; set; }
        public Guid UserEntityId { get; set; }
        public  UserEntity UserEntity { get; set; }
        public Guid AprobadorId { get; set; }
        public  Aprobador.Aprobador Aprobador { get; set; }

    }
}
