using Business.Interfaces;
using Entity.Business;
using Entity.Database;
using Helper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class GetFileSOAPManager : IGetFileSOAPService
    {
        private readonly ILogger<GetFileSOAPManager> _logger;
        private ILogService _logService;
        public LogDto logDto = null;
        public GetFileSOAPManager(ILogService logService, ILogger<GetFileSOAPManager> logger)
        {
            _logService = logService;
            _logger = logger;
        }
        public FileModelResponse GetFileDataSOAP(string apiUrl, string filename)
        {
            try
            {
                GetFileDataSOAPService.Service1SoapClient connection = new GetFileDataSOAPService.Service1SoapClient(GetFileDataSOAPService.Service1SoapClient.EndpointConfiguration.Service1Soap);
                GetFileDataSOAPService.VEDocument data = connection.GetFileData(filename);
                int statusCode = statusCode = data.success == true ? statusCode = 200 : statusCode = 404;
                logDto = ExceptionHelper.CreateLogModel(statusCode, ExceptionHelper.ReportInfoDailyDocuments);
                _logService.Insert(logDto);
                _logger.Log(LogLevel.Information, logDto.Log_Source + ":" + logDto.Log_Message + ":" + logDto.Log_StatusCode + ":" + logDto.Log_Date);
                var model = new FileModelResponse()
                {
                    Data = data.data,
                    Success = data.success,
                    Message = data.message
                };
                return model;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Information, ExceptionHelper.GetFileDataSOAP + ":" + ex.Message);
                throw;
            }
        }
    }
}

