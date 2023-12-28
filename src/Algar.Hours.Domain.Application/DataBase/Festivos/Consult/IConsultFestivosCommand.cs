using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.Festivos.Consult
{
    public interface IConsultFestivosCommand
    {
        Task<List<FestivosModel>> ListAll(Guid CountryId);
    }
}
