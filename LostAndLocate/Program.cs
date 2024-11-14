using System.Reflection;
using System.Text;
using LostAndLocate.Data;
using LostAndLocate.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Setup Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    c.SwaggerDoc("v1", new OpenApiInfo { Title = "LostAndLocate", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });
    c.OperationFilter<AuthOperationFilter>();
});

// Register database and setup database context
builder.Services.AddDbContext<IDbContext, LostAndLocateContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("LostAndLocateContext")));

// Setup Jwt Authentication and Authorization
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
        };
    });
builder.Services.AddAuthorization();

// Setup Mapping between Database types and View types
builder.Services.AddAutoMapper(typeof(Program));

// Setup Modules (Chat, Files, ...)
builder.Services.RegisterModules();

builder.Services.AddResponseCompression();

// Register Controllers
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(x => { x.SuppressMapClientErrors = true; });

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = actionContext =>
        new BadRequestObjectResult(actionContext.ModelState.First().Value?.Errors.First().ErrorMessage);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // Enable Swagger
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

// Setup database and tables
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<LostAndLocateContext>();
    context.Database.EnsureCreated();
}

app.UseRouting();

// Enable Cross-Origin Requests (CORS)
app.UseCors(x =>
{
    x.AllowAnyOrigin();
    x.AllowAnyMethod();
    x.AllowAnyHeader();
});

// Enable Authentication and Authorization
app.UseAuthentication();
app.UseAuthorization();

app.UseResponseCompression();

// Enable WebSockets
app.UseWebSockets();

app.MapControllers();

app.Run();