﻿using Algar.Hours.Application.DataBase.User.Commands.CreateUser;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.User.Commands.Email
{
    public class EmailCommand : IEmailCommand
    {
        private readonly IDataBaseService _databaseService;
        private readonly IMapper _mapper;

        public EmailCommand(IDataBaseService databaseService,IMapper mapper) 
        { 
           _databaseService = databaseService;
           _mapper = mapper; 
        
        }

        public async Task<Boolean> Execute(EmailModel model)
        {
            /*var entity = _databaseService.UserEntity
                .Include(e => e.CountryEntity)
                .Include(e => e.RoleEntity)
                .Where(x => x.Email == model.UserName && x.Password == model.Password).FirstOrDefault();
            var ModelUser = _mapper.Map<CreateUserModel>(entity); */
            return true;

        }

        

        public  string GetBody(EmailModel model)
        {
            string body = "";
            switch (model.Plantilla)
            {
                case "1":
                    body = "Su reporte se encuentra en estado: \"aprobado, rechazado, pendiente\", por favor comuniquese con el respectivo aprobador para dar gestion"; break;
                case "2":
                    body = "Ha llegado un reporte, por favor tome acciones \"aprobar, rechazar\""; break;
                case "3":
                    body = "Su reporte ha sido rechazado, por favor comuníquese con su aprobador para tener más información"; break;
                case "4":
                    body = "El reporte ha sido rechazado correctamente, por favor comuníquese con la persona a la cual se le rechazo el registro"; break;
                default:
                    break;
            }
            return body;

        }

        public  string GetSubject(EmailModel model)
        {
            string body = "";
            switch (model.Plantilla)
            {
                case "1":
                    body = "ALERTA \"PORTAL TLS\" APROBACION DE HORAS"; break;
                case "2":
                    body = "ALERTA \"PORTAL TLS \" Su registro fue \"aprobado, rechazado\""; break;
                case "3":
                    body = "ALERTA \"PORTAL TLS \" Su registro fue \"rechazado\""; break;
                case "4":
                    body = "ALERTA \"PORTAL TLS\" El registro fue rechazado correctamente"; break;
                default:
                    break;
            }
            return body;

        }

        public Boolean SendEmail(EmailModel model)
        {
            
            var smtpClient = new SmtpClient(d642s("c210cC5zZW5kZ3JpZC5uZXQ="))
            {
                Port = 587,
                Credentials = new NetworkCredential(d642s("YXBpa2V5"), d642s("U0cuQ3NwNjlNVE1UVHUwOUEwSThCMDNmUS55WTVNOXRXa1BUbkVVTlhqd1B2NERKcHdWdnlEZl9YblB1OEFZSWhvWWNZ")),
                EnableSsl = true,
            };

            try
            {
                
                smtpClient.Send("notifications@cognos.ibm.com", model.To, GetSubject(model), GetBody(model));
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public static string d642s(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;
            var valueBytes = System.Convert.FromBase64String(value);
            return System.Text.Encoding.UTF8.GetString(valueBytes);
        }


    }
}
