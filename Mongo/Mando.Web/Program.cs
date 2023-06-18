using Mango.Web.Service.Implementation;
using Mango.Web.Service.IService;
using Mango.Web.Utility;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//dependency injection for IHttpClientFactory 
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

//configure the interface of services that use http client
builder.Services.AddHttpClient<ICouponService,CouponService>();


//set api url when the services are being configured
SD.CouponAPIBase = builder.Configuration["ServiceUrls:CouponAPI"];

//dependency injection of interfaces
//todo comment why scoped
builder.Services.AddScoped<ICouponService, CouponService>();
builder.Services.AddScoped<IBaseService, BaseService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
