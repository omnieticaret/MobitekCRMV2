using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using MobitekCRMV2.DataAccess.Repository;
using MobitekCRMV2.DataAccess.UoW;
using MobitekCRMV2.Extensions;
using MobitekCRMV2.Jobs;
using MobitekCRMV2.Middlewares;
using MobitekCRMV2.Model.Models;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.CustomConfigureDbContext(configuration);
builder.Services.AddScoped<CustomReader>();
builder.Services.AddScoped<SpaceSerpJob>();
builder.Services.AddScoped<KeywordJsonModel>();

builder.Services.AddCustomServices();
builder.Services.CustomConfigureIdentity();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRobotsTxt(app.Environment);

app.UseMiddleware<IPControlMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
