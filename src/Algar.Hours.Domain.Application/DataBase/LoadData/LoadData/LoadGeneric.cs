using Algar.Hours.Domain.Entities.Load;
using Algar.Hours.Domain.Entities.ParametrosInicial;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Configuration;
using System.Text.Json.Nodes;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace Algar.Hours.Application.DataBase.LoadData.LoadData
{
   

    public class LoadGeneric : ILoadGeneric
    {

        private readonly IDataBaseService _dataBaseService1;
        private readonly IDataBaseService _dataBaseService2;

        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
   

        public LoadGeneric(IDataBaseService dataBaseService, IMapper mapper, IConfiguration configuration)
        {
            _dataBaseService1 = dataBaseService;
            _dataBaseService2 = dataBaseService;

            _mapper = mapper;
            _configuration = configuration;


        }

        public async Task<bool> UploadHorizontalARP1(JsonArray partition)
        {

            //Serializa la carga
            List<ParametersArpInitialEntity> datos = 
                Newtonsoft.Json.JsonConvert.DeserializeObject<List<ParametersArpInitialEntity>>(partition.ToJsonString());




            _dataBaseService1.ParametersArpInitialEntity.AddRange(datos);
            await _dataBaseService1.SaveAsync();
            return true;
        }

       

        public async Task<bool> UploadHorizontalARP2(JsonArray partition)
        {
            //Serializa la carga
            List<ParametersArpInitialEntity> datos =
                Newtonsoft.Json.JsonConvert.DeserializeObject<List<ParametersArpInitialEntity>>(partition.ToJsonString());




            _dataBaseService2.ParametersArpInitialEntity.AddRange(datos);
            await _dataBaseService2.SaveAsync();
            return true;
        }




        public async Task<bool> UploadHorizontalTSE1(JsonArray partition)
        {

            //Serializa la carga
            List<ParametersTseInitialEntity> datos =
                Newtonsoft.Json.JsonConvert.DeserializeObject<List<ParametersTseInitialEntity>>(partition.ToJsonString());




            _dataBaseService1.ParametersTseInitialEntity.AddRange(datos);
            await _dataBaseService1.SaveAsync();
            return true;
        }



        public async Task<bool> UploadHorizontalTSE2(JsonArray partition)
        {

            //Serializa la carga
            List<ParametersTseInitialEntity> datos =
                Newtonsoft.Json.JsonConvert.DeserializeObject<List<ParametersTseInitialEntity>>(partition.ToJsonString());




            _dataBaseService1.ParametersTseInitialEntity.AddRange(datos);
            await _dataBaseService1.SaveAsync();
            return true;
        }


        public async Task<bool> UploadHorizontalSTE1(JsonArray partition)
        {

            //Serializa la carga
            List<ParametersSteInitialEntity> datos =
                Newtonsoft.Json.JsonConvert.DeserializeObject<List<ParametersSteInitialEntity>>(partition.ToJsonString());




            _dataBaseService1.ParametersSteInitialEntity.AddRange(datos);
            await _dataBaseService1.SaveAsync();
            return true;
        }

        public async Task<bool> UploadHorizontalSTE2(JsonArray partition)
        {

            //Serializa la carga
            List<ParametersSteInitialEntity> datos =
                Newtonsoft.Json.JsonConvert.DeserializeObject<List<ParametersSteInitialEntity>>(partition.ToJsonString());




            _dataBaseService1.ParametersSteInitialEntity.AddRange(datos);
            await _dataBaseService1.SaveAsync();
            return true;
        }


    }
}
