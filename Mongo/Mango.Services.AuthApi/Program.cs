using Mango.MessageBus;
using Mango.Services.AuthApi.Data;
using Mango.Services.AuthApi.Models;
using Mango.Services.AuthApi.Service.Implementation;
using Mango.Services.AuthApi.Service.Iservice;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultDatabase"));
});

//get jwt options settings
builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection("ApiSettings:JwtOptions"));

//define IdentityUser ,the default for now
//if we want the default without adding something
//...AddIdentity<IdentityUser, IdentityRole>...
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();


builder.Services.AddControllers();

//todo comment why scoped
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IAuthService, AuthService>();

//add project reference for MessageBus
builder.Services.AddScoped<IMessageBus, MessageBus>();
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

//before authorization we need authentication
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
ApplyMigration();

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