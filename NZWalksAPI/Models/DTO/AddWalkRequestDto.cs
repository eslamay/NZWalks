using System.ComponentModel.DataAnnotations;

namespace NZWalksAPI.Models.DTO
{
	public class AddWalkRequestDto
	{
		[Required]
		[MaxLength(100)]
		public string Name { get; set; }

		[Required]
		[MaxLength(1000)]
		public string Description { get; set; }

		[Required]
		[Range(0, 100)]
		public double LengthInkm { get; set; }
		public IFormFile? WalkImage { get; set; }

		[Required]
		public Guid DifficultyId { get; set; }

		[Required]
		public Guid RegionId { get; set; }
	}
}
