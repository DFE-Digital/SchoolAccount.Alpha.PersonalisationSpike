using GovUk.Frontend.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using SchoolAccount.Alpha.Services;
using SchoolAccount.Alpha.Services.Config;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie()
.AddOpenIdConnect(options =>
{
    var oidcConfig = builder.Configuration.GetSection("OpenIDConnectSettings");

    options.Authority = oidcConfig["Authority"];
    options.ClientId = oidcConfig["ClientId"];

    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.ResponseType = OpenIdConnectResponseType.IdToken;

    // uncomment to move organisation selection to DSI
    // options.Scope.Add("organisation");
    options.SaveTokens = true;
    options.GetClaimsFromUserInfoEndpoint = true;

    options.MapInboundClaims = false;
});
builder.Services.AddGovUkFrontend();
builder.Services.AddControllersWithViews();

// register app services
builder.Services.AddOptions<DsiApiConfig>()
    .Bind(builder.Configuration.GetSection("DfeSignInApi"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddOptions<AcademiesApiConfig>()
    .Bind(builder.Configuration.GetSection("AcademiesApi"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddScoped<IDsiApiService, DsiApiService>();
builder.Services.AddScoped<IAcademiesApiService, AcademiesApiService>();

var app = builder.Build();
app.UseGovUkFrontend();

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
app.UseAuthentication();
// Authorization is applied for middleware after the UseAuthorization method
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
