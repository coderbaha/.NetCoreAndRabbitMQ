using System.Collections.Generic;

namespace Entity.Business
{
    public class FileListModelResponse
    {
        public List<FileListModel> FileList { get; set; }
        public bool Success { get; set; }
        public string Exception { get; set; }
    }
}
