using Microsoft.EntityFrameworkCore;
using NZWalksAPI.Data;
using NZWalksAPI.Models.Domain;

namespace NZWalksAPI.Repositories
{
	public class SQLWalkRepository : IWalkRepository
	{
		private readonly NZWalksDbContext dbContext;

		public SQLWalkRepository(NZWalksDbContext dbContext)
		{
			this.dbContext = dbContext;
		}
		public async Task<Walk> CreateAsync(Walk walk)
		{
			await dbContext.Walks.AddAsync(walk);
			await dbContext.SaveChangesAsync();
			return walk;
		}

		public async Task<List<Walk>> GetAllAsync(string? filterOn = null, string? filterQuery = null,
			string? sortBy = null, bool isAscending = true, int pageNumber = 1, int pageSize = 1000)
		{
			var walks=dbContext.Walks.Include("Difficulty").Include("Region").AsQueryable();

			//filter
			if (string.IsNullOrWhiteSpace(filterOn)==false&& string.IsNullOrWhiteSpace(filterQuery) == false)
			{
				if (filterOn.Equals("Name",StringComparison.OrdinalIgnoreCase))
				{
					walks=walks.Where(x=>x.Name.Contains(filterQuery));
				}
			}

			//sorting
			if (string.IsNullOrWhiteSpace(sortBy) == false)
			{
				if (sortBy.Equals("Name",StringComparison.OrdinalIgnoreCase))
				{
					walks = isAscending ? walks.OrderBy(x => x.Name) : walks.OrderByDescending(x => x.Name);
				}
				else if (sortBy.Equals("Length", StringComparison.OrdinalIgnoreCase))
				{
					walks = isAscending ? walks.OrderBy(x => x.LengthInkm) : walks.OrderByDescending(x => x.LengthInkm);
				}
			}

			//pagination
			var skipResults = (pageNumber - 1) * pageSize;

			return await walks.Skip(skipResults).Take(pageSize).ToListAsync();
			//return await dbContext.Walks.Include("Difficulty").Include("Region").ToListAsync();
		}

		public async Task<Walk?> GetByIdAsync(Guid id)
		{
			return await dbContext.Walks.Include("Difficulty").Include("Region").FirstOrDefaultAsync(x=>x.Id==id);

		}

		public async Task<Walk?> UpdateAsync(Guid id, Walk walk)
		{
			var existingWalk=await dbContext.Walks.FirstOrDefaultAsync(x=>x.Id==id);
			if (existingWalk == null)
			{
				return null;
			}

			existingWalk.Name = walk.Name;
			existingWalk.Description = walk.Description;
			existingWalk.LengthInkm = walk.LengthInkm;
			existingWalk.DifficultyId=walk.DifficultyId;
			existingWalk.RegionId = walk.RegionId;

			if (!string.IsNullOrEmpty(walk.WalkImageUrl) && !string.IsNullOrEmpty(existingWalk.WalkImageUrl))
			{
				var oldImagePath = Path.Combine("wwwroot", existingWalk.WalkImageUrl.TrimStart('/'));
				if (File.Exists(oldImagePath))
				{
					File.Delete(oldImagePath);
				}

				existingWalk.WalkImageUrl = walk.WalkImageUrl; 
			}

			await dbContext.SaveChangesAsync();

			return existingWalk;

		}

        public async Task<Walk?> DeleteAsync(Guid id)
		{
			var existingWalk= await dbContext.Walks.FirstOrDefaultAsync( x=>x.Id==id);

			if(existingWalk == null)
			  { 
				return null;
			  }

			if (!string.IsNullOrEmpty(existingWalk.WalkImageUrl))
			{
				var imagePath = Path.Combine("wwwroot", existingWalk.WalkImageUrl.TrimStart('/'));
				if (File.Exists(imagePath))
				{
					File.Delete(imagePath);
				}
			}

			dbContext.Remove(existingWalk);
			await dbContext.SaveChangesAsync();

			return existingWalk;
		}

	}
}
