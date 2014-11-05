using System.Collections.Generic;

namespace SbManager.Models.ViewModels
{
    public class Overview
    {
        public List<Queue> Queues { get; set; }
        public List<Topic> Topics { get; set; }
    }
}
