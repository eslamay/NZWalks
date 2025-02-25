using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalksAPI.Models.Domain;
using NZWalksAPI.Models.DTO;
using NZWalksAPI.Repositories;

namespace NZWalksAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ImagesController : ControllerBase
	{
		private readonly IImageRepository imageRepository;

		public ImagesController(IImageRepository imageRepository)
		{
			this.imageRepository = imageRepository;
		}
		[HttpPost]
		[Route("Upload")]
		public async Task<IActionResult> Upload([FromForm] ImageUPloadRequestDto request)
		{
			ValidFileUpload(request);

			if (ModelState.IsValid)
			{
				// Dto to Domain 
				var imageDomainModel = new Image
				{
					File= request.File,
					FileExtension=Path.GetExtension(request.File.FileName),
					FileName=request.FileName,
					FileSizeInBytes=request.File.Length,
					FileDescription=request.FileDescription,
				};
				//User Repository to Upload Image
				await imageRepository.Upload(imageDomainModel);
				return Ok(imageDomainModel);
			}

			return BadRequest(ModelState);
		}

		private void ValidFileUpload(ImageUPloadRequestDto request)
		{
			var allowedExtension = new string[] { ".jpg", ".jpeg", ".png" };

			if (!allowedExtension.Contains(Path.GetExtension(request.File.FileName)))
			{
				ModelState.AddModelError("file", "Unsupported file extension");
			}

			if (request.File.Length > 10485760)
			{
				ModelState.AddModelError("file", "Unsupported file size is more than 10MB ");
			}
		}
	}
}
