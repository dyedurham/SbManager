namespace SbManager.Models.ViewModels
{
    public interface IHaveMessageCounts
    {
        long ActiveMessageCount { get; set; }
        long DeadLetterCount { get; set; }
        long ScheduledMessageCount { get; set; }
        long TransferMessageCount { get; set; }
        long DeadTransferMessageCount { get; set; }
        long? SizeInBytes { get; set; }
    }
}
