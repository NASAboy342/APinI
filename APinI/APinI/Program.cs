using APinI.BE;
using APinI.Schedular;
using APinI.Schedular.Jobs;
using APinI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddRazorPages();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ICicdService, CicdService>();
builder.Services.AddSingleton<IPowerShellService, PowerShellService>();
builder.Services.AddSingleton<UpdateLocalWebsiteIpAddress>();
builder.Services.AddHostedService<UpdateLocalWebsiteIpAddress>();
builder.Services.AddSingleton<IHttpClientService, HttpClientService>();
builder.Services.AddSingleton<IIQOptionService, IQOptionService>();
builder.Services.AddSingleton<SchedulerHealthCheck>();
builder.Services.AddSingleton<IIQOptionService, IQOptionService>();
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      builder =>
                      {
                          builder.AllowAnyOrigin()
                                 .AllowAnyMethod()
                                 .AllowAnyHeader();
                      });
});

var app = builder.Build();

new DeviceSecheduler().Run();

// Configure the HTTP request pipeline.

    app.UseSwagger();
    app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseCors(MyAllowSpecificOrigins);
app.MapControllers();
app.MapRazorPages();
app.UseWebSockets();

app.Run();
