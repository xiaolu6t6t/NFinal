using System;
using System.Collections.Generic;
using System.Web;
using System.Text.RegularExpressions;

namespace NFinal.Compile
{
    /// <summary>
    /// 分析出csharp语句中相关的数据库函数,以及sql语句
    /// </summary>
    public struct DbFunctionData
    {
        public string sql;
        public bool hasSqlError;
        public string sqlError;
        public string type;
        public string varName;
        public string connectionName;
        public string functionName;
        public string[] parameters;
        public string expression;
        public string convertMethodName;
        public int index;
        public int length;
        public List<NFinal.DB.Coding.Table> tables;
        public List<NFinal.DB.Coding.Field> fields;
        public List<NFinal.Compile.SqlVarParameter> sqlVarParameters;
    }
    /// <summary>
    /// csharp魔法函数语句分析和编译类
    /// </summary>
    public class SqlCompiler
    {
        private static Regex dbFuncitonRegex = new Regex(@"(?:(\S+)\s+)?(\S+)\s*=\s*DB\s*.\s*([^\s.]+)\s*.\s*(\S+)\s*\(\s*(?:@?""([^""]*)""(?:,\s*([^\s,\)]+))?)?\s*\)(?:\s*.\s*([^\(\)\s]+)\s*\(\s*\))?\s*;");
        public SqlCompiler()
        {
        }
        /// <summary>
        /// 分析代码
        /// </summary>
        /// <param name="csharpCode">代码</param>
        /// <param name="index">代码开始的位置</param>
        /// <returns></returns>
        public List<DbFunctionData> Compile(string csharpCode)
        {
            List<DbFunctionData> dbFunctionDatas = new List<DbFunctionData>();
            DbFunctionData data;
            MatchCollection macDbFunctions = dbFuncitonRegex.Matches(csharpCode);
            Match matDbFunction = null;
            if (macDbFunctions.Count > 0)
            {
                for (int i = 0; i < macDbFunctions.Count; i++)
                {
                    matDbFunction = macDbFunctions[i];
                    if (matDbFunction.Success)
                    {
                        data = new DbFunctionData();
                        data.expression = matDbFunction.Groups[0].Value;
                        data.type = matDbFunction.Groups[1].Value;
                        data.varName = matDbFunction.Groups[2].Value;
                        data.connectionName = matDbFunction.Groups[3].Value;
                        data.functionName = matDbFunction.Groups[4].Value;
                        data.sql = matDbFunction.Groups[5].Value;
                        if (matDbFunction.Groups[6].Success)
                        {
                            data.parameters = new string[2];
                            //select sql
                            data.parameters[0] = matDbFunction.Groups[5].Value;
                            //pageSize
                            data.parameters[1] = matDbFunction.Groups[6].Value;
                        }
                        else
                        {
                            data.parameters = new string[1];
                            data.parameters[0] = matDbFunction.Groups[5].Value;
                        }
                        data.convertMethodName = matDbFunction.Groups[7].Value;
                        data.index = matDbFunction.Index;
                        data.length = matDbFunction.Length;
                        dbFunctionDatas.Add(data);
                    }
                }
            }
            return dbFunctionDatas;
        }
        //在某段位置替换成另一段代码
        public int Replace(ref string str, int index, int length, string rep)
        {
            if (length > 0)
            {
                str = str.Remove(index, length);
            }
            if (index > 0)
            {
                str = str.Insert(index, rep);
            }
            return rep.Length - length;
        }
        private DB.ConnectionString GetConnectionString(string name)
        {
            if (Config.ConnectionStrings.Count > 0)
            {
                for (int i = 0; i < Config.ConnectionStrings.Count; i++)
                {
                    if (Config.ConnectionStrings[i].name == name)
                    {
                        return Config.ConnectionStrings[i];
                    }
                }
                return null;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 执行数据库魔法函数
        /// </summary>
        /// <param name="methodName">Controller的函数名</param>
        /// <param name="csharpFileCode">Controller的函数内的代码</param>
        /// <param name="dbFunctionDatas">代码内分析出的魔法函数</param>
        /// <param name="appRoot">项目的根目录</param>
        /// <returns></returns>
        public int SetMagicFunction(string methodName, ref string csharpFileCode, int relative_position, List<DbFunctionData> dbFunctionDatas,string appRoot)
        {
            if(dbFunctionDatas.Count>0)
            {
                string webCsharpCode = "";
                VTemplate.Engine.TemplateDocument doc=null;
                NFinal.DB.ConnectionString connectionString=null;
                string fileName = null;
 
                for (int i = 0; i < dbFunctionDatas.Count; i++)
                {
                    connectionString= GetConnectionString(dbFunctionDatas[i].connectionName);
                    if(connectionString!=null)
                    {
                        if (dbFunctionDatas[i].functionName == "QueryAll")
                        {
                            fileName = appRoot +
                                "NFinal\\SqlTemplate\\"+
                                connectionString.type.ToString()+
                                "\\QueryAll.txt";
                            doc=new VTemplate.Engine.TemplateDocument(
                                fileName,System.Text.Encoding.UTF8);
                            doc.SetValue("functionName",methodName);
                            doc.SetValue("varName",dbFunctionDatas[i].varName);
                            doc.SetValue("dbName", dbFunctionDatas[i].connectionName);
                            doc.SetValue("sql", dbFunctionDatas[i].sql);
                            doc.SetValue("fields", dbFunctionDatas[i].fields);
                            doc.SetValue("sqlVarParameters", dbFunctionDatas[i].sqlVarParameters);
                            webCsharpCode = doc.GetRenderText();
                            relative_position += Replace(ref csharpFileCode,
                                relative_position + dbFunctionDatas[i].index,
                                dbFunctionDatas[i].length,
                                webCsharpCode);
                        }
                        if (dbFunctionDatas[i].functionName == "QueryRow")
                        {
                            fileName = appRoot +
                                "NFinal\\SqlTemplate\\" +
                                connectionString.type.ToString() +
                                "\\QueryRow.txt";
                            doc = new VTemplate.Engine.TemplateDocument(
                                fileName,System.Text.Encoding.UTF8
                                );
                            doc = new VTemplate.Engine.TemplateDocument(
                                fileName, System.Text.Encoding.UTF8);
                            doc.SetValue("functionName", methodName);
                            doc.SetValue("varName", dbFunctionDatas[i].varName);
                            doc.SetValue("dbName", dbFunctionDatas[i].connectionName);
                            doc.SetValue("sql", dbFunctionDatas[i].sql);
                            doc.SetValue("fields", dbFunctionDatas[i].fields);
                            doc.SetValue("sqlVarParameters", dbFunctionDatas[i].sqlVarParameters);
                            webCsharpCode = doc.GetRenderText();
                            relative_position += Replace(ref csharpFileCode,
                                relative_position + dbFunctionDatas[i].index,
                                dbFunctionDatas[i].length,
                                webCsharpCode);
                        }
                        if (dbFunctionDatas[i].functionName == "Insert")
                        {   
                            fileName = appRoot +
                                "NFinal\\SqlTemplate\\" +
                                connectionString.type.ToString() +
                                "\\Insert.txt";
                            doc = new VTemplate.Engine.TemplateDocument(
                                fileName, System.Text.Encoding.UTF8
                                );
                            doc = new VTemplate.Engine.TemplateDocument(
                                fileName, System.Text.Encoding.UTF8);
                            doc.SetValue("functionName", methodName);
                            doc.SetValue("varName", dbFunctionDatas[i].varName);
                            doc.SetValue("dbName", dbFunctionDatas[i].connectionName);
                            doc.SetValue("sql", dbFunctionDatas[i].sql);
                            doc.SetValue("fields", dbFunctionDatas[i].fields);
                            doc.SetValue("sqlVarParameters", dbFunctionDatas[i].sqlVarParameters);
                            webCsharpCode = doc.GetRenderText();
                            relative_position += Replace(ref csharpFileCode,
                                relative_position + dbFunctionDatas[i].index,
                                dbFunctionDatas[i].length,
                                webCsharpCode);

                        }
                        if (dbFunctionDatas[i].functionName == "Update")
                        {
                            fileName = appRoot +
                                "NFinal\\SqlTemplate\\" +
                                connectionString.type.ToString() +
                                "\\Update.txt";
                            doc = new VTemplate.Engine.TemplateDocument(
                                fileName, System.Text.Encoding.UTF8
                                );
                            doc = new VTemplate.Engine.TemplateDocument(
                                fileName, System.Text.Encoding.UTF8);
                            doc.SetValue("functionName", methodName);
                            doc.SetValue("varName", dbFunctionDatas[i].varName);
                            doc.SetValue("dbName", dbFunctionDatas[i].connectionName);
                            doc.SetValue("sql", dbFunctionDatas[i].sql);
                            doc.SetValue("fields", dbFunctionDatas[i].fields);
                            doc.SetValue("sqlVarParameters", dbFunctionDatas[i].sqlVarParameters);
                            webCsharpCode = doc.GetRenderText();
                            relative_position += Replace(ref csharpFileCode,
                                relative_position + dbFunctionDatas[i].index,
                                dbFunctionDatas[i].length,
                                webCsharpCode);

                        }
                        if (dbFunctionDatas[i].functionName == "Delete")
                        {
                            fileName = appRoot +
                                "NFinal\\SqlTemplate\\" +
                                connectionString.type.ToString() +
                                "\\Update.txt";
                            doc = new VTemplate.Engine.TemplateDocument(
                                fileName, System.Text.Encoding.UTF8
                                );
                            doc = new VTemplate.Engine.TemplateDocument(
                                fileName, System.Text.Encoding.UTF8);
                            doc.SetValue("functionName", methodName);
                            doc.SetValue("varName", dbFunctionDatas[i].varName);
                            doc.SetValue("dbName", dbFunctionDatas[i].connectionName);
                            doc.SetValue("sql", dbFunctionDatas[i].sql);
                            doc.SetValue("fields", dbFunctionDatas[i].fields);
                            doc.SetValue("sqlVarParameters", dbFunctionDatas[i].sqlVarParameters);
                            webCsharpCode = doc.GetRenderText();
                            relative_position += Replace(ref csharpFileCode,
                                relative_position + dbFunctionDatas[i].index,
                                dbFunctionDatas[i].length,
                                webCsharpCode);

                        }
                        if (dbFunctionDatas[i].functionName == "QueryObject")
                        {
                            fileName = appRoot +
                                "NFinal\\SqlTemplate\\" +
                                connectionString.type.ToString() +
                                "\\QueryObject.txt";
                            doc = new VTemplate.Engine.TemplateDocument(
                                fileName, System.Text.Encoding.UTF8
                                );
                           
                            doc = new VTemplate.Engine.TemplateDocument(
                                fileName, System.Text.Encoding.UTF8);
                            doc.SetValue("functionName", methodName);
                            doc.SetValue("varName", dbFunctionDatas[i].varName);
                            doc.SetValue("dbName", dbFunctionDatas[i].connectionName);
                            doc.SetValue("sql", dbFunctionDatas[i].sql);
                            doc.SetValue("fields", dbFunctionDatas[i].fields);
                            doc.SetValue("sqlVarParameters", dbFunctionDatas[i].sqlVarParameters);
                            doc.SetValue("convertMethodName", dbFunctionDatas[i].convertMethodName);
                            webCsharpCode = doc.GetRenderText();
                            relative_position += Replace(ref csharpFileCode,
                                relative_position + dbFunctionDatas[i].index,
                                dbFunctionDatas[i].length,
                                webCsharpCode);
                        }
                        if (dbFunctionDatas[i].functionName == "Page")
                        {
                            fileName = appRoot +
                            "NFinal\\SqlTemplate\\" +
                            connectionString.type.ToString() +
                            "\\Page.txt";
                            doc = new VTemplate.Engine.TemplateDocument(
                                fileName, System.Text.Encoding.UTF8
                                );

                            doc = new VTemplate.Engine.TemplateDocument(
                                fileName, System.Text.Encoding.UTF8);
                            doc.SetValue("functionName", methodName);
                            doc.SetValue("varName", dbFunctionDatas[i].varName);
                            doc.SetValue("dbName", dbFunctionDatas[i].connectionName);
                            doc.SetValue("fields", dbFunctionDatas[i].fields);

                            //分页参数解析
                            PageSqlAnalyse pageStatement = new PageSqlAnalyse(dbFunctionDatas[i].sql,connectionString.type);
                            pageStatement.Parse();
                            doc.SetValue("pageSql", pageStatement.pageSql);
                            doc.SetValue("countSql",pageStatement.countSql);
                            doc.SetValue("pageSizeVarName", dbFunctionDatas[i].parameters[1]);

                            doc.SetValue("sqlVarParameters", dbFunctionDatas[i].sqlVarParameters);
                            doc.SetValue("convertMethodName", dbFunctionDatas[i].convertMethodName);
                            webCsharpCode = doc.GetRenderText();
                            relative_position += Replace(ref csharpFileCode,
                                relative_position + dbFunctionDatas[i].index,
                                dbFunctionDatas[i].length,
                                webCsharpCode);
                        }
                        if (dbFunctionDatas[i].functionName == "QueryRandom")
                        {
                            fileName = appRoot +
                                "NFinal\\SqlTemplate\\" +
                                connectionString.type.ToString() +
                                "\\QueryRandom.txt";
                            doc = new VTemplate.Engine.TemplateDocument(
                                fileName, System.Text.Encoding.UTF8);
                            doc.SetValue("functionName", methodName);
                            doc.SetValue("varName", dbFunctionDatas[i].varName);
                            doc.SetValue("dbName", dbFunctionDatas[i].connectionName);

                            //select 语句 转为选取某随机行的语句

                            RandomSqlAnalyse random = new RandomSqlAnalyse(dbFunctionDatas[i].sql, connectionString.type);
                            random.Parse();
                            doc.SetValue("topNumber", dbFunctionDatas[i].parameters[1]);
                            
                            doc.SetValue("sql", random.randomSql);
                            doc.SetValue("fields", dbFunctionDatas[i].fields);
                            doc.SetValue("sqlVarParameters", dbFunctionDatas[i].sqlVarParameters);
                            webCsharpCode = doc.GetRenderText();
                            relative_position += Replace(ref csharpFileCode,
                                relative_position + dbFunctionDatas[i].index,
                                dbFunctionDatas[i].length,
                                webCsharpCode);
                        }
                    }
                }
            }
            return relative_position;
        }
        /// <summary>
        /// 数据库函数替换类
        /// </summary>
        /// <param name="methodName">函数名</param>
        /// <param name="dbFunctionData">函数信息</param>
        /// <param name="appRoot">网站根目录</param>
        /// <returns></returns>
        public string SetMagicStruct(string methodName,DbFunctionData dbFunctionData,List<NFinal.Compile.StructField> structFieldList,string appRoot)
        {
            string result = null;
            if (dbFunctionData.functionName == "QueryAll" || dbFunctionData.functionName == "QueryRow" 
                || dbFunctionData.functionName == "Page" || dbFunctionData.functionName == "QueryRandom")
            {
                NFinal.DB.ConnectionString connectionString = null;
                connectionString = GetConnectionString(dbFunctionData.connectionName);
                string type = connectionString.type.ToString();
                string fileName = appRoot + "NFinal\\SqlTemplate\\" +
                    type +
                    "\\Struct.txt";
                VTemplate.Engine.TemplateDocument doc = new VTemplate.Engine.TemplateDocument(fileName, System.Text.Encoding.UTF8);
                doc.SetValue("functionName", methodName);
                doc.SetValue("varName", dbFunctionData.varName);
                doc.SetValue("dbType", type);
                doc.SetValue("fields", dbFunctionData.fields);
                doc.SetValue("structFields", structFieldList);
                result = doc.GetRenderText();
            }
            return result;
        }
       
    }
}