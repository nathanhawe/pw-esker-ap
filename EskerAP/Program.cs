using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace EskerAP
{
	public class Program
	{
		public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
			.AddJsonFile("appsettings.json", true)
			.AddUserSecrets<Program>()
			.Build();

		static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");
		}
	}
}
