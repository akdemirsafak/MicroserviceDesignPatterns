using EventSourcing.Shared.Events;
using EventStore.ClientAPI;
using Newtonsoft.Json;
using System.Text;

namespace EventSourcing.API.EventStores;

public abstract class AbstractStream
{
    protected readonly LinkedList<IEvent> Events = new();
    private string _streamName { get; }
    private readonly IEventStoreConnection _eventStoreConnection;

    protected AbstractStream(
        string streamName, 
        IEventStoreConnection eventStoreConnection)
    {
        _streamName = streamName;
        _eventStoreConnection = eventStoreConnection;
    }

    public async Task SaveAsync()
    {
        var events= Events.ToList().Select(x=> new EventData(
            Guid.NewGuid(),
            x.GetType().Name,
            true,
            Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(x)),
            Encoding.UTF8.GetBytes(x.GetType().FullName))).ToList();

        await _eventStoreConnection.AppendToStreamAsync(_streamName,ExpectedVersion.Any,events);

        Events.Clear();
    }
}