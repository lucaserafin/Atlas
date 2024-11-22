using Atlas.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AtlasDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("AtlasDb"), 
        o => o.UseNetTopologySuite());
});


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



await app.RunAsync();

