using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SerialApi.DatabaseContext;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SerialApi.HelperClasses;

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

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = AuthOptions.ISSUER,
            ValidateAudience = true,
            ValidAudience = AuthOptions.AUDIENCE,
            ValidateLifetime = true,
            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
            ValidateIssuerSigningKey = true
        };
    });

builder.Services.AddControllers().AddNewtonsoftJson();

string serialConnection = builder.Configuration.GetConnectionString("SerialConnection");
string authorizeConnection = builder.Configuration.GetConnectionString("AuthorizeConnection");

builder.Services.AddDbContext<SerialContext>(options => options.UseSqlite(serialConnection));
builder.Services.AddDbContext<AuthorizeContext>(options => options.UseSqlite(authorizeConnection));

var app = builder.Build();



app.UseCors(builder => {
    builder.AllowAnyMethod().AllowAnyHeader().SetIsOriginAllowed(origin => true).AllowCredentials();
});

app.UseAuthentication();
app.UseAuthorization();


app.UseSwagger();
app.UseSwaggerUI(c=> 
{
    string swaggerJsonBasePath = string.IsNullOrWhiteSpace(c.RoutePrefix) ? "." : "..";
    c.SwaggerEndpoint($"{swaggerJsonBasePath}/swagger/v1/swagger.json", "SerialApi V1");
});

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();

app.Run();
