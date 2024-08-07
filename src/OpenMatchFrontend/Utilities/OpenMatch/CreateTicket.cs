
namespace OpenMatchFrontend.Utilities.OpenMatch;

public sealed class CreateTicket
{
    private FrontendService.FrontendServiceClient _client;

    public CreateTicket(FrontendService.FrontendServiceClient client)
    {
        _client = client;
    }

    public async Task<bool> Create(CreateTicketRequest request, Guid guid)
    {
        var id = guid.ToString();
        Metadata metadata = new()
        {
            { "CorrelationId", id }
        };
        var response = await _client.CreateTicketAsync(request, metadata);
        return true;
    }
    
    public sealed class RequestBuilder
    {
        private CreateTicketRequest _request = new CreateTicketRequest();
        
        public RequestBuilder WithTicket(Ticket ticket)
        {
            _request.Ticket = ticket;
            return this;
        }
        
        public CreateTicketRequest Build()
        {
            return _request;
        }
    }

    public sealed class CreateTicketBuilder
    {
        private Ticket _ticket = new();
        private SearchFields _searchFields = new();

        public CreateTicketBuilder AddTag(TagEntry tag)
        {
            _searchFields.Tags.Add(tag.Value);
            return this;
        }

        public CreateTicketBuilder AddDouble(DoubleEntry doubleEntry)
        {
            _searchFields.DoubleArgs.Add(doubleEntry.Key, doubleEntry.Value);
            return this;
        }

        public CreateTicketBuilder AddString(StringEntry stringEntry)
        {
            _searchFields.StringArgs.Add(stringEntry.Key, stringEntry.Value);
            return this;
        }

        public CreateTicketBuilder AddExtension()
        {
            return this;
        }
        
        public CreateTicketBuilder AddPersistence()
        {
            return this;
        }

        public Ticket Build()
        {
            _ticket.SearchFields = _searchFields;
            return _ticket;
        } 
    }

    public sealed record TagEntry(string Value);

    public sealed record DoubleEntry(string Key, double Value);

    public sealed record StringEntry(string Key, string Value);
}