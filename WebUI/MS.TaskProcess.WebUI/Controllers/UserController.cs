

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

    public class UserController : BaseController
    {
        public ActionResult List()
        {
            UserBLL userBll = new UserBLL();
            var users = userBll.GetUserList(PageNo, PageSize);
            return View(users);
        }
    }
}