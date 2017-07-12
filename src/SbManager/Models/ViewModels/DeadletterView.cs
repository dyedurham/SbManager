using System.Collections.Generic;

namespace SbManager.Models.ViewModels
{

    public class DeadletterView
    {
        public DeadletterView()
        {
            Deadletters = new List<Deadletter>();
        }
        public IList<Deadletter> Deadletters { get; set; }
    }

    public class Deadletter
    {
        public string Name { get; set; }
        public string Href { get; set; }
        public long DeadLetterCount { get; set; }
    }
}
