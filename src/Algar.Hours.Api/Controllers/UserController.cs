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
using Algar.Hours.Domain.Entities.User;
using System.Net.Mail;
using Algar.Hours.Application.DataBase.User.Commands.Email;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Algar.Hours.Domain.Entities.Rol;
using Microsoft.AspNetCore.Authorization;
using Algar.Hours.Application.DataBase.UserSession.Commands.CreateLog;


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
        private readonly IConfiguration _config;
        private ICreateLogCommand _logCommand;

        public UserController(IGetListUsuarioCommand getListUsuarioCommand, ICreateUserCommand createUserCommand, IConsultCountryCommand consultCountryCommand, IConfiguration config, ICreateLogCommand logCommand)
        {
            _getListUsuarioCommand = getListUsuarioCommand;
            _createUserCommand = createUserCommand;
            _consultCountryCommand = consultCountryCommand;
            _config = config;
            _logCommand = logCommand;
        }

        [HttpPost("create")]
      //  [Authorize(Roles = "standard")]
        public async Task<IActionResult> Create(
           [FromBody] CreateUserModelc model, [FromServices] ICreateUserCommand createUserCommand)
        {
            var data = await createUserCommand.Execute(model);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));

        }

        [HttpGet("Consult")]
        [Authorize(Roles = "standard")]
        public async Task<IActionResult> Consult(
          [FromQuery] Guid id, [FromServices] IGetListUsuarioCommand createUserCommand)
        {

            var data = await createUserCommand.Consult(id);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));

        }

        [HttpGet("GetManagerByEmployeeCode")]
        [Authorize(Roles = "standard")]
        public async Task<IActionResult> GetManagerByEmployeeCode(
       [FromQuery] string employeeCode, [FromServices] IGetListUsuarioCommand createUserCommand)
        {
            var data = createUserCommand.GetManagerByEmployeeCode(employeeCode);
            return StatusCode(StatusCodes.Status200OK, ResponseApiService.Response(StatusCodes.Status200OK, data));

        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(
         [FromBody] LoginUserModel model, [FromServices] ILoginUserCommand loginuserCommand)
        {
            var data = await loginuserCommand.Execute(model);
            
            var token = data==null?"NA":GenerateToken(model);

            var responseLogin = new
            {
                Email = data == null ? "":data.Email,
                Password = data == null ? "" : data.Password,
                RoleEntity = data == null ? null : data.RoleEntity,
                RoleEntityId = data == null ? new Guid("00000000-0000-0000-0000-000000000000") : data.RoleEntityId,
                CountryEntityId = data == null ? new Guid("00000000-0000-0000-0000-000000000000") : data.CountryEntityId,
                CountryEntity = data == null ? null : data.CountryEntity,
                EmployeeCode = data == null ? "" : data.EmployeeCode,
                IdUser = data == null ? new Guid("00000000-0000-0000-0000-000000000000") : data.IdUser,
                NameUser = data == null ? "" : data.NameUser,
                surnameUser = data == null ? "" : data.surnameUser,
                token = token
            };



            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, responseLogin));

        }


     
        [HttpPost("SendEmail")]
        [Authorize(Roles = "standard")]
        public async Task<IActionResult> SendEmail(
        [FromBody] EmailModel model, [FromServices] IEmailCommand emailCommand)
        {

            
          
            try
            {
                
            
                emailCommand.SendEmail(model);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            

            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, "OK"));
        }

        [HttpPost("Callback")]
        [Produces("application/json")]
       // [Authorize(Roles = "standard")]
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
            // var samlResponse = Request.Form["SAMLResponse"].ToString();

           
           
            var samlResponse = new Response(samlCert, Request.Form["SAMLResponse"]);

           

            

            // 3. DONE!
             try{
                
                var codeEmployee = samlResponse.GetCustomAttribute("uid");

                var dataCountry = await _consultCountryCommand.ConsultIdbyCode(codeEmployee.Substring(codeEmployee.Length-3));

                UserEntity restUs = new();
                var data1 = await _getListUsuarioCommand.GetByEmail(samlResponse.GetCustomAttribute("emailAddress"));
                if (data1==null)
                {
                    CreateUserModelc newUsr = new()
                    {
                        NameUser = samlResponse.GetCustomAttribute("firstName"),
                        Email = samlResponse.GetCustomAttribute("emailAddress"),
                        EmployeeCode = samlResponse.GetCustomAttribute("uid"),
                        Password = "123456789",
                        surnameUser = samlResponse.GetCustomAttribute("lastName"),
                        RoleEntityId = new Guid("5a0ab2a2-f790-4f96-9dee-da0b9111f7c7"),//Rol Standard
                        CountryEntityId = dataCountry.IdCounty

                    };
                    restUs = await _createUserCommand.ExecuteId(newUsr);
                }




                var jsonSaml = new
                {
                    idUser = data1 == null ? restUs.IdUser : data1.IdUser,
                    email = samlResponse.GetCustomAttribute("emailAddress"),
                    nombre = samlResponse.GetCustomAttribute("firstName"),
                    lastName = samlResponse.GetCustomAttribute("lastName"),
                    roleEntityId = data1?.RoleEntityId,
                    nameRole = data1?.RoleEntity.NameRole, 
                    countryEntityId = dataCountry.IdCounty,
                    nameCountry= dataCountry.NameCountry,
                    employeeCode = samlResponse.GetCustomAttribute("uid"),
                    code = "123456789",
                };

                try
                {
                    await _logCommand.Log(jsonSaml.idUser.ToString() , "SAMLResponse:::", samlResponse);
                }
                catch (Exception exx)
                {


                }


                encodedStr = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(jsonSaml)));


                //  return Redirect("http://localhost:4200/dashboard/#/dashboard?uxm_erd=" + encodedStr);
                //return Redirect("https://transversal-portaltls-front.shfyjbr2p4o.us-south.codeengine.appdomain.cloud?uxm_erd=" + encodedStr);
                return Redirect("https://transversal-portaltls-front.shfyjbr2p4o.us-south.codeengine.appdomain.cloud/#/dashboard?uxm_erd=" + encodedStr);



            }
            catch(Exception ex)
            {
                encodedStr = Convert.ToBase64String(Encoding.UTF8.GetBytes("Error al obtener datos SAML"+ex.Message));
                return Redirect("https://transversal-portaltls-front.shfyjbr2p4o.us-south.codeengine.appdomain.cloud/#/dashboard?uxm_erd=" + encodedStr);
            }


            // return Redirect("http://localhost:4200/dashboard?uxm_erd=" + encodedStr);
            return Redirect("https://transversal-portaltls-front.shfyjbr2p4o.us-south.codeengine.appdomain.cloud?uxm_erd=" + encodedStr);



        }

        [HttpGet("Aproved")]
        [Authorize(Roles = "standard")]
        public async Task<IActionResult> Aproved(
       [FromQuery] int nivel, [FromServices] IConsultAprobadorCommand ConsultAprobadorCommand)
        {
            var data = ConsultAprobadorCommand.Execute(nivel);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));

        }

        
        [HttpGet("ListConsult")]
        [Authorize(Roles = "standard")]
        public async Task<IActionResult> GetConsult([FromServices] IGetListUsuarioCommand getListUsuarioCommand)
        {
            var data = await getListUsuarioCommand.List();
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));
        }
        [HttpPost("UpdateAll")]
        [Authorize(Roles = "standard")]
        public async Task<IActionResult> UpdateAll([FromBody] CreateUserModelc model, [FromServices] IUpdateUsuarioCommand updateUsuarioCommand)
        {
            var data = await updateUsuarioCommand.Update(model);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));
        }
        [HttpGet("GetByRolList")]
        [Authorize(Roles = "standard")]
        public async Task<IActionResult> ListByRolId([FromQuery] Guid Id, [FromServices] IGetListUsuarioCommand getListUsuarioCommand)
        {
            var data = await getListUsuarioCommand.ConsultUsersByRoleId(Id);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));
        }
        [HttpGet("GetByCountryList")]
        [Authorize(Roles = "standard")]
        public async Task<IActionResult> ListByCountryId([FromQuery] Guid Id, [FromServices] IGetListUsuarioCommand getListUsuarioCommand)
        {
            var data = await getListUsuarioCommand.ConsultUsersByCountryId(Id);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status200OK, data));
        }
        [HttpGet("GetUserIdByEmployeCode")]
        [Authorize(Roles = "standard")]
        public async Task<IActionResult> GetUserIdByEmployeCode([FromQuery] string EmployeeCode, [FromQuery] Guid? PaisId, [FromServices] IGetListUsuarioCommand getListUsuarioCommand)
        {
            var data = await getListUsuarioCommand.GetUserIdByEmployeeCode(EmployeeCode, PaisId);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));
        }
        [HttpGet("GetByEmployeCode")]
        [Authorize(Roles = "standard")]
        public async Task<IActionResult> GetByEmployeCode([FromQuery] string EmployeeCode, [FromServices] IGetListUsuarioCommand getListUsuarioCommand)
        {
            var data = await getListUsuarioCommand.GetByEmployeeCode(EmployeeCode);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));
        }
        [HttpGet("GetByEmail")]
        [Authorize(Roles = "standard")]
        public async Task<IActionResult> GetByEmail([FromQuery] string EmailUser, [FromServices] IGetListUsuarioCommand getListUsuarioCommand)
        {
            var data = await getListUsuarioCommand.GetByEmail(EmailUser);
            return StatusCode(StatusCodes.Status200OK, ResponseApiService.Response(StatusCodes.Status200OK, data));
        }

        private string GenerateToken(LoginUserModel user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,user.UserName),
                new Claim(ClaimTypes.Role,"standard")
            };
            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: credentials);
            try
            {
                var t = new JwtSecurityTokenHandler().WriteToken(token);

                return t;
            }
            catch(Exception ex)
            {
                return "xxxx";
            }
           

        }

        [HttpPost("log")]
        //[Authorize(Roles = "standard")]
        public async Task<IActionResult> Log(
            string userEntityId,string operation,string parameters, [FromServices] ICreateLogCommand createLogCommand)
        {
            var data = await createLogCommand.Log(userEntityId, operation,parameters);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));

        }

        [HttpGet("log")]
        //[Authorize(Roles = "standard")]
        public async Task<IActionResult> gLog(
           string userEntityId, string operation, string parameters, [FromServices] ICreateLogCommand createLogCommand)
        {
            var data = await createLogCommand.Log(userEntityId, operation, parameters);
            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, data));

        }
    }
}