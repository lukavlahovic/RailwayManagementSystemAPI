using AutoMapper;
using RailwayManagementSystemAPI.Dtos;
using RailwayManagementSystemAPI.Models;

namespace RailwayManagementSystemAPI.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Station
            CreateMap<StationDto, Station>();
            CreateMap<Station, StationResponseDto>();

            // TypeTrain
            CreateMap<CreateTrainTypeDto, TrainType>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type));
            CreateMap<TrainType, TrainTypeResponseDto>()
                .ForMember(dest => dest.TypeOfTrain, opt => opt.MapFrom(src => src.Type));

            // Train
            CreateMap<CreateTrainDto, Train>();
            CreateMap<Train, TrainResponseDto>()
                .ForMember(dest => dest.TrainType, opt => opt.MapFrom(src => src.TrainType));

            // RouteStation
            CreateMap<RouteStationDto, RouteStation>();
            CreateMap<RouteStation, RouteStationResponseDto>()
                .ForMember(dest => dest.StationName, opt => opt.MapFrom(src => src.Station.Name));

            // Route
            CreateMap<CreateRouteDto, Models.Route>()
                .ForMember(dest => dest.RouteStations, opt => opt.MapFrom(src => src.Stations));
            CreateMap<Models.Route, RouteResponseDto>()
                .ForMember(dest => dest.Stations, opt => opt.MapFrom(src =>
                    src.RouteStations.OrderBy(rs => rs.Order)));
        }
    }
}
