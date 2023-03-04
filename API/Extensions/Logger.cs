using System;
using System.IO;
using Entity.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace API.Extensions
{
    public class Logger : ILogger
    {
        private readonly IHostEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;
        private string _categoryName;
        private CustomLogProvider customLogProvider { get; set; }
        private CustomLogProvider CustomLogProvider
        {
            get
            {
                if (customLogProvider == null)
                {
                    customLogProvider = _configuration.GetSection("CustomLogProvider").Get<CustomLogProvider>();

                }
                return customLogProvider;
            }
        }
        public Logger(IHostEnvironment hostingEnvironment, IConfiguration configuration, string categoryName)
        {
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
            _categoryName = categoryName;
        }
        public IDisposable BeginScope<TState>(TState state) => null;
        public bool IsEnabled(LogLevel logLevel) => true;
        public async void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (CustomLogProvider != null && CustomLogProvider.FileLog.Enable)
            {
                using (StreamWriter streamWriter = new StreamWriter(_configuration["LogFilePath"], true))
                {
                    await streamWriter.WriteLineAsync($"Log Level : {logLevel.ToString()} | Event ID : {eventId.Id} | Event Name : {eventId.Name} | Formatter : {formatter(state, exception)}");
                    streamWriter.Close();
                    await streamWriter.DisposeAsync();
                }
            }
        }
    }
}
