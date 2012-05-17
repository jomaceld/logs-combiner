logs-combiner
=============

Combine two or more logs files in to a resulting log file that will be ordered by the column marked as timestamp.

Usage: logscombiner-cshap.exe -f "format" "files" -f ...

-f:

	Indicates that there is a new group of files using the format expressed. 

format:

	String that indicates the format used in the log files. The fields should be separated with the ';' character.
  	possible options are:
  
	- %ut --> Unix Timestamp
    - %v  --> Value
    - %vn --> Value Name
    
	e.g. the string "%t;%v;%v;%vn;%vn" will be evaluated as Time,Value1,Value2,ValueName1,ValueName2

files:
	
	Indicates the absolute or relative path to the files that will be evaluated with the format expressed before, 
	followed by the files in that path. To indicate more than one file, they should be separated with the character ';'.
	
	e.g The string "C:\dir\subdir\filename1;filename2" will analyze the files filename1 and filename2 in the folder 
	C:\dir\subdir\
	
-----------------------
Last revision: 17 May 2012

Jose A Maestre Celdran