using Atlas.Api.Api;
using Atlas.Api.Hubs;
using Atlas.Api.Infrastructure;
using Atlas.Api.Infrastructure.Contracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<JsonOptions>(o => o.SerializerOptions.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals);

builder.Services.AddDbContext<AtlasDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("AtlasDb"), 
        o => o.UseNetTopologySuite());
});

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPointOfInterestRepository, PointOfInterestRepository>();
builder.Services.AddScoped<IUnitOfWork,AtlasDbContext>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());

builder.Services.AddSignalR()
    .AddNewtonsoftJsonProtocol(opts =>
        opts.PayloadSerializerSettings.TypeNameHandling = TypeNameHandling.Auto);

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AtlasDbContext>();
    await dbContext.Database.MigrateAsync();
}
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapUserApi();
app.MapPointOfInterestApi();
app.MapHub<AtlasHub>("/hub");

await app.RunAsync();

public partial class Program { }