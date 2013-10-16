using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Recursivefilesearch
{
    class Program
    {
        static System.Collections.Specialized.StringCollection log = new System.Collections.Specialized.StringCollection();

        static void Main()
        {
            string[] drives = {"C:\\"};

            foreach (string dr in drives)
            {
                System.IO.DriveInfo di = new System.IO.DriveInfo(dr);

                // Here we skip the drive if it is not ready to be read. This 
                // is not necessarily the appropriate action in all scenarios. 
                if (!di.IsReady)
                {
                    Console.WriteLine("The drive {0} could not be read", di.Name);
                    continue;
                }
                System.IO.DirectoryInfo rootDir = di.RootDirectory;
                WalkDirectoryTree(rootDir);
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
            System.IO.FileInfo[] files = null;
            System.IO.DirectoryInfo[] subDirs = null;
            System.IO.StreamWriter filex = new System.IO.StreamWriter("test.txt", true);

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
                filex = new System.IO.StreamWriter("test.txt", true);
                foreach (System.IO.FileInfo fi in files)
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
                foreach (System.IO.DirectoryInfo subds in subDirs)
                {
                    filex = new System.IO.StreamWriter("test.txt", true);
                    Console.WriteLine(subds.FullName);
                    filex.WriteLine(subds.FullName);
                    filex.Close();

                    foreach (System.IO.DirectoryInfo dirInfo in subDirs)
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
