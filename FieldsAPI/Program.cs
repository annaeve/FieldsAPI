using FieldsAPI.Configuration;
using FieldsAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<KmlPathsOptions>(builder.Configuration.GetSection("KmlPaths"));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<KmlLoaderService>();
builder.Services.AddScoped<FieldService>();
builder.Services.AddScoped<GeometryService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
