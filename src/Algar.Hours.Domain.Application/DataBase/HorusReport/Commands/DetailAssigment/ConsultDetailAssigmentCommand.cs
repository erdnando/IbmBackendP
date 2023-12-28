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
            List<ConsultDetailAssigmentModel> ListconsultDetailAssigmentModels = new List<ConsultDetailAssigmentModel>();

            var aprobdaornivel1 = "";
            var aprobadornivel2 = "";
            int estadoaprovadornivel1 = 0;
            int estadoaprovadornivel2 = 0;
;
            var n = _dataBaseService.assignmentReports
                .Include(a => a.UserEntity)
                .Include(b=> b.HorusReportEntity)
                .Where(x => x.HorusReportEntityId == IdReport).ToList();

            foreach (var ns in n)
            {
               ConsultDetailAssigmentModel consultDetailAssigmentModel = new ConsultDetailAssigmentModel();


               var aprobador = _dataBaseService.AprobadorUsuario
                    .Include(a => a.UserEntity)
                    .Include(a => a.Aprobador)
                    .Where(x => x.UserEntityId == ns.UserEntityId).FirstOrDefault();

                if(aprobador != null) 
                {
                    if(aprobador.Aprobador.Nivel == 1) 
                    {
                        aprobdaornivel1 = aprobador.UserEntity.NameUser;
                        estadoaprovadornivel1 = ns.State;
                    }
                    else if(aprobador.Aprobador.Nivel == 2) 
                    {
                        aprobadornivel2 = aprobador.UserEntity.NameUser;
                        estadoaprovadornivel2 = ns.State;  
                    }

                }

                var cliente =  _dataBaseService.ClientEntity.Where(x => x.IdClient == ns.HorusReportEntity.ClientEntityId).FirstOrDefault();
                var pais = _dataBaseService.CountryEntity.Where(x => x.IdCounty == ns.UserEntity.CountryEntityId).FirstOrDefault();

                consultDetailAssigmentModel.Numeroreporte = ns.HorusReportEntity.NumberReport;
                consultDetailAssigmentModel.Horas = ns.HorusReportEntity.CountHours;
                consultDetailAssigmentModel.ClientEntity = cliente;
                consultDetailAssigmentModel.Fechaenvio = ns.HorusReportEntity.CreationDate;
                consultDetailAssigmentModel.Actividad = ns.HorusReportEntity.Acitivity;
                consultDetailAssigmentModel.Aprobaador1 = aprobdaornivel1;
                consultDetailAssigmentModel.EstadoAprobadorNIvel1 = estadoaprovadornivel1;
                consultDetailAssigmentModel.Aprobaador2 = aprobadornivel2;
                consultDetailAssigmentModel.EstadoAprobadorNIvel2 = estadoaprovadornivel2;
                consultDetailAssigmentModel.Observaciones = ns.Description;
                consultDetailAssigmentModel.Pais = pais;

                ListconsultDetailAssigmentModels.Add(consultDetailAssigmentModel);

            }

            return  ListconsultDetailAssigmentModels;

        }
    }
}
