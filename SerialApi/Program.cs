using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SerialApi.DatabaseContext;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Serial Api", Version = "v1" });
    var filePath = Path.Combine(System.AppContext.BaseDirectory, "doc.xml");
    c.IncludeXmlComments(filePath);
});

builder.Services.AddCors();


builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddDbContext<SerialContext>(options => options.UseSqlite("DataSource=DatabaseFiles/SerialInfo.db"));

var app = builder.Build();

app.UseCors(builder => builder.AllowAnyOrigin());



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c=> 
    {
        c.SwaggerEndpoint("v1/swagger.json", "SerialApi V1");
    });
    
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
