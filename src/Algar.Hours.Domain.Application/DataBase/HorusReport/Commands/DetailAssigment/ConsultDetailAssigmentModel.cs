﻿using Algar.Hours.Domain.Entities.Client;
using Algar.Hours.Domain.Entities.Country;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.HorusReport.Commands.DetailAssigment
{
    public class ConsultDetailAssigmentModel
    {
        public int Numeroreporte { get; set; }
        public string Horas  { get; set; }
        public ClientEntity ClientEntity { get; set; }
        public DateTime Fechaenvio { get; set; }
        public int Actividad { get; set; }
        public string Aprobaador1 { get; set; }
        public string Aprobaador2 { get; set; }
        public int EstadoAprobadorNIvel1 { get; set; }
        public int EstadoAprobadorNIvel2 { get; set; }
        public string Observaciones { get; set; }
        public CountryEntity Pais { get; set; }

    }
}
