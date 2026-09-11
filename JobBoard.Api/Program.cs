using JobBoard.Application.Interfaces;
using JobBoard.Infrastructure.Repositories;
using JobBoard.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContextPool<ApplicationDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("JobBoardDBConnection")));
builder.Services.AddScoped<IJobRepository, JobRepository>();
builder.Services.AddScoped<RoleInitializer>();
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();
var app = builder.Build();
var roleInitializer = app.Services.GetRequiredService<RoleInitializer>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
await roleInitializer.InitializeRolesAsync();
app.MapControllers();

app.Run();
