using Boiler.Server;
using Boiler.Server.Framework;
using Boiler.Server.HostedServices;
using Boiler.Server.Hubs;
using Boiler.Server.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

var _assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
#region dependency
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ServerDbContext>()
    .AddDefaultTokenProviders();
var servicesBuilder = new ApiServicesBuilder();
servicesBuilder.ConfigureServices(builder.Services, new string[] { _assemblyName });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSignalR();
builder.Services.AddSingleton<IUserIdProvider, HubUserIdProvider>();
#endregion
#region db
builder.Services.AddApiDbContexts<ServerDbContext>(builder.Configuration, _assemblyName);
#endregion
#region Hosted Service
builder.Services.AddHostedService<DataCleanerService>();
//builder.Services.AddHostedService<ProcessVideosService>();
#endregion
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region Authentication
builder.Services.AddAuthentication(o =>
{
    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey =
                new SymmetricSecurityKey(System.Text.Encoding.UTF8
                .GetBytes(builder.Configuration.GetSection(Constants.Auth.Token).Value)),
            ValidateIssuer = false,
            ValidateAudience = false
        };

        // SignalR authentication
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Cookies[Constants.Auth.AuthTokenCookieName];

                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) &&
                    ((path.StartsWithSegments("/streamNotificationHub")) || (path.StartsWithSegments("/temperatureHub"))))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddAuthorization();
#endregion

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<StreamNotificationHub>("/streamNotificationHub");
app.MapHub<TemperatureHub>("/temperatureHub");
app.MapFallbackToFile("/index.html");

#region Db Seed
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var dbContext = scope.ServiceProvider.GetRequiredService<ServerDbContext>();
    Seed.SeedUp(userManager, roleManager).Wait();
}
#endregion

app.Run();