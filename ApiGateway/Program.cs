namespace ApiGateway
{
    using Ocelot.DependencyInjection;
    using Ocelot.Middleware;
    using System.Threading.Tasks;

    public class Program
    {
        public static async Task Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            // Add Ocelot to the services
            builder.Services.AddOcelot();

            // Load Ocelot configuration from the ocelot.json file
            builder.Configuration.AddJsonFile("ocelot.json");

            var app = builder.Build();

            // Use Ocelot middleware for routing
            await app.UseOcelot();

            app.Run();
        }
    }
}
