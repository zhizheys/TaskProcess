namespace MS.TaskProcess.TaskService
{
    using Topshelf;

    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<TaskManagerServiceBus>(s =>
                {
                    s.ConstructUsing(name => new TaskManagerServiceBus());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });

                //the service run as prompt
                //x.RunAsPrompt();

                //the service run as local system
                x.RunAsLocalSystem();
                x.SetDescription("22TaskManagerServiceBus Host");
                x.SetDisplayName("22TaskManagerServiceBus");
                x.SetServiceName("22TaskManagerServiceBus");
            });

        }
    }
}
