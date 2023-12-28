using Algar.Hours.Application.DataBase.User.Commands.Login;
using Algar.Hours.Domain.Entities.User;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.User.Commands.UpdateUser
{
    public class UpdateUseCommand : IUpdateUserCommand
    {

        private readonly IDataBaseService _databaseService;
        private readonly IMapper _mapper;

        public UpdateUseCommand(IDataBaseService databaseService, IMapper mapper)
        {
            _databaseService = databaseService;
            _mapper = mapper;

        }
        public async Task<bool> Execute(UpdateUserModel model)
        {

            var userEntity =  _databaseService.UserEntity.Where(x => x.IdUser == model.IdUser).FirstOrDefault();
            userEntity.NameUser = model.NameUser;
            userEntity.surnameUser = model.surnameUser;
            userEntity.Email = model.Email;
            _databaseService.UserEntity.Update(userEntity);
            _databaseService.SaveAsync();
            return true;

        }


    }
}
