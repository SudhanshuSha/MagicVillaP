
// logge is already registered inside the create builder when application is being built
// since logging is already registerd in our application we need to reteive that in the dependency injection
using MagicVilla_WebAPI;
using MagicVilla_WebAPI.Configurations;
using MagicVilla_WebAPI.Data;
using MagicVilla_WebAPI.Logging;
using MagicVilla_WebAPI.Models;
using MagicVilla_WebAPI.Repository;
using MagicVilla_WebAPI.Repository.IRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// anything above the level of debug will be loggged into the file
Log.Logger = new LoggerConfiguration().MinimumLevel.Debug()
    .WriteTo.File("log/villaLogs.txt", rollingInterval: RollingInterval.Day).CreateLogger();

// telling application to use serilog instead of buit in logger
builder.Host.UseSerilog();

builder.Services.AddDbContext<ApplicationDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSQLConnection"));
});

// enable chaching
builder.Services.AddResponseCaching();

//add identity service
// we need to define user and role for the apllication
// on that identity we want entity framework as we want to indentofy tables for users and roles in db
// here we added the default identity user but we can add more properties on that user
//builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApllicationDbContext>();
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();

// new object is created per request
builder.Services.AddScoped<IVillaRepository, VillaRepository>();
builder.Services.AddScoped<IVillaNumberRepository, VillaNumberRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddAutoMapper(typeof(MappingConfig));
builder.Services.AddApiVersioning(options =>
{
    // if no version is specified
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    //it will show API version in response headers
    options.ReportApiVersions = true;
});

builder.Services.AddVersionedApiExplorer(options =>
{
    // this will format the API verison as group name
    options.GroupNameFormat = "'v'VVV";

    // if APi version need to be automated to v1
    options.SubstituteApiVersionInUrl = true;
});


var key = builder.Configuration.GetValue<string>("APISettings:Secret");
// we can configure the bearer and authentication
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

// we can add cache profile so all request can be cahched for certain amounto fo tme
builder.Services.AddControllers(options =>
{
    options.CacheProfiles.Add("Default30", new CacheProfile()
    {
        Duration = 30
    });
}).AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // it described how APi is protected through the swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "JWT Authorizationheading using the Bearer scheme. \r\n\r\n" +
        "Enter 'Bearer' [space] and then your token in the text input below. \r\n\r\n" +
        "Example : \"Bearer 12345abcdef \"",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    // add the global security requirement 
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                },
                Scheme="oauth2",
                Name="Bearer",
                In= ParameterLocation.Header,
            },
            new List<string>()
        }
    });

    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1.0",
        Title = "Magic Villa V1",
        Description = "API to manage Villa",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Sample",
            Url = new Uri("https://google.com")
        },
        License = new OpenApiLicense
        {
            Name = "License",
            Url = new Uri("https://google.com")
        }

    });

    options.SwaggerDoc("v2", new OpenApiInfo
    {
        Version = "v2.0",
        Title = "Magic Villa V2",
        Description = "API to manage Villa",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Sample",
            Url = new Uri("https://google.com")
        },
        License = new OpenApiLicense
        {
            Name = "License",
            Url = new Uri("https://google.com")
        }

    });
});

// in order to register it we need to add the service

// there are mutiple lifetime whren register a service maxuimun lifetime is add Singleton
// this is creates when application starts and object will be used evrytime when application request
// ad scoped for every request it will create a new object and provide that where it is requested
// dd transient everytime object is accessed in one request object is accessed ten times it will create 10 diffrent objects
// add assign that where it is needed
// we need singleton as we use one loggwr throughout the apllication
// Registerservice in DI container  
builder.Services.AddSingleton<ILogging, Logging>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    // we want two end points one for version 1 and one for 2
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Magic_Villa1");
        options.SwaggerEndpoint("/swagger/v2/swagger.json", "Magic_Villa2");
    });
}

app.UseHttpsRedirection();

// there is no use Authentication in this we need to add that in pipeline

app.UseAuthentication();
// for user to be authorized they must be authenticated first
app.UseAuthorization();

// we need to add an middelware to incerpt an expection

app.MapControllers();

app.AddGlobalErrorHandler();

app.Run();
