using Algar.Hours.Application.DataBase.AssignmentReport.Commands;
using Algar.Hours.Application.DataBase.User.Commands.Email;
using Algar.Hours.Domain.Entities.HorusReport;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;

namespace Algar.Hours.Application.DataBase.HorusReport.Commands.Create
{
    public class CreateHorusReportCommand : ICreateHorusReportCommand
    {
        private readonly IDataBaseService _dataBaseService;
        private readonly IMapper _mapper;

        public CreateHorusReportCommand(IDataBaseService dataBaseService, IMapper mapper)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;
        }

        public async Task<HorusReportModel> Execute(CreateHorusReportModel model)
        {
            var horusModel = _mapper.Map<HorusReportModel>(model);
            horusModel.IdHorusReport = Guid.NewGuid();
            horusModel.CreationDate = DateTime.Now;
            horusModel.DateApprovalSystem = DateTime.Now;

            var startTime = DateTime.Parse(model.StartTime);
            var endTime = DateTime.Parse(model.EndTime);

            //2023-12-14T00:00:00.0000000
            DateTime fechaHoraOriginal = DateTime.ParseExact(model.StartDate, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
            string nuevaFechaHoraFormato = fechaHoraOriginal.ToString("yyyy-MM-dd 00:00:00");

           ////// DateTime fechaHoraOriginal = DateTime.ParseExact(model.StartDate, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
            //string nuevaFechaHoraFormato = fechaHoraOriginal.ToString("yyyy-MM-ddTHH:mm:ss");

            var data = _dataBaseService.HorusReportEntity
                .Where(h => h.StartDate == DateTime.Parse(nuevaFechaHoraFormato) && h.UserEntityId== model.UserEntityId)
                .AsEnumerable()
                .Where(h => TimeRangesOverlap(h.StartTime, h.EndTime, model.StartTime, model.EndTime) ||
                (TimeInRange(h.StartTime, startTime, endTime) &&
                 TimeInRange(h.EndTime, startTime, endTime)))
                .ToList();


            if (data.Count != 0)
            {
                return null;
            }

            if (horusModel.ClientEntityId == Guid.Empty || string.IsNullOrEmpty(horusModel.ClientEntityId.ToString()))
            {
                horusModel.ClientEntityId = Guid.Parse("DC606C5A-149E-4F9B-80B3-BA555C7689B9");
            }

            var entity = _mapper.Map<HorusReportEntity>(horusModel);

            var horusreporcount = _dataBaseService.HorusReportEntity.Count();
            var Maxen = 0;

            if (horusreporcount > 0)
            {
                Maxen = _dataBaseService.HorusReportEntity.Max(x => x.NumberReport);
            }

            if (Maxen == 0)
            {
                Maxen = 1;
            }

            entity.NumberReport = Maxen + 1;
            entity.StartDate = DateTime.Parse(nuevaFechaHoraFormato).Date;
            await _dataBaseService.HorusReportEntity.AddAsync(entity);

            CreateAssignmentReportModel assignmentReport = new CreateAssignmentReportModel();
            assignmentReport.IdAssignmentReport = Guid.NewGuid();
            assignmentReport.HorusReportEntityId = entity.IdHorusReport;
            assignmentReport.UserEntityId = Guid.Parse(entity.ApproverId);
            assignmentReport.Description = horusModel.Description;
            assignmentReport.State = (byte)Enums.Enums.Aprobacion.Pendiente;

            var assigmentreportentity = _mapper.Map<Domain.Entities.AssignmentReport.AssignmentReport>(assignmentReport);
            _dataBaseService.assignmentReports.Add(assigmentreportentity);

            await _dataBaseService.SaveAsync();


            



            return horusModel;
        }

        private bool TimeRangesOverlap(string existingStartTime, string existingEndTime, string newStartTime, string newEndTime)
        {
            DateTime startTimeExisting = DateTime.Parse(existingStartTime);
            DateTime endTimeExisting = DateTime.Parse(existingEndTime);
            DateTime startTimeNew = DateTime.Parse(newStartTime);
            DateTime endTimeNew = DateTime.Parse(newEndTime);

            return (startTimeNew < endTimeExisting && endTimeNew > startTimeExisting);
        }

        private bool TimeInRange(string timeString, DateTime rangeStart, DateTime rangeEnd)
        {
            DateTime time = DateTime.Parse(timeString);
            return time >= rangeStart && time <= rangeEnd;
        }

    }
}
