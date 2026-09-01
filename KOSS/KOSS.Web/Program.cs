using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using KOSS.Web.Data;

namespace KOSS.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            // التهيئة التلقائية لقاعدة البيانات والبيانات الأساسية
            try
            {
                await DbInitializer.InitializeAsync(host.Services);
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"DB Init Error: {ex.Message}");
            }

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls("http://localhost:5050");
                });
    }
}
