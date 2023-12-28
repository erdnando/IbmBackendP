namespace Algar.Hours.Application.DataBase.Country.Commands.Update
{
	public interface IUpdateCountryCommand
	{
		Task<Boolean> Update(CountryModel model);
	}
}
