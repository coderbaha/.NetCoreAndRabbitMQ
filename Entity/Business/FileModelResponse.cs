using System;

namespace Entity.Business
{
    public class FileModelResponse
    {
        public string Message { get; set; }
        public Byte[] Data { get; set; }
        public bool Success { get; set; }
    }
}
