using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Recursivefilesearch
{
    class Program
    {
        //list your excluded dirs
        private static List<string> _excludedDirectories = new List<string>() { "Windows", "AppData", "$WINDOWS.~BT", "MSOCache", "ProgramData", "Config.Msi", "$Recycle.Bin", "Recovery", "System Volume Information", "Documents and Settings", "Perflogs" };

        static System.IO.StreamWriter filex = new System.IO.StreamWriter("test.txt", true);

        //method to check
        static bool isExcluded(List<string> exludedDirList, string target)
        {
            return exludedDirList.Any(d => new DirectoryInfo(target).Name.Equals(d));
        }

        static System.Collections.Specialized.StringCollection log = new System.Collections.Specialized.StringCollection();
        

        static void Main(string[] args)
        {
            // Specify the starting folder on the command line, or in  
            // Visual Studio in the Project > Properties > Debug pane.

            /*TraverseTree(args[0]);

            Console.WriteLine("Press any key");
            Console.ReadKey();*/

            DriveInfo di = new System.IO.DriveInfo("C:\\");

            // Here we skip the drive if it is not ready to be read. This 
            // is not necessarily the appropriate action in all scenarios. 
            if (di.IsReady)
            {
                DirectoryInfo rootDir = di.RootDirectory;
                WalkDirectoryTree(rootDir);
            }
            else
            {
                Console.WriteLine("The drive {0} could not be read", di.Name);
            }
            
            // Write out all the files that could not be processed.
            Console.WriteLine("Files with restricted access:");
            foreach (string s in log)
            {
                Console.WriteLine(s);
            }
            // Keep the console window open in debug mode.
            Console.WriteLine("Press any key");
            Console.ReadKey();
        }
        
        static void WalkDirectoryTree(System.IO.DirectoryInfo root)
        {
            FileInfo[] files = null;
            DirectoryInfo[] subDirs = null;
            
            // First, process all the folders directly under this folder 
            try
            {
                subDirs = root.GetDirectories();
            }
            // This is thrown if even one of the folders requires permissions greater than the application provides. 
            catch (UnauthorizedAccessException e)
            {
                log.Add(e.Message);
            }
            catch(Exception e)
            {
                log.Add(e.Message);
            }

            // Second, process all the files directly under this folder 
            try
            {
                files = root.GetFiles("*.*");
            }
            // This is thrown if even one of the files requires permissions greater 
            // than the application provides. 
            catch (UnauthorizedAccessException e)
            {
                log.Add(e.Message);
            }
            catch (System.IO.DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                log.Add(e.Message);
            }
            
            if (subDirs != null)
            {
                var filteredDirs = Directory.GetDirectories(root.Name).Where(d => !isExcluded(_excludedDirectories, d));
                foreach (string subds in filteredDirs)
                {
                    Console.WriteLine(subds);
                    filex.WriteLine(subds);
                }
            }

            if (files != null)
            {
                foreach (System.IO.FileInfo fi in files)
                {
                    // In this example, we only access the existing FileInfo object. If we 
                    // want to open, delete or modify the file, then 
                    // a try-catch block is required here to handle the case 
                    // where the file has been deleted since the call to TraverseTree().
                    Console.WriteLine(fi.FullName);
                    filex.WriteLine(fi.FullName);
                }

                // Now find all the subdirectories under this directory.
                
                

                /*foreach (System.IO.DirectoryInfo dirInfo in subDirs)
                {
                    // Resursive call for each subdirectory.
                    WalkDirectoryTree(dirInfo);
                }*/
            }
        }

        
    }
}
