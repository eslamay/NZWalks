using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalksAPI.CustomActionFilters;
using NZWalksAPI.Models.Domain;
using NZWalksAPI.Models.DTO;
using NZWalksAPI.Repositories;

namespace NZWalksAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class WalksController : ControllerBase
	{
		private readonly IMapper mapper;
		private readonly IWalkRepository walkRepository;
		private readonly IHttpContextAccessor httpContextAccessor;

		public WalksController(IMapper mapper,IWalkRepository walkRepository,IHttpContextAccessor httpContextAccessor)
		{
			this.mapper = mapper;
			this.walkRepository = walkRepository;
			this.httpContextAccessor = httpContextAccessor;
		}

		[HttpGet]
		public async Task<IActionResult> GetAll([FromQuery] string? filterOn, [FromQuery]string?filterQuery,
			[FromQuery] string? sortBy, [FromQuery]bool?isAscending,
			[FromQuery]int pageNumber=1, [FromQuery]int pageSize=1000) 
		{
		    var walkDomainModel=await walkRepository.GetAllAsync(filterOn,filterQuery,sortBy,isAscending??true,pageNumber,pageSize);

			var walkDto=mapper.Map<List<WalkDto>>(walkDomainModel);

			return Ok(walkDto);
		}

		[HttpGet]
		[Route("{id:Guid}")]
		public async Task<IActionResult> GetById([FromRoute]Guid id)
		{
            var walkDomainModel=await walkRepository.GetByIdAsync(id);

			if (walkDomainModel == null)
			{
				return NotFound();
			}

			var walkDto=mapper.Map<WalkDto>(walkDomainModel);

			return Ok(walkDto);
		}

		[HttpPost]
		[ValidateModel]
		public async Task<IActionResult> Create([FromForm] AddWalkRequestDto addWalkRequestDto)
		{
			    string imageUrl = null;

			if (addWalkRequestDto.WalkImage != null && addWalkRequestDto.WalkImage.Length > 0)
			{
				var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

				if (!Directory.Exists(uploadsFolder))
				{
					Directory.CreateDirectory(uploadsFolder);
				}

				var fileExtension = Path.GetExtension(addWalkRequestDto.WalkImage.FileName); 
				var uniqueFileName = $"{addWalkRequestDto.Name}{fileExtension}";
				var filePath = Path.Combine(uploadsFolder, uniqueFileName);

				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					await addWalkRequestDto.WalkImage.CopyToAsync(stream);
				}

				imageUrl = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}{httpContextAccessor.HttpContext.Request.PathBase}/images/{uniqueFileName}";
			}

			var walkDomainModel = mapper.Map<Walk>(addWalkRequestDto);

			walkDomainModel.WalkImageUrl = imageUrl;

			await walkRepository.CreateAsync(walkDomainModel);

			var walkDto = mapper.Map<WalkDto>(walkDomainModel);

				return Ok(walkDto);

		}

		[HttpPut]
		[Route("{id:Guid}")]
		[ValidateModel]
		public async Task<IActionResult> Update([FromRoute] Guid id, [FromForm] UpdateWalkRequestDto updateWalkRequestDto)
		{
				var walkDomainModel = mapper.Map<Walk>(updateWalkRequestDto);

			if (updateWalkRequestDto.WalkImage != null)
			{
				var fileExtension = Path.GetExtension(updateWalkRequestDto.WalkImage.FileName);
				var fileName = $"{updateWalkRequestDto.Name}{fileExtension}";
				var filePath = Path.Combine("wwwroot/images", fileName);

				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					await updateWalkRequestDto.WalkImage.CopyToAsync(stream);
				}

				walkDomainModel.WalkImageUrl = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}{httpContextAccessor.HttpContext.Request.PathBase}/images/{fileName}";
			}

			walkDomainModel = await walkRepository.UpdateAsync(id, walkDomainModel);

				if (walkDomainModel == null)
				{
					return NotFound();
				}

				var walkDto = mapper.Map<WalkDto>(walkDomainModel);

				return Ok(walkDto);
		}

		[HttpDelete]
		[Route("{id:Guid}")]
		public async Task<IActionResult> Delete([FromRoute]Guid id)
		{
			var deletedWalkDomainModel= await walkRepository.DeleteAsync(id);

			if (deletedWalkDomainModel == null)
			{
				return NotFound();
			}

			var walkDto= mapper.Map<WalkDto>(deletedWalkDomainModel);
			return Ok(walkDto);
		}
	}
}
