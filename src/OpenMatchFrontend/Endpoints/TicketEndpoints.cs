namespace GameFrontend.Endpoints;

using GameFrontend.OpenMatch;

public static class TicketEndpoints
{
    public static void MapTicketEndpoints(this IEndpointRouteBuilder app)
    {
        
        app.MapPost("/v1/tickets", Create);
        app.MapGet("/v1/tickets/{id}", GetTicket);
        app.MapDelete("/v1/tickets/{id}", DeleteTicket);
    }

    static async Task<Results<Ok, NotFound>> Create(FrontendService.FrontendServiceClient client)
    {
        var ticket = new CreateTicket.CreateTicketBuilder()
            .AddDouble(new CreateTicket.DoubleEntry("latency", 32.0))
            .AddDouble(new CreateTicket.DoubleEntry("skill", 3.0))
            .AddString(new CreateTicket.StringEntry("game", "32432"))
            .Build();

        var request = new CreateTicket.RequestBuilder()
            .WithTicket(ticket)
            .Build();

       
        var response = await client.CreateTicketAsync(request);
        
        return TypedResults.Ok();
    }

    static async Task<Results<Ok, NotFound>> GetTicket(string id)
    {
        return TypedResults.Ok();
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