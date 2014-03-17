﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using KM.JXC.BL;
using KM.JXC.DBA;
using KM.JXC.Common.Util;

namespace KM.JXC.Web.Filters
{
    public class AccessTokenValidation:ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            string user_id = filterContext.HttpContext.User.Identity.Name;

            //Verify cookie
            if (string.IsNullOrEmpty(user_id))
            {
                filterContext.HttpContext.Response.Redirect("Home/Login");
            }

            //Verify if the cookie user is a valid user
            UserManager userMgr = new UserManager();
            KM.JXC.DBA.User user = userMgr.GetUser(int.Parse(user_id));

            if (user == null)
            {
                filterContext.HttpContext.Response.Redirect("Home/Login");
            }

            //Verify if logon user already has access token in db
            KuanMaiEntities db = new KuanMaiEntities();
            var token = from t in db.Access_Token where t.User_ID == user.User_ID && t.Mall_Type_ID == user.Mall_Type select t;

            if (token == null) {
                filterContext.HttpContext.Response.Redirect("Home/Login");
            }

            //Verify if the existed access token is expired
            Access_Token access_token=token.ToList<Access_Token>()[0];

            int timeNow = DateTimeUtil.ConvertDateTimeToInt(DateTime.Now);
            if (timeNow >= access_token.Request_Time+access_token.Expirse_In)
            {
                filterContext.HttpContext.Response.Redirect("Home/Login");
            }
        }
    }
}