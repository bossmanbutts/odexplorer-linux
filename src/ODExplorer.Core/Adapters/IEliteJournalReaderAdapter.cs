using System;

namespace ODExplorer.Adapters
{
    // Adapter to abstract EliteJournalReader types/events into core DTOs.
    public interface IEliteJournalReaderAdapter
    {
        // Map a raw journal event to a simple core DTO (if needed). For now provide minimal types.
        // Implementations in UI/host can provide richer mapping.
        CoreJournalEntry MapEntry(object rawEvent);
    }

    public sealed class CoreJournalEntry
    {
        public string EventName { get; init; } = string.Empty;
        public DateTime Timestamp { get; init; } = DateTime.MinValue;
        public object? Data { get; init; }
    }

    public class NoOpEliteJournalReaderAdapter : IEliteJournalReaderAdapter
    {
        public CoreJournalEntry MapEntry(object rawEvent) => new() { EventName = string.Empty, Timestamp = DateTime.MinValue, Data = null };
    }
}
