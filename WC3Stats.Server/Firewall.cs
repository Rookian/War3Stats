using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace WC3Stats.Server
{
    public class Firewall
    {
        public static async Task CreateFirewallRule()
        {
            Console.WriteLine("Create firewall rule for port 5001");
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "netsh.exe",
                Arguments =
                    "advfirewall firewall add rule name=\"War3Stats Port 5001\" dir=in action=allow protocol=TCP localport=5001",
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
            };

            var process = Process.Start(processStartInfo);
            process.WaitForExit(1000);
            var errors = await process.StandardError.ReadToEndAsync();
            if (errors.Length > 0)
                Console.WriteLine(errors);
            else
            {
                Console.WriteLine("Firewall rule created.");
            }
        }
    }
}