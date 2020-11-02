using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.ServiceProcess;

namespace GmerSdk
{
    public class GmerDriver : IDisposable
    {
        private const string ServiceName = "GmerSdk";
        private readonly string _driverFile;
        private ServiceController _service;
        private IntPtr _ioctlHandle;

        public GmerDriver()
        {
            using (var driverStream = new MemoryStream())
            {
                var resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("GmerSdk.gmerdrv");
                if (resourceStream == null) throw new FileLoadException("Can't find Gmer driver!");
                using (resourceStream)
                using (var compressionStream = new GZipStream(resourceStream, CompressionMode.Decompress))
                    compressionStream.CopyTo(driverStream);

                _driverFile = Path.GetTempFileName();
                File.WriteAllBytes(_driverFile, driverStream.ToArray());
            }
        }

        public bool Initialize()
        {
            if (!ServiceHelper.DeleteService(ServiceName))
                throw new FileLoadException("Can't remove Gmer service!");
            if (!ServiceHelper.CreateService(ServiceName, _driverFile))
                if (!ServiceHelper.DeleteService(ServiceName) || ServiceHelper.CreateService(ServiceName, _driverFile))
                    throw new FileLoadException("Can't create Gmer service!");

            try
            {
                _service = new ServiceController(ServiceName);
                _service.Start();
            }
            catch
            {
                throw new FileLoadException("Can't start Gmer service!");
            }

            _ioctlHandle = Native.CreateFile("\\\\.\\uwadifow", 0xC0000000, 0x03, IntPtr.Zero, 0x03, 0x80, IntPtr.Zero);
            return _ioctlHandle.ToInt32() != -1;
        }

        public byte[] ReadKernelMemory(ulong address, uint size, out uint outSize)
        {
            var result = new byte[size];
            Native.DeviceIoControl(_ioctlHandle, 0x7201C028, ref BitConverter.GetBytes(address)[0], 8, ref result[0],
                size, out outSize, IntPtr.Zero);
            return result;
        }

        public bool WriteKernelMemory(ulong address, byte[] data)
        {
            var buffer = new byte[24 + data.Length];
            Array.Copy(BitConverter.GetBytes(address), 0, buffer, 8, 8);
            Array.Copy(BitConverter.GetBytes((uint)data.Length), 0, buffer, 16, 4);
            Array.Copy(data, 0, buffer, 20, data.Length);
            return Native.DeviceIoControl(_ioctlHandle, 0x7201C034, ref buffer[0], (uint)buffer.Length, IntPtr.Zero, 0, out _, IntPtr.Zero);
        }

        public void Dispose()
        {
            try
            {
                if (_ioctlHandle != IntPtr.Zero)
                    Native.CloseHandle(_ioctlHandle);
                ServiceHelper.DeleteService(ServiceName, _service);
                File.Delete(_driverFile);
            }
            catch
            {
                // ignore
            }
        }
    }
}