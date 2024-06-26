﻿using Algar.Hours.Application.DataBase.HorusReport.Commands;
using Algar.Hours.Domain.Entities.Load;
using Algar.Hours.Domain.Entities.ParametrosInicial;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.LoadData.LoadData
{
    public interface ILoadHoursReport
    {
        Task<ResponseData<string>> GeneraCarga();
        Task<ResponseData<ARPLoadEntity>> CancelarCarga(string idCarga);
         
        Task<List<ARPLoadEntity>> List();
        Task<List<ParametersArpInitialEntity>> ArpParametersList(Guid idLoad);
        Task<List<ParametersTseInitialEntity>> TseParametersList(Guid idLoad);
        Task<List<ParametersSteInitialEntity>> SteParametersList(Guid idLoad);
        Task<ARPLoadEntity> Consult(Guid id);
        Task<string> LoadARP(LoadJsonPais model);
        Task<string> LoadUserGMT(LoadJsonUserGMT model);
        Task<string> LoadTSE(LoadJsonPais model);
        Task<SummaryLoad> LoadSTE(LoadJsonPais model);
        Task<bool> NotificacionesProceso1(string model,string idUserEntiyId);
        Task<SummaryPortalDB> ValidaLimitesExcepcionesOverlapping(string idCarga);
        Task<CountsCarga> CargaAvance(string idCarga);
        Task<List<InconsistenceModel>> GetInconsistences(string? idCarga, string? employeeCode);
        Task<FileStreamResult> GenerateInconsistencesFile(string? idCarga, string? employeeCode);

    }



}
