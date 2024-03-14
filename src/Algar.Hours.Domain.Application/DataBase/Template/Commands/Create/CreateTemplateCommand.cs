using Algar.Hours.Application.DataBase.HorusReport.Commands;
using Algar.Hours.Application.DataBase.ReportException.Commands.Consult;
using Algar.Hours.Domain.Entities.Template;
using Algar.Hours.Domain.Entities.WorkdayException;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.Template.Commands.Create
{
    public class CreateTemplateCommand : ICreateTemplateCommand
    {
        private readonly IDataBaseService _dataBaseService;
        private readonly IMapper _mapper;

        public CreateTemplateCommand(IDataBaseService dataBaseService, IMapper mapper)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;
        }

        public async Task<TemplateEntity> Execute(TemplateModelC createTemplate)
        {
            var template = _mapper.Map<TemplateEntity>(createTemplate);
            template.IdTemplate = Guid.NewGuid();
            await _dataBaseService.TemplateEntity.AddAsync(template);

            await _dataBaseService.SaveAsync();

            return template;
        }
    }
}
