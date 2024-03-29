﻿using Algar.Hours.Domain.Entities.Client;
using Algar.Hours.Domain.Entities.User;


namespace Algar.Hours.Domain.Entities.HorusReport
{
    public class HorusReportEntity
    {
        public Guid IdHorusReport { get; set; }
        public Guid UserEntityId { get; set; }
        //public DateTime StartDate { get; set; }
        public string? StrStartDate { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public ClientEntity ClientEntity { get; set; }
        //public DateTime CreationDate { get; set; }
        public string strCreationDate { get; set; }
        //public int TipoReporte { get; set; }
        public string CountHours { get; set; }
        public string StrReport { get; set; }
        public string? ARPLoadingId { get; set; }

        public UserEntity UserEntity { get; set; }
        
        public Guid? ClientEntityId { get; set; }
        public int Acitivity { get; set; }
        public DateTime DateApprovalSystem { get; set; }
        public int NumberReport { get; set; }
        public int Estado { get; set; }
        public string EstatusOrigen { get; set; }
        public string EstatusFinal { get; set; }
        public string DetalleEstatusFinal { get; set; }
        public string Origen { get; set; }
        public string Semana { get; set; }

    }
}
