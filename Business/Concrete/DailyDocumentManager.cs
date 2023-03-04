using AutoMapper;
using Business.Interfaces;
using Business.MapperProfile;
using Entity.Business;
using Entity.Database;
using Helper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class DailyDocumentManager : IDailyDocumentService
    {
        private readonly ILogger<DailyDocumentManager> _logger;
        public LogDto logDto = null;
        private ILogService _logService;
        public ServiceResult<List<FileListModel>> response = null;
        public DailyDocumentManager(ILogService logService, ILogger<DailyDocumentManager> logger)
        {
            _logService = logService;
            _logger = logger;
        }
        public async Task<T> ReportInfoDailyDocuments<T>(string apiUrl)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var result = await client.GetAsync(apiUrl);
                    int statusCode = ((int)result.StatusCode);
                    result.EnsureSuccessStatusCode();
                    string resultContentString = await result.Content.ReadAsStringAsync();
                    T resultContent = JsonConvert.DeserializeObject<T>(resultContentString)!;
                    logDto = ExceptionHelper.CreateLogModel(statusCode, ExceptionHelper.ReportInfoDailyDocuments);
                    //_logService.Insert(logDto);
                    _logger.Log(LogLevel.Information, logDto.Log_Source + ":" + logDto.Log_Message + ":" + logDto.Log_StatusCode + ":" + logDto.Log_Date);
                    return resultContent;
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Information, ExceptionHelper.ReportInfoDailyDocuments +":"+ex.Message);
                throw;
            }
        }
        public ServiceResult<List<Document>> GetByCreateUser(string apiUrl, string createUser)
        {
            try
            {
                var responseData = MapProfile.Mapper.Map<ServiceResult<List<Document>>>(ReportInfoDailyDocuments<DocumentResponse>(apiUrl).Result);
                if (responseData.Success)
                {
                    responseData.Data = responseData.Data.Where(x => !String.IsNullOrEmpty(createUser) ? x.CreateUser == createUser : true).ToList();
                }
                return responseData;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Information, ExceptionHelper.GetByCreateUser + ":" + ex.Message);
                throw;
            }
        }

        public ServiceResult<List<Document>> GetByCreateDate(string apiUrl, int startHour, int finishHour)
        {
            try
            {
                DateTime startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, startHour != default ? startHour : 0, 0, 0);
                DateTime finishDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, finishHour != default ? finishHour : 23, 0, 0);
                var responseData = MapProfile.Mapper.Map<ServiceResult<List<Document>>>(ReportInfoDailyDocuments<DocumentResponse>(apiUrl).Result);
                if (responseData.Success)
                {
                    responseData.Data = responseData.Data.Where(x => (Convert.ToDateTime(x.CreateDate) >= startDate) && (Convert.ToDateTime(x.CreateDate) <= finishDate)).ToList();
                }
                return responseData;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Information, ExceptionHelper.GetByCreateDate + ":" + ex.Message);
                throw;
            }
        }
        public ServiceResult<List<FileListModel>> PdfCreateByCreateUser(string apiUrl, string createUser)
        {
            List<FileListModel> list = new List<FileListModel>();
            FileListModelResponse modelList = new FileListModelResponse();
            try
            {
                var responseData = GetByCreateUser(apiUrl, createUser);
                if (responseData.Success)
                {
                    var filterData = responseData.Data.Select(x => x.DocumentName).ToList();
                    foreach (var item in filterData)
                    {
                        var model = new FileListModel()
                        {
                            Filename = item,
                            Guid = Guid.NewGuid()
                        };
                        list.Add(model);
                    }
                    modelList.FileList = list;
                    modelList.Exception = responseData.Exception;
                    modelList.Success = responseData.Success;
                    response = MapProfile.Mapper.Map<ServiceResult<List<FileListModel>>>(modelList);
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Information, ExceptionHelper.PdfCreateByCreateUser + ":" + ex.Message);
                throw;
            }
        }

        public ServiceResult<List<FileListModel>> PdfCreateByCreateDate(string apiUrl, int startHour, int finishHour)
        {
            List<FileListModel> list = new List<FileListModel>();
            FileListModelResponse modelList = new FileListModelResponse();
            try
            {
                var responseData = GetByCreateDate(apiUrl, startHour, finishHour);
                if (responseData.Success)
                {
                    var filterData = responseData.Data.Select(x => x.DocumentName).ToList();
                    foreach (var item in filterData)
                    {
                        var model = new FileListModel()
                        {
                            Filename = item,
                            Guid = Guid.NewGuid()
                        };
                        list.Add(model);
                    }
                    modelList.FileList = list;
                    modelList.Exception = responseData.Exception;
                    modelList.Success = responseData.Success;
                    response = MapProfile.Mapper.Map<ServiceResult<List<FileListModel>>>(modelList);
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Information, ExceptionHelper.PdfCreateByCreateDate + ":" + ex.Message);
                throw;
            }
        }
    }
}
