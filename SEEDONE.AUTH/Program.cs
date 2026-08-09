using SEEDONE.REPO.Repo.Master;
using SEEDONE.SERVICE.Contexts;
using SEEDONE.SERVICE.Interfaces.Repo.Master;
using SEEDONE.SERVICE.Interfaces.Service.Business;
using SEEDONE.SERVICE.Service.Business;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình để load appsettings từ thư mục config chung
var configPath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory())?.FullName ?? "", "config");
builder.Configuration
    .AddJsonFile(Path.Combine(configPath, "appsettings.json"), optional: false, reloadOnChange: true)
    .AddJsonFile(Path.Combine(configPath, $"appsettings.{builder.Environment.EnvironmentName}.json"), optional: true, reloadOnChange: true);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITypeService, TypeService>();
builder.Services.AddScoped<IContextService, WebContextService>();
builder.Services.AddScoped<IMasterBaseRepo, MasterBaseRepo>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
