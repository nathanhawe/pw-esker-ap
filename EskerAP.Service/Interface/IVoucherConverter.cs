namespace EskerAP.Service.Interface
{
	public interface IVoucherConverter
	{
		Domain.Voucher ConvertXmlString(string xml);
	}
}
