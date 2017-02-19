
namespace MS.TaskProcess.WebUI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;
    using System.Web.Script.Serialization;
    using MS.TaskProcess.BLL;
    using MS.Common;
    using MS.Common.TaskHelper;

    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult LoginOut()
        {
        
            Session["loginuser"] = null;

            string redirectUrl = string.Empty;
            if (Request.UrlReferrer != null)
            {
                redirectUrl = HttpUtility.UrlEncode(Request.UrlReferrer.PathAndQuery);
            }
            string loginUrl = FormsAuthentication.LoginUrl;
            if (Request.HttpMethod == System.Net.WebRequestMethods.Http.Get && !string.IsNullOrEmpty(redirectUrl))
            {
                loginUrl += "?ReturnUrl=" + redirectUrl;
            }
            return Redirect(loginUrl);
        }
    }
}
