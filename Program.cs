using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Quartz;
using SignalRChat.Data;
using SignalRChat.Hubs;
using SignalRChat.QuartzServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();
builder.Services.AddSignalR();
builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();

    var conconcurrentJobKey = new JobKey("ConconcurrentJob");
    q.AddJob<ConconcurrentJob>(opts => opts.WithIdentity(conconcurrentJobKey));
    q.AddTrigger(opts => opts
        .ForJob(conconcurrentJobKey)
        .WithIdentity("ConconcurrentJob-trigger")
        .WithSimpleSchedule(x => x
            .WithIntervalInSeconds(3)
            .RepeatForever()));

    var nonConconcurrentJobKey = new JobKey("NonConconcurrentJob");
    q.AddJob<NonConconcurrentJob>(opts => opts.WithIdentity(nonConconcurrentJobKey));
    q.AddTrigger(opts => opts
        .ForJob(nonConconcurrentJobKey)
        .WithIdentity("NonConconcurrentJob-trigger")
        .WithSimpleSchedule(x => x
            .WithIntervalInSeconds(3)
            .RepeatForever()));

    var ChangeJobKey = new JobKey("changeJob");
    q.AddJob<changeJob>(opts => opts.WithIdentity(ChangeJobKey));
    q.AddTrigger(opts => opts
        .ForJob(ChangeJobKey)
        .WithIdentity("ChangeJobKey-trigger")
        .WithSimpleSchedule(x => x
            .WithIntervalInSeconds(1)
            .RepeatForever()));

});
builder.Services.AddQuartzHostedService(
    q => q.WaitForJobsToComplete = true);

//var key = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(builder.Configuration["JwtKey"]));
//builder.Services.AddAuthentication().AddJwtBearer(options =>
//{
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        LifetimeValidator = (before, expires, token, parameters) =>
//            expires > DateTime.UtcNow,
//        ValidateAudience = false,
//        ValidateIssuer = false,
//        ValidateActor = false,
//        ValidateLifetime = true,
//        IssuerSigningKey = key,
//        NameClaimType = ClaimTypes.NameIdentifier
//    };

//    options.Events = new JwtBearerEvents
//    {
//        OnMessageReceived = context =>
//        {
//            var accessToken = context.Request.Query["access_token"];
//            if (!string.IsNullOrEmpty(accessToken))
//            {
//                context.Token = accessToken;
//            }
//            return Task.CompletedTask;
//        }
//    };
//});

//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy(JwtBearerDefaults.AuthenticationScheme, policy =>
//    {
//        policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
//        policy.RequireClaim(ClaimTypes.NameIdentifier);
//    });
//});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.UseEndpoints(builder =>
{
    builder.MapHub<Chat>("/chat");
});

app.MapRazorPages();

app.Run();
