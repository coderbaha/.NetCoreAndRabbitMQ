using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace API.Extensions
{
    public class LoggerProvider : ILoggerProvider
    {
        private readonly IHostEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;

        public LoggerProvider(IHostEnvironment hostingEnvironment, IConfiguration configuration)
        {
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
        }
        public ILogger CreateLogger(string categoryName) => new Logger(_hostingEnvironment, _configuration, categoryName);
        public void Dispose() => throw new NotImplementedException();
    }
}
