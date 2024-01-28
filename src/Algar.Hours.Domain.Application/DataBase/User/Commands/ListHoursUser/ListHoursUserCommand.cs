using AutoMapper;
using Google.Protobuf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.User.Commands.ListHoursUser
{
    public class ListHoursUserCommand : IListHoursUserCommand
    {

        private readonly IDataBaseService _dataBaseService;
        private readonly IMapper _mapper;

        public ListHoursUserCommand(IDataBaseService dataBaseService, IMapper mapper)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;
        }


        public async Task<List<ListHorursUserModel>> Execute(Guid IdClient)
        {
            var userRef = _dataBaseService.UserEntity
                .Include(a => a.RoleEntity)
                .Where(x => x.IdUser == IdClient).FirstOrDefault();

            if(userRef.RoleEntity.NameRole=="Usuario estandar")
            {
                //standar
                var listStandar = _dataBaseService.HorusReportEntity
                    .Include(a => a.UserEntity)
                    .Include(a => a.ClientEntity)
                    .Where(x => x.UserEntityId == IdClient).ToList();

                foreach (var item in listStandar)
                {
                    var assigment = _dataBaseService.assignmentReports
                       .Include(a => a.UserEntity)
                       .Where(x => x.HorusReportEntityId == item.IdHorusReport).FirstOrDefault();
                    if (assigment != null)
                    {
                        item.ApproverId = assigment.UserEntity.NameUser + " " + assigment.UserEntity.surnameUser;
                        item.StartTime = assigment.State.ToString();
                    }
                }

                var listmodel = _mapper.Map<List<ListHorursUserModel>>(listStandar);
                return listmodel;
            }
            else 
            {
                //aprobador
                var listAprob = _dataBaseService.HorusReportEntity
                    .Include(a => a.UserEntity)
                    .Include(a => a.ClientEntity)
                    .Where(x => x.ApproverId == IdClient.ToString()).ToList();

                foreach (var item in listAprob)
                {
                    var assigment = _dataBaseService.assignmentReports
                       .Include(a => a.UserEntity)
                       .Where(x => x.HorusReportEntityId == item.IdHorusReport).FirstOrDefault();
                    if (assigment != null)
                    {
                        item.ApproverId = assigment.UserEntity.NameUser + " " + assigment.UserEntity.surnameUser;
                        item.StartTime = assigment.State.ToString();
                    }
                }

                var listmodel = _mapper.Map<List<ListHorursUserModel>>(listAprob);
                return listmodel;
            }





        }
    }
}
