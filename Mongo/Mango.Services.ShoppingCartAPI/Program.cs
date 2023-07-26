using AutoMapper;
using Mango.Services.ShoppingCartAPI;
using Mango.Services.ShoppingCartAPI.Data;
using Mango.Services.ShoppingCartAPI.Extensions;
using Mango.Services.ShoppingCartAPI.Service.Implementation;
using Mango.Services.ShoppingCartAPI.Service.IService;
using Mango.Services.ShoppingCartAPI.Utility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

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
IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();
builder.Services.AddSingleton(mapper);
//use automapper with dependency injection
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<BackendApiAuthenticationHttpClientHandler>();
builder.Services.AddScoped<ICouponService, CouponService>();



builder.Services.AddHttpClient("Product", u => u.BaseAddress =
new Uri(builder.Configuration["ServiceUrls:ProductAPI"]))
    .AddHttpMessageHandler<BackendApiAuthenticationHttpClientHandler>();

builder.Services.AddHttpClient("Coupon", u => u.BaseAddress =
new Uri(builder.Configuration["ServiceUrls:CouponAPI"]))
    .AddHttpMessageHandler<BackendApiAuthenticationHttpClientHandler>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
//add authorization to swagger, to work with this for api
//basic authentication for swagger documentation
builder.Services.AddSwaggerGen(option =>
{
    option.AddSecurityDefinition(name: "Bearer",
        securityScheme: new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Description = "Bearer Authorization String",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference= new OpenApiReference
                {
                    Type= ReferenceType.SecurityScheme,
                    Id=JwtBearerDefaults.AuthenticationScheme
                }
            },new string[]{ }
        }
    });
});
//to get token i go to authAPI
//login with the user i want to log to couponApi
//get the token for swagger 
//as description says
// Bearer and then the token



//add authentication in programm
builder.AddAppAuthentication();

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//without authentication we get internal server error
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

ApplyMigration();

app.Run();

//to run migration automatically
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
