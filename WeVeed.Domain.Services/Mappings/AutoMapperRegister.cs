using AutoMapper;
using WeVeed.Application.Dtos;
using WeVeed.Application.Dtos.Toko;
using WeVeed.Domain.Entities;
using WeVeed.Domain.Entities.Toko;

namespace WeVeed.Domain.Services.Mappings
{
    public static class AutoMapperRegister
    {
        public static void RegisterMappings(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<User, UserAuthenticateDto>().ForMember(a => a.Id, x => x.MapFrom(s => s.Id.ToString()));
            cfg.CreateMap<User, ProducerListViewDto>().ForMember(a => a.Id, x => x.MapFrom(s => s.Id.ToString()));
            cfg.CreateMap<User, ProducerViewDto>().ForMember(a => a.Id, x => x.MapFrom(s => s.Id.ToString()));
            cfg.CreateMap<User, UserBasicInfoDto>().ForMember(a => a.Id, x => x.MapFrom(s => s.Id.ToString()));

            cfg.CreateMap<Series, ProducerSeriesDto>();
            cfg.CreateMap<Series, SeriesLastEpisodeDto>().ForMember(a => a.Id, x => x.MapFrom(s => s.Id.ToString()));
            cfg.CreateMap<Series, SeriesViewDto>().ForMember(a => a.Id, x => x.MapFrom(s => s.Id.ToString()));
            cfg.CreateMap<Series, SeriesViewListDto>().ForMember(a => a.Id, x => x.MapFrom(s => s.Id.ToString()));
            cfg.CreateMap<Series, SeriesUpdateDto>();

            cfg.CreateMap<Video, VideoDisplayCarouselDto>().ForMember(a => a.Id, x => x.MapFrom(s => s.Id.ToString()));
            cfg.CreateMap<Video, VideoDisplayUiDto>().ForMember(a => a.Id, x => x.MapFrom(s => s.Id.ToString()));
            cfg.CreateMap<Video, VideoUpdateDto>();
            cfg.CreateMap<Video, VideoWatchDto>().ForMember(a => a.Id, x => x.MapFrom(s => s.Id.ToString()));
            cfg.CreateMap<Video, VideoPlayingNowDto>().ForMember(a => a.Id, x => x.MapFrom(s => s.Id.ToString()));

            cfg.CreateMap<Comment, CommentDisplayUiDto>().ForMember(a => a.Id, x => x.MapFrom(s => s.Id.ToString()));

            cfg.CreateMap<TokoRoom, TokoRoomViewDto>();
        }
    }
}
