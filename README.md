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
	"Quickbase":{
		"Realm" : "gerawan",
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
	}
}
```

## Secrets
| Key | Description |
|-----|-------------|
| Quickbase:UserToken | User token with access to the Purchase Order Request and Approval application |
| Quickbase:Realm | The subdomain name used to access Quickbase e.g. "gerawan" |
| Oracle:UserId | The user id portion of the Oracle connection string e.g. "company_2_rpt". |
| Oracle:Password | The password portion of the Oracle connection string. |
| Oracle:DataSource | The Data Source portion of the Oracle connection string e.g. "xxx.xxx.xxx.xxx:1521/Famous". |
| Oracle:Schema | The Oracle database schema to use in queries. |
| Famous:UserId | The Famous user used to import vouchers. |
| Famous:Password | The password for the Famous user used to import vouchers. |
