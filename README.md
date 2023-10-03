# Zendesk Api Client
[![NuGet Version](https://img.shields.io/nuget/vpre/ZendeskApi.Client.svg?style=flat-square)](https://www.nuget.org/packages/ZendeskApi.Client)
[![NuGet Downloads](https://img.shields.io/nuget/dt/ZendeskApi.Client.svg?style=flat-square)](https://www.nuget.org/packages/ZendeskApi.Client)
[![ci](https://github.com/justeat/ZendeskApiClient/actions/workflows/ci.yml/badge.svg)](https://github.com/justeat/ZendeskApiClient/actions/workflows/ci.yml)

A .netstandard NuGet package for use with the  Zendesk v2 API.

# Breaking Changes

## 6.x.x
#### Added support for .NET 6

ZendeskApiClient now targets both .NET Standard 2.0 and .NET 6, enabling users to leverage the latest runtime features and performance optimizations for their projects. Please note that with this update, support for .NET Core 3.1 is no longer targeted.

## 4.x.x
This will allow for cursor pagination on some endpoints. We have kept the original offset pagination available so that this release is backward compatible, however due to Zendesk adding a variation of cursor pagination to this single [endpoint](https://developer.zendesk.com/api-reference/ticketing/tickets/ticket_audits/#pagination), we have modified the original `cursorPager` class to now be `cursorPagerVariant`. If you use the TicketAudits endpoint please update your code.

Unfortunately Zendesk has not adopted cursor pagination on all endpoints. A list of compatible endpoints used in this API are as follows:

Users:
- GET /api/v2/deleted_users
- GET /api/v2/users
- GET /api/v2/users/{user_id}/identities
- GET /api/v2/user_fields
- GET /api/v2/users/{user_id}/groups

Groups:
- GET /api/v2/groups
- GET /api/v2/groups/{group_id}/users

Help Center:
- GET api/v2/help_center/articles
- GET api/v2/help_center/categories

Organization:
- GET /api/v2/organization_fields
- GET /api/v2/organization_memberships
- GET /api/v2/organizations
- GET /api/v2/organizations/{organization_id}/users
- GET /api/v2/organizations/{organization_id}/tickets

Tickets:
- GET /api/v2/deleted_tickets
- GET /api/v2/tickets
- GET /api/v2/tickets/{ticketId}/comments
- GET /api/v2/ticket_fields
- GET /api/v2/ticket_audits - [Cursor Variant](https://developer.zendesk.com/api-reference/ticketing/tickets/ticket_audits/#pagination)

Satisfaction ratings:
- GET /api/v2/satisfaction_ratings

[Further reading on Zendesk Pagination changes](https://support.zendesk.com/hc/en-us/articles/4402610093338-Introducing-Pagination-Changes-Zendesk-API)

## 3.x.x
This is a complete rewrite so expect breaking changes.

Some noteworthy changes include:
- `PostAsync` replaced with `CreateAsync`
- `PutAsync` replaced with `UpdateAsync`
- `Search` resource now uses `SearchAsync` instead of `Find`, and introduces a new fluent api to replace the old `ZendeskQuery<T>` class.


## Creating a client
To register this in your DI, you just need to call...
```c#
services.AddZendeskClient("https://[your_url].zendesk.com", "username", "token");
```
then you can inject in IZendeskClient anywhere you want to use it. Here you can access all the resources available.

If however you want to use the client in a DI less environment you can do this

```c#
var zendeskOptions = new ZendeskOptions
{
   EndpointUri = "https://[your_url].zendesk.com",
   Username = "username"
   Token = "token"
};

var loggerFactory = new LoggerFactory();
var zendeskOptionsWrapper = new OptionsWrapper<ZendeskOptions>(zendeskOptions);
var client = new ZendeskClient(new ZendeskApiClient(zendeskOptionsWrapper), loggerFactory.CreateLogger<ZendeskClient>());
```

## Example methods
```c#
var ticket = await client.Tickets.GetAsync(1234L); // Get ticket by Id
var tickets = await client.Tickets.GetAllAsync(new [] { 1234L, 4321L }); //
var ticket = await client.Tickets.UpdateAsync(ticket);
var ticket = await client.Tickets.CreateAsync(ticket);
await client.Tickets.DeleteAsync(1234L);
```

## Searching and filtering
```c#
await client.Search.SearchAsync<User>(q =>
    q.WithFilter("email", "jazzy.b@just-eat.com")
);

await client.Search.SearchAsync<Organization>(q =>
    q.WithFilter("name", "Cupcake Cafe")
);

// All non closed tickets
await client.Search.SearchAsync<Ticket>(q =>
    q.WithFilter("status", "closed", FilterOperator.LessThan)
);
```

## The Zendesk API

The zendesk api documentation is available at http://developer.zendesk.com/documentation/rest_api/introduction.html
Querying and searching is limited by the searchable fields on the zendesk api

> Please note that starting from August 15, 2023, if you make offset-based pagination (OBP) requests beyond the first 100 pages (10,000 records), you will receive an error message: "400 Bad Request." To retrieve data sets larger than 10,000 records, customers must transition to cursor-based pagination (CBP). You can find more information on the transition [here](https://developer.zendesk.com/documentation/api-basics/pagination/paginating-through-lists-using-cursor-pagination/). This library designates the OBP endpoints as `Obsolete` and introduces the new CBP endpoints in version 4.x.x. Therefore, users who wish to make the transition will need to upgrade.

## Integration Tests

In order to run integration tests against your own zendesk instance use the Cake script provided by:

```powershell
.\build.ps1 --target=Run-Integration-Tests --zendeskUrl="<your zendesk url>" --zendeskUsername="<your zendesk username>" --zendeskToken="<your zendesk token>"
```

### Integration Tests with Visual Studio

This could be a good option if you are using MAC OSX.
[Download Here](https://visualstudio.microsoft.com/downloads/?cid=learn-onpage-download-cta)

In Visual Studio, open the root folder of this project and open the `test` view (has a little lightening bolt icon).
You should see a structure that looks like:
    * ZendeskApiClient
        * test
            * ZendeskApi.Client.IntegrationTests
            * ZendeskApi.Client.Tests
To run the integration tests, right click on it and select `Run Test With` ==> `Custom Parameters` - make sure you add the following variables: `ZendeskApi_Credentials_Url, ZendeskApi_Credentials_Username, ZendeskApi_Credentials_Token`
You can save this configuration and give it a name, in order to run it multiple times.

# Contributing

We are happy for anyone to contribute into this client, and help us evolve it over time.

## Versioning

We aim to follow [Semantic Versioning](https://semver.org/) guidelines within this library. When increasing the version please increment [ZendeskApi.Commons.props](https://github.com/justeat/ZendeskApiClient/blob/master/src/ZendeskApi.Build/ZendeskApi.Commons.props).
