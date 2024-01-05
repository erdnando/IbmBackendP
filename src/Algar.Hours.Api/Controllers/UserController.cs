using Algar.Hours.Application.DataBase.Aprobador.Commands.Consult;
using Algar.Hours.Application.DataBase.User.Commands.Consult;
using Algar.Hours.Application.DataBase.User.Commands.CreateUser;
using Algar.Hours.Application.DataBase.User.Commands.Login;
using Algar.Hours.Application.DataBase.User.Commands.Update;
using Algar.Hours.Application.Exceptions;
using Algar.Hours.Application.Feature;
using Microsoft.AspNetCore.Mvc;
using Saml;
using Microsoft.Extensions.Logging;
using System.Xml.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using Algar.Hours.Domain.Entities.Country;
using Algar.Hours.Application.DataBase.Country.Commands.Consult;


namespace Algar.Hours.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [TypeFilter(typeof(ExceptionManager))]

    public class UserController : ControllerBase
    {
        private IGetListUsuarioCommand _getListUsuarioCommand;
        private ICreateUserCommand _createUserCommand;
        private IConsultCountryCommand _consultCountryCommand;
        public UserController(IGetListUsuarioCommand getListUsuarioCommand, ICreateUserCommand createUserCommand, IConsultCountryCommand consultCountryCommand)
        {
            _getListUsuarioCommand = getListUsuarioCommand;
            _createUserCommand = createUserCommand;
            _consultCountryCommand = consultCountryCommand;
        }
        [HttpPost("create")]
        public async Task<IActionResult> Create(
           [FromBody] CreateUserModelc model, [FromServices] ICreateUserCommand createUserCommand)
        {
            var data = await createUserCommand.Execute(model);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));

        }
        [HttpGet("Consult")]
        public async Task<IActionResult> Consult(
          [FromQuery] Guid id, [FromServices] IGetListUsuarioCommand createUserCommand)
        {

            var data = await createUserCommand.Consult(id);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));

        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(
         [FromBody] LoginUserModel model, [FromServices] ILoginUserCommand loginuserCommand)
        {
            var data = await loginuserCommand.Execute(model);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));

        }

       


        [HttpPost("Callback")]
        [Produces("application/json")]
        public async Task<IActionResult> Callback()
        {



            string encodedStr = "";
            string samlCert = @"-----BEGIN CERTIFICATE-----
MIIDcTCCAligAwIBAgIBADANBgkqhkiG9w0BAQ0FADBSMQswCQYDVQQGEwJ1czEL
MAkGA1UECAwCVVMxDDAKBgNVBAoMA0lCTTEMMAoGA1UEAwwDVExTMQwwCgYDVQQH
DANJQk0xDDAKBgNVBAsMA1RMUzAeFw0yMzEyMzAyMzEwMzNaFw0yNjA5MjQyMzEw
MzNaMFIxCzAJBgNVBAYTAnVzMQswCQYDVQQIDAJVUzEMMAoGA1UECgwDSUJNMQww
CgYDVQQDDANUTFMxDDAKBgNVBAcMA0lCTTEMMAoGA1UECwwDVExTMIIBIzANBgkq
hkiG9w0BAQEFAAOCARAAMIIBCwKCAQIArIoPbCNScq8fqbLcVUCgNp3OJJUReHL4
qKiFh6xR9Ywl0a+hhDgIGrjUhJ0NTsizs/sy2NiOYrq3iLAmtdSfBMmLRxcHiAMH
v2fDhDp+SqTa0UaX8lhzNyyCI/3j7w2tl3O7uJohxuz39S2exfyDEyk8KFAGx7+C
NnsLN757C6CprvmqP8dxorAZTeS8bFY61ZXJiwudldMx22lQLPtiveuYLDnb5RQv
J3Rps5XWgedSBL5kRZQPaTjeo160QTlzzTSGxt/BJYzjFj+PQWU0w5cL1OHBmr68
iR6G+evEp5mk53P5xDpMauWdlNxUAzL1IrKwmOmVMOqqO8ChoLZGx0ECAwEAAaNQ
ME4wHQYDVR0OBBYEFLURZdq3KbgorQO3JwxETsBItVzbMB8GA1UdIwQYMBaAFLUR
Zdq3KbgorQO3JwxETsBItVzbMAwGA1UdEwQFMAMBAf8wDQYJKoZIhvcNAQENBQAD
ggECAAfRmZ4gjta7n6PuXC5BNtyhdWs3udGUZDmSF8G5TGaSZy6Zzo3jGDVcKqDC
D6pCWiI3YheYQOaO0eryVeORCRFZ4hdmuUphF4ffyWIAIZrr6ZUBUtC1xtzdokG7
IFpw9RlzCNF8wIruimdmNPZjyPkc7zVmVM1HxaMh7hylvfDXc0vpCCoUy9LqEvpG
2UwhmgeQTC76B0Dmorf6guEN+/r+cegxI8UdY2Cv68Im3F1VytkCmreP099r4bnd
e5iji31qV7XiducvqxW7POWG3vi9dPg2nhP6UGae5TgRpWJksh/uhTn/zeVOznY4
AB7XkC7atqVVYhLhRXClgxt45wme
-----END CERTIFICATE-----";


            //using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
            //////ILogger logger = factory.CreateLogger("Program");
            //logger.LogInformation(Request.Form["SAMLResponse"]);
            // 2. Let's read the data - SAML providers usually POST it into the "SAMLResponse" var
            var samlResponse = Request.Form["SAMLResponse"].ToString();
           // var samlResponse = new Response(samlCert, Request.Form["SAMLResponse"]);

           /* Console.WriteLine(samlResponse);
            try
            {
                Console.WriteLine(samlResponse.IsValid());
                Console.WriteLine(samlResponse.GetEmail());
                Console.WriteLine(samlResponse.GetFirstName());
                Console.WriteLine(samlResponse.GetUpn());
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }*/
            


            // 3. DONE!
            /*
            if (samlResponse.IsValid()) 
             {
                //WOOHOO!!! the user is logged in
                // var username = samlResponse.GetNameID(); //let's get the username
                // var officeLocation = samlResponse.GetCustomAttribute("OfficeAddress");


                CreateUserModelc restUs = new();
                var data1 = await _getListUsuarioCommand.GetByEmail(samlResponse.GetEmail());
                if (data1==null)
                {
                    CreateUserModelc newUsr = new()
                    {
                        NameUser = samlResponse.GetFirstName(),
                        Email = samlResponse.GetEmail(),
                        EmployeeCode = samlResponse.GetNameID(),
                        Password = "123456789",
                        surnameUser = samlResponse.GetLastName(),
                        RoleEntityId = new Guid("5a0ab2a2-f790-4f96-9dee-da0b9111f7c7"),//Rol Standar
                        CountryEntityId = new Guid("2028f36a-bfab-4cf4-b1f6-e2c5cc63bb31"),//Mexico

                    };
                    restUs = await _createUserCommand.Execute(newUsr);
                }

                //var dataCountry = await _consultCountryCommand.ConsultIdbyName(samlResponse.GetLocation());
                var dataCountry = await _consultCountryCommand.ConsultIdbyName("Mexico");

                var jsonSaml = new
                {
                    idUser= data1==null? restUs.IdUser: data1.IdUser,
                    email = samlResponse.GetEmail(),
                    nombre = samlResponse.GetFirstName(),
                    lastName = samlResponse.GetLastName(),
                    location = samlResponse.GetLocation(),
                    upn = samlResponse.GetUpn(),
                    company = samlResponse.GetCompany(),
                    department = samlResponse.GetDepartment(),
                    nameid = samlResponse.GetNameID(),
                    phone = samlResponse.GetPhone(),
                    roleEntityId = "5a0ab2a2-f790-4f96-9dee-da0b9111f7c7",
                    nameRole= "Usuario estandar",
                    countryEntityId = dataCountry,
                    nameCountry= "Mexico"
                };


                encodedStr = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(jsonSaml)));
                */

            // return Redirect("http://localhost:4200/dashboard?uxm_erd=" + encodedStr);
            // return Redirect("https://transversal-portaltls-front.shfyjbr2p4o.us-south.codeengine.appdomain.cloud?uxm_erd=" + encodedStr);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, samlResponse));


           //if   }


            // return Redirect("http://localhost:4200/dashboard?uxm_erd=" + encodedStr);
            //return Redirect("https://transversal-portaltls-front.shfyjbr2p4o.us-south.codeengine.appdomain.cloud?uxm_erd=" + encodedStr);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, samlResponse));


        }

        [HttpGet("Aproved")]
        public async Task<IActionResult> Aproved(
       [FromQuery] int nivel, [FromServices] IConsultAprobadorCommand ConsultAprobadorCommand)
        {
            var data = ConsultAprobadorCommand.Execute(nivel);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));

        }
        [HttpGet("ListConsult")]
        public async Task<IActionResult> GetConsult([FromServices] IGetListUsuarioCommand getListUsuarioCommand)
        {
            var data = await getListUsuarioCommand.List();
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));
        }
        [HttpPost("UpdateAll")]
        public async Task<IActionResult> UpdateAll([FromBody] CreateUserModelc model, [FromServices] IUpdateUsuarioCommand updateUsuarioCommand)
        {
            var data = await updateUsuarioCommand.Update(model);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));
        }
        [HttpGet("GetByRolList")]
        public async Task<IActionResult> ListByRolId([FromQuery] Guid Id, [FromServices] IGetListUsuarioCommand getListUsuarioCommand)
        {
            var data = await getListUsuarioCommand.ConsultUsersByRoleId(Id);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));
        }
        [HttpGet("GetByEmployeCode")]
        public async Task<IActionResult> GetByEmployeCode([FromQuery] string EmployeeCode, [FromQuery] Guid PaisId, [FromServices] IGetListUsuarioCommand getListUsuarioCommand)
        {
            var data = await getListUsuarioCommand.GetUserIdByEmployeeCode(EmployeeCode, PaisId);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));
        }
        [HttpGet("GetByEmail")]
        public async Task<IActionResult> GetByEmail([FromQuery] string EmailUser, [FromServices] IGetListUsuarioCommand getListUsuarioCommand)
        {
            var data = await getListUsuarioCommand.GetByEmail(EmailUser);
            return StatusCode(StatusCodes.Status200OK, ResponseApiService.Response(StatusCodes.Status200OK, data));
        }
    }
}