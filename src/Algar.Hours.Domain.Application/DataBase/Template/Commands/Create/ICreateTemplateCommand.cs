using Algar.Hours.Domain.Entities.Template;
using Algar.Hours.Domain.Entities.WorkdayException;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.Template.Commands.Create
{
    public interface ICreateTemplateCommand
    {
        Task<TemplateEntity> Execute(TemplateModelC createTemplate);
    }
}
