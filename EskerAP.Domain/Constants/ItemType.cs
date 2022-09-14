namespace EskerAP.Domain.Constants
{
	public enum ItemType
	{
		Unknown = 0,

		/// <summary>
		/// The item is a service with a global amount
		/// </summary>
		AmountBased = 1,

		/// <summary>
		/// The item has as price per quantity
		/// </summary>
		QuantityBased = 2,
	}
}
