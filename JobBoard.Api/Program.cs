using JobBoard.Application;
using JobBoard.Infrastructure;
using JobBoard.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text.Json.Serialization;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    // Every enum is sent and accepted as text ("FullTime", "Remote", "JobSeeker") instead of 0/1/2,
    // so the frontend never needs its own number-to-name tables. Numbers are still accepted on input.
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Unhandled exceptions and empty error responses become RFC 7807 ProblemDetails JSON,
// the same shape the controllers already use for 404/409, instead of an HTML error page.
builder.Services.AddProblemDetails();

// Liveness only: it deliberately does NOT query the database. Azure pings this regularly, and a
// database check would keep waking the auto-pausing free Azure SQL database and burn its free allowance.
builder.Services.AddHealthChecks();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });
});

// Each layer registers its own services (see DependencyInjection.cs in Application and Infrastructure).
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// CORS: which browser origins (the React frontend) may call this API. Configured, not hard-coded:
// appsettings.Development.json lists the local dev servers, and on Azure the deployed frontend's URL
// is added as the App Service setting Cors__AllowedOrigins__0. The JWT travels in the Authorization
// header, not a cookie, so credentials (cookies) are not allowed.
const string FrontendCorsPolicy = "Frontend";
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendCorsPolicy, policy => policy
        .WithOrigins(allowedOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod());
});

// JWT settings are validated HERE, at startup, with a message naming the missing setting. Before,
// a missing Jwt:SecretKey made Convert.FromBase64String(null) throw a bare ArgumentNullException,
// which on Azure only shows up as "HTTP 500.30 - app failed to start".
var jwtIssuer = RequireSetting(builder.Configuration, "Jwt:Issuer");
var jwtAudience = RequireSetting(builder.Configuration, "Jwt:Audience");
var jwtSigningKey = ReadSigningKey(RequireSetting(builder.Configuration, "Jwt:SecretKey"));

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(option =>
    {
        option.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(jwtSigningKey),
            ValidateLifetime = true,
        };
    });

var app = builder.Build();

// Make sure the Admin/Employer/JobSeeker roles exist. If the database can't be reached even after
// the retries configured in AddInfrastructure, log it loudly but keep the app running: the roles
// normally exist already, and a running app can still answer /health and show the error in its logs
// instead of the whole site failing with HTTP 500.30.
using (var scope = app.Services.CreateScope())
{
    try
    {
        var roleInitializer = scope.ServiceProvider.GetRequiredService<RoleInitializer>();
        await roleInitializer.InitializeRolesAsync();
    }
    catch (Exception ex)
    {
        app.Logger.LogCritical(ex, "Role initialization failed at startup. Check the database connection.");
    }
}

// Configure the HTTP request pipeline.

// Converts unhandled exceptions into a ProblemDetails 500 (in Development the detailed
// developer exception page still takes precedence).
app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment()|| app.Configuration.GetValue<bool>("EnableSwagger"))
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(FrontendCorsPolicy);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

static string RequireSetting(IConfiguration configuration, string key)
{
    var value = configuration[key];
    if (string.IsNullOrWhiteSpace(value))
    {
        throw new InvalidOperationException(
            $"Missing configuration value '{key}'. Set it in User Secrets locally, " +
            $"or as the App Service setting '{key.Replace(":", "__")}' on Azure.");
    }
    return value;
}

static byte[] ReadSigningKey(string base64Key)
{
    byte[] key;
    try
    {
        key = Convert.FromBase64String(base64Key);
    }
    catch (FormatException)
    {
        throw new InvalidOperationException("Jwt:SecretKey must be a Base64 string.");
    }

    // HMAC-SHA256 needs a key of at least 256 bits; a shorter key would only fail later, at the
    // first login, when the token is signed.
    if (key.Length < 32)
    {
        throw new InvalidOperationException("Jwt:SecretKey must decode to at least 32 bytes (256 bits) for HS256.");
    }
    return key;
}


/* {
  "email": "user@example.com",
  "password": "Password123!"


{
  "email": "ali@example.com",
  "password": "TestUser@12345"
}

{
  "email": "ahmedali@example.com",
  "password": "TestUser@12345"
}
}*/