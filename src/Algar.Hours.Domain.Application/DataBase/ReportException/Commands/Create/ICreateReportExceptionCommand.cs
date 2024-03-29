﻿using Algar.Hours.Domain.Entities.ReportException;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.ReportException.Commands.Create
{
    public interface ICreateReportExceptionCommand
    {
        Task<ReportExceptionEntity> Execute(ReportExceptionModelC createReportException);
    }
}
