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
                CreateHostBuilder(args).Build().Run();
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
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
