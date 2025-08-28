namespace ModularMonolithPoC.Persons.Contracts;

public static class MessagingConstants
{
	public static class Exchanges
	{
		public const string PersonCreatedExchange = "person-created-exchange";

		public const string PersonUpdatedExchange = "person-updated-exchange";

		public const string PersonDeletedExchange = "person-deleted-exchange";
	}
}
