using System.Collections.Generic;
using Serilog;

namespace ToolService
{
    public abstract class Tool
    {
        // Common properties
        public string Name { get; set; }
        public string Id { get; set; }
        public List<Port> Ports { get; set; }

        protected Tool()
        {
            // Setup Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File(@"C:\Logs\Log.txt",
                    rollingInterval: RollingInterval.Day,
                    rollOnFileSizeLimit: true)
                .CreateLogger();
        }

        // Tool specific operations
        public abstract void Initialize(string eqSvr, int timeout);

    }
}
