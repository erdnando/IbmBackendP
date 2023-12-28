namespace Algar.Hours.Application.DataBase.Country.Commands.Create
{
    public interface ICreateCountryCommand
    {
        Task<CountryModel> Execute(CountryModel model);
    }
}
