using GolbonWebRoad.Api.CacheRevalidations;
using GolbonWebRoad.Application;
using GolbonWebRoad.Application.Exceptions;
using GolbonWebRoad.Infrastructure;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text.Json;




var builder = WebApplication.CreateBuilder(args);
#region Config Problem Details For Exception Handler
builder.Services.AddProblemDetails(options =>
{
    options.Map<NotFoundException>(ex => new ProblemDetails
    {
        Status=StatusCodes.Status404NotFound,
        Title="Not Found",
        Detail= ex.Message
    });
    options.Map<BadRequestException>(ex => new ProblemDetails
    {
        Status=StatusCodes.Status500InternalServerError,
        Title="Bad Request",
        Detail= ex.Message
    });
    options.MapToStatusCode<Exception>(StatusCodes.Status500InternalServerError);
});
#endregion
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();

builder.Services.AddSingleton<CacheRevalidation>();
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});
#region add session
builder.Services.AddSession(options =>
{
    options.IdleTimeout= TimeSpan.FromSeconds(30);
    options.Cookie.HttpOnly=true;
    options.Cookie.IsEssential=true;
});
#endregion
#region Jwt
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme=JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme=JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters=new TokenValidationParameters
    {
        ValidateIssuer=true,
        ValidateAudience=true,
        ValidateLifetime=true,
        ValidateIssuerSigningKey=true,
        ValidIssuer=builder.Configuration["Jwt:Issuer"],
        ValidAudience=builder.Configuration["Jwt:Audience"],
        IssuerSigningKey=new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))

    };
    options.Events =new JwtBearerEvents
    {
        OnChallenge=context =>
        {
            context.HandleResponse();
            context.Response.StatusCode=401;
            context.Response.ContentType="application/json";
            var result = JsonSerializer.Serialize(new { message = "هویت شما مشخص نیست اجازه دسترسی ندارید." });
            return context.Response.WriteAsync(result);

        },
        OnForbidden=context =>
        {
            context.Response.StatusCode=403;
            context.Response.ContentType="application/json";
            var result = JsonSerializer.Serialize(new { message = "شما مجاز به دسترسی این بخش نیستید." });
            return context.Response.WriteAsync(result);
        }

    };
});
#endregion
#region Swagger
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    // قدم اول: تعریف طرح امنیتی (این بخش در کدهای قبلی هم درست بود)
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "لطفاً توکن JWT را در این قسمت وارد کنید",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    // قدم دوم: ایجاد و افزودن نیازمندی امنیتی (روش جدید و مطمئن)
    var securityRequirement = new OpenApiSecurityRequirement
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
            new string[] {}
        }
    };

    options.AddSecurityRequirement(securityRequirement);
});
builder.Services.AddMemoryCache();
#endregion

var app = builder.Build();
app.UseProblemDetails();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.InjectStylesheet("/swagger/css/dark-theme.css");

    });
}
app.UseStaticFiles();
app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();



app.UseSession();



app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    try
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        await GolbonWebRoad.Infrastructure.DataSeeders.IdentitySeedData.Initialize(userManager, roleManager);

    }
    catch (Exception ex)
    {
        Console.WriteLine("error seed identity");
    }

}


app.Run();
