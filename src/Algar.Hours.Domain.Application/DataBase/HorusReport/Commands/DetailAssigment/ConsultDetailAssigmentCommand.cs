using Algar.Hours.Application.DataBase.HorusReport.Commands.Create;
using Algar.Hours.Domain.Entities.AssignmentReport;
using AutoMapper;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyModel;
using NetTopologySuite.Index.HPRtree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.HorusReport.Commands.DetailAssigment
{
    public class ConsultDetailAssigmentCommand : IConsultDetailAssigmentCommand
    {
        private readonly IDataBaseService _dataBaseService;
        private readonly IMapper _mapper;

        public ConsultDetailAssigmentCommand(IDataBaseService dataBaseService, IMapper mapper)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;
        }
        public async Task<List<ConsultDetailAssigmentModel>> Execute(Guid IdReport)
        {
            var ListconsultDetailAssigment = new List<ConsultDetailAssigmentModel>();

            var empleado = "";
            var aprobdaornivel1 = "";
            var aprobadornivel2 = "";
            var observaciones = "";
            int estadoaprovadornivel1 = 0;
            int estadoaprovadornivel2 = 0;

            var currentHReport = _dataBaseService.HorusReportEntity
                    .Include(a => a.UserEntity)
                    .Include(a => a.ClientEntity)
                    .Where(x => x.IdHorusReport == IdReport).FirstOrDefault();

            //obtiene ref a la list de assignments asociadas al reporte (horusreport)
            var assignmentQuery = _dataBaseService.assignmentReports
                .Include(a => a.UserEntity.RoleEntity)
                .Include(b => b.HorusReportEntity)
                .Where(x => x.HorusReportEntityId == IdReport)
                .AsEnumerable()
                .OrderBy(x => DateTime.ParseExact(x.strFechaAtencion, "dd/MM/yyyy HH:mm", null));

            var assignmentList = assignmentQuery.ToList();
            var assignments = new List<Domain.Entities.AssignmentReport.AssignmentReport>();

            //Recorriendo los assignments encontrados
            foreach (var assig in assignmentList)
            {
                if (currentHReport.EstatusOrigen == "STANDBY" && assig.UserEntity.RoleEntity.IdRole == Guid.Parse("5a0ab2a2-f790-4f96-9dee-da0b9111f7c7")) continue;
                assignments.Add(assig);

                    if (assig.Nivel == 0)
                {
                    empleado = assig.UserEntity.NameUser +" "+ assig.UserEntity.surnameUser;
                }else if (assig.Nivel == 1)
                {
                    aprobdaornivel1 = assig.UserEntity.NameUser + " " + assig.UserEntity.surnameUser;
                }
                else if (assig.Nivel == 2)
                {
                    aprobadornivel2 = assig.UserEntity.NameUser + " " + assig.UserEntity.surnameUser;
                    observaciones = assig.Description;
                }


                
            }

            var detailAssigment = new ConsultDetailAssigmentModel();
            var cliente = _dataBaseService.ClientEntity.Where(x => x.IdClient == currentHReport.ClientEntityId).FirstOrDefault();
            var pais = _dataBaseService.CountryEntity.Where(x => x.IdCounty == currentHReport.UserEntity.CountryEntityId).FirstOrDefault();

            detailAssigment.Numeroreporte = currentHReport.NumberReport;
            detailAssigment.strNumeroreporte = currentHReport.StrReport;
            detailAssigment.Horas = currentHReport.CountHours;
            detailAssigment.ClientEntity = cliente;
            detailAssigment.UserEntity = currentHReport.UserEntity;
            detailAssigment.Fechaenvio = currentHReport.strCreationDate;
            detailAssigment.Actividad = currentHReport.Acitivity;
            detailAssigment.Pais = pais;
            detailAssigment.Aprobaador1 = aprobdaornivel1;
            detailAssigment.Aprobaador2 = aprobadornivel2;
            detailAssigment.Assignments = assignments;
            detailAssigment.EstadoReporte = currentHReport.Estado;
            
            detailAssigment.Observaciones = currentHReport.Estado==2||currentHReport.Estado==3?observaciones:"";//observacion final
            


            ListconsultDetailAssigment.Add(detailAssigment);




            return ListconsultDetailAssigment;

        }
    }
}
