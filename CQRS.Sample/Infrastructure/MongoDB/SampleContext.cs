using CQRS.Sample.Infrastructure.MongoDB.Base;
using CQRS.Sample.Models.Products;

using Microsoft.Extensions.Options;

using MongoDB.Driver;

namespace CQRS.Sample.Infrastructure.MongoDB
{
	public class SampleContext : MongoDBContext
	{
		public SampleContext(IOptions<MongoDBSettings> settings) : base(settings)
		{
		}

		public IMongoCollection<Product> Products => _db.GetCollection<Product>(nameof(Products));
	}
}
