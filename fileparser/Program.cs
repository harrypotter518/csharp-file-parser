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
            int block_count = 0;
            int block_id = 0;
            bool first_flag = true;
            string[,] data = new string[900, 100];
            int[] repeat_cnt = new int[900];
            ////// initialize array//////
     
            for (int j = 0; j < 900; j++)
            {
                repeat_cnt[j] = 0;
                for (int k = 0; k < 100; k++)
                    data[j, k] = "";
            }

            StreamReader sr = File.OpenText(fileurl);
            string str = "";
            string line;
           
            while ((line = sr.ReadLine()) != null)
            {
              
                if (line.Contains("200~"))
                {
                    block_count++;
                    string[] str_lines = str.Split('`');
                    str = "";
                    /////////////store data into array//////////
                   
                    for (int k = 0; k < str_lines.Length; k++)
                    {
                        if (str_lines[k].Contains('~'))
                        {
                            str_lines[k] = str_lines[k].Replace("\r\n", "");
                            string[] subdata = str_lines[k].Split('~');
                            int record_id = int.Parse(subdata[0]);

                            if (record_id == 200)
                            {
                                if (first_flag == false)
                                    block_id++;
                            }
                            if (record_id != 899)
                            {
                                data[ record_id, 0] = subdata[0];
                                repeat_cnt[record_id]++;

                                for (int j = 1; j < subdata.Length; j++)
                                {
                                    int field_id = int.Parse(subdata[j].Substring(0, 2));
                                    string value = subdata[j].Substring(2);

                                    if (record_id >= 100 && record_id < 200)
                                        data[record_id, field_id] = value;
                                    else
                                    {
                                        check_flag = false;
                                        for (int t = 0; t < date_set_id.Length / 2; t++)
                                        {
                                            if (record_id == date_set_id[t * 2] && field_id == date_set_id[t * 2 + 1])
                                            {
                                                string date = value;
                                                date = date.Substring(0, 2) + "/" + date.Substring(2, 2) + "/" + date.Substring(4);
                                                data[record_id, field_id] += repeat_cnt[record_id] + ";" + date + ";";
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
                                                        data[record_id, field_id] += repeat_cnt[record_id] + ";" + string.Format("{0:0.00}", num_val / 100) + ";";
                                                    else
                                                        data[record_id, field_id] += repeat_cnt[record_id] + ";" + string.Format("{0:0,000.00}", num_val / 100) + ";";
                                                }
                                                else if (double.TryParse(value.Substring(0, value.Length - 1), out num_val) && value[value.Length - 1] == '-') //negative number
                                                {
                                                    if (num_val / 100 < 1000)
                                                        data[record_id, field_id] += repeat_cnt[record_id] + ";" + string.Format("{0:0.00}", (-1) * num_val / 100) + ";";
                                                    else
                                                        data[record_id, field_id] += repeat_cnt[record_id] + ";" + string.Format("{0:0,000.00}", (-1) * num_val / 100) + ";";

                                                }
                                                check_flag = true; break;
                                            }
                                        }
                                        if (check_flag == false)
                                            data[record_id, field_id] += repeat_cnt[record_id] + ";" + value + ";";
                                    }
                                }
                            }
                        }
                    }

                    /////////////print data//////////   
          
                    if (first_flag == true)
                    {
                        first_flag = false;
                        for (int j = 100; j < 200; j++)
                        {
                            if (data[j, 0] != "")
                            {
                                string temp = "";
                                for (int k = 1; k < 100; k++)
                                    if (data[j, k] != "")
                                    {
                                        temp = "Block:" + (block_id).ToString() + " Record :" + data[j, 0] + "  Field:" + string.Format("{0:00}", k) + " val: " + data[j, k];
                                        Console.WriteLine(temp);
                                    }
                                Console.WriteLine("\n");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("-----------------Block" + (block_id).ToString() + "-------------------------");
                        for (int j = 200; j < 899; j++)
                        {
                            if (data[j, 0] != "")
                            {
                                for (int cnt = 1; cnt <= repeat_cnt[j]; cnt++)
                                {
                                    string temp = "";
                                    for (int k = 1; k < 100; k++)
                                    {
                                        if (data[j, k] != "")
                                        {
                                            string[] each_item = data[j, k].Split(';');


                                            for (int ii = 0; ii < (each_item.Length - 1) / 2; ii++)
                                            {
                                                if (int.Parse(each_item[2 * ii]) == cnt)
                                                {
                                                    temp = "Block:" + (block_id).ToString() + " Record :" + data[j, 0] + "  Field:" + string.Format("{0:00}", k) + " val: " + each_item[ii * 2 + 1];
                                                    Console.WriteLine(temp);
                                                }
                                            }
                                        }
                                    }

                                    Console.WriteLine("\n");
                                }

                            }
                        }
                        Console.WriteLine("Block:" + (block_id).ToString() + " Record :899 " + " the end of block" + "\n");
                    }
                       
                }

                str += line;

            }
                      
            sr.Close();          
            Console.ReadLine();    
        }
    }
}
