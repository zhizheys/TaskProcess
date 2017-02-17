namespace MS.TaskProcess.TaskService
{
    using MS.Common.TaskHelper;
    using MS.Common.ConfigHelper;

    public class TaskManagerServiceBus
    {
        public void Start()
        {
            //配置信息读取
            ConfigInit.InitConfig();
            QuartzHelper.InitScheduler();
            QuartzHelper.StartScheduler();
        }

        public void Stop()
        {
            QuartzHelper.StopSchedule();

            System.Environment.Exit(0);
        }
    }
}
