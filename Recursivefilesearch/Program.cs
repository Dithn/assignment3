using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Recursivefilesearch
{
    class Program
    {
        static System.Collections.Specialized.StringCollection log = new System.Collections.Specialized.StringCollection();
        private static List<string> _excludedDirectories = new List<string>() { "Windows", "AppData", "$WINDOWS.~BT", "MSOCache", "ProgramData", "Config.Msi", "$Recycle.Bin", "Recovery", "System Volume Information", "Documents and Settings", "Perflogs" };

        //method to check
        static bool isExcluded(List<string> exludedDirList, string target)
        {
            return exludedDirList.Any(d => new DirectoryInfo(target).Name.Equals(d));
        }

        static void Main()
        {
            
            string[] drives = {"C:\\"};

            foreach (string dr in drives)
            {
                DriveInfo di = new System.IO.DriveInfo(dr);

                // Here we skip the drive if it is not ready to be read. 
                if (di.IsReady)
                {
                    DirectoryInfo rootDir = di.RootDirectory;
                    WalkDirectoryTree(rootDir);
                }
                else
                {
                    Console.WriteLine("The drive {0} could not be read", di.Name);
                    continue;
                }
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
            StreamWriter filex = new System.IO.StreamWriter("test.txt", true);

            if (filex != null)
            {
                filex.Close();
            }

            // Process all the folders directly under the root 
            try
            {
                subDirs = root.GetDirectories();
            }// This is thrown if even one of the folders requires permissions greater than the application provides. 
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
                Console.WriteLine(e.Message);
            }

            // Process all the files directly under the root 
            try
            {
                files = root.GetFiles("*.*");
            }// This is thrown if even one of the files requires permissions greater than the application provides. 
            catch (UnauthorizedAccessException e)
            {
                log.Add(e.Message);
            }
            catch (System.IO.DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

            if (files != null)
            {
                filex = new StreamWriter("test.txt", true);
                foreach (FileInfo fi in files)
                {
                    // In this example, we only access the existing FileInfo object. If we 
                    // want to open, delete or modify the file, then 
                    // a try-catch block is required here to handle the case 
                    // where the file has been deleted since the call to TraverseTree().
                    Console.WriteLine(fi.FullName);
                    filex.WriteLine(fi.FullName);
                }
                filex.Close();
            }

            if (subDirs != null)
            {
                //var filteredDirs = Directory.GetDirectories(root.Name).Where(d => !isExcluded(_excludedDirectories, d));
                foreach (DirectoryInfo subds in subDirs.Where(d => !isExcluded(_excludedDirectories, d.Name)))
                {
                    filex = new StreamWriter("test.txt", true);
                    Console.WriteLine(subds.FullName);
                    filex.WriteLine(subds.FullName);
                    filex.Close();

                    foreach (DirectoryInfo dirInfo in subDirs.Where(d => !isExcluded(_excludedDirectories, d.Name)))
                    {
                        // Resursive call for each subdirectory.
                        WalkDirectoryTree(dirInfo);
                    }
                }
                filex.Close();// Because at end filestream needs to close
            }
        }
    }
}
