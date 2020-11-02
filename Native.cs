using System;
using System.Runtime.InteropServices;

namespace GmerSdk
{
    internal class Native
    {
        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr OpenSCManagerW(string machineName, string databaseName, uint dwAccess);

        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateServiceW(IntPtr serviceControlManager, string serviceName, string displayName, uint desiredControlAccess, uint serviceType, uint startType, uint errorSeverity, string binaryPath, string loadOrderGroup, IntPtr outUIntTagId, string dependencies, string serviceUserName, string servicePassword);

        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern bool DeleteService(IntPtr service);

        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern bool CloseServiceHandle(IntPtr handle);

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr CreateFile(string fileName, uint fileAccess, uint fileShare, IntPtr securityAttributes, uint creationDisposition, uint flagsAndAttributes, IntPtr template);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool DeviceIoControl(IntPtr hDevice, uint ioControlCode, ref byte inBuffer, uint nInBufferSize, ref byte outBuffer, uint nOutBufferSize, out uint pBytesReturned, IntPtr overlapped);
       
        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool DeviceIoControl(IntPtr hDevice, uint ioControlCode, ref byte inBuffer, uint nInBufferSize, IntPtr outBuffer, uint nOutBufferSize, out uint pBytesReturned, IntPtr overlapped);
    }
}