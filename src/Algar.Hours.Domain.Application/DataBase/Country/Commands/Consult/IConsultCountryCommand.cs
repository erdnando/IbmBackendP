using Algar.Hours.Application.DataBase.Country.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.Country.Commands.Consult
{
	public interface IConsultCountryCommand
	{
		Task<CountryModel> Consult(Guid id);
		Task<List<CountryModel>> List();
        Task<Guid> ConsultIdbyName(string pais);
    }
}
