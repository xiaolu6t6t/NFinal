using System;
using System.Collections.Generic;
using System.Web;

namespace {$project}.{$app}.Models
{
    public class ConnectionStrings
    {
		<vt:foreach from="$connectionStrings" item="connectionString">
        public static string {$connectionString.name} = @"{$connectionString.value}";
		</vt:foreach>
    }
}