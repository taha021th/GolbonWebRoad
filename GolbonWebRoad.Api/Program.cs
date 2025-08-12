using GolbonWebRoad.Application;
using GolbonWebRoad.Infrastructure;
using Microsoft.AspNetCore.Identity;




var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSession(options =>
{
    options.IdleTimeout=TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly=true;
    options.Cookie.IsEssential=true;

}
);
// Add services to the container.
builder.Services.AddInfrastructureServices(builder.Configuration);


builder.Services.AddApplicationServices();

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSession();
app.UseHttpsRedirection();

app.UseAuthorization();

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
