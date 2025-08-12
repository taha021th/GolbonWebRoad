//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Design;
//using Microsoft.Extensions.Configuration;

//namespace GolbonWebRoad.Infrastructure.Persistence
//{
//    public class GolbonWebRoadDbContextFactory : IDesignTimeDbContextFactory<GolbonWebRoadDbContext>
//    {
//        public GolbonWebRoadDbContext CreateDbContext(string[] args)
//        {
//            var basePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"../GolbonWebRoad.Api"));
//            IConfigurationRoot configuration = new ConfigurationBuilder()
//                .SetBasePath(basePath)
//                .AddJsonFile("appsettings.json")
//                .Build();
//            var builder = new DbContextOptionsBuilder<GolbonWebRoadDbContext>();
//            var connectionString = configuration.GetConnectionString("GolbonWebRoadDb");
//            builder.UseNpgsql(connectionString);
//            return new GolbonWebRoadDbContext(builder.Options);
//        }
//    }
//}
