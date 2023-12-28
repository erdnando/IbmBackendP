using System.ComponentModel.DataAnnotations;

namespace Algar.Hours.Domain.Entities.Load.Philadedata
{
    public class Philadedata
    {
        [Key]
        public Guid IdPhiladedata { get; set; }
        public DateTime Fecha { get; set; }
        public string HoraInicio { get; set; }
        public string HoraFin { get; set; }
        public double TotalHoras { get; set; }
        public string Comentarios { get; set; }
        public int Estado { get; set; }

    }
}
