using System;
using System.Data;
using System.IO;
using System.Text;
using UnityEngine;

public class CSV
{
    public static DataTable ToDataTable(TextAsset ta)
    {
        DataTable dt = new DataTable(ta.name.ToString());
        string[] array;
        string[] tableHead;
        int column = 0;
        bool isFirst = true;
        foreach (string s in ta.text.Split("\n"))
        {
            //创建表头
            if (isFirst)
            {
                //表头
                tableHead = s.Split(',');
                isFirst = false;
                column = tableHead.Length;
                //创建列
                foreach (string head in tableHead)
                {
                    DataColumn dc = new DataColumn(head);
                    dt.Columns.Add(dc);
                }
                //设置主键
                dt.PrimaryKey = new DataColumn[] { dt.Columns[0] };
            }
            //创建行数据
            else
            {
                array = s.Split(',');
                DataRow dr = dt.NewRow();
                if (array.Length == column)
                {
                    for (int i = 0; i < column; i++)
                    {
                        dr[i] = array[i];
                    }
                    dt.Rows.Add(dr);
                }

            }
        }
        return dt;
    }
    public static DataTable ToDataTable(string path)
    {
        DataTable dt = new DataTable();
        using(FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
        {
            using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
            {
                //每行记录
                string line = "";
                string[] array = null;
                string[] tableHead = null;
                int column = 0;
                bool isFirst = true;
                while((line = sr.ReadLine()) != null)
                {
                    //创建表头
                    if (isFirst)
                    {
                        //表头
                        tableHead = line.Split(',');
                        isFirst = false;
                        column = tableHead.Length;
                        //创建列
                        foreach(string head in tableHead)
                        {
                            var col = head.Split('#');
                            DataColumn dc = new DataColumn(col[0], Type.GetType(col[1]));
                            dt.Columns.Add(dc);
                        }
                        //设置主键
                        dt.PrimaryKey = new DataColumn[] { dt.Columns[0] };
                    }
                    //创建行数据
                    else
                    {
                        
                        array = line.Split(',');
                        DataRow dr = dt.NewRow();
                        for(int i =0; i< column; i++)
                        {
                            switch (dt.Columns[i].DataType.Name)
                            {
                                case "string":
                                    dr[i] = array[i];
                                    break;
                                case "int":
                                    dr[i] = int.Parse(array[i]);
                                    break;
                                case "Color":
                                    if (ColorUtility.TryParseHtmlString(array[i], out Color c))
                                    {
                                        dr[i] = c;
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
                sr.Close();
            }
            fs.Close();
            return dt;
        }
    }
    public static void Log(DataTable dt)
    {
        string list = "";
        foreach(DataRow row in dt.Rows)
        {
            foreach(DataColumn col in dt.Columns)
            {
                list += row[col].ToString() + "\t";
            }
            list += "\n";
        }
        Debug.Log(list);
    }
}
