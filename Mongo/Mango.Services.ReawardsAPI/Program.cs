using Mango.Services.RewardsAPI;
using Mango.Services.RewardsAPI.Extension;
using Mango.Services.RewardsAPI.Messaging;
using Mango.Services.RewardsAPI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultDatabase"));
});

var optionsBuilser = new DbContextOptionsBuilder<AppDbContext>();
optionsBuilser.UseSqlServer(builder.Configuration.GetConnectionString("DefaultDatabase"));
builder.Services.AddSingleton(new RewardService(optionsBuilser.Options));

builder.Services.AddSingleton<IAzureServiceBusConsumer, AzureServiceBusConsumer>();

// Add services to the container.
//builder.Services.AddScoped<IRewardService, RewardService>();


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

ApplyMigration();

//todo critical 
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