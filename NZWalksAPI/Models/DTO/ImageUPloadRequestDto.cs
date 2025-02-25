using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NZWalksAPI.Models.DTO
{
	public class ImageUPloadRequestDto
	{

		[Required]
		public IFormFile File { get; set; }

		[Required]
		public string FileName { get; set; }

		[Required]
		public string? FileDescription { get; set; }
	}
}
