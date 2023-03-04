using AutoMapper;
using Entity.Business;
using Entity.Business.View;

namespace API.MapperProfile
{
    public class MapProfile : Profile
    {
        public MapProfile()
        {
            CreateMap<DocumentView, Document>().ReverseMap();
            CreateMap<FileListModelView, FileListModel>().ReverseMap();
        }
    }
}
