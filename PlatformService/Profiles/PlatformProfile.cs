using AutoMapper;
using PlatformService.Dtos;
using PlatfromService.Models;

namespace PlatformService.Profiles
{
    public class PlatformProfile : Profile
    {
        public PlatformProfile()
        {
            // Sources (Models) -> Target (DTOs)
            CreateMap<Platform, PlatformReadDto>();
            CreateMap<Platform, PlatformCreateDto>();
        }
    }
}