using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.CodeDom.Compiler;
using System.Data;
using Microsoft.CSharp;
namespace FQuery
{
    /// <summary>
    /// 对DataTable的补充方法
    /// </summary>
   public class EntityExtend
    {
       /// <summary>
       /// 全局缓存，只增不减
       /// </summary>
       private static Dictionary<string, Assembly> PrimaryCache = new Dictionary<string, Assembly>();
       /// <summary>
       /// 默认命名空间，实际使用的是调用实体的命名空间
       /// </summary>
       private  string fNameSpace = "_FQuery";
       /// <summary>
       /// 新建类的后缀
       /// </summary>
       private  string basic_Entity = "_entity";
       /// <summary>
       /// 创建类
       /// </summary>
       /// <typeparam name="T"></typeparam>
       /// <param name="isOrder"></param>
       /// <returns></returns>
       private StringBuilder GetCode<T>(bool isOrder=false) where T : new()
       {
          
           StringBuilder sb = new StringBuilder();
           string className = typeof(T).Name;
           fNameSpace = typeof(T).Namespace;
           sb.Append(@" using System;
                        using System.Collections.Generic;
                        using System.Text;");
           sb.Append(string.Format("namespace {0}\n",fNameSpace));
           sb.Append(" {\n");
           sb.Append(string.Format("public   class {0}", className + basic_Entity));
           sb.Append(" \n{\n");
           sb.Append(string.Format(" public  List<{0}> ToList(System.Data.DataTable dt)", className));
           sb.Append("\n{\n");
           sb.Append(string.Format("List<{0}> list = new List<{0}>();", className));
           sb.Append(" foreach (System.Data.DataRow dr in dt.Rows)");
           sb.Append("\n{\n");
           sb.Append(string.Format("{0} entity = new {0}();",className));
           int index = 0;
           foreach (var item in typeof(T).GetProperties())
           {
               if (isOrder)
               {
                   sb.Append(string.Format(@" entity.{0} = {1};", item.Name, string.Format(GetEvaluation(item,string.Format("dr[{0}]",index)))));
               }
               else
               {
                   sb.Append(string.Format(@" entity.{0} = {1};", item.Name, string.Format(GetEvaluation(item, string.Format("dr[{0}]","\""+ item.Name+"\"")))));
               }
               index++;
           }
           sb.Append(@" 
                list.Add(entity);
            }
            return list;
        }

    }
}");
           return sb;
       }
       /// <summary>
       /// 数据转换
       /// </summary>
       /// <param name="m"></param>
       /// <param name="value"></param>
       /// <returns></returns>
       private string GetEvaluation(MemberInfo m,string value)
       {
           string result = "";
           Type memberType = ((PropertyInfo)m).PropertyType;
           if (memberType == typeof(string))
           {
                result = value+".ToString()";
           }
           else if (memberType == typeof(bool))
           {
               result = "bool.Parse("+value+".ToString())";
           }
           else if (memberType == typeof(bool?))
           {
                result = value+"==null?null:bool.Parse("+value+".ToString())";
           }
           else if (memberType == typeof(Int16))
           {
                result = "int.Parse("+value+".ToString())";
           }
           else if (memberType == typeof(Int16?))
           {
               result = "" + value + "==null?null:int.Parse(" + value + ".ToString())";
           }
           else if (memberType == typeof(Int32))
           {
               result = "int.Parse(" + value + ".ToString())";
           }
           else if (memberType == typeof(Int32?))
           {
               result = "" + value + "==null?null:int.Parse(" + value + ".ToString())";
           }
           else if (memberType == typeof(Int64))
           {
               result = "int.Parse(" + value + ".ToString())";
           }
           else if (memberType == typeof(Int64?))
           {
               result = "" + value + "==null?null:int.Parse(" + value + ".ToString())";
           }
           else if (memberType == typeof(byte))
           {
               result = "byte.Parse(" + value + ".ToString())";
           }
           else if (memberType == typeof(byte?))
           {
               result = "" + value + "==null?null:byte.Parse(" + value + ".ToString())";
           }
           else if (memberType == typeof(char))
           {
               result = "char.Parse(" + value + ".ToString())";
           }
           else if (memberType == typeof(char?))
           {
               result = "" + value + "==null?null:char.Parse(" + value + ".ToString())";
           }
           else if (memberType == typeof(DateTime))
           {
               result = "DateTime.Parse(" + value + ".ToString())";
           }
           else if (memberType == typeof(DateTime?))
           {
               result = "" + value + "==null?null:DateTime.Parse(" + value + ".ToString())";
           }
           else if (memberType == typeof(decimal))
           {
               result = "decimal.Parse(" + value + ".ToString())";
           }
           else if (memberType == typeof(decimal?))
           {
               result = "" + value + "==null?null:decimal.Parse(" + value + ".ToString())";
           }
           else if (memberType == typeof(double))
           {
               result = "double.Parse(" + value + ".ToString())";
           }
           else if (memberType == typeof(double?))
           {
               result = "" + value + "==null?null:double.Parse(" + value + ".ToString())";
           }
           else if (memberType == typeof(float))
           {
               result = "float.Parse(" + value + ".ToString())";
           }
           else if (memberType == typeof(float?))
           {
               result = "" + value + "==null?null:float.Parse(" + value + ".ToString())";
           }
           else if (memberType == typeof(Guid))
           {
               result = "new Guid(" + value + ".ToString())";
           }
           else if (memberType == typeof(Guid?))
           {
               result = "" + value + "==null?null:new Guid(" + value + ".ToString())";
           }
           return result;
       }
       /// <summary>
       /// 引用关系设置
       /// </summary>
       /// <typeparam name="T"></typeparam>
       /// <returns></returns>
       private CompilerParameters GetParameters<T>() where T:new()
       {
           CompilerParameters paras = new CompilerParameters();
           paras.GenerateExecutable = false;
           paras.GenerateInMemory = true;
           //paras.ReferencedAssemblies.Add("System.Xml.dll");
           //paras.ReferencedAssemblies.Add("System.Data.dll");
           //foreach (var item in typeof(T).Assembly.GetReferencedAssemblies())
           //{
           //    if (item.Name != "System.Core")
           //        paras.ReferencedAssemblies.Add(item.Name + ".dll");
           //}
           paras.ReferencedAssemblies.Add("System.dll");
           paras.ReferencedAssemblies.Add("System.Data.dll");
           paras.ReferencedAssemblies.Add("mscorlib.dll");
           paras.ReferencedAssemblies.Add("System.Xml.dll");
           paras.ReferencedAssemblies.Add("mscorlib.dll");
           paras.ReferencedAssemblies.Add(typeof(T).Module.ToString());
           return paras;
       }
       /// <summary>
       /// 转换DataTable为实体
       /// </summary>
       /// <typeparam name="T"></typeparam>
       /// <param name="dt"></param>
       /// <returns></returns>
       public List<T> ToList<T>(DataTable dt) where T : new()
       {
           string fullName =typeof(T).Namespace+"."+ typeof(T).Name + basic_Entity;
           Assembly assembly = Complier<T>();
           var obj = assembly.CreateInstance(fullName);
           MethodInfo m = obj.GetType().GetMethod("ToList");
           object[] objs = new object[1];
           objs[0] = dt;
           return (List<T>)m.Invoke(obj, objs);
       }
       /// <summary>
       /// 编译实体
       /// </summary>
       /// <typeparam name="T"></typeparam>
       /// <returns></returns>
       private Assembly Complier<T>() where T:new()
       {
           string key = fNameSpace + "." + typeof(T).Name + basic_Entity;
           if (PrimaryCache.Keys.Contains(key))
           {
               return PrimaryCache[key];
           }
           CSharpCodeProvider csProvider = new CSharpCodeProvider();
           CompilerParameters paras = GetParameters<T>();
           CompilerResults result = csProvider.CompileAssemblyFromSource(paras, GetCode<T>().ToString());
           if (result.Errors.HasErrors == false)
           {
               key = fNameSpace + "." + typeof(T).Name + basic_Entity;
               PrimaryCache.Add(key, result.CompiledAssembly);
               return result.CompiledAssembly;
           }
           else
           {
               return null;
           }

       }
    }
}
