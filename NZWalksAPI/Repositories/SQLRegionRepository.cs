using Microsoft.EntityFrameworkCore;
using NZWalksAPI.Data;
using NZWalksAPI.Models.Domain;

namespace NZWalksAPI.Repositories
{
	public class SQLRegionRepository : IRegionRepository
	{
		private readonly NZWalksDbContext dbContext;

		public SQLRegionRepository(NZWalksDbContext dbContext)
		{
			this.dbContext = dbContext;
		}


		public async Task<List<Region>> GetAllAsync()
		{
		    return await dbContext.Regions.ToListAsync();
		}

		public async Task<Region?> GetByIdAsync(Guid id)
		{
			return await dbContext.Regions.FindAsync(id);
		}

        public async Task<Region> CreateAsync(Region region)
		{
			await dbContext.Regions.AddAsync(region);
			await dbContext.SaveChangesAsync();
			return region;
		}

		public async Task<Region?> UpdateAsync(Guid id, Region region)
		{
			var existingRegion = await dbContext.Regions.FindAsync(id);
			if (existingRegion == null)
			{
				return null;
			}

			existingRegion.Code = region.Code;
			existingRegion.Name = region.Name;


			if (!string.IsNullOrEmpty(region.RegionImageUrl) && !string.IsNullOrEmpty(existingRegion.RegionImageUrl))
			{
				var oldImagePath = Path.Combine("wwwroot", existingRegion.RegionImageUrl.TrimStart('/'));
				if (File.Exists(oldImagePath))
				{
					File.Delete(oldImagePath);
				}

				existingRegion.RegionImageUrl = region.RegionImageUrl;
			}

			await dbContext.SaveChangesAsync();
			return existingRegion;
		}

		public async Task<Region?> DeleteAsync(Guid id)
		{
			var existingRegion = await dbContext.Regions.FindAsync(id);

			if (existingRegion == null) 
			{
				return null; 
			}

			if (!string.IsNullOrEmpty(existingRegion.RegionImageUrl))
			{
				var imagePath = Path.Combine("wwwroot", existingRegion.RegionImageUrl.TrimStart('/'));
				if (File.Exists(imagePath))
				{
					File.Delete(imagePath);
				}
			}

			dbContext.Regions.Remove(existingRegion);
			await dbContext.SaveChangesAsync();
			return existingRegion;
		}
	}
}
