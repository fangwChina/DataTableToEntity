using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;
using EmitMapper;
using EmitMapper.MappingConfiguration;
using EmitMapper.MappingConfiguration.MappingOperations;
using EmitMapper.Utils;
using System.Data.SqlClient;
using System.Data.Common;
using EmitMapper.Mappers;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
namespace Test
{
    public class ReflectionTest
    {
        Dictionary<string, Assembly> dic = new Dictionary<string, Assembly>();
        /// <summary>
        /// 传统方法转换实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public List<T> ToList<T>(DataTable dt) where T : new()
        {
            List<T> list = new List<T>();
            var obj = typeof(T).Assembly.CreateInstance(typeof(T).FullName);
            foreach (DataRow dr in dt.Rows)
            {
                foreach (DataColumn dc in dt.Columns)
                {
                    try
                    {
                        obj.GetType().GetProperty(dc.ColumnName).SetValue(obj, dr[dc.ColumnName], null);
                        foreach (var item in obj.GetType().GetProperties())
                        {
                            if (item.Name == dc.ColumnName)
                            {
                                item.SetValue(obj, dr[dc.ColumnName], null);
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
                list.Add((T)obj);
            }
            return list;
        }

        //public List<T> ToList1<T>(DataTable dt) where T : new()
        //{
        //    //dt.AsEnumerable().s

        //}
        /// <summary>
        /// 使用委托进行性能优化[实际效果更差]
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public List<T> ToList_Delegate<T>(DataTable dt) where T : new()
        {
            List<T> list = new List<T>();
            foreach (DataRow dr in dt.Rows)
            {
                var obj = typeof(T).Assembly.CreateInstance(typeof(T).FullName);
                foreach (DataColumn dc in dt.Columns)
                {
                    try
                    {
                        foreach (var item in obj.GetType().GetProperties())
                        {
                            if (item.Name == dc.ColumnName)
                            {
                                //item.SetValue(obj, dr[dc.ColumnName], null);
                                ((Action<T, object>)Delegate.CreateDelegate(typeof(Action<T, object>), item.GetSetMethod()))((T)obj, dr[dc.ColumnName]);
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
                list.Add((T)obj);
            }
            return list;
        }

        public List<TestEntiy> ToList_Direct(DataTable dt)
        {
            List<TestEntiy> list = new List<TestEntiy>();
            foreach (DataRow dr in dt.Rows)
            {
                TestEntiy entity = new TestEntiy();
                entity.ID = dr[0].ToString();
                entity.Name = dr[1].ToString();
                entity.a = dr[2].ToString();
                entity.a1 = dr[3].ToString();
                entity.a2 = dr[4].ToString();
                entity.a3 = dr[5].ToString();
                entity.a4 = dr[6].ToString();
                entity.a5 = dr[7].ToString();
                entity.a6 = dr[8].ToString();
                entity.a7 = dr[9].ToString();
                entity.a8 = dr[10].ToString();
                entity.a9 = dr[11].ToString();
                list.Add(entity);
            }
            return list;
        }
        /// <summary>
        /// 使用扩展方法调用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public List<T> ToList_Extend<T>(DataTable dt) where T : new()
        {
            var obj = typeof(T).Assembly.CreateInstance(typeof(ObjectExtend).FullName);
            System.Reflection.MethodInfo minfo = typeof(ObjectExtend).GetMethod("ToList");
            if (minfo != null)
            {
                object[] objs = new object[1];
                objs[0]  = dt;
                return (List<T>)minfo.Invoke(obj, objs);
            }
            return null;
        }

        public List<T> ToList_Emit<T>(DataTable dt) where T : new()
        {
         
            List<T> list= new DataTableToObjectMapper<T>().ReadCollection(dt).ToList();
            var mapper = ObjectMapperManager.DefaultInstance.GetMapper<DataRow, T>(new DataTableMappingConfig());
            foreach (DataRow dr in dt.Rows)
            {
                T obj = mapper.Map(dr);
            }
            
            return list;
        }

        public List<T> ToList_Emit2<T>(DataTable dt) where T:new()
        {
            List<T> list = new List<T>();
            if (dic.Keys.Contains("Test.ObjectExtend1"))
            {
                Assembly assembly = dic["Test.ObjectExtend1"];
                var obj = assembly.CreateInstance("Test.ObjectExtend1");
                System.Reflection.MethodInfo minfo = obj.GetType().GetMethod("ToList1");
                if (minfo != null)
                {
                    object[] objs = new object[1];
                    objs[0] = dt;
                    return (List<T>)minfo.Invoke(obj, objs);
                }
                return list;
            }

          
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
namespace Test
{

public  partial class ObjectExtend1
    {
        public  System.Collections.Generic.List<Test.TestEntiy> ToList1(System.Data.DataTable dt)
        {
            System.Collections.Generic.List<Test.TestEntiy> list = new System.Collections.Generic.List<Test.TestEntiy>();
            foreach (System.Data.DataRow dr in dt.Rows)
            {
                Test.TestEntiy entity = new Test.TestEntiy();
                entity.ID = dr[0].ToString();
                entity.Name = dr[1].ToString();
                entity.a = dr[2].ToString();
                entity.a1 = dr[3].ToString();
                entity.a2 = dr[4].ToString();
                entity.a3 = dr[5].ToString();
                entity.a4 = dr[6].ToString();
                entity.a5 = dr[7].ToString();
                entity.a6 = dr[8].ToString();
                entity.a7 = dr[9].ToString();
                entity.a8 = dr[10].ToString();
                entity.a9 = dr[11].ToString();
                list.Add(entity);
            }
            return list;
        }

    }
}");

            CSharpCodeProvider csProvider = new CSharpCodeProvider();
            CompilerParameters paras = new CompilerParameters();
            paras.GenerateExecutable = false;
            paras.GenerateInMemory = true;
            paras.ReferencedAssemblies.Add("System.dll");
            paras.ReferencedAssemblies.Add("System.Data.dll");
            paras.ReferencedAssemblies.Add("mscorlib.dll");
            paras.ReferencedAssemblies.Add("Test.exe");
            paras.ReferencedAssemblies.Add("System.Xml.dll");
            CompilerResults result = csProvider.CompileAssemblyFromSource(paras, sb.ToString());
            if (result.Errors.HasErrors==false)
            {
                Assembly assembly = result.CompiledAssembly;
                dic.Add("Test.ObjectExtend1", assembly);
                var obj = assembly.CreateInstance("Test.ObjectExtend1");
                System.Reflection.MethodInfo minfo = obj.GetType().GetMethod("ToList1");
                if (minfo != null)
                {
                    object[] objs = new object[1];
                    objs[0] = dt;
                    return (List<T>)minfo.Invoke(obj, objs);
                }
                return list;
            }
            return null;
        }

  
    
    }

    /// <summary>
    /// 部分类
    /// </summary>
    public  partial class ObjectExtend
    {
        public  List<TestEntiy> ToList(DataTable dt)
        {
           List<TestEntiy> list = new System.Collections.Generic.List<TestEntiy>();
            foreach (DataRow dr in dt.Rows)
            {
               TestEntiy entity = new TestEntiy();
                entity.ID = dr[0].ToString();
                entity.Name = dr[1].ToString();
                entity.a = dr[2].ToString();
                entity.a1 = dr[3].ToString();
                entity.a2 = dr[4].ToString();
                entity.a3 = dr[5].ToString();
                entity.a4 = dr[6].ToString();
                entity.a5 = dr[7].ToString();
                entity.a6 = dr[8].ToString();
                entity.a7 = dr[9].ToString();
                entity.a8 = dr[10].ToString();
                entity.a9 = dr[11].ToString();
                list.Add(entity);
            }
            return list;
        }

    }

 

    public class DataTableToObjectMapper<T> : ObjectsMapper<DataRow, T>
    {
        class DataTableMappingConfig : IMappingConfigurator
        {
            public DataTableMappingConfig()
            {
            }

            public IRootMappingOperation GetRootMappingOperation(Type from, Type to)
            {
                return null;
            }

           

            public IMappingOperation[] GetMappingOperations(Type from, Type to)
            {
                return ReflectionUtils
                    .GetPublicFieldsAndProperties(to)
                    .Where(m => m.MemberType == MemberTypes.Field || m.MemberType == MemberTypes.Property && ((PropertyInfo)m).GetSetMethod() != null)
                    .Select(
                        (m, ind) =>
                            new DestWriteOperation()
                            {
                                Destination = new MemberDescriptor(new[] { m }),
                                Getter = (ValueGetter<object>)((value, state) => {
                                                                Func<object, object> converter = StaticConvertersManager.DefaultInstance.GetStaticConverterFunc(typeof(object), ReflectionUtils.GetMemberType(m));
                                                                var dr = value as DataRow;
                                                                return ValueToWrite<object>.ReturnValue(converter(dr[m.Name]));
                                                            })
                            }
                    ).ToArray();
            }

            public string GetConfigurationName()
            {
                return "datarow_";
            }

            public StaticConvertersManager GetStaticConvertersManager()
            {
                return null;
            }
        }

        public DataTableToObjectMapper() : base(ObjectMapperManager.DefaultInstance.GetMapperImpl(typeof(DataRow), typeof(T), new DataTableMappingConfig()))
        {
        }

        public IEnumerable<T> ReadCollection(DataTable dt)
        {
            foreach (DataRow dr in dt.Rows)
            {
                yield return Map(dr);
            }
        }
    }

    class DataTableMappingConfig : IMappingConfigurator
    {
        public DataTableMappingConfig()
        {
        }

        public IRootMappingOperation GetRootMappingOperation(Type from, Type to)
        {
            return null;
        }



        public IMappingOperation[] GetMappingOperations(Type from, Type to)
        {
            return ReflectionUtils
                .GetPublicFieldsAndProperties(to)
                .Where(m => m.MemberType == MemberTypes.Field || m.MemberType == MemberTypes.Property && ((PropertyInfo)m).GetSetMethod() != null)
                .Select(
                    (m, ind) =>
                        new DestWriteOperation()
                        {
                            Destination = new MemberDescriptor(new[] { m }),
                            Getter = (ValueGetter<object>)((value, state) =>
                            {
                                Func<object, object> converter = StaticConvertersManager.DefaultInstance.GetStaticConverterFunc(typeof(object), ReflectionUtils.GetMemberType(m));
                                var dr = value as DataRow;
                                return ValueToWrite<object>.ReturnValue(converter(dr[m.Name]));
                            })
                        }
                ).ToArray();
        }

        public string GetConfigurationName()
        {
            return "datarow_";
        }

        public StaticConvertersManager GetStaticConvertersManager()
        {
            return null;
        }
    }

    public class Map2DataRowConfig : MapConfigBase<Map2DataRowConfig>
    {
        public override IMappingOperation[] GetMappingOperations(Type from, Type to)
        {
            var objectMembers = ReflectionUtils.GetPublicFieldsAndProperties(to);
            return base.FilterOperations(
                from,
                to,
                objectMembers.Select(
                    m => (IMappingOperation)new DestWriteOperation
                    {
                         Destination = new MemberDescriptor(m),
                         Getter = (ValueGetter<object>)((obj, value) =>
                        {
                            Func<object, object> converter = StaticConvertersManager.DefaultInstance.GetStaticConverterFunc(typeof(object), ReflectionUtils.GetMemberType(m));
                            var dr= obj as DataRow;
                            return ValueToWrite<object>.ReturnValue(converter(dr[m.Name]));

                        })
                    }
                )
            ).ToArray();
        }
    }
}


