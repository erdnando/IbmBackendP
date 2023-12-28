namespace Algar.Hours.Application.DataBase.Client.Commands.Create
{
    public interface ICreateClientCommand
    {
        Task<ClientModel> Execute(ClientModel model);
    }
}
