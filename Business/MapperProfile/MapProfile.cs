using AutoMapper;
using Entity.Business;
using System;
using System.Collections.Generic;

namespace Business.MapperProfile
{
    public class MapProfile : Profile
    {
        public MapProfile()
        {
            CreateMap<ServiceResult<List<Document>>, DocumentResponse>()
                .ForMember(d => d.documentList, o => o.MapFrom(a => a.Data))
                .ForMember(d => d.Success, o => o.MapFrom(a => a.Success))
                .ForMember(d => d.Exception, o => o.MapFrom(a => a.Exception))
                .ReverseMap();
            CreateMap<ServiceResult<List<FileListModel>>, FileListModelResponse>()
                .ForMember(d => d.FileList, o => o.MapFrom(a => a.Data))
                .ForMember(d => d.Success, o => o.MapFrom(a => a.Success))
                .ForMember(d => d.Exception, o => o.MapFrom(a => a.Exception))
                .ReverseMap();

        }
        private static readonly Lazy<IMapper> Lazy = new Lazy<IMapper>(() =>
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.ShouldMapProperty = p => p.GetMethod.IsPublic || p.GetMethod.IsAssembly;
                cfg.AddProfile<MapProfile>();
            });
            var mapper = config.CreateMapper();
            return mapper;
        });
        public static IMapper Mapper => Lazy.Value;
    }
}
