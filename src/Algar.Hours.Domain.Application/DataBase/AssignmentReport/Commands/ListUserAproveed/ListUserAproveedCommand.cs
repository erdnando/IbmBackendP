﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Algar.Hours.Application.DataBase.AssignmentReport.Commands.ListUserAproveed
{
    public class ListUserAproveedCommand : IListUserAproveedCommand
    {
        private readonly IDataBaseService _dataBaseService;
        private readonly IMapper _mapper;

        public ListUserAproveedCommand(IDataBaseService dataBaseService, IMapper mapper)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;
        }
        public List<ListAproveedModel> Execute(Guid UserId)
        {
            var listAssigment = _dataBaseService.assignmentReports
                  .Include(a => a.UserEntity)
                  .Include(b => b.HorusReportEntity)
                  .Where(x => x.UserEntityId == UserId).ToList();

            foreach (var item in listAssigment)
            {
                var userentity = _dataBaseService.UserEntity.Where(x => x.IdUser == item.HorusReportEntity.UserEntityId).FirstOrDefault();
                item.HorusReportEntity.UserEntity = userentity;

            }

            var model = _mapper.Map<List<ListAproveedModel>>(listAssigment);

            return model;



        }
    }
}
