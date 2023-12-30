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

namespace Algar.Hours.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [TypeFilter(typeof(ExceptionManager))]

    public class UserController : ControllerBase
    {
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
        public async Task<IActionResult> Callback()
        {

           
            // 1. TODO: specify the certificate that your SAML provider gave you
            string samlCertificate = @"-----BEGIN CERTIFICATE-----
                MIIDUDCCAjgCCQDPNT2FaAHozzANBgkqhkiG9w0BAQsFADBqMQswCQYDVQQGEwJVUzELMAkGA1UECAwCTlkxDzANBgNVBAcMBkFybW9uazEMMAoGA1UECgwDSUJNMQwwCgYDVQQLDANDSU8xITAfBgNVBAMMGHByZXByb2QubG9naW4udzMuaWJtLmNvbTAeFw0xOTEwMTAxMjA1NDRaFw0yNDEwMDgxMjA1NDRaMGoxCzAJBgNVBAYTAlVTMQswCQYDVQQIDAJOWTEPMA0GA1UEBwwGQXJtb25rMQwwCgYDVQQKDANJQk0xDDAKBgNVBAsMA0NJTzEhMB8GA1UEAwwYcHJlcHJvZC5sb2dpbi53My5pYm0uY29tMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAul2TrFWH1h+0ReRRwT2EsCsS6EXFd6zsWm51f2VfqJnt3tswO5UgjmReLKAJc5T+nritP8fbfDc8I+i3B1ITeYrcH24Ho8crfJGj34TNkGW/rDjB2q3YGy05+Vw3LXuqTwkiaoKAjkrf4wj4EFfcNT6G3xYlw2cg9ThCWptpWPjMBTINbpIwtjM7k0PoAqsjiooVDuW6dmiAkzhxX/tnxQda0jmp9VWU1DxnGbSSHcAqVB7ea2jmG5P6hZcR7jCh7TXRGazgcZmNhgzx6AyqE2Ae0S8dyXcADrTeXefmbgpTxvsC21kVx43i9KkIBnPNqr8hdwTUwZZLIYOS2s9b2QIDAQABMA0GCSqGSIb3DQEBCwUAA4IBAQA5DByy7f6pgwT+ZaRW5Hz5JS1jCpoEz9GrZIE1I4/i/OoGw+1L+5PnctsRMcYP/7TzyyQjM9Aouzk4TJCqVff3BFFFNsBFYmkgmGTKNLiCF9rtRILEATtF5C6rcb/DU8bbc9gTIpIj4M0QzlQcHvL5Aj1lVNYHMb5jNYNQpzqbniG1IuUXx9jcjwyDF/KsQUsIexGVXMfwYnSeXBEIJ9+EPiSzTdh6Da9wyK8I1PdiQ/iqhlBxJfbv0qPHLpj/ftgrxOiYLKuDcD4P6AmD5hDClk/fMBhinVZwb33DFObubIMLZOA7e1k+qTDaM5kiOcARFYLl59iWQXj/cnxJX5d/
               -----END CERTIFICATE-----";

           

            using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
            ILogger logger = factory.CreateLogger("Program");
            logger.LogInformation(Request.Form["SAMLResponse"]);

            // 2. Let's read the data - SAML providers usually POST it into the "SAMLResponse" var
            var samlResponse = new Response(samlCertificate, Request.Form["SAMLResponse"]);

            Console.WriteLine(samlResponse);

            // 3. DONE!
            if (samlResponse.IsValid()) //all good?
            {
                //WOOHOO!!! the user is logged in
                var username = samlResponse.GetNameID(); //let's get the username

                var officeLocation = samlResponse.GetCustomAttribute("OfficeAddress");

                //the user has been authenticated
                //now call context.SignInAsync() for ASP.NET Core
                //or call FormsAuthentication.SetAuthCookie() for .NET Framework
                //or do something else, like set a cookie or something...

                ////FOR EXAMPLE this is how you sign-in a user in ASP.NET Core 3,5,6,7
                //await context.SignInAsync(new ClaimsPrincipal(
                //    new ClaimsIdentity(
                //        new[] { new Claim(ClaimTypes.Name, username) },
                //        CookieAuthenticationDefaults.AuthenticationScheme)));

                return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, ""));

            }
            

            return StatusCode(StatusCodes.Status201Created, ResponseApiService.Response(StatusCodes.Status201Created, ""));

            


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
    }
}
