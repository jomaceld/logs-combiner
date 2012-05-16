using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    class main
    {
        public static bool verbose = false;
        public static bool debug = false;
        public static SortedDictionary<string,  Dictionary<string,string>> dataDictionary;
        public static List<string> variablesNames;
        
        static void Main(string[] args)
        {
            dataDictionary = new SortedDictionary<string,  Dictionary<string,string>>();
            variablesNames = new List<string>();
            List<string> fixedOutput = null ;

            for (int i = 0; i < args.Length; i++) // Loop through arguments array
            {
                string argument = args[i];
                if (args[i] == "-v")
                {
                    verbose = true;
                }else if (args[i] == "-f")
                { 
                    i++;
                    string format = args[i];
                    i++;
                    Console.WriteLine(args[i]);
                    string path = args[i].Substring(0,args[i].LastIndexOf('\\'));
                    string[] files = args[i].Substring(args[i].LastIndexOf('\\')+1).Split(';');

                    foreach (string file in files)
                    {
                        Console.WriteLine(file);
                        readFile(path +"\\"+ file,format, true);
                    }
                    // Usage e.g.: -f "%ut;%vn;%v;%vn;%v;%vn;%v" "C\:dir\subdir\filename1;filename2;filename3"
                }
            }

            
            //readFile("C:\\Users\\LAX\\Desktop\\Nueva carpeta\\8-02-2012\\Clio 2012 02 07\\ACC_07_02_2012_08h_01mn_18sec.csv",
              //       "%ut;%vn;%v;%vn;%v;%vn;%v",true);

           // readFile("C:\\Users\\LAX\\Desktop\\Nueva carpeta\\8-02-2012\\Clio 2012 02 07\\can-MID_2012-02-07_07-43-30.log",
             //        "%ut;%i;%v",true);

            variablesNames.Sort();

            if (fixedOutput != null)
                writeResults("C:\\Users\\LAX\\Desktop\\out.log",fixedOutput);
            else
                writeResults("C:\\Users\\LAX\\Desktop\\out.log", variablesNames);

            if (verbose)
            {
                Console.WriteLine("\n\nReading finished, press ENTER to close...");
                // Wait before close
                Console.ReadLine(); 
            }
        }

        public static void writeResults(string fileName,List<string> output)
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter(fileName);

            // Header
            file.Write("Time");
            foreach (string aux in variablesNames)
                file.Write(";"+aux);

            foreach (KeyValuePair<string, Dictionary<string,string>> kvp in dataDictionary)
            {
                file.Write("\n"+kvp.Key);
                foreach (string aux in output)
                {
                    if(kvp.Value.Keys.Contains(aux))
                    {
                        file.Write(";"+kvp.Value[aux]);
                    }else
                        file.Write("; ");
                }
            }

            file.Close();
        }

        /// <summary>
        /// Read the file specified by the parameter fileName.
        /// </summary>
        /// <param name="fileName">Path to the file.</param>
        /// <param name="format">
        /// Format used to extract the data from each line of the file.
        /// ----------------------------
        /// Sintax (each type of value must be separated by ';'):
        /// 
        /// %ut --> Linux timeStamp
        /// %i --> ID
        /// %v --> Value
        /// %vn -- Value Name
        /// 
        /// e.g. "%t;%i;%v;%v;%vn;%vn" --> Time,ID,Value1,Value2,ValueName1,ValueName2
        /// </param>
        /// /// <param name="excludeFirstLine">Boolean. True --> exclude first line from the analysis.</param>
        public static void readFile(string fileName,string format, bool excludeFirstLine)
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
                if (excludeFirstLine && counter > 0)
                {
                    if (line.Length != 0)
                    {
                        string[] splittedLine = line.Split(';');

                        if (splittedFormat.Length != splittedLine.Length)
                        {
                            Console.WriteLine("Number of line's parameters doesn't correspond with format: " + line);

                        }
                        else
                        {
                            List<string> values = new List<string>();
                            List<string> valueNames = new List<string>();
                            string id = "";
                            string timestamp = "";

                            for (int i = 0; i < splittedLine.Length; i++)
                            {
                                string type = splittedFormat[i];
                                if (type == "%v")
                                {
                                    values.Add(splittedLine[i]);

                                }
                                else if (type == "%i")
                                {
                                    id = splittedLine[i];
                                }
                                else if (type == "%ut")
                                {
                                    timestamp = splittedLine[i];
                                }
                                else if (type == "%vn")
                                {
                                    valueNames.Add(splittedLine[i]);
                                    if (!variablesNames.Contains(splittedLine[i]))
                                    {
                                        variablesNames.Add(splittedLine[i]);
                                    }
                                }
                            }

                            if(valueNames.Count == 0)
                                valueNames.Add(id);

                            if (!variablesNames.Contains(id) && id.Length > 0)
                                variablesNames.Add(id);
 
                            if (verbose)
                            {
                                Console.Write("line: " + counter);
                                Console.Write("\tid: " + id);
                                Console.WriteLine("\tTimestamp: " + timestamp);
                                int auxCont = 0;
                                foreach (string auxvalue in values)
                                {
                                    Console.Write(auxCont + ": ");
                                    if (auxCont < valueNames.Count && valueNames[auxCont] != null)
                                    {
                                        Console.Write(valueNames[auxCont] + " --> ");
                                    }
                                    Console.Write(auxvalue + ", ");
                                    auxCont++;
                                }
                                Console.WriteLine("\n-------------------------");
                            }

                            if (!dataDictionary.Keys.Contains(timestamp))
                            {
                                dataDictionary[timestamp] = new Dictionary<string, string>();
                            }
                            
                            for (int e = 0; e < values.Count; e++)
                            {
                                if (valueNames.Count >= e)
                                {
                                    if (!dataDictionary[timestamp].Keys.Contains(valueNames[e]))
                                    dataDictionary[timestamp].Add(valueNames[e], values[e].Replace('.',','));
                                    
                                }
                                else if (!dataDictionary[timestamp].Keys.Contains(valueNames[0]))
                                    dataDictionary[timestamp].Add(valueNames[0], values[e].Replace('.', ','));
                            }
                     
                        }
                        counter++;
                    }
                }
                else
                    counter++;
            }
            file.Close(); 
        }
    }
}
