using GolbonWebRoad.Application;
using GolbonWebRoad.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text.Json;




var builder = WebApplication.CreateBuilder(args);


builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();

builder.Services.AddControllers();
//builder.Services.AddSession(options =>
//{
//    options.IdleTimeout= TimeSpan.FromSeconds(30);
//    options.Cookie.HttpOnly=true;
//    options.Cookie.IsEssential=true;
//});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme=JwtBearerDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme=JwtBearerDefaults.AuthenticationScheme;
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
            var result = JsonSerializer.Serialize(new { message = "شما مجوز دسترسی به این بخش را ندارید." });
            return context.Response.WriteAsync(result);

        },
        OnForbidden=context =>
        {
            context.Response.StatusCode=401;
            context.Response.ContentType="application/json";
            var result = JsonSerializer.Serialize(new { message = "سطح دسترسی شما به این بخش مجاز نیست." });
            return context.Response.WriteAsync(result);
        }

    };
});

// Add services to the container.




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
#endregion
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
//app.UseSession();



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
