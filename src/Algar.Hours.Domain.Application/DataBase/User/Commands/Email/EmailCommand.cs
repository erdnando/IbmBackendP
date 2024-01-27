using Algar.Hours.Application.DataBase.User.Commands.CreateUser;
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
                    body = "Su reporte se encuentra en estado: \"aprobado, rechazado, pendiente\", por favor comuníquese con el respectivo aprobador para dar gestión"; break;
                case "2":
                    body = "Ha llegado un reporte, por favor tome acciones \"aprobar, rechazar\""; break;
                case "3":
                    body = "Su reporte ha sido rechazado, por favor comuníquese con su aprobador para obtener más información"; break;
                case "4":
                    body = "El reporte ha sido rechazado correctamente, por favor comuníquese con la persona a la cual se le rechazo el registro"; break;
                case "5":
                    body = "Su reporte ha sido aprobado, por favor comuníquese con su aprobador para obtener más información"; break;
                case "6":
                    body = "Su reporte ha sido aprobado, por favor comuníquese con su aprobador para obtener más información"; break;
                case "7":
                    body = "Sus límites legales han sido modificados"; break;
                case "8":
                    body = "Su reporte de horas no puede ser procesado, debido a que no se ha registrado un horario, por favor comuníquese con su aprobador para obtener más información"; break;
                case "9":
                    body = "Su reporte de horas no puede ser procesado, debido a que los conceptos:  \"Vacations\", \"Absence\", \"Holiday\", \"Stand By\" no aplican, por favor comuníquese con su aprobador para obtener más información"; break;
                case "10":
                    body = "Su reporte de horas no puede ser procesado, debido a que se han encontrado duplicidades de reporte de horas en diferentes áreas de negocio (ARP, TSE o STE), por favor comuníquese con su aprobador para obtener más información"; break;
                case "11": 
                    body="Se han detectado usuarios sin correo registrado en el portal TLS. Favor de solicitar su alta antes de procesarlos";break;
                case "12":
                    body = "Su reporte no puede ser procesado, debido a que no se encuentran datos en la pestaña de hora inicio y/o hora fin, por favor comuníquese con su aprobador para obtener más información"; break;
                case "13":
                    body = "Su reporte no puede ser procesado, debido a que usted excedió los límites legales para overtime o stand by, por favor, comuníquese con su coordinador para buscar una excepción gerencial o en su defecto confirme que la información esté correctamente registrada"; break;
                case "14":
                    body = "Debido a que las horas reportadas se encuentran dentro de su horario laboral su reporte no podrá ser procesado, por favor corrija su reporte y vuelva a intentarlo."; break;
                default:
                    break;
            }
            return body;

        }

        public  string GetSubject(EmailModel model)
        {
            string subject = "";
            switch (model.Plantilla)
            {
                case "1":
                    subject = "ALERTA \"PORTAL TLS\" APROBACION DE HORAS"; break;
                case "2":
                    subject = "ALERTA \"PORTAL TLS \" Su registro fue \"aprobado, rechazado\""; break;
                case "3":
                    subject = "ALERTA \"PORTAL TLS \" Su registro fue \"rechazado\""; break;
                case "4":
                    subject = "ALERTA \"PORTAL TLS\" El registro fue rechazado correctamente"; break;
                case "5":
                    subject = "ALERTA \"PORTAL TLS \" Su registro fue \"aprobado\""; break;
                case "6":
                    subject = "ALERTA \"PORTAL TLS \" Su horario ha sido modificado"; break;
                case "7":
                    subject = "ALERTA \"PORTAL TLS \" Sus límites legales han sido modificados"; break;
                case "8":
                    subject = "ALERTA \"PORTAL TLS \" NO APLICA POR HORARIO"; break;
                case "9":
                    subject = "ALERTA \"PORTAL TLS \" NO APLICA POR OVERTIME"; break;
                case "10":
                    subject = "ALERTA \"PORTAL TLS \" NO APLICA POR OVERLAPING"; break;
                case "11":
                    subject = "ALERTA \"PORTAL TLS \" USUARIOS SIN EMAIL EN SISTEMA TLS DURANTE EL PROCESO DE CARGA (ARP, TSE O STE"; break;
                case "12":
                    subject = "ALERTA \"PORTAL TLS \" NO APLICA SU REPORTE POR FALTA DE DATOS (HORA INICIO/ HORA FIN)"; break;
                case "13":
                    subject = "ALERTA \"PORTAL TLS \"NO APLICA SU REPORTE POR EXCESO DE LÍMITES LEGALES DE TRABAJO"; break;
                case "14":
                    subject = "ALERTA \"PORTAL TLS \"NO APLICA SU REPORTE POR QUE SUS HORAS REPORTADAS SE ENCUENTRAN DENTRO DEL HORARIO LABORAL"; break;
                default:
                    break;
            }
            return subject;

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
                model.To = "santiagoael@algartech.com";
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
