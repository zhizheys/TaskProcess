
namespace MS.TaskProcess.WebUI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web.Mvc;
    using System.Web.Script.Serialization;
    using MS.TaskProcess.BLL;
    using MS.Common;
    using MS.Common.TaskHelper;
    using MS.TaskProcess.Model;


    public class TaskController : BaseController
    {

        MS.TaskProcess.BLL.TaskBLL task = new MS.TaskProcess.BLL.TaskBLL();

        public ActionResult Grid()
        {
            var result = task.GetTaskList(PageNo, PageSize);

            return View(result);
        }

        public ActionResult Edit(string taskId)
        {
            TaskModel model = new TaskModel();
            if (!string.IsNullOrEmpty(taskId))
            {
                model = task.GetTaskById(taskId);
            }
            return PartialView("_Edit", model);
        }

        public JsonResult Save(string data, string action)
        {
            try
            {
                var taskmodel = JsonHelper.ToObject<TaskModel>(data);

                var result = false;
                if (QuartzHelper.ValidExpression(taskmodel.CronExpressionString))
                {
                    result = MS.Common.TaskHelper.TaskActionHelper.AddTask(taskmodel, action);
                    return Json(new { result = false, msg = "保存成功" });
                }
                else
                {
                    return Json(new { result = false, msg = "Cron表达式错误" });
                }

                
            }
            catch (Exception ex)
            {
                return Json(new { result = false, msg = ex.Message });
            }
        }

        public ActionResult UpdateStatus(string taskId, int status)
        {
            try
            {
                MS.Common.TaskHelper.TaskActionHelper.UpdateTaskStatus(taskId, (TaskStatus)status);

                return Json(new { result = true, msg = "操作成功" });
            }
            catch (Exception ex)
            {
                return Json(new { result = false, msg = ex.Message });
            }
        }
    }
}