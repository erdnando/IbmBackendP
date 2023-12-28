namespace Algar.Hours.Application.DataBase.HorusReport.Commands.DetailAssigment
{
    public interface IConsultDetailAssigmentCommand
    {
        Task<List<ConsultDetailAssigmentModel>> Execute(Guid IdReport);
    }
}
