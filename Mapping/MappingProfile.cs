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
        }
    }
}
