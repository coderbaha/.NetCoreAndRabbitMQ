using Business.Interfaces;
using Entity.Database;
using Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PdfSearchController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly ILogger<PdfSearchController> _logger;
        private IGetFileService _getFileService;
        private ILogService _logService;
        private string _pdfFilesPath;
        public LogDto logDto = null;
        public PdfSearchController(IConfiguration configuration, ILogService logService,
             IGetFileService getFileService, ILogger<PdfSearchController> logger)
        {
            _configuration = configuration;
            _getFileService = getFileService;
            _logService = logService;
            _logger = logger;
            _pdfFilesPath = _configuration["PdfFilesPath"];
        }

        [HttpGet("PdfCheck")]
        [ResponseCache(CacheProfileName = "Duration45")]
        public IActionResult PdfCheck(string guid)
        {
            try
            {
                logDto = ExceptionHelper.CreateLogModel(Response.StatusCode, ExceptionHelper.PdfCheck);
                _logService.Insert(logDto);
                _logger.Log(LogLevel.Information, logDto.Log_Source + ":" + logDto.Log_Message + ":" + logDto.Log_StatusCode + ":" + logDto.Log_Date);
                return Ok(_getFileService.CheckPdfControl(_pdfFilesPath, guid));
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Information, ExceptionHelper.PdfCheck +":"+ex.Message);
                return BadRequest("Beklenmedik hata oluştu");
            }
        }
        [HttpGet("GetPdfCheck")]
        [ResponseCache(CacheProfileName = "Duration45")]
        public IActionResult GetPdfCheck(string guid)
        {
            try
            {
                logDto = ExceptionHelper.CreateLogModel(Response.StatusCode, ExceptionHelper.GetPdfCheck);
                _logService.Insert(logDto);
                _logger.Log(LogLevel.Information, logDto.Log_Source + ":" + logDto.Log_Message + ":" + logDto.Log_StatusCode + ":" + logDto.Log_Date);
                return Ok(_getFileService.GetPdfControl(_pdfFilesPath, guid));
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Information, ExceptionHelper.GetPdfCheck + ":" + ex.Message);
                return BadRequest("Beklenmedik hata oluştu");
            }
        }
    }
}
