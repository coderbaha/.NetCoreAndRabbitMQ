using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Business.Interfaces;
using Entity.Business;
using Entity.Business.View;
using Entity.Database;
using Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Publisher.Publisher;

namespace API.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DailyDocumentController : ControllerBase
    {
        private IDailyDocumentService _documentService;
        private readonly ILogger<DailyDocumentController> _logger;
        private ILogService _logService;
        private IConfiguration _configuration;
        private IGetFileService _getFileService;
        private IMapper _mapper;
        private RabbitMQPublisher _rabbitMQPublisher;
        private string _getDailyDocumentApiUrl;
        private string _getFileDataApiUrl;
        private string _pdfFilesPath;
        public LogDto logDto = null;
        public FileModelResponse model = null;
        public DailyDocumentController(IDailyDocumentService documentService, IConfiguration configuration,
             IGetFileService getFileService, IGetFileSOAPService getFileSOAPService, IMapper mapper, ILogService logService, ILogger<DailyDocumentController> logger, RabbitMQPublisher rabbitMQPublisher)
        {
            _documentService = documentService;
            _configuration = configuration;
            _getFileService = getFileService;
            _mapper = mapper;
            _logger = logger;
            _logService = logService;
            _getDailyDocumentApiUrl = _configuration["GetDailyDocument"];
            _getFileDataApiUrl = _configuration["GetFileData"];
            _pdfFilesPath = _configuration["PdfFilesPath"];
            _rabbitMQPublisher = rabbitMQPublisher;
        }

        [HttpGet("ReportInfoDailyDocumentsByCreateUser")]
        [ResponseCache(CacheProfileName = "Duration45")]
        public IActionResult ReportInfoDailyDocumentsByCreateUser(string createUser)
        {
            try
            {
                var responseData = _mapper.Map<List<DocumentView>>(_documentService.GetByCreateUser(_getDailyDocumentApiUrl, createUser).Data).ToList();
                if (responseData == null)
                {
                    return BadRequest("Bulunamadı");
                }

                logDto = ExceptionHelper.CreateLogModel(Response.StatusCode, ExceptionHelper.ReportInfoDailyDocumentsByCreateUser);
                //_logService.Insert(logDto);
                return Ok(responseData);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ExceptionHelper.ReportInfoDailyDocumentsByCreateUser + ":" + ex.Message);
                return BadRequest("Beklenmedik hata oluştu");
            }
        }
        [HttpGet("ReportInfoDailyDocumentsByCreateDate")]
        [ResponseCache(CacheProfileName = "Duration45")]
        public IActionResult ReportInfoDailyDocumentsByCreateDate(int startHour, int finishHour)
        {
            try
            {
                var responseData = _mapper.Map<List<DocumentView>>(_documentService.GetByCreateDate(_getDailyDocumentApiUrl, startHour, finishHour).Data.ToList());
                if (responseData == null)
                {
                    return BadRequest("Bulunamadı");
                }
                logDto = ExceptionHelper.CreateLogModel(Response.StatusCode, ExceptionHelper.ReportInfoDailyDocumentsByCreateDate);
                //_logService.Insert(logDto);
                _logger.Log(LogLevel.Information, logDto.Log_Source + ":" + logDto.Log_Message + ":" + logDto.Log_StatusCode + ":" + logDto.Log_Date);
                return Ok(responseData);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ExceptionHelper.ReportInfoDailyDocumentsByCreateDate + ":" + ex.Message);
                return BadRequest("Beklenmedik hata oluştu");
            }
        }
        [HttpGet("PdfCreateTriggerByCreateUser")]
        [ResponseCache(CacheProfileName = "Duration45")]
        public IActionResult PdfCreateTriggerByCreateUser(string createUser)
        {
            try
            {
                var responseData = _documentService.PdfCreateByCreateUser(_getDailyDocumentApiUrl, createUser);
                if (!responseData.Success)
                {
                    return BadRequest(Response.StatusCode);
                }
                List<RabbitMQFileModel> rabbitMQModel = new List<RabbitMQFileModel>();
                foreach (var item in responseData.Data)
                {
                    model = _getFileService.GetFileData<FileModelResponse>(_getFileDataApiUrl, item.Filename).Result;
                    var temp = new RabbitMQFileModel()
                    {
                        Data = model.Data,
                        Filename = item.Guid,
                    };
                    rabbitMQModel.Add(temp);
                }
                _rabbitMQPublisher.Publish(rabbitMQModel);
                return Ok(_mapper.Map<List<FileListModelView>>(responseData.Data));
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ExceptionHelper.PdfCreateTriggerByCreateUser + ":" + ex.Message);
                return BadRequest("Beklenmedik hata oluştu");
            }
        }
        [HttpGet("PdfCreateTriggerByCreateDate")]
        [ResponseCache(CacheProfileName = "Duration45")]
        public IActionResult PdfCreateTriggerByCreateDate(int startHour, int finishHour)
        {
            try
            {
                var responseData = _documentService.PdfCreateByCreateDate(_getDailyDocumentApiUrl, startHour, finishHour);
                if (!responseData.Success)
                {
                    return BadRequest(Response.StatusCode);
                }

                List<RabbitMQFileModel> rabbitMQModel = new List<RabbitMQFileModel>();
                foreach (var item in responseData.Data)
                {
                    model = _getFileService.GetFileData<FileModelResponse>(_getFileDataApiUrl, item.Filename).Result;
                    var temp = new RabbitMQFileModel()
                    {
                        Data = model.Data,
                        Filename = item.Guid,
                    };
                    rabbitMQModel.Add(temp);
                }
                _rabbitMQPublisher.Publish(rabbitMQModel);
                return Ok(_mapper.Map<List<FileListModelView>>(responseData.Data));
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ExceptionHelper.PdfCreateTriggerByCreateUser + ":" + ex.Message);
                return BadRequest("Beklenmedik hata oluştu");
            }
        }
        [HttpGet("RabbitMQTest")]
        [ResponseCache(CacheProfileName = "Duration45")]
        public IActionResult RabbitMQTest(string user)
        {
            try
            {
                _rabbitMQPublisher.Publish(user);
                return Ok("Başarılı");
            }
            catch (Exception ex)
            {
                return BadRequest("Beklenmedik hata oluştu");
            }
        }
    }
}
