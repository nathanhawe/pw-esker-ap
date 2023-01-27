# Esker AP

Esker AP is an ASP.NET core application that synchronizes Prima Wawona purchase order and related data between Quickbase, Famous, 
and Esker.

This application is currently in development.

## Get Started

1. Add the Prima Wawona private package repository to the NuGet package manager https://nuget.pkg.github.com/Prima-Wawona/index.json.
1. Restore nuget packages. 
1. Add required app secrets using `dotnet user-secrets set "<Key>" "<Value>" --project EskerAP`

## Deployment
The application is intended to be deployed as a command line application with a configuration file ("appsettings.json") that contains 
the application secrets.  The structure of the file is:

```json
{
	"CompanyCode" : "PW01",
	"PaidInvoiceDaysPast" : "10",
	"Quickbase":{
		"Realm" : "prima",
		"UserToken": "..."
	},
	"Oracle":{
		"UserId" : "...",
		"Password" : "...",
		"DataSource" : "...",
		"Schema" : "..."
	},
	"Famous":{
		"UserId" : "...",
		"Password" : "..."
	},
	"Esker": {
		"Folders": {
			"MasterData" : "...",
			"Invoices" : "...",
			"Ack" : "...",
			"UnpaidInvoices" : "...",
			"PaidInvoices" : "..."
		},
		"SFTP": {
			"Host" : "...",
			"Port" : 22,
			"Username" : "...",
			"Password" : "..."
		}
	},
	"Serilog": {
		...
	}
}
```
### Configuration
Default serilog configuration is included in the appsettings.json file in the project. This file can be expanded to include the other configuration information.

## Secrets
| Key | Description |
|-----|-------------|
| CompanyCode | The company code used in Esker e.g. "PW01" |
| PaidInvoiceDaysPast | The number of days in the past to search for paid invoices e.g. 10 |
| Quickbase:UserToken | User token with access to the Purchase Order Request and Approval application |
| Quickbase:Realm | The subdomain name used to access Quickbase e.g. "gerawan" |
| Oracle:UserId | The user id portion of the Oracle connection string e.g. "company_2_rpt". |
| Oracle:Password | The password portion of the Oracle connection string. |
| Oracle:DataSource | The Data Source portion of the Oracle connection string e.g. "xxx.xxx.xxx.xxx:1521/Famous". |
| Oracle:Schema | The Oracle database schema to use in queries. |
| Famous:UserId | The Famous user used to import vouchers. |
| Famous:Password | The password for the Famous user used to import vouchers. |
| Esker:Folders:MasterData | The folder used to export ERP data to Esker e.g. "In_MasterData" |
| Esker:Folders:Invoices | The folder used to import invoice XML files from Esker e.g. "Out" |
| Esker:Folders:Ack | The folder used to export ERP acknowledgement files e.g. "ErpAck" |
| Esker:Folders:UnpaidInvoices | The folder used to import CSVs of unpaid invoices from Esker e.g. "Out_UnpaidInvoices" |
| Esker:Folders:PaidInvoices | The folder used to export a CSV of invoice payments to Esker e.g. "In_PaidInvoices" |
| Esker:SFTP:Host | The SFTP host name |
| Esker:SFTP:Port | The SFTP port number |
| Esker:SFTP:Username | The SFTP username |
| Esker:SFTP:Password | The SFTP password |
