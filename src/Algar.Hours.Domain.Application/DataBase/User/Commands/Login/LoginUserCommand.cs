﻿using Algar.Hours.Application.DataBase.User.Commands.CreateUser;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.User.Commands.Login
{
    public class LoginUserCommand : ILoginUserCommand
    {
        private readonly IDataBaseService _databaseService;
        private readonly IMapper _mapper;

        public LoginUserCommand(IDataBaseService databaseService,IMapper mapper) 
        { 
           _databaseService = databaseService;
           _mapper = mapper; 
        
        }

        public async Task<CreateUserModel> Execute(LoginUserModel model)
        {
            var entity = _databaseService.UserEntity
                .Include(e => e.CountryEntity)
                .Include(e => e.RoleEntity)
                .Where(x => x.Email == model.UserName && x.Password == model.Password).FirstOrDefault();
            var ModelUser = _mapper.Map<CreateUserModel>(entity); 
            return ModelUser;

        }
    }
}
