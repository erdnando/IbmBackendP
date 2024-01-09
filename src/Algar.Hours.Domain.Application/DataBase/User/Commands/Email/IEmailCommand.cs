using Algar.Hours.Application.DataBase.User.Commands.CreateUser;

namespace Algar.Hours.Application.DataBase.User.Commands.Email
{
    public interface IEmailCommand
    {
        //Task<Boolean> Execute(EmailModel model);
       // Task<string> GetPlantilla(EmailModel model);
        string GetSubject(EmailModel model);
        string GetBody(EmailModel model);

    }
}
