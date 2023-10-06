// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ZendeskApi.Client;
using ZendeskApi.Client.Models;
using ZendeskApi.Client.Options;
using ZendeskApi.Client.Extensions;
using ZendeskApi.Client.Responses;
using Microsoft.Extensions.DependencyInjection;
using ZendeskApi.Client.Resources;

var zendeskOptions = new ZendeskOptions
{
   EndpointUri = "https://z3n-api-client-rb.zendesk.com",
   Username = "api-client-rb+agent@zendesk.com",
   Token = "tLzGI6Mfi4TwABtJKpRHGs4bE23JOecqUxTyE9Wx"
};

var loggerFactory = new LoggerFactory();
var zendeskOptionsWrapper = new OptionsWrapper<ZendeskOptions>(zendeskOptions);
//var client = new ZendeskClient(new ZendeskApiClient(zendeskOptionsWrapper), loggerFactory.CreateLogger<ZendeskClient>());

var services = new ServiceCollection();

services.AddZendeskClientWithHttpClientFactory("https://z3n-api-client-rb.zendesk.com", "a-rb+agent@zendesk.com", ""); 
var serviceProvider = services.BuildServiceProvider();
var client = (ZendeskClient)serviceProvider.GetRequiredService<IZendeskClient>();

var pager = new CursorPager { Size = 2 };
var ticketResponseCursor = (TicketsListCursorResponse) await client.Tickets.GetAllAsync(pager);

while (ticketResponseCursor.Meta.HasMore)
{
    // at this point, we have ticketResponseCursor.Tickets with a count of 2 (page size)
    Console.WriteLine(ticketResponseCursor.Tickets);
    ticketResponseCursor.Next();
    // the lines below doesn't work... but it's somewhat what I would expect
    // ticketResponseCursor = (TicketsListCursorResponse) client.fetch/get(nextPageUrl);
    // ticketReponseCursor.getNext() // would execute the request for the next page
    // ticketResponseCursor.Tickets // expected to be List<Ticket> count 2 - from the second page
}

var blah = (TicketsResource)client.Tickets;
blah.ExecuteRequest("does not work");


// 1 - find a way to make a random request - more likely with a Resource instance.
// can already do client.Tickets (REsource) client.Tickets.GetAsync()
// GOAL: client.Tickets.fetchURL(google.com, TicketListResponse)
// return TicketListResponse

// 2 - In CursorPaginationResponse (abstract class)
//  have methods to fetch NEXT and PREV pages
// Inject something that will make the fetchURL
