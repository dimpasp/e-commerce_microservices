using AutoMapper;
using Mango.Services.CouponAPI;
using Mango.Services.CouponAPI.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//add sxolia
builder.Services.AddDbContext<AppDbContext>(options =>
{

    //GetConnectionString this method works only with ConnectionStrings block in settings
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultDatabase"));
});
//register mapping config
//the basic configuration for automapper 
IMapper mapper=MappingConfig.RegisterMaps().CreateMapper();
builder.Services.AddSingleton(mapper);
//use automapper with dependency injection
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


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

app.Run();

//to run migration automatically
void ApplyMigration()
{
    //we get all the services
    using(var scope = app.Services.CreateScope())
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
