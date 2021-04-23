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
            string fileurl = "DataFile.txt";
            string[] str_lines = File.ReadAllLines(fileurl);
            int block_count = 0;
            for (int i = 0; i < str_lines.Length; i++)
                if (str_lines[i].Contains("899~"))
                    block_count++;

            string[, ,] data = new string[block_count, 900, 100];            
            ////// initialize array//////
            for (int i = 0; i < block_count; i++)
                for (int j = 0; j < 900; j++)
                    for (int k = 0; k < 100; k++)
                        data[i,j,k] = "";

            /////////////store data into array//////////
            int block_id = 0;
            for (int k = 0; k < str_lines.Length; k++)
            {
                if (str_lines[k].Contains('~'))
                {
                    str_lines[k] = str_lines[k].Substring(0, str_lines[k].Length - 1);
                    string[] subdata = str_lines[k].Split('~');
                    int record_id = int.Parse(subdata[0]);

                    if (record_id != 899)
                    {
                        data[block_id,record_id, 0] = subdata[0];
                        
                        for (int j = 1; j < subdata.Length; j++)
                        {
                            int number = int.Parse(subdata[j].Substring(0, 2));
                            string value = subdata[j].Substring(2);
                            double num_val;                                
                            if (double.TryParse(value, out num_val)) // positive number
                            {
                                if (value.Length == 8)// date format
                                {
                                    string date = value;
                                    date = date.Substring(0, 2) + "/" + date.Substring(2, 2) + "/" + date.Substring(4);
                                    if (record_id < 100 || record_id > 199)
                                        data[block_id, record_id, number] += date + "; ";
                                    else
                                        data[block_id, record_id, number] = date ;
                                }
                                else
                                {
                                    if (num_val/100 <1000)
                                    {
                                        if (record_id < 100 || record_id > 199)
                                            data[block_id, record_id, number] += string.Format("{0:0.00}", num_val / 100) + " ; ";
                                        else
                                            data[block_id, record_id, number] = string.Format("{0:0.00}", num_val / 100);
                                    }    
                                    else
                                    {
                                        if (record_id < 100 || record_id > 199)
                                            data[block_id, record_id, number] += string.Format("{0:0,000.00}", num_val / 100) + " ; ";
                                        else
                                            data[block_id, record_id, number] = string.Format("{0:0,000.00}", num_val / 100);
                                    }                                            
                                }                                    
                            }
                            else if(double.TryParse(value.Substring(0, value.Length-1), out num_val) && value[value.Length-1] == '-') //negative number
                            {
                                num_val = (-1) * num_val;
                                if (num_val / 100 < 1000)
                                {
                                    if (record_id < 100 || record_id > 199)
                                        data[block_id, record_id, number] += string.Format("{0:0.00}", num_val / 100) + " ; ";
                                    else
                                        data[block_id, record_id, number] = string.Format("{0:0.00}", num_val / 100) ;
                                }                                        
                                else
                                {
                                    if (record_id < 100 || record_id > 199)
                                        data[block_id, record_id, number] += string.Format("{0:0,000.00}", num_val / 100) + " ; ";
                                    else
                                        data[block_id, record_id, number] = string.Format("{0:0,000.00}", num_val / 100) ;
                                }   
                            }
                            else
                            {
                                if (record_id < 100 || record_id > 199)
                                    data[block_id, record_id, number] += subdata[j].Substring(2) + " ; ";
                                else
                                    data[block_id, record_id, number] = subdata[j].Substring(2);
                            } 
                        } 
                    }
                    else
                        block_id++;
                }
            }

            /////////////print data//////////
            for (int i= 0; i< block_count; i++)
            {
                Console.WriteLine("-----------------Block"+ (i+1).ToString()+"-------------------------");         
                for (int j = 100; j < 899; j++)
                {
                    if (data[i, j, 0] != "")
                    {
                        string temp = "";
                        for (int k = 1; k < 100; k++)
                            if (data[i,j,k] != "")
                            {
                                temp = "Block:" + (i + 1).ToString() + " Record :" + data[i, j, 0] + "  Field:" + string.Format("{0:00}", k) + " val: " + data[i, j, k];
                                Console.WriteLine(temp);
                            }
                        Console.WriteLine("\n");
                    }  
                }
                string tmp = "Block:" + (i+1).ToString() + " Record :899" + "  the end of it again";
                Console.WriteLine(tmp + '\n');
            }              
            Console.ReadLine();    
        }
    }
}
