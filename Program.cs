using Microsoft.EntityFrameworkCore;
using URLShortner.API.Data;
using URLShortner.API.Interfaces;
using URLShortner.API.Services;
using URLShortner.API.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<UrlShortnerDbContext>(option=> option.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddSingleton<IShortCodeGenerator, ShortCodeGenerator>();
builder.Services.AddSingleton<IQRCodeService, QRCodeService>();
builder.Services.Configure<ShortCodeOptions>(builder.Configuration.GetSection("ShortCode"));
builder.Services.AddScoped<IUrlService, UrlService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();
