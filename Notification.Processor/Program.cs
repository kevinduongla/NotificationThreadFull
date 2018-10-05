using Topshelf;

namespace Notification.Processor
{
    class Program
    {
        static void Main(string[] args)
        {
            Host host = HostFactory.New(c =>
            {
                c.Service<NotificationServer>(s =>
                {
                    s.ConstructUsing(builder =>
                    {
                        var server = new NotificationServer();
                        server.Initialize();
                        return server;
                    });

                    s.WhenStarted(server => server.Start());
                    s.WhenPaused(server => server.Pause());
                    s.WhenContinued(server => server.Resume());
                    s.WhenStopped(server => server.Stop());
                });

                c.RunAsLocalSystem();
                c.SetDescription("Notification Processor");
                c.SetDisplayName("Notification Processor");
                c.SetServiceName("NotificationProcessor");
            });

            host.Run();
        }
    }
}
