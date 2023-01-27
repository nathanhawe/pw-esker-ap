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

			try
			{


				string companyCode = Configuration["CompanyCode"];
				string masterDataDir = Configuration["Esker:Folders:MasterData"];
				string ackDir = Configuration["Esker:Folders:Ack"];
				string invoiceDir = Configuration["Esker:Folders:Invoices"];
				string paidInvoiceDir = Configuration["Esker:Folders:PaidInvoices"];
				string unpaidInvoiceDir = Configuration["Esker:Folders:UnpaidInvoices"];
				int paidInvoiceDaysPast = (int.TryParse(Configuration["PaidInvoiceDaysPast"], out int days) ? days : 10);
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

						services.AddSingleton<Data.Famous.IApVoucherRepo>(x =>
							ActivatorUtilities.CreateInstance<Data.Famous.ApVoucherRepo>(x, oracleConnectionString, oracleSchema)); 
						
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
						services.AddScoped<Service.Interface.IErpAckService, Service.ErpAckService>();
						services.AddScoped<Service.Interface.IGLAccountExporter, Service.GLAccountExporter>();
						services.AddScoped<Service.Interface.IPaymentTermsExporter, Service.PaymentTermsExporter>();
						services.AddScoped<Service.Interface.IPaidInvoiceExporter, Service.PaidInvoiceExporter>();
						services.AddScoped<Service.Interface.IPhaseExporter, Service.PhaseExporter>();
						services.AddScoped<Service.Interface.IPurchaseOrderExporter, Service.PurchaseOrderExporter>();
						services.AddSingleton<Service.Interface.ISftpService>(x =>
							ActivatorUtilities.CreateInstance<Service.SftpService>(x, sftpConfig));
						services.AddScoped<Service.Interface.IUnpaidInvoiceReader, Service.UnpaidInvoiceReader>();
						services.AddScoped<Service.Interface.IVendorExporter, Service.VendorExporter>();
						services.AddScoped<Service.Interface.IVoucherConverter, Service.VoucherConverter>();
						services.AddScoped<Service.Interface.IMasterDataExportService, Service.MasterDataExportService>();
						services.AddScoped<Service.Interface.IVoucherImportService, Service.VoucherImportService>();
						services.AddScoped<Service.Interface.IVoucherExportService, Service.VoucherExportService>();
					})
					.UseSerilog()
					.Build();

				Log.Logger.Information("Application started with {args}.", args);

				if (args.Length == 1)
				{
					switch (args[0].ToLower())
					{
						case "export":
							{
								var exporter = host.Services.GetService<Service.Interface.IMasterDataExportService>();
								exporter.ExportMasterData(masterDataDir, masterDataDir, companyCode, false);
								return;
							}
						case "export-po":
							{
								var exporter = host.Services.GetService<Service.Interface.IMasterDataExportService>();
								exporter.ExportMasterData(masterDataDir, masterDataDir, companyCode, true);
								return;
							}
						case "paid-invoice":
							{
								var exporter = host.Services.GetService<Service.Interface.IVoucherExportService>();
								exporter.ExportPaidInvoices(paidInvoiceDir, paidInvoiceDir, unpaidInvoiceDir, companyCode);
								return;
							}
						case "import":
							{
								var importer = host.Services.GetService<Service.Interface.IVoucherImportService>();
								importer.ImportVouchers(invoiceDir, invoiceDir, ackDir);
								return;
							}
					}
				}

				// Display help on fallthrough
				Console.WriteLine("This program is used to integrate Esker AP with company ERP Famous/Quickbase.  The following are accepted commands:");
				Console.WriteLine("    EXPORT - Exports all master data including purchase orders from ERP to Esker.");
				Console.WriteLine("    EXPORT-PO - Exports only purchase orders from ERP to Esker.");
				Console.WriteLine("    PAID-INVOICE - Exports paid invoices.");
				Console.WriteLine("    IMPORT - Imports all pending vouchers from Esker into ERP.");
			}
			catch(Exception ex)
			{
				Log.Logger.Error("An unhandle exception was thrown: {ex}", ex);
			}
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
