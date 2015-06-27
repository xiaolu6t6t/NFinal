using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace {$aspxPageNameSpace}
{
    public partial class {$aspxPageClassName} : System.Web.UI.Page
    {
		//数据库模型
        {$StructDatas}
		//函数内变量
        public class {$aspxPageClassName}_AutoComplete:{$baseName}
        {
			//参数变量
			<vt:foreach from="$parameterDataList" item="parameterData">
				<vt:if var="parameterData.isArray" value="True">
					public {$parameterData.type}[] {$parameterData.name};
				<vt:else>
					public {$parameterData.type} {$parameterData.name};
				</vt:if>
			</vt:foreach>
			//数据库变量
			<vt:foreach from="$functionDataList" item="functionData">
				<vt:if var="functionData.functionName" value="Delete">
					public int {$functionData.varName};
				</vt:if>
				<vt:if var="functionData.functionName" value="ExecuteNonQuery">
					public int {$functionData.varName};
				</vt:if>
				<vt:if var="functionData.functionName" value="Insert">
					public int {$functionData.varName};
				</vt:if>
				<vt:if var="functionData.functionName" value="Page">
					public NFinal.DB.NList<__{$methodName}_{$functionData.varName}__> {$functionData.varName};
				</vt:if>
				<vt:if var="functionData.functionName" value="QueryAll">
					public NFinal.DB.NList<__{$methodName}_{$functionData.varName}__> {$functionData.varName};
				</vt:if>
				<vt:if var="functionData.functionName" value="QueryObject">
					public NFinal.DB.NList<__{$methodName}_{$functionData.varName}__> {$functionData.varName};
				</vt:if>
				<vt:if var="functionData.functionName" value="QueryRandom">
					public NFinal.DB.NList<__{$methodName}_{$functionData.varName}__> {$functionData.varName};
				</vt:if>
				<vt:if var="functionData.functionName" value="QueryTop">
					public NFinal.DB.NList<__{$methodName}_{$functionData.varName}__> {$functionData.varName};
				</vt:if>
				<vt:if var="functionData.functionName" value="QueryRow">
					public __{$methodName}_{$functionData.varName}__ {$functionData.varName};
				</vt:if>
				<vt:if var="functionData.functionName" value="QueryRandom">
					public NFinal.DB.NList<__{$methodName}_{$functionData.varName}__> {$functionData.varName};
				</vt:if>
				<vt:if var="functionData.functionName" value="Update">
					public int {$functionData.varName};
				</vt:if>
			</vt:foreach>
			//一般变量
			<vt:foreach from="$csharpDeclarationList" item="csharpDeclaration">
				public {$csharpDeclaration.typeName} {$csharpDeclaration.varName};
			</vt:foreach>
			//DAL函数声明变量
        }
		//变量存储类,用于自动完成.
        public {$aspxPageClassName}_AutoComplete ViewBag = new {$aspxPageClassName}_AutoComplete();
    }
}