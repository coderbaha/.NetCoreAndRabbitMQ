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
using System;
using System.Collections.Generic;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DailyDocumentSOAPController : ControllerBase
    {
        private IDailyDocumentService _documentService;
        private readonly ILogger<DailyDocumentSOAPController> _logger;
        private IConfiguration _configuration;
        private IGetFileService _getFileService;
        private IGetFileSOAPService _getFileSOAPService;
        private IMapper _mapper;
        private ILogService _logService;
        private RabbitMQPublisher _rabbitMQPublisher;
        private string _getDailyDocumentApiUrl;
        private string _getFileDataSOAPApiUrl;

        private string _pdfFilesPath;
        public FileModelResponse model = null;
        public LogDto logDto = null;
        public DailyDocumentSOAPController(IDailyDocumentService documentService, IConfiguration configuration,
             IGetFileService getFileService, IGetFileSOAPService getFileSOAPService, IMapper mapper, ILogService logService, ILogger<DailyDocumentSOAPController> logger, RabbitMQPublisher rabbitMQPublisher)
        {
            _documentService = documentService;
            _configuration = configuration;
            _getFileService = getFileService;
            _getFileSOAPService = getFileSOAPService;
            _logService = logService;
            _mapper = mapper;
            _logger = logger;
            _getDailyDocumentApiUrl = _configuration["GetDailyDocument"];
            _getFileDataSOAPApiUrl = _configuration["GetFileDataSOAP"];
            _pdfFilesPath = _configuration["PdfFilesPath"];
            _rabbitMQPublisher = rabbitMQPublisher;
        }

        [HttpGet("PdfCreateTriggerByCreateUserSOAPService")]
        [ResponseCache(CacheProfileName = "Duration45")]
        public IActionResult PdfCreateTriggerByCreateUserSOAPService(string createUser)
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
                    model = _getFileService.GetFileData<FileModelResponse>(_getFileDataSOAPApiUrl, item.Filename).Result;
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
                _logger.Log(LogLevel.Error, ExceptionHelper.PdfCreateTriggerByCreateUserSOAPService + ":" + ex.Message);
                return BadRequest("Beklenmedik hata oluştu");
            }
        }
        [HttpGet("PdfCreateTriggerByCreateDateSOAPService")]
        [ResponseCache(CacheProfileName = "Duration45")]
        public IActionResult PdfCreateTriggerByCreateDateSOAPService(int startHour, int finishHour)
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
                    model = _getFileService.GetFileData<FileModelResponse>(_getFileDataSOAPApiUrl, item.Filename).Result;
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
                _logger.Log(LogLevel.Error, ExceptionHelper.PdfCreateTriggerByCreateUserSOAPService + ":" + ex.Message);
                return BadRequest("Beklenmedik hata oluştu");
            }
        }

    }
}
