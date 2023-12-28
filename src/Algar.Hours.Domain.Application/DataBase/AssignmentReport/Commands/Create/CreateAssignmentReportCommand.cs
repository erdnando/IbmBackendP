using AutoMapper;

namespace Algar.Hours.Application.DataBase.AssignmentReport.Commands.Create
{
    public class CreateAssignmentReportCommand : ICreateAssignmentReportCommand
    {
        private readonly IDataBaseService _dataBaseService;
        private readonly IMapper _mapper;

        public CreateAssignmentReportCommand(IDataBaseService dataBaseService, IMapper mapper)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;
        }

        public async Task<CreateAssignmentReportModel> Execute(CreateAssignmentReportModel model)
        {
            model.IdAssignmentReport = Guid.NewGuid();
            var entity = _mapper.Map<Domain.Entities.AssignmentReport.AssignmentReport>(model);
            await _dataBaseService.assignmentReports.AddAsync(entity);
            await _dataBaseService.SaveAsync();
            return model;
        }
    }
}
