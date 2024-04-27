using NLog;

namespace TrackingSystem.Api
{
    public class Program
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        public static void Main(string[] args)
        {
            try
            {
                logger.Info("Запуск программы");

                var host = CreateHostBuilder(args).Build();

                using var scope = host.Services.CreateScope();
                var services = scope.ServiceProvider;

                host.Run();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Аварийное завершение");
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.AddConsole();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
