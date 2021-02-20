using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CQRS.Sample.Infrastructure.MongoDB.Base;
using CQRS.Sample.Models.Products;

using MongoDB.Driver;

namespace CQRS.Sample.Infrastructure.MongoDB.Repositories
{
	public class ProductsRepository : MongoDBRepository<Product>, IProductsRepository
	{
		public ProductsRepository(SampleContext context)
		{
			if (context is null)
			{
				throw new ArgumentNullException(nameof(context));
			}

			collection = context.Products;
		}

		public async Task<IEnumerable<Product>> Get(int? take, int skip = 0, FilterDefinition<Product> filter = null,
			SortDefinition<Product> sort = null)
		{
			if (take <= 0)
			{
				throw new ArgumentException($"{nameof(take)} arg can't be less or equal 0.", nameof(take));
			}

			if (skip < 0)
			{
				throw new ArgumentException($"{nameof(skip)} arg can't be less 0.", nameof(take));
			}

			filter ??= FilterDefinition<Product>.Empty;

			IFindFluent<Product, Product> clicks = collection.Find(filter)
				.Skip(skip);

			if (take.HasValue)
			{
				clicks = clicks.Limit(take);
			}

			return await clicks.ToListAsync();
		}
	}
}
