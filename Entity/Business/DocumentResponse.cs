using System.Collections.Generic;

namespace Entity.Business
{
    public class DocumentResponse
    {
        public List<Document> documentList { get; set; }
        public bool Success { get; set; }
        public string Exception { get; set; }
    }
}
