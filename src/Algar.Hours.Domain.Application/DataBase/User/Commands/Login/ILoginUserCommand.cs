using Algar.Hours.Application.DataBase.User.Commands.CreateUser;

namespace Algar.Hours.Application.DataBase.User.Commands.Login
{
    public interface ILoginUserCommand
    {
        Task<CreateUserModel> Execute(LoginUserModel model);

    }
}
