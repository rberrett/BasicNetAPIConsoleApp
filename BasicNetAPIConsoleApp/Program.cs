using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BasicNetAPIConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            do
            {
                List<NetAPI32.FILE_INFO_3> list = NetAPI32.GetNetFileEnumList();
                //Console.WriteLine($"Num file connections = {list.Count}");
                foreach (NetAPI32.FILE_INFO_3 file in list)
                {
                    int id = file.fi3_id;
                    int perm = file.fi3_permission;
                    string pathname = file.fi3_pathname;
                    string username = file.fi3_username;
                    //Console.WriteLine($"FILE_INFO_3:pathname = {pathname}");
                }
                Thread.Sleep(100);
            } while (true);

        }
    }
}
