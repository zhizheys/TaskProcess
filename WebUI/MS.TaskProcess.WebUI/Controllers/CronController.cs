

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


    public class CronController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult NextFireTime()
        {
            string cronExpressionString = Request.Params["CronExpression"].ToString();
            try
            {
                var result = QuartzHelper.GetNextFireTime(cronExpressionString, 5);

                string msg = JsonHelper.ToJson(result);

                return Json(new { result = true, msg = msg });
            }
            catch
            {
                return Json(new { result = false, msg = "" });
            }
        }
    }
}