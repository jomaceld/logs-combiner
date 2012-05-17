/**************************
 *     Logs Combiner      *
 *-------------------------
 * Jose A Maestre Celdran *
 *************************/
/* 
 * Usage e.g.: logsextractor-csharp.exe -f "%ut;%vn;%v;%vn;%v;%vn;%v" "C\:dir\subdir\filename1;filename2;filename3"
 *
 */

 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace logsextractor
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
                   
                    string path = args[i].Substring(0,args[i].LastIndexOf('\\'));
                    string[] files = args[i].Substring(args[i].LastIndexOf('\\')+1).Split(';');

                    foreach (string file in files)
                    {
                        Console.WriteLine(file);
                        readFile(path +"\\"+ file,format, true);
                    }
                    // Usage e.g.: -f "%ut;%vn;%v;%vn;%v;%vn;%v" "C\:dir\subdir\filename1;filename2;filename3"
                } else if (args[i] == "-nmea")
                {
                    i++;
                    
                    string path = args[i].Substring(0,args[i].LastIndexOf('\\'));
                    string[] files = args[i].Substring(args[i].LastIndexOf('\\')+1).Split(';');

                    foreach (string file in files)
                    {
                        Console.WriteLine(file);
                        readNmeaGPRMCFile(path + "\\" + file);
                    }
                    
                }
            }


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
        /// %v --> Value
        /// %vn -- Value Name
        /// 
        /// e.g. "%t;%v;%v;%vn;%vn" --> Time,ID,Value1,Value2,ValueName1,ValueName2
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
                            string timestamp = "";

                            for (int i = 0; i < splittedLine.Length; i++)
                            {
                                string type = splittedFormat[i];
                                if (type == "%v")
                                {
                                    values.Add(splittedLine[i]);

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
 
                            if (verbose)
                            {
                                Console.Write("line: " + counter);
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

        public static double convertToUnixTimestamp(DateTime date)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0); //epoch
            TimeSpan diff = date - dtDateTime;
            return Math.Floor(diff.TotalSeconds);
        }

        public static void readNmeaGPRMCFile(string fileName)
        {
            variablesNames.Add("GPS_Status");
            variablesNames.Add("GPS_Latitude");
            variablesNames.Add("GPS_Longitude");
            variablesNames.Add("GPS_Speed_Knots");

            if (verbose){
                Console.WriteLine("Reading .Nmea GPS file: " + fileName);
            }
            
            int counter = 0;
            string line;

            System.IO.StreamReader file = new System.IO.StreamReader(fileName);

            while ((line = file.ReadLine()) != null)
            {
                string[] parts = line.Split(',');

                Console.WriteLine(line);

                if (parts.Length == 13 && parts[0] == "$GPRMC")
                {
                    string GPS_Status = "valid";
                    if(parts[2] == "V")
                        GPS_Status = "warning";

                    string GPS_Latitude= parts[3]+","+parts[4];
                    string GPS_Longitude = parts[5]+","+parts[6];
                    string GPS_Speed = parts[7];

                    int day = int.Parse(parts[9].Substring(0,2));
                    int month = int.Parse(parts[9].Substring(2,2));
                    int year = int.Parse("20"+parts[9].Substring(4,2));

                    int hh = int.Parse(parts[1].Substring(0, 2));
                    int mm = int.Parse(parts[1].Substring(2, 2));
                    int ss = int.Parse(parts[1].Substring(4, 2));
                    int fss = 0;
                    if(parts[1].Length == 8)
                        fss = int.Parse(parts[1].Substring(6, 2));

                    
                    DateTime time = new DateTime(year,month,day, hh,mm,ss,fss);
                    string timestamp = convertToUnixTimestamp(time).ToString();
                    

                    if (!dataDictionary.Keys.Contains(timestamp))
                    {
                        Dictionary<string, string> d = new Dictionary<string, string>();
                        d.Add("GPS_Status", GPS_Status);
                        d.Add("GPS_Latitude", GPS_Latitude);
                        d.Add("GPS_Longitude", GPS_Longitude);
                        d.Add("GPS_Speed_Knots", GPS_Speed);

                        dataDictionary.Add(timestamp, d);
                    }
                    else
                    {
                        Dictionary<string, string> d = dataDictionary[timestamp];
                        if(!d.Keys.Contains("GPS_Status"))
                            d.Add("GPS_Status", GPS_Status);
                        if (!d.Keys.Contains("GPS_Latitude"))
                            d.Add("GPS_Latitude", GPS_Latitude);
                        if (!d.Keys.Contains("GPS_Longitude"))
                            d.Add("GPS_Longitude", GPS_Longitude);
                        if (!d.Keys.Contains("GPS_Speed_Knots"))
                            d.Add("GPS_Speed_Knots", GPS_Speed);
                    }
                    
               }

                counter++;
            }
            file.Close();
        }          
    }
}
