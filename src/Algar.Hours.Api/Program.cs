using Algar.Hours.Api;
using Algar.Hours.Common;
using Algar.Hours.External;
using Algar.Hours.Persistence;
using Algar.Hours.Application;
using System.Text.Json;
using System.Text.Json.Serialization;
using Algar.Hours.Application.DataBase;
using Algar.Hours.Persistence.DataBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddWebApi()
    .AddCommon()
    .AddExternal(builder.Configuration)
    .AddPersistence(builder.Configuration)
    .AddApplication();


builder.Services.AddControllers()
            .AddXmlSerializerFormatters();

builder.Services.AddControllers();

builder.Services.AddCors(options =>

    options.AddPolicy("NuevaPolitica", app =>
    {
        app.AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod()
        .WithExposedHeaders(["Content-Disposition"]);
    })
    );

//JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = long.MaxValue;
});

builder.WebHost.ConfigureKestrel(opc =>
{
    opc.Limits.MaxRequestBodySize = 512*1024*1024;
});

builder.Services.Configure<FormOptions>(opc => {
    opc.MultipartBodyLengthLimit = 512*1024*1024;
});

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
}
);

JsonSerializerOptions options = new()
{
    ReferenceHandler = ReferenceHandler.IgnoreCycles,
    WriteIndented = true
};
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

app.UseHttpsRedirection();
app.UseCors("NuevaPolitica");

//JWT
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();