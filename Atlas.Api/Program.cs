using Atlas.Api.Api;
using Atlas.Api.Infrastructure;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
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

await app.RunAsync();

