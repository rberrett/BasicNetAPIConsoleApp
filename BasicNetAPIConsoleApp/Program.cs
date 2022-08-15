using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicNetAPIConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            StreamWriter sw = new StreamWriter(@"C:\temp\FileAccess\leftopen.txt");

            List<NetAPI32.FileInfo3> fileconnections = NetAPI32.BuildNetFileEnumList();
            Console.WriteLine($"Num file connections = {fileconnections.Count}");
            foreach (NetAPI32.FileInfo3 fileconnection in fileconnections)
            {
                int remoteUserPrimition = fileconnection.Permission;
                string remoteUsername = fileconnection.UserName;
                string pathName = fileconnection.PathName;
                int shareID = fileconnection.SessionID;
                Console.WriteLine($"FileInfo3:pathName = {pathName}");
            }
            Console.WriteLine("All done. Hit enter key to quit...");
            Console.ReadLine();

            sw.WriteLine("Blah blah blah.");
            sw.Close();
        }
    }
}
