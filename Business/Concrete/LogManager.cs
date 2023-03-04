using System;
using Business.Interfaces;
using Entity.Database;
using Helper;
using Microsoft.Extensions.Logging;
using Repository.Interfaces;

namespace Business.Concrete
{
    public class LogManager : ILogService
    {
        private ILogRepository _logRepository;
        private readonly ILogger<LogManager> _logger;
        public LogManager(ILogRepository logRepository, ILogger<LogManager> logger)
        {
            _logRepository = logRepository;
            _logger = logger;
        }
        public void Insert(LogDto logDto)
        {
            try
            {
                if (logDto is not null) _logRepository.Add(logDto);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Information, ExceptionHelper.LogServiceInsert + ":" + ex.Message);
            }
        }
    }
}
