using Algar.Hours.Application.DataBase.Aprobador.Commands.Consult;
using Algar.Hours.Application.DataBase.Aprobador.Commands.Create;
using Algar.Hours.Domain.Entities.Aprobador;
using Algar.Hours.Domain.Entities.HorusReport;
using Algar.Hours.Domain.Entities.User;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.Data.MySqlClient.Replication;
using System.Diagnostics;
using System;
using System.Globalization;

namespace Algar.Hours.Application.DataBase.Dashboard.Commands.Consult
{

    public class Reporte1Command : IReporte1Command
    {
        private readonly IDataBaseService _dataBaseService;
        private readonly IMapper _mapper;

        public Reporte1Command(IDataBaseService dataBaseService, IMapper mapper)
        {
            _dataBaseService = dataBaseService;
            _mapper = mapper;
        }

        public async Task<GralReportesHrsTSO> Reporte1(int semana, string usuario, int anio)
        {
            GralReportesHrsTSO GralReportes = new();
            GralReportHoras GralReportesHoras = new();
            List<ReporteHorasTLS> reporteHorasTLS = new();

            var firstDate = FirstDateOfWeek(anio, semana);
            var allWeekDays = new List<DateTime>();
            allWeekDays.Add(firstDate);
            var currentDate = firstDate;
            for (int d = 1; d < 7; d++)
            {
                currentDate = currentDate.AddDays(1);
                allWeekDays.Add(currentDate);
            }



            var arp = await (from p in _dataBaseService.ParametersArpInitialEntity
                            // join a in _dataBaseService.ARPLoadDetailEntity
                            // on p.ARPLoadDetailEntityId equals a.IdDetail
                             where (p.Semana == semana && p.EmployeeCode == usuario)
                             select new
                             {
                                 fechaRep = p.FECHA_REP,
                                 totalMinutos = p.TOTAL_MINUTOS
                             }).Distinct().ToListAsync();

            var StandByRep = await (from us in _dataBaseService.UserEntity
                                    join hor in _dataBaseService.HorusReportEntity on us.IdUser equals hor.UserEntityId
                                    where (us.EmployeeCode == usuario && hor.Acitivity == 1)
                                    select new
                                    {
                                        fechaRep = hor.DateApprovalSystem,
                                        totalHoras = hor.CountHours
                                    }
                              ).Distinct().ToListAsync();

            var OverTimeRep = await (from us in _dataBaseService.UserEntity
                                     join hor in _dataBaseService.HorusReportEntity on us.IdUser equals hor.UserEntityId
                                     where (us.EmployeeCode == usuario && hor.Acitivity == 0)
                                     select new
                                     {
                                         fechaRep = hor.DateApprovalSystem,
                                         totalHoras = hor.CountHours
                                     }
            ).Distinct().ToListAsync();


            var StandByRepGraf = await (from us in _dataBaseService.UserEntity
                                        join hor in _dataBaseService.HorusReportEntity on us.IdUser equals hor.UserEntityId
                                        where (us.EmployeeCode == usuario && hor.Acitivity == 1)
                                        select new
                                        {
                                            Mes = hor.DateApprovalSystem.Month,
                                            totalHoras = hor.CountHours
                                        }
                              ).Distinct().ToListAsync();

            var OverTimeRepGraf = await (from us in _dataBaseService.UserEntity
                                         join hor in _dataBaseService.HorusReportEntity on us.IdUser equals hor.UserEntityId
                                         where (us.EmployeeCode == usuario && hor.Acitivity == 0)
                                         select new
                                         {
                                             Mes = hor.DateApprovalSystem.Month,
                                             totalHoras = hor.CountHours
                                         }
                              ).Distinct().ToListAsync();

            var tse = await (from p in _dataBaseService.ParametersTseInitialEntity
                            // join a in _dataBaseService.TSELoadEntity
                            // on p.TSELoadEntityIdTSELoad equals a.IdTSELoad
                             where (p.Semana == semana && p.EmployeeCode == usuario)
                             select new
                             {
                                 fechaRep = p.FECHA_REP,
                                 totalMinutos = p.TOTAL_MINUTOS
                             }).Distinct().ToListAsync();

            var ste = await (from p in _dataBaseService.ParametersSteInitialEntity
                            // join a in _dataBaseService.STELoadEntity
                            // on p.STELoadEntityId equals a.IdSTELoad
                             where (p.Semana == semana && p.EmployeeCode == usuario)
                             select new
                             {
                                 fechaRep = p.FECHA_REP,
                                 totalMinutos = p.TOTAL_MINUTOS
                             }).Distinct().ToListAsync();


            ReporteHorasTLS repHorarp = new()
            {
                Tool = "ARP",
            };
            repHorarp.weekDaysTls = new();


            ReporteHorasTLS repHoraTSE = new()
            {
                Tool = "TSE",
            };
            repHoraTSE.weekDaysTls = new();

            ReporteHorasTLS repHoraSTE = new()
            {
                Tool = "STE",
            };
            repHoraSTE.weekDaysTls = new();

            ReporteHorasTLS repHorStanBy = new()
            {
                Tool = "StandBy",
            };
            repHorStanBy.weekDaysTls = new();

            ReporteHorasTLS repHorOverTime = new()
            {
                Tool = "OverTime",
            };
            repHorOverTime.weekDaysTls = new();





            foreach (var a in allWeekDays)
            {
                /* var restARP = arp.Where(op => op.fechaRep == ($"{a.Day.ToString("00")}/{a.Month.ToString("00")}/{a.Year}"));
                 var restStanBy = StandByRep.Where(op => op.fechaRep.Date == DateTime.Parse(($"{a.Day.ToString("00")}/{a.Month.ToString("00")}/{a.Year}")));
                 var restOverTime = OverTimeRep.Where(op => op.fechaRep.Date == DateTime.Parse(($"{a.Day.ToString("00")}/{a.Month.ToString("00")}/{a.Year}")));
                 var restTSE = tse.Where(op => op.fechaRep == ($"{a.Day.ToString("00")}/{a.Month.ToString("00")}/{a.Year}"));
                 var restSTE = ste.Where(op => op.fechaRep == ($"{a.Day.ToString("00")}/{a.Month.ToString("00")}/{a.Year}"));*/

                var restARP = arp.Where(op => op.fechaRep == ($"{a.Year}/{a.Month.ToString("00")}/{a.Day.ToString("00")}"));
                var restStanBy = StandByRep.Where(op => op.fechaRep.Date == DateTime.Parse(($"{a.Year}/{a.Month.ToString("00")}/{a.Day.ToString("00")}")));
                var restOverTime = OverTimeRep.Where(op => op.fechaRep.Date == DateTime.Parse(($"{a.Year}/{a.Month.ToString("00")}/{a.Day.ToString("00")}")));
                var restTSE = tse.Where(op => op.fechaRep == ($"{a.Year}/{a.Month.ToString("00")}/{a.Day.ToString("00")}"));
                var restSTE = ste.Where(op => op.fechaRep == ($"{a.Year}/{a.Month.ToString("00")}/{a.Day.ToString("00")}"));

                var SumaMinutos = 0.0;
                var SumaHoras = 0.0;
                foreach (var itemrest in restARP)
                {
                    SumaMinutos += double.Parse(itemrest.totalMinutos);
                }
                WeekDaysTls weekDaysTls = new()
                {
                    Fecha = $"{a.Year}/{a.Month.ToString("00")}/{a.Day.ToString("00")}",
                    TotalHoras = SumaMinutos / 60
                };
                SumaMinutos = 0.0;
                repHorarp.weekDaysTls.Add(weekDaysTls);

                foreach (var itemStanBy in restStanBy)
                {
                    SumaHoras += double.Parse(itemStanBy.totalHoras);
                }
                weekDaysTls = new()
                {
                    Fecha = $"{a.Year}/{a.Month.ToString("00")}/{a.Day.ToString("00")}",
                    TotalHoras = SumaHoras
                };
                SumaHoras = 0.0;
                repHorStanBy.weekDaysTls.Add(weekDaysTls);

                foreach (var itemOverTime in restOverTime)
                {
                    SumaHoras += double.Parse(itemOverTime.totalHoras);
                }
                weekDaysTls = new()
                {
                    Fecha = $"{a.Year}/{a.Month.ToString("00")}/{a.Day.ToString("00")}",
                    TotalHoras = SumaHoras
                };
                SumaHoras = 0.0;
                repHorOverTime.weekDaysTls.Add(weekDaysTls);

                foreach (var itemTSE in restTSE)
                {
                    SumaMinutos += double.Parse(itemTSE.totalMinutos);
                }
                weekDaysTls = new()
                {
                    Fecha = $"{a.Year}/{a.Month.ToString("00")}/{a.Day.ToString("00")}",
                    TotalHoras = SumaMinutos
                };
                SumaMinutos = 0.0;
                repHoraTSE.weekDaysTls.Add(weekDaysTls);


                foreach (var itemSTE in restSTE)
                {
                    SumaMinutos += double.Parse(itemSTE.totalMinutos);
                }
                weekDaysTls = new()
                {
                    Fecha = $"{a.Year}/{a.Month.ToString("00")}/{a.Day.ToString("00")}",
                    TotalHoras = SumaMinutos
                };
                SumaMinutos = 0.0;
                repHoraSTE.weekDaysTls.Add(weekDaysTls);

                SumaMinutos = 0.0;
                SumaHoras = 0.0;

            }

            reporteHorasTLS.Add(repHorarp);
            reporteHorasTLS.Add(repHoraTSE);
            reporteHorasTLS.Add(repHoraSTE);
            GralReportesHoras.ReporteName = "TLS";
            GralReportesHoras.ReportesTLS = new(reporteHorasTLS);
            GralReportes.ReposterGral = new() { GralReportesHoras };

            reporteHorasTLS = [repHorStanBy];

            GralReportesHoras = new();
            GralReportesHoras.ReporteName = "StandBy";
            GralReportesHoras.ReportesTLS = new(reporteHorasTLS);
            GralReportes.ReposterGral.Add(GralReportesHoras);

            reporteHorasTLS = [repHorOverTime];
            GralReportesHoras = new();
            GralReportesHoras.ReporteName = "OverTime";
            GralReportesHoras.ReportesTLS = new(reporteHorasTLS);
            GralReportes.ReposterGral.Add(GralReportesHoras);

            return GralReportes;

        }

        public async Task<GralReporteHorasMesTLS> ReporteGraficas(int anio, string usuario)
        {
            GralReporteHorasMesTLS GralReportes = new();
            GralReportes.ReportesGral = new();


            var StandByRepGraf = await (from us in _dataBaseService.UserEntity
                                        join hor in _dataBaseService.HorusReportEntity on us.IdUser equals hor.UserEntityId
                                        where (us.EmployeeCode == usuario && hor.Acitivity == 1 && hor.DateApprovalSystem.Year == anio)
                                        select new
                                        {
                                            Mes = hor.DateApprovalSystem.Month,
                                            totalHoras = hor.CountHours
                                        }
                              ).Distinct().ToListAsync();

            var OverTimeRepGraf = await (from us in _dataBaseService.UserEntity
                                         join hor in _dataBaseService.HorusReportEntity on us.IdUser equals hor.UserEntityId
                                         where (us.EmployeeCode == usuario && hor.Acitivity == 0 && hor.DateApprovalSystem.Year == anio)
                                         select new
                                         {
                                             Mes = hor.DateApprovalSystem.Month,
                                             totalHoras = hor.CountHours
                                         }
                              ).Distinct().ToListAsync();


            ReporteHorasMesTLS repHorStanBy = new()
            {
                Tool = "StandBy",
                Anio = anio
            };
            repHorStanBy.monthTls = new();

            ReporteHorasMesTLS repHorOverTime = new()
            {
                Tool = "OverTime",
                Anio = anio
            };
            repHorOverTime.monthTls = new();

            for (int i = 0; i < 12; i++)
            {
                MonthTls NewMes = new()
                {
                    Mes = i + 1,
                    TotalHoras = 0
                };
                repHorStanBy.monthTls.Add(NewMes);
                NewMes = new()
                {
                    Mes = i + 1,
                    TotalHoras = 0
                };
                repHorOverTime.monthTls.Add(NewMes);
            }

            foreach (var item in StandByRepGraf)
            {
                repHorStanBy.monthTls[item.Mes - 1].TotalHoras += double.Parse(item.totalHoras);

            }

            GralReportes.ReportesGral.Add(repHorStanBy);

            foreach (var itemOVT in OverTimeRepGraf)
            {
                repHorOverTime.monthTls[itemOVT.Mes - 1].TotalHoras += double.Parse(itemOVT.totalHoras);
            }

            GralReportes.ReportesGral.Add(repHorOverTime);
            return GralReportes;

        }

        public static DateTime FirstDateOfWeek(int year, int weekOfYear)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = Convert.ToInt32(System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek) - Convert.ToInt32(jan1.DayOfWeek);
            DateTime firstWeekDay = jan1.AddDays(daysOffset);
            System.Globalization.CultureInfo curCulture = System.Globalization.CultureInfo.CurrentCulture;
            int firstWeek = curCulture.Calendar.GetWeekOfYear(jan1, curCulture.DateTimeFormat.CalendarWeekRule, curCulture.DateTimeFormat.FirstDayOfWeek);
            if (firstWeek <= 1)
            {
                weekOfYear -= 1;
            }
            return firstWeekDay.AddDays((weekOfYear * 7) + 1);
        }
    }
}
