using Algar.Hours.Application.DataBase.Festivos.Consult;
using Algar.Hours.Application.DataBase.HorusReport.Commands;
using Algar.Hours.Application.DataBase.User.Commands.CreateUser;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.Template.Commands.Consult
{
    public class ConsultTemplateCommand : IConsultTemplateCommand
    {
        private readonly IDataBaseService _dataBaseService;
        private readonly IMapper _mapper;

        public ConsultTemplateCommand(IDataBaseService dataBaseService, IMapper mapper)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;
        }

        public async Task<TemplateModel> Consult(Guid id)
        {
            var data = _dataBaseService.TemplateEntity.Where(d => d.IdTemplate == id).FirstOrDefault();
            var entity = _mapper.Map<TemplateModel>(data);
            return entity;
        }

        public async Task<List<TemplateModel>> List()
        {
            var entity = await _dataBaseService.TemplateEntity
                .ToListAsync();
            var model = _mapper.Map<List<TemplateModel>>(entity);
            return model;
        }
    }
}
