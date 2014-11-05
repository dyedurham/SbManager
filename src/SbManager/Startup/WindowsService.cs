using Topshelf;

namespace SbManager.Startup
{
    public class WindowsService : ServiceControl
    {
        private readonly IService _service;

        public WindowsService(IService service)
        {
            _service = service;
        }

        public bool Start(HostControl hostControl)
        {
            _service.Start();
            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            _service.Stop();
            return true;
        }
    }
}

