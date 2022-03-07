using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MonitorSwitcher
{
    internal class PInvoke
    {



        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        enum KeyModifier
        {
            None = 0,
            Alt = 1,
            Control = 2,
            Shift = 4,
            WinKey = 8
        }

        public static void RegisterHotKey(IntPtr Handle)
        {
            RegisterHotKey(Handle, 0, (int)KeyModifier.None, Keys.Scroll.GetHashCode());
        }

        public static void UnregisterHotKey(IntPtr Handle)
        {
            UnregisterHotKey(Handle, 0);
        }



        [DllImport("dxva2.dll")]
        //public static extern bool SetVCPFeature(IntPtr Handle, ref byte bVCPCode, ref uint dwNewValue);
        public static extern bool SetVCPFeature(IntPtr Handle, byte bVCPCode, uint dwNewValue);

        [DllImport("dxva2.dll", EntryPoint = "GetPhysicalMonitorsFromHMONITOR")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetPhysicalMonitorsFromHMONITOR(
           IntPtr hMonitor, uint dwPhysicalMonitorArraySize, [Out] PHYSICAL_MONITOR[] pPhysicalMonitorArray);

        [DllImport("user32")]
        internal static extern bool EnumDisplayMonitors(
            [In] IntPtr dcHandle,
            [In] IntPtr clip,
            MonitorEnumProcedure callback,
            IntPtr callbackObject
        );

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate int MonitorEnumProcedure(
            IntPtr monitorHandle,
            IntPtr dcHandle,
            IntPtr rect,
            IntPtr callbackObject
        );

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct PHYSICAL_MONITOR
        {
            public IntPtr hPhysicalMonitor;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)] public string szPhysicalMonitorDescription;
        }

        [DllImport("user32.dll", EntryPoint = "GetDesktopWindow")]
        private static extern IntPtr GetDesktopWindow();

        [DllImport("dxva2.dll", EntryPoint = "GetNumberOfPhysicalMonitorsFromHMONITOR")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetNumberOfPhysicalMonitorsFromHMONITOR(
            IntPtr hMonitor, ref uint pdwNumberOfPhysicalMonitors);


        [DllImport("user32.dll")]
        static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

        public static void ChangeInputSelect(bool IsHDMI1)
        {
            byte Operation = IsHDMI1 ? (byte)17 : (byte)15;


            IntPtr ptr = MonitorFromWindow(GetDesktopWindow(), 0);
            uint pdwNumberOfPhysicalMonitors = 0u;
            bool b1 = GetNumberOfPhysicalMonitorsFromHMONITOR(ptr, ref pdwNumberOfPhysicalMonitors);

            bool handeled = false;

            var callback = new MonitorEnumProcedure(
                (IntPtr handle, IntPtr dcHandle, IntPtr rect, IntPtr callbackObject) =>
                {
                    //if (handeled)
                     //   return 1;

                    uint dwPhysicalMonitorArraySize = pdwNumberOfPhysicalMonitors;
                    PHYSICAL_MONITOR[] pPhysicalMonitorArray = new PHYSICAL_MONITOR[pdwNumberOfPhysicalMonitors];
                    bool success = GetPhysicalMonitorsFromHMONITOR(ptr, dwPhysicalMonitorArraySize, pPhysicalMonitorArray);
                    byte InputSelect = 0x60;
                    var res = SetVCPFeature(pPhysicalMonitorArray[0].hPhysicalMonitor, InputSelect, Operation);
                    handeled = true;
                    return 1;
                }
            );
            EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, callback, IntPtr.Zero);
        }

    }
}
