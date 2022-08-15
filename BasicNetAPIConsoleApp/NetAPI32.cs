using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BasicNetAPIConsoleApp
{
    internal class NetAPI32
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct FileInfo3
        {
            public int SessionID;
            public int Permission;
            public int NumLocks;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string PathName;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string UserName;
        }

        [DllImport("netapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern int NetFileEnum(
            string serverName,
            string basePath,
            string userName,
            int level,
            ref IntPtr buffer,
            // MAX_PREFERRED_LENGTH == -1
            int prefMaxLength,
            out int entriesRead,
            out int totalEntries,
            ref int resumeHandle
            );

        [DllImport("Netapi32.dll", SetLastError = true)]
        public static extern int NetApiBufferFree(IntPtr Buffer);

        public static List<FileInfo3> BuildNetFileEnumList(string server = null)
        {
            List<FileInfo3> list = new List<FileInfo3>();

            int entriesRead;
            int totalEntries;
            int resumeHandle = 0;
            IntPtr pBuffer = IntPtr.Zero;
            int status = NetFileEnum(server, null, null, 3, ref pBuffer, -1, out entriesRead, out totalEntries, ref resumeHandle);

            if (status == 0 && entriesRead > 0)
            {
                Type shareinfoType = typeof(FileInfo3);
                int offset = Marshal.SizeOf(shareinfoType);

                for (int i = 0, item = pBuffer.ToInt32(); i < entriesRead; i++, item += offset)
                {
                    IntPtr pItem = new IntPtr(item);

                    FileInfo3 fileInfo3 = (FileInfo3)Marshal.PtrToStructure(pItem, shareinfoType);
                    list.Add(fileInfo3);
                }
                NetApiBufferFree(pBuffer);
            }
            return list;
        }

    }
}
