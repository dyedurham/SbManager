using System;
using Humanizer;

namespace SbManager.Models.ViewModels
{
    public class Queue : IHaveMessageCounts, IHaveStatusInformation, IHaveSettings
    {
        public string Name { get; set; }
        public long ActiveMessageCount { get; set; }
        public long DeadLetterCount { get; set; }
        public long ScheduledMessageCount { get; set; }
        public long TransferMessageCount { get; set; }
        public long DeadTransferMessageCount { get; set; }
        public long? SizeInBytes { get; set; }
        public Time AutoDeleteOnIdle { get; set; }
        public Time DefaultMessageTTL { get; set; }
        public Time DuplicateDetectionWindow { get; set; }
        public Time LockDuration { get; set; }
        public string Status { get; set; }
        public bool IsReadOnly { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime AccessedAt { get; set; }

        public string SizeInHumanReadable
        {
            get
            {
                if (SizeInBytes == null) return null;
                var bits = (SizeInBytes.GetValueOrDefault() * 8).Bits();
                return bits.LargestWholeNumberValue.ToString("0.#") + bits.LargestWholeNumberSymbol;
            }
        }
    }
}
