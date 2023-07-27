using Mango.Services.EmailAPI.Data;
using Mango.Services.EmailAPI.Extension;
using Mango.Services.EmailAPI.Messaging;
using Mango.Services.EmailAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
{

    //GetConnectionString this method works only with ConnectionStrings block in settings
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultDatabase"));
});

//for singleton service create new db conection  
var optionsBuilser = new DbContextOptionsBuilder<AppDbContext>();
optionsBuilser.UseSqlServer(builder.Configuration.GetConnectionString("DefaultDatabase"));
builder.Services.AddSingleton(new EmailService(optionsBuilser.Options));

//we want one object for all request
builder.Services.AddSingleton<IAzureServiceBusConsumer, AzureServiceBusConsumer>();

builder.Services.AddControllers();
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

//automatically update database if there are changes
ApplyMigration();
//configure in pipeline the service bus consumer
app.UseAzureServiceBusConsumer();

app.Run();


void ApplyMigration()
{
    //we get all the services
    using (var scope = app.Services.CreateScope())
    {
        //get only services about db 
        var _db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        //check if exist pending migrations
        if (_db.Database.GetPendingMigrations().Count() > 0)
        {
            _db.Database.Migrate();
        }
    }
}