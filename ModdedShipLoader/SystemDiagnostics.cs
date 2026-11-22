using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace ModdedShipLoader
{
    // Writes machine and memory figures to the BepInEx log, so a player's LogOutput.log is enough
    // to tell whether a ship load ran out of memory. Every line starts with "[MSL-Diag]".
    // Nothing that identifies the player (user name, machine name, paths) is logged.
    internal static class SystemDiagnostics
    {
        const string Tag = "[MSL-Diag] ";
        const double GB = 1024.0 * 1024.0 * 1024.0;

        [StructLayout(LayoutKind.Sequential)]
        class MEMORYSTATUSEX
        {
            public uint dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct PROCESS_MEMORY_COUNTERS_EX
        {
            public uint cb;
            public uint PageFaultCount;
            public UIntPtr PeakWorkingSetSize;
            public UIntPtr WorkingSetSize;
            public UIntPtr QuotaPeakPagedPoolUsage;
            public UIntPtr QuotaPagedPoolUsage;
            public UIntPtr QuotaPeakNonPagedPoolUsage;
            public UIntPtr QuotaNonPagedPoolUsage;
            public UIntPtr PagefileUsage;
            public UIntPtr PeakPagefileUsage;
            public UIntPtr PrivateUsage;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool GlobalMemoryStatusEx([In, Out] MEMORYSTATUSEX buffer);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetCurrentProcess();

        [DllImport("psapi.dll", SetLastError = true)]
        static extern bool GetProcessMemoryInfo(IntPtr process, out PROCESS_MEMORY_COUNTERS_EX counters, uint size);

        public static void Log(string message)
        {
            ModdedShipLoader.LoggerInstance.LogInfo(Tag + message);
        }

        static string Gb(double bytes)
        {
            return (bytes / GB).ToString("F1", System.Globalization.CultureInfo.InvariantCulture) + " GB";
        }

        // Machine description, logged once at startup.
        public static void LogSystem()
        {
            try
            {
                Log($"System: {SystemInfo.operatingSystem} | CPU {SystemInfo.processorType}, {SystemInfo.processorCount} threads | RAM {SystemInfo.systemMemorySize} MB | GPU {SystemInfo.graphicsDeviceName}, {SystemInfo.graphicsMemorySize} MB");
            }
            catch (Exception ex)
            {
                Log($"System: not available ({ex.Message})");
            }

            try
            {
                var drives = new StringBuilder();
                foreach (var drive in DriveInfo.GetDrives())
                {
                    if (drive.DriveType != DriveType.Fixed || !drive.IsReady) continue;
                    if (drives.Length > 0) drives.Append(" | ");
                    drives.Append($"{drive.Name} {Gb(drive.AvailableFreeSpace)} free of {Gb(drive.TotalSize)}");
                }
                Log($"Drives: {drives}");
            }
            catch (Exception ex)
            {
                Log($"Drives: not available ({ex.Message})");
            }

            LogMemory("Startup");
        }

        // Memory figures at a named moment.
        // "Commit" is memory Windows has promised to programs (RAM + page file); a load fails when the
        // game asks for more than "commit free". "Game peak" is the most the game has held since it started.
        public static void LogMemory(string moment)
        {
            var line = new StringBuilder(moment).Append(": ");

            try
            {
                var status = new MEMORYSTATUSEX();
                if (GlobalMemoryStatusEx(status))
                {
                    line.Append($"RAM free {Gb(status.ullAvailPhys)} of {Gb(status.ullTotalPhys)} | commit free {Gb(status.ullAvailPageFile)} of {Gb(status.ullTotalPageFile)} (page file about {Gb((double)status.ullTotalPageFile - status.ullTotalPhys)})");
                }
                else
                {
                    line.Append("system memory not available");
                }
            }
            catch (Exception ex)
            {
                line.Append($"system memory not available ({ex.Message})");
            }

            try
            {
                var counters = new PROCESS_MEMORY_COUNTERS_EX();
                counters.cb = (uint)Marshal.SizeOf(typeof(PROCESS_MEMORY_COUNTERS_EX));
                if (GetProcessMemoryInfo(GetCurrentProcess(), out counters, counters.cb))
                {
                    line.Append($" | game now {Gb(counters.PrivateUsage.ToUInt64())}, game peak {Gb(counters.PeakPagefileUsage.ToUInt64())}, in RAM {Gb(counters.WorkingSetSize.ToUInt64())}");
                }
                else
                {
                    line.Append(" | game memory not available");
                }
            }
            catch (Exception ex)
            {
                line.Append($" | game memory not available ({ex.Message})");
            }

            Log(line.ToString());
        }
    }
}
