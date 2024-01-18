using Algar.Hours.Application.DataBase.AssignmentReport.Commands.ListUserAproveed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.AssignmentReport.Commands.UpdateAproveedNivel1
{
    public interface IUpdateAproveedCommand
    {
        Task<ModelAproveed> Execute(ModelAproveed modelAprobador);
        CreateAssignmentReportModel CrearNivel2(CreateAssignmentReportModel model);
    }
}