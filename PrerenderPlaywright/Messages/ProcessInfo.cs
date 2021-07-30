using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PrerenderPlaywright.Messages
{
    public class ProcessInfo
    {
        public ProcessInfo(
            int processId,
            int parentProcessId,
            string processName,
            string executablePath)
        {
            Id = processId;
            ParentProcessId = parentProcessId;
            Name = processName;
            ExecutablePath = executablePath;
            Children = new List<ProcessInfo>();
        }

        public int Id { get; }

        [JsonIgnore]
        public int ParentProcessId { get; }
        public string Name { get; }
        public string ExecutablePath { get; }
        public List<ProcessInfo> Children { get; }
    }   
}
