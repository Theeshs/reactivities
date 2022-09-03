using Persistence;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<DataContext>(opt=>
{
    opt.UseSqlite("Name=DefaultConncetion");
});
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

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try{
        var context = services.GetRequiredService<DataContext>();
        await context.Database.MigrateAsync();
        await Seed.SeedData(context);
    } catch(Exception ex) {
        var logger = services.GetRequiredService<ILogger>();
        logger.LogError(ex, "An error occured during the migration");
    }
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
