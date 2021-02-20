using System;

using CQRS.Sample.Infrastructure.MongoDB.Base;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CQRS.Sample.Models.Products
{
	public class Product : IEntity
	{
		/// <summary>
		///     Алиас продукта
		/// </summary>
		[BsonRequired]
		public string Alias { get; set; }

		/// <summary>
		///     Название продукта
		/// </summary>
		[BsonRequired]
		public string Name { get; set; }

		/// <summary>
		///     Тип продукта
		/// </summary>
		[BsonRequired]
		public ProductType Type { get; set; }

		/// <summary>
		///     Дата создания
		/// </summary>
		[BsonRequired]
		public DateTime Created { get; set; }

		/// <summary>
		///     ID
		/// </summary>
		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; }
	}
}
