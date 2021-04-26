using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;

namespace fileparser
{
    class Program
    {

        static void Main(string[] args)
        { 
            string fileurl = "DataFile";

            int[] date_set_id = {400,1,499,1};// 400-record, 1-field
            int[] number_set_id = { 430,5, 499,3}; // 430-record, 5-filed 
            bool check_flag = false;

            StreamReader sr = File.OpenText(fileurl);
            string str = sr.ReadToEnd();
            sr.Close();
            //string str = File.ReadAllText(fileurl);
            string[] str_lines = str.Split('`');

            int block_count = 0;
            for (int i = 0; i < str_lines.Length; i++)
                if (str_lines[i].Contains("200~"))
                    block_count++;

            string[, ,] data = new string[block_count, 900, 100];
            int[,] repeat_cnt = new int[block_count,900];       
            ////// initialize array//////
            for (int i = 0; i < block_count; i++)
                for (int j = 0; j < 900; j++)
                {
                    repeat_cnt[i, j] = 0;
                    for (int k = 0; k < 100; k++)
                        data[i, j, k] = "";
                }
                    

            /////////////store data into array//////////
            int block_id = 0; bool first_flag = true;
            for (int k = 0; k < str_lines.Length; k++)
            {
                if (str_lines[k].Contains('~'))
                {
                    str_lines[k] = str_lines[k].Replace("\r\n","");
                    string[] subdata = str_lines[k].Split('~');
                    int record_id = int.Parse(subdata[0]);

                    if (record_id == 200)
                    {
                        if (first_flag == true)
                            first_flag = false;
                        else
                            block_id++;                        
                    }
                    if (record_id != 899)
                    {                       
                        data[block_id, record_id, 0] = subdata[0];
                        repeat_cnt[block_id, record_id] ++;                      

                        for (int j = 1; j < subdata.Length; j++)
                        {
                            int field_id = int.Parse(subdata[j].Substring(0, 2));
                            string value = subdata[j].Substring(2);

                            if (record_id >= 100 && record_id < 200)
                                data[0, record_id, field_id] = value;
                            else
                            {
                                check_flag = false;
                                for (int t = 0; t < date_set_id.Length / 2; t++)
                                {
                                    if (record_id == date_set_id[t * 2] && field_id == date_set_id[t * 2 + 1])
                                    {
                                        string date = value;
                                        date = date.Substring(0, 2) + "/" + date.Substring(2, 2) + "/" + date.Substring(4);
                                        data[block_id, record_id, field_id] += repeat_cnt[block_id, record_id] + ";" + date + ";";
                                        check_flag = true; break;
                                    }
                                }
                                for (int t = 0; t < number_set_id.Length / 2; t++)
                                {
                                    if (record_id == number_set_id[t * 2] && field_id == number_set_id[t * 2 + 1])
                                    {
                                        double num_val;
                                        if (double.TryParse(value, out num_val)) // positive number
                                        {
                                            if (num_val / 100 < 1000)
                                                data[block_id, record_id, field_id] += repeat_cnt[block_id, record_id] + ";" + string.Format("{0:0.00}", num_val / 100) + ";";
                                            else
                                                data[block_id, record_id, field_id] += repeat_cnt[block_id, record_id] + ";" + string.Format("{0:0,000.00}", num_val / 100) + ";";
                                        }
                                        else if (double.TryParse(value.Substring(0, value.Length - 1), out num_val) && value[value.Length - 1] == '-') //negative number
                                        {
                                            if (num_val / 100 < 1000)
                                                data[block_id, record_id, field_id] += repeat_cnt[block_id, record_id] + ";" + string.Format("{0:0.00}", (-1) * num_val / 100) + ";";
                                            else
                                                data[block_id, record_id, field_id] += repeat_cnt[block_id, record_id] + ";" + string.Format("{0:0,000.00}", (-1) * num_val / 100) + ";";
                                          
                                        }
                                        check_flag = true; break;
                                    }
                                }
                                if (check_flag == false)
                                    data[block_id, record_id, field_id] += repeat_cnt[block_id, record_id] + ";" + value + ";";
                            }
                        }
                    }
                }
            }

            /////////////print data//////////   
            for (int i= 0; i< block_count; i++)
            {
                Console.WriteLine("-----------------Block"+ (i+1).ToString()+"-------------------------");   
                if (i == 0)
                {
                    for (int j = 100; j < 200; j++)
                    {
                        if (data[i, j, 0] != "")
                        {
                            string temp = "";
                            for (int k = 1; k < 100; k++)
                                if (data[i, j, k] != "")
                                {
                                    temp = "Block:" + (i + 1).ToString() + " Record :" + data[i, j, 0] + "  Field:" + string.Format("{0:00}", k) + " val: " + data[i, j, k];
                                    Console.WriteLine(temp);
                                }
                            Console.WriteLine("\n");
                        }
                    }
                }      
                for (int j = 200; j < 899; j++)
                {
                    if (data[i, j, 0] != "")
                    {
                        for (int cnt =1;cnt<= repeat_cnt[i, j]; cnt++)
                        {  
                            string temp = "";
                            for (int k = 1; k < 100; k++)
                            {
                                if (data[i, j, k] != "")
                                {
                                    string[] each_item = data[i, j, k].Split(';');

                                   
                                        for (int ii = 0; ii < (each_item.Length-1)/2; ii++)
                                        {
                                            if (int.Parse(each_item[2 * ii]) == cnt)
                                            {
                                                temp = "Block:" + (i + 1).ToString() + " Record :" + data[i, j, 0] + "  Field:" + string.Format("{0:00}", k) + " val: " + each_item[ii*2 +1];
                                                Console.WriteLine(temp);
                                            }
                                        }
                                }
                            }

                            Console.WriteLine("\n");
                        }
                    
                    }  
                }
                Console.WriteLine("Block:" + (i + 1).ToString()  +" Record :899 " + " the end of block" + "\n"); 
            }              
            Console.ReadLine();    
        }
    }
}
