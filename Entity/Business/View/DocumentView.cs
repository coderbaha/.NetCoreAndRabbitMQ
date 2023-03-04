using System;

namespace Entity.Business.View
{
    public class DocumentView
    {
        public string DocumentName { get; set; }
        public long PolicyNumber { get; set; }
        public string Status { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
