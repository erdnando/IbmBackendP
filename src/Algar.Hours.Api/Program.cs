using Algar.Hours.Api;
using Algar.Hours.Common;
using Algar.Hours.External;
using Algar.Hours.Persistence;
using Algar.Hours.Application;
using System.Text.Json;
using System.Text.Json.Serialization;

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
        .AllowAnyMethod();
    })
    ); 


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

app.UseHttpsRedirection();
app.UseCors("NuevaPolitica");
app.MapControllers();
app.Run();


