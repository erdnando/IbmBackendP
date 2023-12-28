namespace Algar.Hours.Application.DataBase.Rol.Commands
{
    public interface ICreateRolCommand
    {
        Task<CreateRolModel> Execute(CreateRolModel model);

    }
}
