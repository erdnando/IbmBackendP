using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.Festivos.Create
{
    public interface ICreateFestivoCommand
    {
        Task<Boolean> Execute(List<CreateFestivoModel> model);
    }
}
