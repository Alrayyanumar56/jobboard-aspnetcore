using JobBoard.Application.Interfaces;
using JobBoard.Application.Jobs.ApplyToJob;
using JobBoard.Application.Jobs.CreateJob;
using JobBoard.Application.Jobs.GetJobById;
using JobBoard.Application.Jobs.GetJobApplicants;
using JobBoard.Application.Jobs.GetJobs;
using JobBoard.Application.Jobs.GetMyApplications;
using JobBoard.Application.Services;
using JobBoard.Infrastructure.Data;
using JobBoard.Infrastructure.Repositories;
using JobBoard.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
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
builder.Services.AddDbContextPool<ApplicationDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("JobBoardDBConnection")));
builder.Services.AddScoped<IJobRepository, JobRepository>();
builder.Services.AddScoped<CreateJobHandler>();
builder.Services.AddScoped<GetJobsHandler>();
builder.Services.AddScoped<GetJobByIdHandler>();
builder.Services.AddScoped<IJobApplicationRepository, JobApplicationRepository>();
builder.Services.AddScoped<ApplyToJobHandler>();
builder.Services.AddScoped<GetMyApplicationsHandler>();
builder.Services.AddScoped<GetJobApplicantsHandler>();
builder.Services.AddScoped<RoleInitializer>();
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddScoped<IRegistrationService, RegistrationService>();
builder.Services.AddScoped<IJWTService, JWTService>();
builder.Services.AddScoped<ILoginService, LoginService>();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(option =>
    {
        String Issuer = builder.Configuration["Jwt:Issuer"];
        String Audience = builder.Configuration["Jwt:Audience"];
        byte[] SecurityKey = Convert.FromBase64String(builder.Configuration["Jwt:SecretKey"]);

        option.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = Issuer,
            ValidateAudience = true,
            ValidAudience = Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(SecurityKey),
            ValidateLifetime = true,
        };
    });

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var roleInitializer = scope.ServiceProvider.GetRequiredService<RoleInitializer>();
    await roleInitializer.InitializeRolesAsync();
}

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();


/* {
  "email": "user@example.com",
  "password": "Password123!"


TestUser@12345
}*/