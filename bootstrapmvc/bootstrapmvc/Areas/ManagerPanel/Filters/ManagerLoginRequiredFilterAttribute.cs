using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc.Filters;
using System.Web.Mvc;
using bootstrapmvc.Models;

namespace bootstrapmvc.Areas.ManagerPanel.Filters
{
    public class ManagerLoginRequiredFilterAttribute : ActionFilterAttribute, IAuthenticationFilter
    {
        Model1 db= new Model1();
        public void OnAuthentication(AuthenticationContext filterContext)
        {
            
            if (string.IsNullOrEmpty(Convert.ToString(filterContext.HttpContext.Session["ManagerSession"])))
            {
                if (filterContext.HttpContext.Request.Cookies["ManagerCookie"] != null)
                {
                    HttpCookie SavedCookie = filterContext.HttpContext.Request.Cookies["ManagerCookie"];
                    string mail = SavedCookie.Values["mail"];
                    string password = SavedCookie.Values["password"];
                    Manager m = db.Managers.FirstOrDefault(x => x.Mail == mail && x.Password == password);
                    if (m != null)
                    {
                        if (m.IsActive)
                        {
                            filterContext.HttpContext.Session["ManagerSession"] = m;
                        }
                    }
                }
                else
                {
                    filterContext.Result = new HttpUnauthorizedResult();
                }
            }

        }

        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            
            if (filterContext.Result == null || filterContext.Result is HttpUnauthorizedResult)
            {
                filterContext.Result = new RedirectResult("~/ManagerPanel/Default/Index");
            }
        }

    }
}