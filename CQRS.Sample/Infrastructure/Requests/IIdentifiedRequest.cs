namespace CQRS.Sample.Infrastructure.Requests
{
	public interface IIdentifiedRequest
	{
		public string Id { get; }
	}
}
