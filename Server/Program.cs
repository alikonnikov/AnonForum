using MongoDB.Driver;
using Server.Services;

var builder = WebApplication.CreateBuilder(args);

var mongoConnectionString = builder.Configuration.GetConnectionString("MongoDb") ?? "mongodb://localhost:27017";
builder.Services.AddSingleton<IMongoClient>(new MongoClient(mongoConnectionString));
builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<ForumService>();

app.Run();