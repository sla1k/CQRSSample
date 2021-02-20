using System.Collections.Generic;
using System.Threading.Tasks;

using CQRS.Sample.Infrastructure.MongoDB.Base;
using CQRS.Sample.Models.Products;

using MongoDB.Driver;

namespace CQRS.Sample.Infrastructure.MongoDB.Repositories
{
	public interface IProductsRepository : IRepository<Product>
	{
		Task<IEnumerable<Product>> Get(int? take, int skip = 0, FilterDefinition<Product> filter = null,
			SortDefinition<Product> sort = null);
	}
}
