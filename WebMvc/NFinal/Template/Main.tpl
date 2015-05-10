using System;
using System.Collections.Generic;
using System.Web;
using System.Threading;
using System.Globalization;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace NFinal
{
    public class Main
    {
        protected string GetString(string val, string de)
        {
            return string.IsNullOrEmpty(val) ? de : val;
        }
        protected string UCFirst(string val)
        {
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            return cultureInfo.TextInfo.ToTitleCase(val);
        }
		  //第一个是app/第二个是Controller/第三个是Folder/第四个是Controller_Short/第五个是方法名/第六个是参数
        private static Regex reg = new Regex(@"(/[^/\s]+)(((?:/[^/\s]+)*)/([^/\s]+){$ControllerSuffix})/([^/\s.]+)((?:/[^/\s.]+)*)",  RegexOptions.IgnoreCase);
        public void Run(HttpContext context)
        {
            string folder = "/Manage";
            string controller = "/Manage/Index";
            string app = "/App";
            string controller_short = "Index";
            string action = "Index";
            string[] parameters = null;
            string url = context.Request.RawUrl;

            NameValueCollection get = new NameValueCollection();
            Match mat= reg.Match(context.Request.Path);
            if (mat.Success)
            {
                app =GetString(mat.Groups[1].Value,app);
                controller = GetString(mat.Groups[2].Value, folder);
                folder =mat.Groups[3].Value;
                controller_short =GetString(mat.Groups[4].Value,controller_short);
                action =GetString(mat.Groups[5].Value,action);
                if(mat.Groups[6].Success)
                {
                    parameters = mat.Groups[6].Value.Split('/');
                    //如果长度为奇数说明正确
                    if((parameters.Length&1)==1)
                    {
                        int count=parameters.Length>>1;
						if (count > 0)
                        {
                            for (int i = 0; i < count; i++)
                            {
                                get.Add(parameters[((i << 1)+1)], parameters[(i << 1) + 2]);
                            }
                        }
                    }
                }

                if (context.Request.Form.Count > 0)
                {
                    for (int i = 0; i < context.Request.Form.Count; i++)
                    {
                        get.Add(context.Request.Form.Keys[i], context.Request.Form[i]);
                    }
                }

				switch (app)
				{
					<vt:foreach from="$apps" item="app">
                    case "/{$app}":
					{ 
						{$project}.{$app}.Router router = new {$project}.{$app}.Router();
                        router.Run(context, app, folder, controller, action,get); break;
					}
					</vt:foreach>
                    default: break;
                }
            }
            else
            {
                context.Response.Write("wrong url");
            }
        }
    }
}