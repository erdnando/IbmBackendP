namespace Algar.Hours.Application.DataBase.Client.Commands.Consult
{
	public interface IConsultClientCommand
    {
        Task<ClientModel> Consult(Guid id);
        Task<List<ClientModel>> List();
    }
}
