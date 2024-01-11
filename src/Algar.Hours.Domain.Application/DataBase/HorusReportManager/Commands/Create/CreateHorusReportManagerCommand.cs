using Algar.Hours.Application.DataBase.AssignmentReport.Commands;
using Algar.Hours.Application.DataBase.HorusReport.Commands;
using Algar.Hours.Domain.Entities.HorusReport;
using Algar.Hours.Domain.Entities.HorusReportManagerEntity;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.HorusReportManager.Commands.Create
{
    public class CreateHorusReportManagerCommand: ICreateHorusReportManagerCommand
    {
        private readonly IDataBaseService _dataBaseService;
        private readonly IMapper _mapper;

        public CreateHorusReportManagerCommand(IDataBaseService dataBaseService, IMapper mapper)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;
        }

        public async Task<HorusReportManagerModel> Execute(CreateHorusReportManagerModel model)
        {
            var horusManagerModel = _mapper.Map<HorusReportManagerModel>(model);
            horusManagerModel.IdHorusReportManager = Guid.NewGuid();
            horusManagerModel.CreationDate = model.CreationDate;

            var startTime = DateTime.Parse(model.StartTime);
            var endTime = DateTime.Parse(model.EndTime);

            var data = _dataBaseService.HorusReportManagerEntity
                .Where(h => h.CreationDate == model.CreationDate && h.UserEntityId == model.UserEntityId)
                .AsEnumerable()
                .Where(h => TimeRangesOverlap(h.StartTime, h.EndTime, model.StartTime, model.EndTime) ||
                (TimeInRange(h.StartTime, startTime, endTime) &&
                 TimeInRange(h.EndTime, startTime, endTime)))
                .ToList();


            if (data.Count == 0)
            {
                var entity = _mapper.Map<HorusReportManagerEntity>(horusManagerModel);

                var top=await _dataBaseService.HorusReportManagerEntity.AddAsync(entity);

                //CreateAssignmentReportModel assignmentReport = new CreateAssignmentReportModel();
                //assignmentReport.IdAssignmentReport = Guid.NewGuid();
                //assignmentReport.HorusReportEntityId = entity.IdHorusReport;
                //assignmentReport.UserEntityId = Guid.Parse(entity.ApproverId);
                //assignmentReport.Description = horusModel.Description;
                //assignmentReport.State = (byte)Enums.Enums.Aprobacion.Pendiente;

                //var assigmentreportentity = _mapper.Map<Domain.Entities.AssignmentReport.AssignmentReport>(assignmentReport);
                //_dataBaseService.assignmentReports.Add(assigmentreportentity);

                await _dataBaseService.SaveAsync();
            }
            else
            {

            }

            






            return horusManagerModel;
        }

        public async Task<bool> ExecuteLst(List<CreateHorusReportManagerModel> model)
        {
            HorusReportManagerModel returnRow = new();
            //var horusManagerModel = _mapper.Map<List<HorusReportManagerModel>>(model);
            List<CreateHorusReportManagerModel> horusManagerModel = model;
            foreach (var item in horusManagerModel)
            {
                var startTime = DateTime.Parse(item.StartTime);
                var endTime = DateTime.Parse(item.EndTime);
                var data = _dataBaseService.HorusReportManagerEntity
                    .Where(h => h.CreationDate == item.CreationDate && h.UserEntityId == item.UserEntityId)
                    .AsEnumerable()
                    .Where(h => TimeRangesOverlap(h.StartTime, h.EndTime, item.StartTime, item.EndTime) ||
                    (TimeInRange(h.StartTime, startTime, endTime) &&
                     TimeInRange(h.EndTime, startTime, endTime)))
                    .ToList();


                if (data.Count == 0)
                {
                    item.IdHorusReportManager = Guid.NewGuid();

                    
                    var entity = _mapper.Map<HorusReportManagerEntity>(item);

                    var top = await _dataBaseService.HorusReportManagerEntity.AddAsync(entity);

                    //CreateAssignmentReportModel assignmentReport = new CreateAssignmentReportModel();
                    //assignmentReport.IdAssignmentReport = Guid.NewGuid();
                    //assignmentReport.HorusReportEntityId = entity.IdHorusReport;
                    //assignmentReport.UserEntityId = Guid.Parse(entity.ApproverId);
                    //assignmentReport.Description = horusModel.Description;
                    //assignmentReport.State = (byte)Enums.Enums.Aprobacion.Pendiente;

                    //var assigmentreportentity = _mapper.Map<Domain.Entities.AssignmentReport.AssignmentReport>(assignmentReport);
                    //_dataBaseService.assignmentReports.Add(assigmentreportentity);

                    //return true;
                }
                else
                {

                }
            }
            await _dataBaseService.SaveAsync();

            return true;








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
