using Algar.Hours.Application.DataBase.User.Commands.CreateUser;
using Algar.Hours.Application.DataBase.UserSession.Commands.CreateLog;
using AutoMapper;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.User.Commands.Login
{
    public class LoginUserCommand : ILoginUserCommand
    {
        private readonly IDataBaseService _databaseService;
        private readonly IMapper _mapper;
        private ICreateLogCommand _logCommand;

        CreateLogModel _log;

        public LoginUserCommand(IDataBaseService databaseService,IMapper mapper, ICreateLogCommand logCommand) 
        { 
           _databaseService = databaseService;
           _mapper = mapper;
            _logCommand = logCommand;

        }

        public async Task<CreateUserModel> Execute(LoginUserModel model)
        {
            var entity = _databaseService.UserEntity
                .Include(e => e.CountryEntity)
                .Include(e => e.RoleEntity)
                .Where(x => x.Email.ToLower().Trim() == model.UserName.ToLower().Trim() && x.Password == model.Password).FirstOrDefault();


            var ModelUser = _mapper.Map<CreateUserModel>(entity);
            /*await daemon10();*/

            if (entity != null)
            {
                await _logCommand.Log(entity.IdUser.ToString(), "Log In al sistema", model);
            }
            else
            {
                await _logCommand.Log(model.UserName.ToString(), "Intento fallido de Log In al sistema", model);
            }


            return ModelUser;

        }

        private async Task daemon10()
        {
            try
            {

            
            var RowGralFS = _databaseService.HorusReportEntity.Where(op => (op.EstatusOrigen == "FINAL" || op.EstatusOrigen == "SUBMITTED") && op.EstatusFinal != "DESCARTADO").ToList();
            var RowGralFS10 = RowGralFS.Where(op => (
                                DateTime.ParseExact(DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture)
                                -
                                DateTime.ParseExact(op.strCreationDate.Substring(0, 10), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture)).Days > 10
                                && op.EstatusFinal != "DESCARTADO"
                                ).ToList();


            List<Domain.Entities.AssignmentReport.AssignmentReport> rowAssignments = new();
            foreach (var item10 in RowGralFS10)
            {
                item10.EstatusFinal = "DESCARTADO";
                item10.Estado= (int)Enums.Enums.AprobacionPortalDB.Descartado;
                item10.DetalleEstatusFinal = "Su reporte no ha podido ser actualizado a EXTRACTED debido a que han pasado 10 dias sin tener alguna actualizacion. Por lo cual pasa es estatus DESCARTADO. Por favor contacte a su gerente para mas informacion";
                Domain.Entities.AssignmentReport.AssignmentReport rowAddAssig = new();
                rowAddAssig = new()
                {
                    IdAssignmentReport = Guid.NewGuid(),
                    UserEntityId = new Guid("53765c41-411f-4add-9034-7debaf04f276"),
                    HorusReportEntityId = item10.IdHorusReport,
                    State = 1,
                    Description = "DESCARTADO, debido a que han pasado 10 dias sin tener alguna actualizacion ",
                    strFechaAtencion = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                    Resultado = (byte)Enums.Enums.AprobacionPortalDB.Descartado,
                    Nivel = 2
                };
                rowAssignments.Add(rowAddAssig);
                await _logCommand.Log("53765c41-411f-4add-9034-7debaf04f276", "DESCARTA REGISTRO", item10);
            }
            _databaseService.assignmentReports.AddRangeAsync(rowAssignments);
            await _databaseService.SaveAsync();
            }catch(Exception ex)
            {

            }
        }

    }
}
