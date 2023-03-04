using Business.Interfaces;
using Entity.Business;
using Entity.Database;
using Helper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class GetFileManager : IGetFileService
    {
        private readonly ILogger<GetFileManager> _logger;
        private ILogService _logService;
        public LogDto logDto = null;
        public GetFileManager(ILogService logService, ILogger<GetFileManager> logger)
        {
            _logService = logService;
            _logger = logger;
        }
        public async Task<T> GetFileData<T>(string apiUrl, string filename)
        {
            try
            {
                var model = new FileListModel()
                {
                    Filename = filename
                };
                var json = JsonConvert.SerializeObject(model);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                using (var client = new HttpClient())
                {
                    var result = await client.PostAsync(apiUrl, data);
                    int statusCode = ((int)result.StatusCode);
                    string response = result.Content.ReadAsStringAsync().Result;
                    T resultContent = JsonConvert.DeserializeObject<T>(response);
                    logDto = ExceptionHelper.CreateLogModel(statusCode, ExceptionHelper.GetFileData);
                    //_logService.Insert(logDto);
                    _logger.Log(LogLevel.Information, logDto.Log_Source + ":" + logDto.Log_Message + ":" + logDto.Log_StatusCode + ":" + logDto.Log_Date);
                    return resultContent;
                }
            }

            catch (Exception ex)
            {
                _logger.Log(LogLevel.Information, ExceptionHelper.GetFileData + ":" + ex.Message);
                throw;
            }
        }

        public void PdfCreater(string path, Byte[] data, Guid guid)
        {
            try
            {
                File.WriteAllBytes(path + guid + ".pdf", data);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Information, ExceptionHelper.PdfCreater + ":" + ex.Message);
            }
        }
        public string CheckPdfControl(string path, string filename)
        {
            try
            {
                DirectoryInfo info = new DirectoryInfo(path);
                return info.GetFiles("*" + filename + ".pdf").Length > 0 ? "Dosya oluşturuldu" : "Dosya oluşturulamadı";
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Information, ExceptionHelper.CheckPdfControl + ":" + ex.Message);
                throw;
            }
        }
        public string GetPdfControl(string path, string filename)
        {
            try
            {
                DirectoryInfo info = new DirectoryInfo(path);
                return info.GetFiles("*" + filename + ".pdf").Length > 0 ? path + filename + ".pdf" : "Dosya bulunamadı";
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Information, ExceptionHelper.GetPdfControl + ":" + ex.Message);
                throw;
            }
        }

    }
}
