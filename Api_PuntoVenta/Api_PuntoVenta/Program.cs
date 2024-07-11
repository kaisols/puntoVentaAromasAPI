using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Microsoft.Extensions.Options;
using System.Reflection;
using Microsoft.Extensions.FileProviders;
using Api_PuntoVenta.Models.Seguridad;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "API SISTEMA DE PUNTO DE VENTAS AROMAS",
        Description = "SISTEMA DE VENTAS DE PANADERIA AROMAS."
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Scheme = "bearer",
        Description = "Ingrese el Token"
    });


    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
    {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        },
        new string[] { }
    }});

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

});


// Adding CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyPolicy",
        builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = clsDatos.JWT_AUDIENCE_TOKEN,
        ValidIssuer = clsDatos.JWT_ISSUER_TOKEN,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(clsDatos.JWT_SECRET_KEY))
    };
});


var app = builder.Build();


//app.UseStaticFiles(new StaticFileOptions()
//{
//    FileProvider = new
//        PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Recursos")),
//    RequestPath = new PathString("/Recursos")
//});



app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.EnableTryItOutByDefault();
});

// Shows UseCors with CorsPolicyBuilder.
app.UseCors("MyPolicy");


app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});


app.MapControllers();

app.Run();
