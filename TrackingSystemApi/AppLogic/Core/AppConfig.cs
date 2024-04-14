namespace TrackingSystem.Api.AppLogic.Core
{
    /// <summary>
    /// Конфигурация приложения
    /// appsettings.json
    /// </summary>
    public class AppConfig
    {
        /// <summary>
        /// Строка подключения к БД
        /// </summary>
        public string DBConnectionString { get; set; } = null!;

        /// <summary>
        /// Издатель токена
        /// </summary>
        public string JWTIssuer { get; set; } = null!;

        /// <summary>
        /// Потребитель токена
        /// </summary>
        public string JWTAudience { get; set; } = null!;

        /// <summary>
        /// Ключ для шифрации
        /// </summary>
        public string JWTAccessKey { get; set; } = null!;

        /// <summary>
        /// Ключ для обновления
        /// </summary>
        public string JWTRefreshKey { get; set; } = null!;

        /// <summary>
        /// Почта отправителя
        /// </summary>
        public string SenderEmail { get; set; } = null!;

        /// <summary>
        /// Пароль отправителя
        /// </summary>
        public string SenderPassword { get; set; } = null!;

        /// <summary>
        /// Smtp хост
        /// </summary>
        public string SmtpHost { get; set; } = null!;

        /// <summary>
        /// Smtp порт
        /// </summary>
        public int SmtpPort { get; set; }

        /// <summary>
        /// Включает SSL при отправки сообщения на почту
        /// </summary>
        public bool EnableSsl { get; set; }

        /// <summary>
        /// Логин для доступа в lms.lk.ulstu
        /// </summary>
        public string LkLogin { get; set; } = null!;

        /// <summary>
        /// Пароль для доступа в lms.lk.ulstu
        /// </summary>
        public string LkPassword { get; set; } = null!;
    }
}
