using System;
using System.Collections.Generic;
using System.Web;

namespace {$namespace}
{
    public static class Extension
    {
        public static void Error(this NFinal.BaseAction action, string msg, string url, int second)
        {
            Web.Default.Common.Public.ErrorAction errorAction = new Web.Default.Common.Public.ErrorAction(action._tw);
            errorAction.Error(msg, url, second);
        }
        public static void Success(this NFinal.BaseAction action, string msg, string url, int second)
        {
            Web.Default.Common.Public.SuccessAction successAction = new Web.Default.Common.Public.SuccessAction(action._tw);
            successAction.Success(msg, url, second);
        }
		public static bool IsNullOrWhiteSpace(this string str)
        {
            return (str == null || str.Trim() == string.Empty);
        }
    }
}