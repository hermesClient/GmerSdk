using System;
using System.ServiceProcess;

namespace GmerSdk
{
    internal class ServiceHelper
    {
        public static bool CreateService(string name, string path)
        {
            var scManager = IntPtr.Zero;
            var service = IntPtr.Zero;
            try
            {
                scManager = Native.OpenSCManagerW(null, null, 0xF003F);
                if (scManager == IntPtr.Zero) return false;
                service = Native.CreateServiceW(scManager, name, name, 0xF01FF, 0x1, 0x3, 0x1,
                    path, null, IntPtr.Zero, null, null, null);
                return service != IntPtr.Zero;
            }
            finally
            {
                if (service != IntPtr.Zero)
                    Native.CloseServiceHandle(service);
                if (scManager != IntPtr.Zero)
                    Native.CloseServiceHandle(scManager);
            }
        }

        public static bool DeleteService(string name, ServiceController service = null)
        {
            try
            {
                using (var serviceController = service ?? new ServiceController(name))
                {
                    if (serviceController.Status != ServiceControllerStatus.Stopped)
                        serviceController.Stop();
                    serviceController.WaitForStatus(ServiceControllerStatus.Stopped);
                    return Native.DeleteService(serviceController.ServiceHandle.DangerousGetHandle());
                }
            }
            catch
            {
                return true;
            }
        }
    }
}