using PrerenderPlaywright.Messages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Threading.Tasks;

namespace PrerenderPlaywright.Clients
{
    public class ProcessClient : IProcessClient
    {
        private int pid;
        public ProcessClient()
        {
            pid = System.Environment.ProcessId;
        }

        public Task<ProcessInfo> GetProcessesAsync()
        {
            if (!TryGetProcess(pid, out var process))
            {
                return null;
            }

            AddChildren(process);
            var rootProcess = GetRootProcess(process);
            return Task.FromResult(rootProcess);
        }

        private static void AddChildren(ProcessInfo processInfo)
        {
            var children = GetChildren(processInfo.Id).ToArray();
            
            foreach (var child in children)
            {
                AddChildren(child);
                processInfo.Children.Add(child);
            }
        }

        private static ProcessInfo GetRootProcess(ProcessInfo processInfo)
        {
            if (!TryGetProcess(processInfo.ParentProcessId, out var parent))
            {
                return processInfo;
            }

            parent.Children.Add(processInfo);

            if (parent.ParentProcessId != 0)
            {
                return GetRootProcess(parent);
            }

            return parent;
        }

#pragma warning disable CA1416 // Validate platform compatibility
        private static IEnumerable<ProcessInfo> GetChildren(int id)
        {
            foreach (var mo in Search($"Select ProcessID, ParentProcessID, Name, ExecutablePath From Win32_Process Where ParentProcessID={id}"))
            {
                yield return MapProcessInfo(mo);
            }
        }

        private static bool TryGetProcess(int id, out ProcessInfo process)
        {
            process = null;
            try
            {
                foreach (var mo in Search($"Select ProcessID, ParentProcessID, Name, ExecutablePath From Win32_Process Where ProcessID = {id}"))
                {
                    process = MapProcessInfo(mo);
                    return true;
                }
            }
            catch(ManagementException ex)
            {
                Debug.WriteLine(ex);
            }

            return false;
        }

        private static IEnumerable<ManagementObject> Search(string query)
        {
            using var searcher = new ManagementObjectSearcher(query);
            foreach (ManagementObject mo in searcher.Get())
            {
                yield return mo;
            }
        }

        private static ProcessInfo MapProcessInfo(ManagementObject managementObject)
        {
            object GetValue(string name)
            {
                try
                {
                    return managementObject[name];
                }
                catch (ManagementException ex)
                {
                    throw new KeyNotFoundException($"{name} is not found.", ex);
                }
            }

            return new ProcessInfo(
                Convert.ToInt32(GetValue("ProcessID")),
                Convert.ToInt32(GetValue("ParentProcessId")),
                Convert.ToString(GetValue("Name")),
                Convert.ToString(GetValue("ExecutablePath")));
        }

#pragma warning restore CA1416 // Validate platform compatibility
    }
}
