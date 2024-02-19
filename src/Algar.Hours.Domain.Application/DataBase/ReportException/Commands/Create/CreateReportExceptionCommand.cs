using Algar.Hours.Application.DataBase.HorusReport.Commands;
using Algar.Hours.Application.DataBase.ReportException.Commands.Consult;
using Algar.Hours.Domain.Entities.ReportException;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.ReportException.Commands.Create
{
    public class CreateReportExceptionCommand : ICreateReportExceptionCommand
    {
        private readonly IDataBaseService _dataBaseService;
        private readonly IMapper _mapper;

        public CreateReportExceptionCommand(IDataBaseService dataBaseService, IMapper mapper)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;
        }

        public async Task<ReportExceptionEntity> Execute(ReportExceptionModelC createReportException)
        {
            var reportException = _mapper.Map<ReportExceptionEntity>(createReportException);
            reportException.IdReportException = Guid.NewGuid();
            await _dataBaseService.ReportExceptionEntity.AddAsync(reportException);

            await _dataBaseService.SaveAsync();

            var report = _dataBaseService.HorusReportEntity
                .Where(x => (x.StrReport == reportException.Report && x.EstatusFinal == "RECHAZADO"))
                .FirstOrDefault();

            if (report != null) {
                var assignment = _dataBaseService.assignmentReports
                    .Where(x => (x.HorusReportEntityId == report.IdHorusReport && (x.Nivel == 1 || x.Nivel == 2)))
                    .OrderByDescending(x => x.Nivel)
                    .FirstOrDefault();

                if (assignment != null) {
                    report.EstatusFinal = "APROBADO";
                    report.Estado = assignment.Nivel == 1 ? (byte)Enums.Enums.AprobacionPortalDB.AprobadoN1 : (byte)Enums.Enums.AprobacionPortalDB.AprobadoN2;
                    _dataBaseService.HorusReportEntity.Update(report);
                    await _dataBaseService.SaveAsync();

                    reportException.ExceptionDate = DateTime.Now;
                    reportException.ExceptionUserEntityId = reportException.UserEntityId;
                    _dataBaseService.ReportExceptionEntity.Update(reportException);
                    await _dataBaseService.SaveAsync();

                    var newAssignment = new Domain.Entities.AssignmentReport.AssignmentReport();
                    newAssignment.IdAssignmentReport = Guid.NewGuid();
                    newAssignment.UserEntityId = Guid.Parse("53765c41-411f-4add-9034-7debaf04f276");
                    newAssignment.HorusReportEntityId = report.IdHorusReport;
                    newAssignment.State = 1;
                    newAssignment.Resultado = assignment.Nivel == 1? (byte)Enums.Enums.AprobacionPortalDB.AprobadoN1 : (byte)Enums.Enums.AprobacionPortalDB.AprobadoN2;
                    newAssignment.strFechaAtencion = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                    newAssignment.Description = "APROBADO POR EXCEPCION";
                    newAssignment.Nivel = assignment.Nivel;
                    await _dataBaseService.assignmentReports.AddAsync(newAssignment);
                    await _dataBaseService.SaveAsync();
                    
                }
            }

            return reportException;
        }
    }
}
