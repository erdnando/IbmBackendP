﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.AssignmentReport.Commands.UpdateAproveedNivel1
{
    public class ModelAproveed
    {

        public Guid HorusReportEntityId { get; set; }
        public int State { get; set; }
        public string Description { get; set; }
        public Guid UserId { get; set; }


    }
}
