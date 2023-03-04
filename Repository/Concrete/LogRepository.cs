using System;
using System.Collections.Generic;
using Dapper;
using Entity.Database;
using Helper;
using Microsoft.Extensions.Logging;
using Repository.Interfaces;

namespace Repository.Concrete
{
    public class LogRepository : ILogRepository
    {
        private readonly ILogger<LogRepository> _logger;
        private readonly DapperContext _context;
        public LogRepository(DapperContext context, ILogger<LogRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        public List<LogDto> GetAll()
        {
            throw new NotImplementedException();
        }

        public void Add(LogDto entity)
        {
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var query = "INSERT INTO WEB959LOGTABLE (log_message,log_source,log_statuscode,log_date) VALUES (:Log_Message,:Log_Source,:Log_StatusCode,:Log_Date)";
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add(":log_message", entity.Log_Message);
                    parameters.Add(":log_source", entity.Log_Source);
                    parameters.Add(":log_statuscode", entity.Log_StatusCode);
                    parameters.Add(":log_date", entity.Log_Date);
                    var id = connection.QuerySingle<int>(query, parameters);
                }
                catch (Exception ex)
                {
                    _logger.Log(LogLevel.Information, ExceptionHelper.LogRepositoryAdd + ":" + ex.Message);
                    connection.Dispose();
                    throw;
                }
            }
        }

        public void Update(LogDto entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(LogDto entity)
        {
            throw new NotImplementedException();
        }
    }
}