using GovUk.Frontend.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;
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
builder.Services.AddSingleton<ITaxonService, TaxonService>();
builder.Services.AddOptions<DsiApiConfig>()
    .Bind(builder.Configuration.GetSection("DfeSignInApi"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddOptions<AcademiesApiConfig>()
    .Bind(builder.Configuration.GetSection("AcademiesApi"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddOptions<GovUkSearchApiConfig>()
    .Bind(builder.Configuration.GetSection("GovUkSearchApi"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddHttpClient<IDsiApiService, DsiApiService>((serviceProvider, client) =>
{
    var config = serviceProvider.GetRequiredService<IOptions<DsiApiConfig>>().Value;
    client.BaseAddress = new Uri(config.PublicUrl);
});

builder.Services.AddHttpClient<IAcademiesApiService, AcademiesApiService>((serviceProvider, client) =>
{
    var config = serviceProvider.GetRequiredService<IOptions<AcademiesApiConfig>>().Value;
    client.BaseAddress = new Uri(config.PublicUrl);
    client.DefaultRequestHeaders.Add("ApiKey", config.ApiKey);
});
builder.Services.AddHttpClient<IGovUkSearchService, GovUkSearchService>((serviceProvider, client) =>
{
    var config = serviceProvider.GetRequiredService<IOptions<GovUkSearchApiConfig>>().Value;
    client.BaseAddress = new Uri(config.PublicUrl);
});

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