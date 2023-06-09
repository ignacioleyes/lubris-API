using Lubris_API.Entities;
using Lubris_API.Services;
using Lubris_API.Services.Interfaces;
using Lubris_API.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddControllersWithViews().AddNewtonsoftJson();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<IUserServices, UserServices>();


builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
builder.Services.AddHttpClient();

string? connectionString = Environment.GetEnvironmentVariable("LUBRIS_DB_CONNECTION");
if (connectionString == null) throw new Exception("LUBRIS_DB_CONNECTION environment variable not set");
builder.Services.AddEntityFrameworkNpgsql()
    .AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseNpgsql(connectionString.BuildPostgresConnectionString());
    });


string? jwtKey = Environment.GetEnvironmentVariable("LUBRIS_JWT_KEY");
if (jwtKey == null) throw new Exception("LUBRIS_JWT_KEY environment variable not set");
builder.Services.AddAuthentication()
    .AddJwtBearer(options =>
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                ClockSkew = TimeSpan.Zero
            });

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.User = new UserOptions
    {
        RequireUniqueEmail = true
    };
}).AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[]{}
                    }
                });

    var fileXML = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var routeXML = Path.Combine(AppContext.BaseDirectory, fileXML);
    c.IncludeXmlComments(routeXML, includeControllerXmlComments: true);
});

builder.Services.AddCors(c => c.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseDeveloperExceptionPage();

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Lubris API");
});

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();