using System.Collections.Concurrent;
using StreamDeck;
using StreamDeck.Events.Received;

#if DEBUG
System.Diagnostics.Debugger.Launch();
#endif

// Declare the dictionary of counts.
var counts = new ConcurrentDictionary<string, int>();

// Create the connection, register the handler, and connect.
var connection = new StreamDeckConnection();
connection.WillAppear += Connection_WillAppear;
connection.KeyDown += Connection_KeyDown;

await connection.ConnectAsync();
await connection.WaitForDisconnectAsync();

// Show the initial count.
void Connection_WillAppear(object? sender, ActionEventArgs<AppearancePayload> e)
    => connection.SetTitleAsync(e.Context, counts.GetOrAdd(e.Context, 0).ToString());

// Increment the count of the button on key down.
void Connection_KeyDown(object? sender, ActionEventArgs<KeyPayload> e)
{
    var count = counts.AddOrUpdate(e.Context, 1, (_, value) => ++value);
    _ = connection.SetTitleAsync(e.Context, count.ToString());
}
