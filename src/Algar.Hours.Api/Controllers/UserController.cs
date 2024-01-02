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
        [Produces("application/json")]
        public async Task<IActionResult> Callback()
        {
           



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
            var samlResponse = new Response(samlCert, Request.Form["SAMLResponse"]);
            //Console.WriteLine(samlResponse);

            //samlCert = "%3Csamlp%3AResponse%20xmlns%3Asamlp%3D%22urn%3Aoasis%3Anames%3Atc%3ASAML%3A2.0%3Aprotocol%22%20xmlns%3Asaml%3D%22urn%3Aoasis%3Anames%3Atc%3ASAML%3A2.0%3Aassertion%22%20ID%3D%22_8e8dc5f69a98cc4c1ff3427e5ce34606fd672f91e6%22%20Version%3D%222.0%22%20IssueInstant%3D%222014-07-17T01%3A01%3A48Z%22%20Destination%3D%22http%3A%2F%2Fsp.example.com%2Fdemo1%2Findex.php%3Facs%22%20InResponseTo%3D%22ONELOGIN_4fee3b046395c4e751011e97f8900b5273d56685%22%3E%0A%20%20%3Csaml%3AIssuer%3Ehttp%3A%2F%2Fidp.example.com%2Fmetadata.php%3C%2Fsaml%3AIssuer%3E%0A%20%20%3Csamlp%3AStatus%3E%0A%20%20%20%20%3Csamlp%3AStatusCode%20Value%3D%22urn%3Aoasis%3Anames%3Atc%3ASAML%3A2.0%3Astatus%3ASuccess%22%20%2F%3E%0A%20%20%3C%2Fsamlp%3AStatus%3E%0A%20%20%3Csaml%3AAssertion%20xmlns%3Axsi%3D%22http%3A%2F%2Fwww.w3.org%2F2001%2FXMLSchema-instance%22%20xmlns%3Axs%3D%22http%3A%2F%2Fwww.w3.org%2F2001%2FXMLSchema%22%20ID%3D%22_d71a3a8e9fcc45c9e9d248ef7049393fc8f04e5f75%22%20Version%3D%222.0%22%20IssueInstant%3D%222014-07-17T01%3A01%3A48Z%22%3E%0A%20%20%20%20%3Csaml%3AIssuer%3Ehttp%3A%2F%2Fidp.example.com%2Fmetadata.php%3C%2Fsaml%3AIssuer%3E%0A%20%20%20%20%3Csaml%3ASubject%3E%0A%20%20%20%20%20%20%3Csaml%3ANameID%20SPNameQualifier%3D%22http%3A%2F%2Fsp.example.com%2Fdemo1%2Fmetadata.php%22%20Format%3D%22urn%3Aoasis%3Anames%3Atc%3ASAML%3A2.0%3Anameid-format%3Atransient%22%3E_ce3d2948b4cf20146dee0a0b3dd6f69b6cf86f62d7%3C%2Fsaml%3ANameID%3E%0A%20%20%20%20%20%20%3Csaml%3ASubjectConfirmation%20Method%3D%22urn%3Aoasis%3Anames%3Atc%3ASAML%3A2.0%3Acm%3Abearer%22%3E%0A%20%20%20%20%20%20%20%20%3Csaml%3ASubjectConfirmationData%20NotOnOrAfter%3D%222024-01-18T06%3A21%3A48Z%22%20Recipient%3D%22http%3A%2F%2Fsp.example.com%2Fdemo1%2Findex.php%3Facs%22%20InResponseTo%3D%22ONELOGIN_4fee3b046395c4e751011e97f8900b5273d56685%22%20%2F%3E%0A%20%20%20%20%20%20%3C%2Fsaml%3ASubjectConfirmation%3E%0A%20%20%20%20%3C%2Fsaml%3ASubject%3E%0A%20%20%20%20%3Csaml%3AConditions%20NotBefore%3D%222014-07-17T01%3A01%3A18Z%22%20NotOnOrAfter%3D%222024-01-18T06%3A21%3A48Z%22%3E%0A%20%20%20%20%20%20%3Csaml%3AAudienceRestriction%3E%0A%20%20%20%20%20%20%20%20%3Csaml%3AAudience%3Ehttp%3A%2F%2Fsp.example.com%2Fdemo1%2Fmetadata.php%3C%2Fsaml%3AAudience%3E%0A%20%20%20%20%20%20%3C%2Fsaml%3AAudienceRestriction%3E%0A%20%20%20%20%3C%2Fsaml%3AConditions%3E%0A%20%20%20%20%3Csaml%3AAuthnStatement%20AuthnInstant%3D%222014-07-17T01%3A01%3A48Z%22%20SessionNotOnOrAfter%3D%222024-07-17T09%3A01%3A48Z%22%20SessionIndex%3D%22_be9967abd904ddcae3c0eb4189adbe3f71e327cf93%22%3E%0A%20%20%20%20%20%20%3Csaml%3AAuthnContext%3E%0A%20%20%20%20%20%20%20%20%3Csaml%3AAuthnContextClassRef%3Eurn%3Aoasis%3Anames%3Atc%3ASAML%3A2.0%3Aac%3Aclasses%3APassword%3C%2Fsaml%3AAuthnContextClassRef%3E%0A%20%20%20%20%20%20%3C%2Fsaml%3AAuthnContext%3E%0A%20%20%20%20%3C%2Fsaml%3AAuthnStatement%3E%0A%20%20%20%20%3Csaml%3AAttributeStatement%3E%0A%20%20%20%20%20%20%3Csaml%3AAttribute%20Name%3D%22uid%22%20NameFormat%3D%22urn%3Aoasis%3Anames%3Atc%3ASAML%3A2.0%3Aattrname-format%3Abasic%22%3E%0A%20%20%20%20%20%20%20%20%3Csaml%3AAttributeValue%20xsi%3Atype%3D%22xs%3Astring%22%3Etest%3C%2Fsaml%3AAttributeValue%3E%0A%20%20%20%20%20%20%3C%2Fsaml%3AAttribute%3E%0A%20%20%20%20%20%20%3Csaml%3AAttribute%20Name%3D%22mail%22%20NameFormat%3D%22urn%3Aoasis%3Anames%3Atc%3ASAML%3A2.0%3Aattrname-format%3Abasic%22%3E%0A%20%20%20%20%20%20%20%20%3Csaml%3AAttributeValue%20xsi%3Atype%3D%22xs%3Astring%22%3Etest%40example.com%3C%2Fsaml%3AAttributeValue%3E%0A%20%20%20%20%20%20%3C%2Fsaml%3AAttribute%3E%0A%20%20%20%20%20%20%3Csaml%3AAttribute%20Name%3D%22eduPersonAffiliation%22%20NameFormat%3D%22urn%3Aoasis%3Anames%3Atc%3ASAML%3A2.0%3Aattrname-format%3Abasic%22%3E%0A%20%20%20%20%20%20%20%20%3Csaml%3AAttributeValue%20xsi%3Atype%3D%22xs%3Astring%22%3Eusers%3C%2Fsaml%3AAttributeValue%3E%0A%20%20%20%20%20%20%20%20%3Csaml%3AAttributeValue%20xsi%3Atype%3D%22xs%3Astring%22%3Eexamplerole1%3C%2Fsaml%3AAttributeValue%3E%0A%20%20%20%20%20%20%3C%2Fsaml%3AAttribute%3E%0A%20%20%20%20%3C%2Fsaml%3AAttributeStatement%3E%0A%20%20%3C%2Fsaml%3AAssertion%3E%0A%3C%2Fsamlp%3AResponse%3E";
            string encodedStr = Convert.ToBase64String(Encoding.UTF8.GetBytes(samlResponse.Xml));
           // var samlResponse = encodedStr;

            // 3. DONE!
            // if (samlResponse.IsValid()) //all good?
                    {
                //WOOHOO!!! the user is logged in
                // var username = samlResponse.GetNameID(); //let's get the username
                // var officeLocation = samlResponse.GetCustomAttribute("OfficeAddress");


               // return Redirect("http://localhost:4200/dashboard?uxm_erd=" + encodedStr);
                return Redirect("https://transversal-portaltls-front.shfyjbr2p4o.us-south.codeengine.appdomain.cloud/dashboard?uxm_erd=" + encodedStr);
               

            }
            // return Redirect("http://localhost:4200/dashboard?uxm_erd=" + encodedStr);
            return Redirect("https://transversal-portaltls-front.shfyjbr2p4o.us-south.codeengine.appdomain.cloud/dashboard?uxm_erd=" + encodedStr);



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