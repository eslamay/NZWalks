using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZWalksAPI.CustomActionFilters;
using NZWalksAPI.Data;
using NZWalksAPI.Models.Domain;
using NZWalksAPI.Models.DTO;
using NZWalksAPI.Repositories;

namespace NZWalksAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class RegionsController : ControllerBase
	{
		private readonly IRegionRepository regionRepository;
		private readonly IMapper mapper;
		private readonly IHttpContextAccessor httpContextAccessor;

		public RegionsController(IRegionRepository regionRepository,IMapper mapper, IHttpContextAccessor httpContextAccessor)
		{
			this.regionRepository = regionRepository;
			this.mapper = mapper;
			this.httpContextAccessor = httpContextAccessor;
		}

		[HttpGet]
		public async Task <IActionResult> GetAll()
		{
			var regions = await regionRepository.GetAllAsync();

			var regionDto=mapper.Map<List<RegionDto>>(regions);
            
			return Ok(regionDto);
		}

		[HttpGet]
		[Route("{id:Guid}")]
		public async Task <IActionResult> GetById(Guid id) 
		{
           		var region=await regionRepository.GetByIdAsync(id);
			if (region==null)
			{
				return NotFound();
			}

			var regionDto = mapper.Map<RegionDto>(region);
			return Ok(regionDto);
		}

		[HttpPost]
		[ValidateModel]
		public async Task<IActionResult> Create([FromForm] AddRegionRequestDto addRegionRequestDto)
		{
			string imageUrl = null;

			if (addRegionRequestDto.RegionImage != null && addRegionRequestDto.RegionImage.Length > 0)
			{
				var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

				if (!Directory.Exists(uploadsFolder))
				{
					Directory.CreateDirectory(uploadsFolder);
				}

				var fileExtension = Path.GetExtension(addRegionRequestDto.RegionImage.FileName);
				var uniqueFileName = $"{addRegionRequestDto.Name}{fileExtension}";
				var filePath = Path.Combine(uploadsFolder, uniqueFileName);

				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					await addRegionRequestDto.RegionImage.CopyToAsync(stream);
				}

				imageUrl = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}{httpContextAccessor.HttpContext.Request.PathBase}/images/{uniqueFileName}";
			}

			var regionDomainModel = mapper.Map<Region>(addRegionRequestDto);

			regionDomainModel.RegionImageUrl = imageUrl;

			regionDomainModel=await regionRepository.CreateAsync(regionDomainModel);

			var regionDto = mapper.Map<RegionDto>(regionDomainModel);
				
			return CreatedAtAction(nameof(GetById), new {id=regionDto.Id},regionDto);
		}

		[HttpPut]
		[Route("{id:Guid}")]
		[ValidateModel]
		public async Task<IActionResult> Update([FromRoute] Guid id, [FromForm] UpdateRegionRequestDto updateRegionRequestDto)
		{

			var regionDomainModel = mapper.Map<Region>(updateRegionRequestDto);

			if (updateRegionRequestDto.RegionImage != null)
			{
				var fileExtension = Path.GetExtension(updateRegionRequestDto.RegionImage.FileName);
				var fileName = $"{updateRegionRequestDto.Name}{fileExtension}";
				var filePath = Path.Combine("wwwroot/images", fileName);

				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					await updateRegionRequestDto.RegionImage.CopyToAsync(stream);
				}

				regionDomainModel.RegionImageUrl = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}{httpContextAccessor.HttpContext.Request.PathBase}/images/{fileName}";
			}

			regionDomainModel =await regionRepository.UpdateAsync(id, regionDomainModel);

			if (regionDomainModel == null)
			{
				return NotFound();
			}

			var regionDto = mapper.Map<RegionDto>(regionDomainModel);

			return Ok(regionDto);
		}

		[HttpDelete]
		[Route("{id:Guid}")]
		public async Task<IActionResult> Delete([FromRoute] Guid id)
		{
			var regionDomainModel=await regionRepository.DeleteAsync(id);

			if (regionDomainModel == null) 
			{
				return NotFound();
			}

            var regionDto = mapper.Map<RegionDto> (regionDomainModel);

			return Ok(regionDto);
		}
	}
}
