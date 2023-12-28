namespace Algar.Hours.Application.DataBase.RolMenu.Commands.Consult
{
    public interface IConsultRolMenuCommand
    {
        Task<List<RolMenuModel>> ListByIdRol(Guid RolId);
    }
}
