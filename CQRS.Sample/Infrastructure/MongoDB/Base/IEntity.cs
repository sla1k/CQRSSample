using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CQRS.Sample.Infrastructure.MongoDB.Base
{
	/// <summary>
	///     Generic Entity interface.
	/// </summary>
	public interface IEntity
	{
		/// <summary>
		///     Gets or sets the Id of the Entity.
		/// </summary>
		/// <value>Id of the Entity.</value>
		[BsonRepresentation(BsonType.ObjectId)]
		string Id { get; set; }
	}
}
