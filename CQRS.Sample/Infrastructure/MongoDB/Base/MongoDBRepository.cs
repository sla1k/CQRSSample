using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using MongoDB.Bson;
using MongoDB.Driver;

namespace CQRS.Sample.Infrastructure.MongoDB.Base
{
	/// <summary>
	///     Deals with entities in MongoDb.
	/// </summary>
	/// <typeparam name="T">The type contained in the repository.</typeparam>
	public abstract class MongoDBRepository<T> : IRepository<T>
		where T : IEntity
	{
		/// <summary>
		///     MongoCollection field.
		/// </summary>
		protected internal IMongoCollection<T> collection;

		/// <summary>
		///     Gets the Mongo collection (to perform advanced operations).
		/// </summary>
		/// <remarks>
		///     One can argue that exposing this property (and with that, access to it's Database property for instance
		///     (which is a "parent")) is not the responsibility of this class. Use of this property is highly discouraged;
		///     for most purposes you can use the MongoRepositoryManager&lt;T&gt;
		/// </remarks>
		/// <value>The Mongo collection (to perform advanced operations).</value>
		public IMongoCollection<T> Collection => collection;

		/// <summary>
		///     Returns the T by its given id.
		/// </summary>
		/// <param name="id">The Id of the entity to retrieve.</param>
		/// <returns>The Entity T.</returns>
		public virtual async Task<T> GetById(string id) => await GetById(new ObjectId(id));

		/// <summary>
		///     Returns the T by its given id.
		/// </summary>
		/// <param name="id">The Id of the entity to retrieve.</param>
		/// <returns>The Entity T.</returns>
		public virtual async Task<T> GetById(ObjectId id) =>
			await collection.Find(new BsonDocument("_id", id)).FirstOrDefaultAsync<T>();

		/// <summary>
		///     Adds the new entity in the repository.
		/// </summary>
		/// <param name="entity">The entity T.</param>
		/// <returns>The added entity including its new ObjectId.</returns>
		public virtual async Task<T> Add(T entity)
		{
			await collection.InsertOneAsync(entity);

			return entity;
		}

		/// <summary>
		///     Adds the new entities in the repository.
		/// </summary>
		/// <param name="entities">The entities of type T.</param>
		public virtual async Task Add(IEnumerable<T> entities) => await collection.InsertManyAsync(entities);

		/// <summary>
		///     Upserts an entity.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <returns>The updated entity.</returns>
		public virtual async Task<bool> Update(T entity)
		{
			ReplaceOneResult actionResult =
				await collection.ReplaceOneAsync(new BsonDocument("_id", new ObjectId(entity.Id)), entity);

			return actionResult.IsAcknowledged
			       && actionResult.ModifiedCount > 0;
		}

		/// <summary>
		///     Upserts the entities.
		/// </summary>
		/// <param name="entities">The entities to update.</param>
		public virtual async Task Update(IEnumerable<T> entities)
		{
			if (entities is null)
			{
				throw new ArgumentNullException(nameof(entities));
			}

			foreach (T entity in entities)
			{
				await collection.ReplaceOneAsync(new BsonDocument("_id", new ObjectId(entity.Id)), entity);
			}
		}

		/// <summary>
		///     Deletes an entity from the repository by its id.
		/// </summary>
		/// <param name="id">The entity's id.</param>
		public virtual async Task Delete(string id) =>
			await collection.DeleteOneAsync(new BsonDocument("_id", new ObjectId(id)));

		/// <summary>
		///     Deletes an entity from the repository by its ObjectId.
		/// </summary>
		/// <param name="id">The ObjectId of the entity.</param>
		public virtual async Task Delete(ObjectId id) => await collection.DeleteOneAsync(new BsonDocument("_id", id));

		/// <summary>
		///     Deletes the given entity.
		/// </summary>
		/// <param name="entity">The entity to delete.</param>
		public virtual async Task Delete(T entity) => await Delete(entity.Id);

		/// <summary>
		///     Deletes the entities matching the predicate.
		/// </summary>
		/// <param name="predicate">The expression.</param>
		public virtual async Task Delete(Expression<Func<T, bool>> predicate)
		{
			foreach (T entity in collection.AsQueryable().Where(predicate))
			{
				await Delete(entity.Id);
			}
		}

		/// <summary>
		///     Deletes all entities in the repository.
		/// </summary>
		public virtual async Task DeleteAll() => await collection.DeleteManyAsync(FilterDefinition<T>.Empty);

		/// <summary>
		///     Counts the total entities in the repository.
		/// </summary>
		/// <returns>Count of entities in the collection.</returns>
		public virtual async Task<long> Count(FilterDefinition<T> filter = null)
		{
			if (filter == null)
			{
				filter = FilterDefinition<T>.Empty;
			}

			return await collection.CountDocumentsAsync(filter);
		}

		/// <summary>
		///     Checks if the entity exists for given predicate.
		/// </summary>
		/// <param name="predicate">The expression.</param>
		/// <returns>True when an entity matching the predicate exists, false otherwise.</returns>
		public virtual bool Exists(Expression<Func<T, bool>> predicate) => collection.AsQueryable().Any(predicate);

		#region IQueryable<T>

		/// <summary>
		///     Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>An IEnumerator&lt;T&gt; object that can be used to iterate through the collection.</returns>
		public virtual IEnumerator<T> GetEnumerator() => collection.AsQueryable().GetEnumerator();

		/// <summary>
		///     Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>An IEnumerator object that can be used to iterate through the collection.</returns>
		IEnumerator IEnumerable.GetEnumerator() => collection.AsQueryable().GetEnumerator();

		/// <summary>
		///     Gets the type of the element(s) that are returned when the expression tree associated with this instance of
		///     IQueryable is executed.
		/// </summary>
		public virtual Type ElementType => collection.AsQueryable().ElementType;

		/// <summary>
		///     Gets the expression tree that is associated with the instance of IQueryable.
		/// </summary>
		public virtual Expression Expression => collection.AsQueryable().Expression;

		/// <summary>
		///     Gets the query provider that is associated with this data source.
		/// </summary>
		public virtual IQueryProvider Provider => collection.AsQueryable().Provider;

		#endregion IQueryable<T>
	}
}
