using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    class main
    {
        public static bool verbose = true;
        public static SortedDictionary<string, List<Data>> dataDictionary;

        static void Main(string[] args)
        {
            dataDictionary = new SortedDictionary<string, List<Data>>();
            readFile("C:\\Users\\LAX\\Desktop\\Nueva carpeta\\8-02-2012\\Clio 2012 02 07\\ACC_07_02_2012_07h_43mn_35sec.csv",
                     "%ut;%vn;%v;%vn;%v;%vn;%v");

            readFile("C:\\Users\\LAX\\Desktop\\Nueva carpeta\\8-02-2012\\Clio 2012 02 07\\can-MID_2012-02-07_00-23-45.log",
                     "%ut;%i;%v");
            
            
            if (verbose)
            {
                Console.WriteLine("\n\nReading finished, press ENTER to close...");
                // Wait before close
                Console.ReadLine();
                
            }
        }

        /// <summary>
        /// Read the file specified by the parameter fileName.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="format">
        /// %ut --> Linux timeStamp
        /// %i --> ID
        /// %v --> Value
        /// %vn -- Value Name
        /// e.g. "%t;%i;%v;%v" --> Time,ID,Value1,Value2
        /// </param>
        public static void readFile(string fileName,string format)
        {
            if (verbose){
                Console.WriteLine("Reading file: " + fileName);
                Console.WriteLine("Format: " + format+ "\n\n");
            }
            int counter = 0;
            string line;

            string[] splittedFormat =  format.Split(';');

            System.IO.StreamReader file = new System.IO.StreamReader(fileName);
            while ((line = file.ReadLine()) != null)
            {
                if (line.Length != 0)
                {
                    string[] splittedLine = line.Split(';');

                    if (splittedFormat.Length != splittedLine.Length) {
                        Console.WriteLine("Number of line's parameters doesn't correspond with format: " + line);

                    } else {

                        List<string> values = new List<string>();
                        List<string> valueNames = new List<string>();
                        string id = "";
                        string timestamp = "";

                        for (int i = 0; i < splittedLine.Length; i++) { 
                            string type = splittedFormat[i]; 
                            if(type == "%v"){
                                values.Add(splittedLine[i]);

                            } else if( type == "%i") {
                                id = splittedLine[i];
                            } else if(type == "%ut")  {
                                timestamp = splittedLine[i];
                            }
                            else if (type == "%vn")  {
                                valueNames.Add(splittedLine[i]);
                            }
                        }

                        if(verbose) {
                            Console.Write("line: " + counter);
                            Console.Write("\tid: "+id);
                            Console.WriteLine("\tTimestamp: "+timestamp);
                            int auxCont = 0;
                            foreach(string auxvalue in values) {
                                Console.Write(auxCont+": ");
                                if (auxCont < valueNames.Count && valueNames[auxCont] != null) {
                                    Console.Write(valueNames[auxCont] + " --> ");
                                }
                                Console.Write(auxvalue+", ");
                                auxCont++;
                            }
                            Console.WriteLine("\n-------------------------");
                        }
                    }
                    counter++;
                }
            }
            file.Close(); 
        }
    }

    class Data
    {
        string id;
        string value;
    }
}
