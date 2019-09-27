using AutoMapper;
using Bll.Queries.Folder;
using WebHost.ClientApi.Folder;

namespace WebHost.Infrasctructure
{
    public sealed class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            CreateMap<FolderInfo, FolderDto>();
        }
    }
}