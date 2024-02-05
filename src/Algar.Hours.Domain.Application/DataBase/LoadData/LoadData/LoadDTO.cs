using Algar.Hours.Domain.Entities.ParametrosInicial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Algar.Hours.Application.DataBase.LoadData.LoadData
{
    public class LoadDTO
    {
        public JsonArray Data { get; set; }
        public JsonArray Data2 { get; set; }
        public JsonArray Data3 { get; set; }

    }

    public class LoadJsonPais
    {
        public JsonArray Data { get; set; }
        public string PaisSel { get; set; }
        public string IdCarga { get; set; }

    }
    public class LoadGenericDTO
    {
        public JsonArray Data { get; set; }

    }
    

    public class SummaryLoad {

        public string Mensaje { get; set; }
        public string NO_APLICA_X_HORARIO_ARP { get; set; }
        public string NO_APLICA_X_OVERTIME_ARP { get; set; }
        public string NO_APLICA_X_OVERLAPING_ARP { get; set; }
        public string EN_PROCESO_ARP { get; set; }
        public string NO_APLICA_X_HORARIO_TSE { get; set; }
        public string NO_APLICA_X_OVERTIME_TSE { get; set; }
        public string NO_APLICA_X_OVERLAPING_TSE { get; set; }
        public string EN_PROCESO_TSE { get; set; }
        public string NO_APLICA_X_HORARIO_STE { get; set; }
        public string NO_APLICA_X_OVERTIME_STE { get; set; }
        public string NO_APLICA_X_OVERLAPING_STE { get; set; }
        public string EN_PROCESO_STE { get; set; }
        public string IdCarga { get; set; }
        public string ARP_CARGA { get; set; }
        public string TSE_CARGA { get; set; }
        public string STE_CARGA { get; set; }
        public string ARPOmitidosXDuplicidad { get; set; }
        public string TSEOmitidosXDuplicidad { get; set; }
        public string STEOmitidosXDuplicidad { get; set; }

        public string ARPXDatosNovalidos { get; set; }
        public string TSEXDatosNovalidos { get; set; }
        public string STEXDatosNovalidos { get; set; }
        
    }

    public class CountsCarga
    {
        public int arp { get; set; }
        public int tse { get; set; }
        public int ste { get; set; }
        public int total { get; set; }
        public int estadoCarga { get; set; }
        
    }
    public class SummaryPortalDB
    {

        public string Mensaje { get; set; }

        public string REGISTROS_PORTALDB { get; set; }
        public string NO_APLICA_X_OVERLAPING_ARP { get; set; }
        public string NO_APLICA_X_OVERLAPING_TSE { get; set; }
        public string NO_APLICA_X_OVERLAPING_STE { get; set; }

    }

    public class Loadnotificaciones
    {

        public string IdCarga { get; set; }

    }

    public class Carga
    {

        public string IdCarga { get; set; }

    }

    
}
