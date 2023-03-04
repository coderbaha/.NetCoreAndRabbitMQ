using System;

namespace Entity.Business
{
    public class RabbitMQFileModel
    {
        public Guid Filename { get; set; }
        public Byte[] Data { get; set; }
    }
}
