﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BasicNetAPIConsoleApp
{
    internal class NetAPI32
    {
        // https://docs.microsoft.com/en-us/openspecs/windows_protocols/ms-srvs/789ec187-e01b-4da6-a7ff-9cb9e0857230
        private const int MAX_PREFERRED_LENGTH = -1;

        // https://docs.microsoft.com/en-us/windows/win32/api/lmshare/ns-lmshare-file_info_3
        // http://www.pinvoke.net/default.aspx/Structures/FILE_INFO_3.html
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct FILE_INFO_3
        {
            // the identification number assigned to the resource when it is opened
            public int fi3_id;
            // access permissions associated with the opening application; can be one of the 
            // following values: PERM_FILE_READ, PERM_FILE_WRITE, or PERM_FILE_CREATE
            public int fi3_permission;
            // the number of file locks on the file, device, or pipe
            public int fi3_num_locks;
            // the path of the opened resource
            [MarshalAs(UnmanagedType.LPWStr)]
            public string fi3_pathname;
            // which user opened the resource
            [MarshalAs(UnmanagedType.LPWStr)]
            public string fi3_username;
        }

        // https://docs.microsoft.com/en-us/windows/win32/api/lmshare/nf-lmshare-netfileenum
        // https://www.pinvoke.net/default.aspx/netapi32.NetFileEnum
        [DllImport("netapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern int NetFileEnum(
            // the DNS or NetBIOS name of the remote server on which the function is to execute.
            // If this parameter is NULL, the local computer is used.
            string servername,
            // a string that specifies a qualifier for the returned information. If this parameter
            // is NULL, all open resources are enumerated. If this parameter is not NULL, the
            // enumerates only resources that have the value of the basepath parameter as a prefix
            string basepath,
            // a string that specifies the name of the user or the name of the connection.  If
            // this parameter is not NULL, its value serves as a qualifier for the enumeration.
            // The files returned are limited to those that have user names or connection names
            // that match the qualifier. If parameter is NULL, no user-name qualifier is used. 
            string username,
            // the information level of the data. This parameter can be one of the following:
            //   2: bufptr parameter points to an array of FILE_INFO_2 structures
            //   3: bufptr parameter points to an array of FILE_INFO_3 structures
            int level,
            // the address of the buffer that receives the information
            ref IntPtr bufptr,
            //  the preferred maximum length of returned data, in bytes. If MAX_PREFERRED_LENGTH
            //  is specified, then the call allocates the amount of memory required for the data.
            int prefmaxlen,
            // the count of elements actually enumerated
            out int entriesread,
            // the total number of entries that could have been enumerated from the current resume
            // position. Note that applications should consider this value only as a hint.
            out int totalentries,
            // a resume handle which is used to continue an existing file search. The handle
            // should be zero on the first call and left unchanged for subsequent calls. If this
            // parameter is NULL, no resume handle is stored
            IntPtr resume_handle
        );

        [DllImport("Netapi32.dll", SetLastError = true)]
        private static extern int NetApiBufferFree(IntPtr Buffer);

        internal static List<FILE_INFO_3> GetNetFileEnumList(string server = null)
        {
            List<FILE_INFO_3> list = new List<FILE_INFO_3>();

            int dwReadEntries;
            int dwTotalEntries;
            IntPtr pBuffer = IntPtr.Zero;
            FILE_INFO_3 pCurrent = new FILE_INFO_3();

            int dwStatus = NetFileEnum(null, null, null, 3, ref pBuffer, MAX_PREFERRED_LENGTH, 
                                       out dwReadEntries, out dwTotalEntries, IntPtr.Zero);

            if (dwStatus == 0)
            {
                for (int dwIndex = 0; dwIndex < dwReadEntries; dwIndex++)
                {
                    // create the struct from the buffer
                    IntPtr iPtr = new IntPtr(pBuffer.ToInt32() + (dwIndex * Marshal.SizeOf(pCurrent)));
                    pCurrent = (FILE_INFO_3)Marshal.PtrToStructure(iPtr, typeof(FILE_INFO_3));
                    list.Add(pCurrent);
                    // debug
                    Console.WriteLine("dwIndex={0}", dwIndex);
                    Console.WriteLine("    id={0}", pCurrent.fi3_id);
                    Console.WriteLine("    num_locks={0}", pCurrent.fi3_num_locks);
                    Console.WriteLine("    pathname={0}", pCurrent.fi3_pathname);
                    Console.WriteLine("    permission={0}", pCurrent.fi3_permission);
                    Console.WriteLine("    username={0}", pCurrent.fi3_username);
                }

                NetApiBufferFree(pBuffer);
            }
            return list;
        }

    }
}
