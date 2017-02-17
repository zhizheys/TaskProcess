using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using MS.TaskProcess.Model;
using MS.Common;
using MS.Common.ConfigHelper;
using MS.Common.DBHelper;

namespace MS.Common.TaskHelper
{
    /// <summary>
    /// 任务帮助类
    /// </summary>
    public class TaskActionHelper
    {
        /// <summary>
        /// 配置文件地址
        /// </summary>
        private static readonly string TaskPath = FileHelper.GetAbsolutePath("Config/TaskConfig.xml");

        private static TaskBLL task = new TaskBLL();

        public static bool AddTask(TaskModel model, string action)
        {
            var result = false;

            if (action == "edit")
            {
                result = task.Edit(model);
            }
            else
            {
                model.TaskID = Guid.NewGuid();
                result = task.Add(model);
            }

            if (result)
            {
                QuartzHelper.ScheduleJob(model, true);
            }
            return result;
        }

        /// <summary>
        /// 删除指定id任务
        /// </summary>
        /// <param name="TaskID">任务id</param>
        public static void DeleteById(string taskId)
        {
            QuartzHelper.DeleteJob(taskId);

            task.DeleteById(taskId);
        }

        /// <summary>
        /// 更新任务运行状态
        /// </summary>
        /// <param name="TaskID">任务id</param>
        /// <param name="Status">任务状态</param>
        public static void UpdateTaskStatus(string taskId, TaskStatus Status)
        {
            if (Status == TaskStatus.RUN)
            {
                QuartzHelper.ResumeJob(taskId);
            }
            else
            {
                QuartzHelper.PauseJob(taskId);
            }
            task.UpdateTaskStatus(taskId, (int)Status);
        }

        /// <summary>
        /// 更新任务下次运行时间
        /// </summary>
        /// <param name="TaskID">任务id</param>
        /// <param name="NextFireTime">下次运行时间</param>
        public static void UpdateNextFireTime(string taskId, DateTime nextFireTime)
        {
            task.UpdateNextFireTime(taskId, nextFireTime);
        }

        /// <summary>
        /// 任务完成后，更新上次执行时间
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="recentRunTime">上次执行时间</param>
        public static void UpdateRecentRunTime(string taskId, DateTime recentRunTime)
        {
            task.UpdateRecentRunTime(taskId, recentRunTime);
        }

        /// <summary>
        /// 从数据库中读取全部任务列表
        /// </summary>
        /// <returns></returns>
        private static IList<TaskModel> TaskInDB()
        {
            return task.GetAllTaskList();
        }

        /// <summary>
        /// 从配置文件中读取任务列表
        /// </summary>
        /// <returns></returns>
        private static IList<TaskModel> ReadTaskConfig()
        {
            return XmlHelper.XmlToList<TaskModel>(TaskPath, "Task");
        }

        /// <summary>
        /// 获取所有启用的任务
        /// </summary>
        /// <returns>所有启用的任务</returns>
        public static IList<TaskModel> GetAllTaskList()
        {
            if (SysConfig.StorageMode == 1)
            {
                return TaskInDB();
            }
            else
            {
                return ReadTaskConfig();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IList<TaskModel> CurrentTaskList()
        {
            return task.GetAllTaskList();
        }
    }


    public class TaskBLL
    {
        private readonly TaskDAL dal = new TaskDAL();

        /// <summary>
        /// 获取任务列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        //public PageOf<TaskModel> GetTaskList(int pageIndex, int pageSize)
        //{
        //    return dal.GetTaskList(pageIndex, pageSize);
        //}

        /// <summary>
        /// 读取数据库中全部的任务
        /// </summary>
        /// <returns></returns>
        public List<TaskModel> GetAllTaskList()
        {
            return dal.GetAllTaskList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public TaskModel GetById(string taskId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 删除任务
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public bool DeleteById(string taskId)
        {
            return dal.UpdateTaskStatus(taskId, -1);
        }

        /// <summary>
        /// 修改任务
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool UpdateTaskStatus(string taskId, int status)
        {
            return dal.UpdateTaskStatus(taskId, status);
        }

        /// <summary>
        /// 修改任务的下次启动时间
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="nextFireTime"></param>
        /// <returns></returns>
        public bool UpdateNextFireTime(string taskId, DateTime nextFireTime)
        {
            return dal.UpdateNextFireTime(taskId, nextFireTime);
        }

        /// <summary>
        /// 根据任务Id 修改 上次运行时间
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="recentRunTime"></param>
        /// <returns></returns>
        public bool UpdateRecentRunTime(string taskId, DateTime recentRunTime)
        {
            return dal.UpdateRecentRunTime(taskId, recentRunTime);
        }

        /// <summary>
        /// 根据任务Id 获取任务
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public TaskModel GetTaskById(string taskId)
        {
            return dal.GetTaskById(taskId);
        }

        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public bool Add(TaskModel task)
        {
            return dal.Add(task);
        }

        /// <summary>
        /// 修改任务
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public bool Edit(TaskModel task)
        {
            return dal.Edit(task);
        }
    }

    public class TaskDAL
    {
        /// <summary>
        /// 获取任务列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        //public PageOf<TaskModel> GetTaskList(int pageIndex, int pageSize)
        //{
        //    var QUERY_SQL = @"( SELECT TaskID,TaskName,TaskParam,CronExpressionString,AssemblyName,ClassName,Status,IsDelete,CreatedTime,ModifyTime,RecentRunTime,NextFireTime,CronRemark,Remark
	       //                     FROM p_Task(nolock)
        //                        WHERE IsDelete=0 ";

        //    QUERY_SQL += ") pp ";
        //    string SQL = string.Format(@" select * from (select ROW_NUMBER() OVER(order by pp.ModifyTime desc) AS RowNum,* from {0}
								//		) as A where A.RowNum BETWEEN (@PageIndex-1)* @PageSize+1 AND @PageIndex*@PageSize ORDER BY RowNum;",
        //                          QUERY_SQL);

        //    SQL += string.Format(@" SELECT COUNT(1) FROM {0};", QUERY_SQL);

        //    object param = new { pageIndex = pageIndex, pageSize = pageSize };

        //    DataSet ds = SQLHelper.FillDataSet(SQL, param);
        //    return new PageOf<TaskModel>()
        //    {
        //        PageIndex = pageIndex,
        //        PageSize = pageSize,
        //        Total = Convert.ToInt32(ds.Tables[1].Rows[0][0]),
        //        Items = DataMapHelper.DataSetToList<TaskModel>(ds)
        //    };
        //}

        /// <summary>
        /// 读取数据库中全部的任务
        /// </summary>
        /// <returns></returns>
        public List<TaskModel> GetAllTaskList()
        {
            var sql = @"SELECT TaskID,TaskName,TaskParam,CronExpressionString,AssemblyName,ClassName,Status,IsDelete,CreatedTime,ModifyTime,RecentRunTime,NextFireTime,CronRemark,Remark
	                    FROM p_Task(nolock)
                        WHERE IsDelete=0 and Status =1";

            var result = SQLHelper.ToList<TaskModel>(sql);

            return result;

        }

        /// <summary>
        /// 删除任务
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public bool UpdateTaskStatus(string taskId, int status)
        {
            var sql = @" UPDATE p_Task
                           SET Status = @Status 
                         WHERE TaskID=@TaskID
                        ";
            object param = new { TaskID = taskId, Status = status };

            return SQLHelper.ExecuteNonQuery(sql, param) > 0;
        }

        /// <summary>
        /// 修改任务的下次启动时间
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="nextFireTime"></param>
        /// <returns></returns>
        public bool UpdateNextFireTime(string taskId, DateTime nextFireTime)
        {
            var sql = @" UPDATE p_Task
                           SET NextFireTime = @NextFireTime 
                               ,ModifyTime = GETDATE()
                         WHERE TaskID=@TaskID
                        ";
            object param = new { TaskID = taskId, NextFireTime = nextFireTime };

            return SQLHelper.ExecuteNonQuery(sql, param) > 0;
        }

        /// <summary>
        /// 根据任务Id 修改 上次运行时间
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="recentRunTime"></param>
        /// <returns></returns>
        public bool UpdateRecentRunTime(string taskId, DateTime recentRunTime)
        {
            var sql = @" UPDATE p_Task
                           SET RecentRunTime = @RecentRunTime 
                               ,ModifyTime = GETDATE()
                         WHERE TaskID=@TaskID
                        ";
            object param = new { TaskID = taskId, RecentRunTime = recentRunTime };

            return SQLHelper.ExecuteNonQuery(sql, param) > 0;
        }

        /// <summary>
        /// 根据任务Id 获取任务
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public TaskModel GetTaskById(string taskId)
        {
            var sql = @"SELECT TaskID,TaskName,TaskParam,CronExpressionString,AssemblyName,ClassName,Status,IsDelete,CreatedTime,ModifyTime,RecentRunTime,NextFireTime,CronRemark,Remark
	                    FROM p_Task(nolock)
                        WHERE TaskID=@TaskID";

            object param = new { TaskID = taskId };
            var result = SQLHelper.Single<TaskModel>(sql, param);

            return result;
        }

        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public bool Add(TaskModel task)
        {
            var sql = @" INSERT INTO p_Task
                               (TaskID,TaskName,TaskParam,CronExpressionString,AssemblyName,ClassName,Status,IsDelete,CreatedTime,ModifyTime,CronRemark,Remark)
                         VALUES
                               (@TaskID ,@TaskName,@TaskParam,@CronExpressionString,@AssemblyName,@ClassName,@Status,0,getdate(),getdate(),@CronRemark,@Remark)";

            object param = new
            {
                TaskID = task.TaskID,
                TaskName = task.TaskName,
                TaskParam = task.TaskParam,
                CronExpressionString = task.CronExpressionString,
                AssemblyName = task.AssemblyName,
                ClassName = task.ClassName,
                Status = task.Status,
                CronRemark = task.CronRemark,
                Remark = task.Remark
            };

            return SQLHelper.ExecuteNonQuery(sql, param) > 0;
        }

        /// <summary>
        /// 修改任务
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public bool Edit(TaskModel task)
        {

            var sql = @" UPDATE p_Task
                           SET TaskName = @TaskName,TaskParam = @TaskParam,CronExpressionString = @CronExpressionString,AssemblyName = @AssemblyName,ClassName = @ClassName,
                               Status = @Status,IsDelete = 0,ModifyTime =getdate() ,CronRemark = @CronRemark,Remark = @Remark
                         WHERE TaskID = @TaskID";

            object param = new
            {
                TaskID = task.TaskID,
                TaskName = task.TaskName,
                TaskParam = task.TaskParam,
                CronExpressionString = task.CronExpressionString,
                AssemblyName = task.AssemblyName,
                ClassName = task.ClassName,
                Status = task.Status,
                CronRemark = task.CronRemark,
                Remark = task.Remark
            };

            return SQLHelper.ExecuteNonQuery(sql, param) > 0;
        }
    }
}
