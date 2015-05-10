using System;
using System.Collections.Generic;
using System.Web;

namespace {$project}.{$App}
{
    public class ConnectionString
    {
		<vt:foreach from="$connectionStrings" item="conStr">
        public static string {$conStr.name} = @"{$conStr.value}";
		</vt:foreach>
	}
}