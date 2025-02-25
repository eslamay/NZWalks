using AutoMapper;
using NZWalksAPI.Models.Domain;
using NZWalksAPI.Models.DTO;

namespace NZWalksAPI.Mappings
{
	public class AutoMapperProfiles:Profile
	{
		public AutoMapperProfiles()
		{
			CreateMap<Region,RegionDto>().ReverseMap();

			CreateMap<AddRegionRequestDto,Region>().ForMember(dest => dest.RegionImageUrl, opt => opt.Ignore()); ;

			CreateMap<UpdateRegionRequestDto,Region>().ForMember(dest => dest.RegionImageUrl, opt => opt.Ignore());

			CreateMap<AddWalkRequestDto,Walk>().ForMember(dest => dest.WalkImageUrl, opt => opt.Ignore());

			CreateMap<UpdateWalkRequestDto, Walk>().ForMember(dest => dest.WalkImageUrl, opt => opt.Ignore());

			CreateMap<Walk,WalkDto>().ReverseMap();

			CreateMap<Difficulty,DifficultyDto>().ReverseMap();
		}
	}
}
