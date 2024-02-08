using Algar.Hours.Application.DataBase.HorusReport.Commands.Create;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
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

            var aprobdaornivel1 = "";
            var aprobadornivel2 = "";
            int estadoaprovadornivel1 = 0;
            int estadoaprovadornivel2 = 0;
            
            //obtiene ref a la list de assignments asociadas al reporte (horusreport)
            var assignmentList = _dataBaseService.assignmentReports
                .Include(a => a.UserEntity)
                .Include(b=> b.HorusReportEntity)
                .Where(x => x.HorusReportEntityId == IdReport).ToList();

           
            //Recorriendo los assignments encontrados
            foreach (var assig in assignmentList)
            {
                var detailAssigment = new ConsultDetailAssigmentModel();

                var aprobador = _dataBaseService.AprobadorUsuario
                        .Include(a => a.UserEntity)
                        .Include(a => a.Aprobador)
                        .Where(x => x.UserEntityId == assig.UserEntityId).FirstOrDefault();

                if (aprobador == null) continue;

                if(aprobador.Aprobador.Nivel == 1) 
                {
                    aprobdaornivel1 = aprobador.UserEntity.NameUser;
                    estadoaprovadornivel1 = assig.State;
                }
                else if(aprobador.Aprobador.Nivel == 2) 
                {
                    aprobadornivel2 = aprobador.UserEntity.NameUser;
                    estadoaprovadornivel2 = assig.State;  
                }



                var cliente = _dataBaseService.ClientEntity.Where(x => x.IdClient == assig.HorusReportEntity.ClientEntityId).FirstOrDefault();
                var pais = _dataBaseService.CountryEntity.Where(x => x.IdCounty == assig.UserEntity.CountryEntityId).FirstOrDefault();

                detailAssigment.Numeroreporte = assig.HorusReportEntity.NumberReport;
                detailAssigment.strNumeroreporte = assig.HorusReportEntity.StrReport;
                detailAssigment.Horas = assig.HorusReportEntity.CountHours;
                detailAssigment.ClientEntity = cliente;
                detailAssigment.Fechaenvio = assig.HorusReportEntity.CreationDate;
                detailAssigment.Actividad = assig.HorusReportEntity.Acitivity;
                detailAssigment.Aprobaador1 = aprobdaornivel1;
                detailAssigment.EstadoAprobadorNIvel1 = estadoaprovadornivel1;
                detailAssigment.Aprobaador2 = aprobadornivel2;
                detailAssigment.EstadoAprobadorNIvel2 = estadoaprovadornivel2;
                detailAssigment.Observaciones = assig.Description;
                detailAssigment.Pais = pais;

                ListconsultDetailAssigment.Add(detailAssigment);
                
                
            }

            return ListconsultDetailAssigment;

        }
    }
}
