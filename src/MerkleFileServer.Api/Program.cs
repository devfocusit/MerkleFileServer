using MerkleFileServer.Api;

if(args.Length == 1)
{
    var newArgs = new List<string>();
    newArgs.Add("--file");
    newArgs.Add(args[0]);

    args = newArgs.ToArray();
}

//var config = new ConfigurationBuilder()
//    .AddCommandLine(args)
//    .Build();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var startup = new Startup(builder.Configuration);

startup.ConfigureServices(builder.Services);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

startup.Configure(app, app.Environment);

app.Run();
