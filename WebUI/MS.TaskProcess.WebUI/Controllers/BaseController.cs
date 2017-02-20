using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MS.TaskProcess.Model;

namespace MS.TaskProcess.WebUI.Controllers
{
    public class BaseController : Controller
    {
        //public static string strLoginUrl = "/Account/Login";

        //public static string LoginUrl = UrlHelper.GenerateUrl()

        public UserModel CurrentUser
        {
            get
            {
                if (Session["loginuser"] != null)
                {
                    var currentUser = (UserModel)Session["loginuser"];
                    return currentUser;
                }
                return null;
            }
        }

        #region 分页信息
        protected int PageSize
        {
            get
            {
                if (Request["PageSize"] != null)
                    return int.Parse(Request["PageSize"]);
                else return 20;
            }
        }
        protected int PageNo
        {
            get
            {
                if (Request["PageNo"] != null)
                    return int.Parse(Request["PageNo"]);
                else return 1;
            }
        }
        #endregion

        #region 当前请求信息
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (CurrentUser == null)
            {
                var returnUrl = filterContext.HttpContext.Request.Url.AbsoluteUri;
                var res = Redirect(ConstantData.LOGINURL + "?ReturnUrl=" + returnUrl);
                filterContext.Result = res;
            }

            #region 身份验证

            ViewBag.UserModel = CurrentUser;

            #endregion

            string currPageName = string.Empty;

            if (Request["PageName"] != null)
            {
                currPageName = Request["PageName"];
            }
            else
            {
                string url = filterContext.HttpContext.Request.Url.AbsolutePath;
                currPageName = url.Substring(url.LastIndexOf('/') + 1);
            }
            //当前页面
            ViewBag.PageName = currPageName;

        }

        /// <summary>
        /// 返回登录Url
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private string GetLoginUrl(HttpRequestBase request)
        {
            var url = request.Url;
            String returnUrl = null;
            if (request.HttpMethod == System.Net.WebRequestMethods.Http.Get)
            {
                if (String.Equals(url.AbsolutePath, "/Home/LoginOut", StringComparison.OrdinalIgnoreCase))
                {
                    var urlReferrer = request.UrlReferrer;
                    if (urlReferrer != null)
                    {
                        if (String.Equals(url.Scheme, urlReferrer.Scheme, StringComparison.OrdinalIgnoreCase) && String.Equals(url.Host, urlReferrer.Host, StringComparison.OrdinalIgnoreCase) && url.Port == urlReferrer.Port)
                        {
                            returnUrl = urlReferrer.PathAndQuery;
                        }
                        else
                        {
                            returnUrl = urlReferrer.OriginalString;
                        }
                    }
                }
                else
                {
                    returnUrl = url.PathAndQuery;
                }
            }
            String loginUrl;
            if (String.IsNullOrEmpty(returnUrl))
            {
                loginUrl = ConstantData.LOGINURL;
            }
            else
            {
                loginUrl = ConstantData.LOGINURL + "?ReturnUrl=" + HttpUtility.UrlEncode(returnUrl);
            }
            return loginUrl;
        }
        #endregion

    }
}
