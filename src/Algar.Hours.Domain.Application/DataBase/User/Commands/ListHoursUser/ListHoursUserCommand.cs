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
           

            var listHorus = _dataBaseService.HorusReportEntity
                .Include(a => a.UserEntity)
                .Include(a => a.ClientEntity)
                .Where(x => x.UserEntityId == IdClient).ToList();

            foreach (var item in listHorus)
            {
                if (item.ApproverId != "")
                {
                    var ApproverId1 = _dataBaseService.UserEntity
                 .Where(x => x.IdUser == Guid.Parse(item.ApproverId)).FirstOrDefault();

                    if (ApproverId1 != null)
                        item.ApproverId = ApproverId1.NameUser + " " + ApproverId1.surnameUser;
                }

                if (item.ApproverId2 != "")
                {
                    var ApproverId2 = _dataBaseService.UserEntity
                .Where(x => x.IdUser == Guid.Parse(item.ApproverId2)).FirstOrDefault();



                    if (ApproverId2 != null)
                        item.ApproverId2 = ApproverId2.NameUser + " " + ApproverId2.surnameUser;
                }
                  
            }
          

           
            try
            {
                var listmodel = _mapper.Map<List<ListHorursUserModel>>(listHorus);
                return listmodel;
            }
            catch (Exception ex)
            {

                throw;
            }
            
        





        }
    }
}
