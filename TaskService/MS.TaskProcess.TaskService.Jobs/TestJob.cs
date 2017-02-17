
namespace MS.TaskProcess.TaskService.Jobs
{
    using System;
    using System.Threading;
    using MS.Common.TaskHelper;
    using MS.Common.LogHelper;
    using Quartz;

    /// <summary>
    /// 测试任务
    /// </summary>
    ///<remarks>DisallowConcurrentExecution属性标记任务不可并行，要是上一任务没运行完即使到了运行时间也不会运行</remarks>
    [DisallowConcurrentExecution]
    public class TestJob : JobBase, IJob
    {
        /// <summary>
        /// IJob 接口
        /// </summary>
        /// <param name="context"></param>
        public void Execute(IJobExecutionContext context)
        {
            base.ExecuteJob(context, () =>
            {
                try
                {
                    // 3. 开始执行相关任务
                    var tastName = "TestJob " + context.Trigger.Description;
                    LogHelper.CreateInstance().Info(tastName + ",任务发生的当前系统时间:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    //Thread.Sleep(9000);
                }
                catch (Exception ex)
                {
                    LogHelper.CreateInstance().Error(context.Trigger.Description, ex);
                }

            });
        }
    }
}
