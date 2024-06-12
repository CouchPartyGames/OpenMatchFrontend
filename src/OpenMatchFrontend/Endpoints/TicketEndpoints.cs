using OpenMatchFrontend.Utilities.OpenMatch;

namespace OpenMatchFrontend.Endpoints;

public static class TicketEndpoints
{
    public static void MapTicketEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/v1/tickets", Create);
            //.RequireAuthorization();
        app.MapGet("/v1/tickets/{id}", GetTicket);
            //.RequireAuthorization();
        app.MapDelete("/v1/tickets/{id}", DeleteTicket);
            //.RequireAuthorization();
    }

    static async Task<Results<Ok<string>, NotFound>> Create(FrontendService.FrontendServiceClient client,
        CancellationToken token)
    {
        var ticket = new CreateTicket.CreateTicketBuilder()
            .AddDouble(new CreateTicket.DoubleEntry("latency", 32.0))
            .AddDouble(new CreateTicket.DoubleEntry("skill", 3.0))
            .AddString(new CreateTicket.StringEntry("game", "32432"))
            .Build();

        var request = new CreateTicket.RequestBuilder()
            .WithTicket(ticket)
            .Build();

       
        var response = await client.CreateTicketAsync(request, cancellationToken: token);
        //Console.WriteLine("Reresponse);
        
        return TypedResults.Ok(response.Id);
    }

    static async Task<Results<Ok<Ticket>, NotFound>> GetTicket(string id, 
        FrontendService.FrontendServiceClient client,
        CancellationToken token)
    {
        GetTicketRequest ticket = new GetTicketRequest { TicketId = id };
        var response = await client.GetTicketAsync(ticket, cancellationToken: token);
        if (response is not null)
        {
            return TypedResults.Ok(response);
        }
        
        return TypedResults.NotFound();
    }

    static async Task<Results<Ok, NotFound>> DeleteTicket(string id, FrontendService.FrontendServiceClient client)
    {
        var ticketId = new DeleteTicket.TicketId(id);
        var request = new DeleteTicket.RequestBuilder()
            .WithTicketId(ticketId)
            .Build();

        var response = await client.DeleteTicketAsync(request);
        
        return TypedResults.Ok();
    }
}