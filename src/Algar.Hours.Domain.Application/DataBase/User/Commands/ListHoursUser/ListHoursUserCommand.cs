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
           
            //Get history from Horus
            var listHorus = _dataBaseService.HorusReportEntity
                .Include(a => a.UserEntity)
                .Include(a => a.ClientEntity)
                .Where(x => x.UserEntityId == IdClient).ToList();

           
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
