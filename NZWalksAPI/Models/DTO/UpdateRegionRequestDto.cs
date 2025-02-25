using System.ComponentModel.DataAnnotations;

namespace NZWalksAPI.Models.DTO
{
	public class UpdateRegionRequestDto
	{
		[Required]
		[MaxLength(100, ErrorMessage = "Name has to be a maximum of 100 characters")]
		public string Name { get; set; }

		[Required]
		[MaxLength(4, ErrorMessage = "Code has to be a maximum of 4 characters")]
		[MinLength(2, ErrorMessage = "Code has to be a minimum of 2 characters")]
		public string Code { get; set; }
		public IFormFile? RegionImage { get; set; }
	}
}
