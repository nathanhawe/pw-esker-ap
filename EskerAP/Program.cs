using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;


namespace EskerAP
{
	public class Program
	{

		public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
			.AddJsonFile("appsettings.json", true)
#if DEBUG
			.AddUserSecrets<Program>()
#endif
			.Build();

		static void Main(string[] args)
		{
			Log.Logger = new LoggerConfiguration()
				.ReadFrom.Configuration(Configuration)
				.CreateLogger();

			string oracleConnectionString = GetOracleConnectionStringFromConfiguration();
			string oracleSchema = Configuration["Oracle:Schema"];
			string famousUserId = Configuration["Famous:UserId"];
			string famousPassword = Configuration["Famous:Password"];
			string qbRealm = Configuration["Quickbase:Realm"];
			string qbUserToken = Configuration["Quickbase:UserToken"];
			Service.SftpConfig sftpConfig = GetSftpConfigFromConfiguration();

			using IHost host = Host
				.CreateDefaultBuilder(args)
				.ConfigureServices((_, services) => 
				{
					/* Famous Repos */
					services.AddSingleton<Data.Famous.IApPayTermsRepo>(x =>
						ActivatorUtilities.CreateInstance<Data.Famous.ApPayTermsRepo>(x, oracleConnectionString, oracleSchema));

					services.AddSingleton<Data.Famous.IApVendorRepo>(x =>
						ActivatorUtilities.CreateInstance<Data.Famous.ApVendorRepo>(x, oracleConnectionString, oracleSchema));

					services.AddSingleton<Data.Famous.ICaCostCenterRepo>(x =>
						ActivatorUtilities.CreateInstance<Data.Famous.CaCostCenterRepo>(x, oracleConnectionString, oracleSchema));

					services.AddSingleton<Data.Famous.ICaPhaseRepo>(x =>
						ActivatorUtilities.CreateInstance<Data.Famous.CaPhaseRepo>(x, oracleConnectionString, oracleSchema));

					services.AddSingleton<Data.Famous.IGLAccountRepo>(x =>
						ActivatorUtilities.CreateInstance<Data.Famous.GLAccountRepo>(x, oracleConnectionString, oracleSchema));

					services.AddSingleton<Data.Famous.IImportApVouchersRepo>(x =>
						ActivatorUtilities.CreateInstance<Data.Famous.ImportApVouchersRepo>(x, oracleConnectionString, oracleSchema, famousUserId, famousPassword)); 
					
					services.AddSingleton<Data.Famous.IPurchaseOrderDetailRepo>(x =>
						ActivatorUtilities.CreateInstance<Data.Famous.PurchaseOrderDetailRepo>(x, oracleConnectionString, oracleSchema));

					services.AddSingleton<Data.Famous.IPurchaseOrderHeaderRepo>(x =>
						ActivatorUtilities.CreateInstance<Data.Famous.PurchaseOrderHeaderRepo>(x, oracleConnectionString, oracleSchema));

					/* Quickbase Repos */
					services.AddSingleton<QuickBase.Api.IQuickBaseConnection>(x =>
						ActivatorUtilities.CreateInstance<QuickBase.Api.QuickBaseConnection>(x, qbRealm, qbUserToken));
					
					services.AddScoped<Data.Quickbase.IItemsRepo, Data.Quickbase.ItemsRepo>();
					services.AddScoped<Data.Quickbase.IPurchaseOrdersRepo, Data.Quickbase.PurchaseOrdersRepo>();

					/* Services */
					services.AddScoped<Service.Interface.ICostCenterExporter, Service.CostCenterExporter>();
					services.AddScoped<Service.Interface.IGLAccountExporter, Service.GLAccountExporter>();
					services.AddScoped<Service.Interface.IPaymentTermsExporter, Service.PaymentTermsExporter>();
					services.AddScoped<Service.Interface.IPhaseExporter, Service.PhaseExporter>();
					services.AddScoped<Service.Interface.IPurchaseOrderExporter, Service.PurchaseOrderExporter>();
					services.AddSingleton<Service.Interface.ISftpService>(x => 
						ActivatorUtilities.CreateInstance<Service.SftpService>(x, sftpConfig));
					services.AddScoped<Service.Interface.IVendorExporter, Service.VendorExporter>();
					services.AddScoped<Service.Interface.IMasterDataExportService, Service.MasterDataExportService>();
				})
				.UseSerilog()
				.Build();


			Log.Logger.Information("Application started.");

			Console.WriteLine("Hello World!");
		}

		private static Service.SftpConfig GetSftpConfigFromConfiguration()
		{
			return new Service.SftpConfig
			{
				Host = Configuration["Esker:SFTP:Host"],
				Port = (int.TryParse(Configuration["Esker:SFTP:Port"], out int port) ? port : 0),
				Username = Configuration["Esker:SFTP:Username"],
				Password = Configuration["Esker:SFTP:Password"]
			};
		}

		private static string GetOracleConnectionStringFromConfiguration()
		{
			var userId = Configuration["Oracle:UserId"];
			var password = Configuration["Oracle:Password"];
			var dataSource = Configuration["Oracle:DataSource"];
			return $"User id={userId};Password={password};Data Source={dataSource}";
		}
	}
}
