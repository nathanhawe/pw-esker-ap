using Microsoft.Extensions.Configuration;
using EskerAP;

namespace EskerAp.UnitTests
{
	internal static class ConfigurationHelper
	{
		internal static IConfigurationRoot GetIConfigurationRoot()
		{
			var builder = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json", true)
				.AddUserSecrets(System.Reflection.Assembly.GetAssembly(typeof(Program)));
			
			return builder.Build();
		}

	}
}
