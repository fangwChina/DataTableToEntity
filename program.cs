using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FQuery.Interface;
using FQuery.Utill;
using System.Data;
using System.Diagnostics;
using FQuery;


namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {

            //FQuery.Tools.FQuery query = new FQuery.Tools.FQuery();
            //IEnity entity = query.GetScheme("Clients");
            //entity.Field["name"] = "ss";
            //entity.Field["id"] = "1";
            //entity.Insert();
            //entity.Update();
            //entity.Delete();
            //List<IEnity> list = query.Form("select * from clients").ToList<IEnity>() ;
            //foreach (IEnity item in list)
            //{
            //    item.Field[""] = "";
            //}
            
            
            DataTable dt = new DataTable();
            dt.Columns.Add("ID");
            dt.Columns.Add("Name");
            dt.Columns.Add("a");
            dt.Columns.Add("a1");
            dt.Columns.Add("a2");
            dt.Columns.Add("a3");
            dt.Columns.Add("a4");
            dt.Columns.Add("a5");
            dt.Columns.Add("a6");
            dt.Columns.Add("a7");
            dt.Columns.Add("a8");
            dt.Columns.Add("a9");
            for(int i=0;i<1000000;i++)
            {
                DataRow dr =dt.NewRow();
                dr[0]=Guid.NewGuid();
                dr[1]="aasdddddddddddddddddddddddddddddddddddddddddaa";
                dr[2] = "aadddddddddddddddddddddddddddddddaaaa";
                dr[3] = "aaddddddddddddddddddddddddddaaaa";
                dr[4] = "aaaddddddddddddddddddddddddddddddddddddddddaaa";
                dr[5] = "aaadddddddddddddddddaaa";
                dr[6] = "aaaddddddddddddddaaa";
                dr[7] = "aaaddddddddddddddddddddddddddddddddddddddddddddddddddddaaa";
                dr[8] = "aaadddddddddddddddddddddddaaa";
                dr[9] = "aaadddddddddddddddddddaaa";
                dr[10] = "aadddddddddddddddddddddddddaaaa";
                dr[11] = "aaadddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddaaa";
                dt.Rows.Add(dr);
            }
            ReflectionTest test = new ReflectionTest();
            Stopwatch stop = new Stopwatch();
            stop.Start();
            List<TestEntiy> list = test.ToList<TestEntiy>(dt);
            stop.Stop();
            double time1 = stop.Elapsed.TotalSeconds;
            stop.Reset();
            Console.WriteLine("方法：ToList耗时-" + time1.ToString());
            stop.Start();
            List<TestEntiy> list2 = test.ToList_Direct(dt);
            stop.Stop();
            Console.WriteLine("方法：ToList_Direct耗时-" + stop.Elapsed.TotalSeconds.ToString());
            stop.Reset();
            stop.Start();
            List<TestEntiy> list1 = test.ToList_Extend<TestEntiy>(dt);
            stop.Stop();
            double time2 = stop.Elapsed.TotalSeconds;
            Console.WriteLine("方法：ToList_Extend耗时-" + time2.ToString());
            stop.Reset();
            stop.Start();
            List<TestEntiy> list3 = test.ToList_Emit<TestEntiy>(dt);
            stop.Stop();
            Console.WriteLine("方法：ToList_Emit耗时：" + stop.Elapsed.TotalSeconds.ToString());
            stop.Reset();
            stop.Start();
            List<TestEntiy> list4 = test.ToList_Emit<TestEntiy>(dt);
            stop.Stop();
            Console.WriteLine("方法：第二次调用ToList_Emit耗时：" + stop.Elapsed.TotalSeconds.ToString());
            //stop.Reset();
            //stop.Start();
            //List<TestEntiy> list5 = test.ToList_Emit2<TestEntiy>(dt);
            //stop.Stop();
            //Console.WriteLine("方法：ToList_Emit2耗时：" + stop.Elapsed.TotalSeconds.ToString());

            //stop.Reset();
            //stop.Start();
            //List<TestEntiy> list6 = test.ToList_Emit2<TestEntiy>(dt);
            //stop.Stop();
            //Console.WriteLine("方法：第二次调用ToList_Emit2耗时：" + stop.Elapsed.TotalSeconds.ToString());
            //stop.Reset();
            //stop.Start();
            //List<TestEntiy> list7 = test.ToList_Emit2<TestEntiy>(dt);
            //stop.Stop();
            //Console.WriteLine("方法：第三次调用ToList_Emit2耗时：" + stop.Elapsed.TotalSeconds.ToString());

            //stop.Reset();
            //stop.Start();
            //List<TestEntiy> list8 = test.ToList_Emit2<TestEntiy>(dt);
            //stop.Stop();
            //Console.WriteLine("方法：第四次调用ToList_Emit2耗时：" + stop.Elapsed.TotalSeconds.ToString());

            EntityExtend ee = new EntityExtend();
            stop.Reset();
            stop.Start();
            var a= ee.ToList<TestEntiy>(dt);
            stop.Stop();
            Console.WriteLine("方法：第一次调用ToList耗时：" + stop.Elapsed.TotalSeconds.ToString());
            stop.Reset();
            stop.Start();
             a = ee.ToList<TestEntiy>(dt);
            stop.Stop();
            Console.WriteLine("方法：第二次调用ToList耗时：" + stop.Elapsed.TotalSeconds.ToString());
            stop.Reset();
            stop.Start();
            a = ee.ToList<TestEntiy>(dt);
            stop.Stop();
            Console.WriteLine("方法：第三次调用ToList耗时：" + stop.Elapsed.TotalSeconds.ToString());
            stop.Reset();
            stop.Start();
             a = ee.ToList<TestEntiy>(dt);
            stop.Stop();
            Console.WriteLine("方法：第四次调用ToList耗时：" + stop.Elapsed.TotalSeconds.ToString());
            stop.Reset();
            stop.Start();
             a = ee.ToList<TestEntiy>(dt);
            stop.Stop();
            Console.WriteLine("方法：第五次调用ToList耗时：" + stop.Elapsed.TotalSeconds.ToString());

            Console.Read();
        }
    }


    public class InsVersion
    {
        public Guid ID { set; get; }

        public string Name { set; get; }

        public string DataSourceName { set; get; }

        public Guid CreateMan { set; get; }

        public DateTime CreateTime { set; get; }

        public Guid ClientID { set; get; }

        public bool Insert()
        {
            return true;
        }
        public int Update()
        {
            return 1;
        }
        public int Delete()
        {
            return 1;
        }

        public static List<InsVersion> ToList(System.Data.DataTable dt)
        {
            List<InsVersion> list = new List<InsVersion>();
            foreach (DataRow item in dt.Rows)
            {
                InsVersion version = new InsVersion();
                version.ID = new Guid(item["ID"].ToString());
                version.Name = item["name"].ToString();
                version.DataSourceName = item["DataSourceName"].ToString();
                //...
                list.Add(version);
            }
            return list;
        }
    }

    public class TestEntiy
    {
        public string ID { set; get; }

        public string Name { set; get; }

        public string a { set; get; }

        public string a1 { set; get; }
        public string a2 { set; get; }
        public string a3 { set; get; }
        public string a4 { set; get; }
        public string a5 { set; get; }
        public string a6 { set; get; }
        public string a7 { set; get; }
        public string a8 { set; get; }
        public string a9 { set; get; }
    }
   
}
