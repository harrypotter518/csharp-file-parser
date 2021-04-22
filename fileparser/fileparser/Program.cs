using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace fileparser
{
    class Program
    {

        static void Main(string[] args)
        {
            string line;
            string fileurl = "data.txt";


            //    StreamReader sr = new StreamReader(fileurl);
            //     line = sr.ReadLine();
            //Continue to read until you reach end of file
            //       while (line != null)
            //        {
            //write the lie to console window
            //        Console.WriteLine(line);
            //Read the next line
            //          line = sr.ReadLine();
            //      }
            //close the file
            //      sr.Close();

            string[] str_lines = File.ReadAllLines(fileurl);
            string[,] data = new string[900, 100];

            for (int i = 0; i < 900; i++)
            {
                for (int j = 0; j < 100; j++)
                    data[i, j] = "";
            }

            for (int k = 0; k < str_lines.Length; k++)
            {

                
                if (str_lines[k].Contains('~') )
                {
                    str_lines[k] = str_lines[k].Substring(0, str_lines[k].Length - 1);
                    string[] subdata = str_lines[k].Split('~');
                    int id = int.Parse(subdata[0]);

                    if (id !=899 && (id < 100 || id > 199) )
                    {
                        data[id, 0] = subdata[0];
                        for (int j = 1; j < subdata.Length; j++)
                        {
                            int number = int.Parse(subdata[j].Substring(0, 2));
                            if (number ==6 )
                            {
                                string date = subdata[j].Substring(2);
                                date = date.Substring(0, 2) + "/" + date.Substring(2, 2) + "/" + date.Substring(4);
                                data[id, number] = date;
                            }
                            else
                                data[id, number] = subdata[j].Substring(2);

                        }

                    }
                   
                }
                //int id = int.Parse(str_lines[k].Split('~')[0]);
               
            }


            for (int i = 0; i < 900; i++)
            {
                if (data[i,0] !="" )
                {
                    Console.WriteLine( "Record " + data[i,0] + " with field" );
                    string temp = "";
                    for (int j = 1; j < 100; j++)
                        if (data[i, j] != "")
                        {
                            Console.WriteLine(string.Format("{0:00}", j) +" - "+ data[i,j]);
                        }
                    Console.WriteLine("\n");

                }
              
            }
            Console.WriteLine("Record 899 - the end of it again");















            Console.ReadLine();

           


        }
    }
}
