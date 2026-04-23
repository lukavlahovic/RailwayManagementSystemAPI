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
            CreateMap<CreateTrainTypeDto, TrainType>();
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

            // Trip
            CreateMap<CreateTripDto, Trip>();
            CreateMap<Trip, TripResponseDto>()
                .ForMember(dest => dest.SerialNumber, opt => opt.MapFrom(src => src.Train.SerialNumber))
                .ForMember(dest => dest.TrainTypeName, opt => opt.MapFrom(src => src.Train.TrainType.Name))
                .ForMember(dest => dest.RouteName, opt => opt.MapFrom(src => src.Route.Name));
            CreateMap<Trip, TripScheduleDto>()
                .ForMember(dest => dest.TripId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Train, opt => opt.MapFrom(src => src.Train.SerialNumber))
                .ForMember(dest => dest.Route, opt => opt.MapFrom(src => src.Route.Name));

            // Delay
            CreateMap<CreateDelayDto, Delay>();
            CreateMap<Delay, DelayResponseDto>()
                .ForMember(dest => dest.StationName, opt => opt.MapFrom(src => src.Station.Name));
        }
    }
}
