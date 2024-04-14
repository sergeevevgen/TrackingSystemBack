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
                logger.Info("������ ���������");

                var host = CreateHostBuilder(args).Build();


                // �������� IServiceScope, ����� ������� IMqttService
                using var scope = host.Services.CreateScope();
                var services = scope.ServiceProvider;

                host.Run();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "��������� ����������");
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
                    webBuilder.UseUrls("http://*:7001").UseStartup<Startup>();
                });
    }
}
