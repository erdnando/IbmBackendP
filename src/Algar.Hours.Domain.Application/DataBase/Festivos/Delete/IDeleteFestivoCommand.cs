using Algar.Hours.Application.DataBase.Festivos.Create;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.Festivos.Delete
{
    public interface IDeleteFestivoCommand
    {
        Task<Boolean> Delete(CreateFestivoModel model);
    }
}
