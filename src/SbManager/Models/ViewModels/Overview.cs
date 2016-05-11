using System.Collections.Generic;

namespace SbManager.Models.ViewModels
{
    public class Overview
    {
        public List<Queue> Queues { get; set; }
        public List<Topic> Topics { get; set; }

        public long TotalDeadLetters { get; set; }
        public long TotalActiveMessages { get; set; }
        public long TotalScheduledMessages { get; set; }
    }
}
