using System;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
namespace HalfShot.MagnetHS.Core
{
    class Program
    {
        static Dictionary<Process, ServiceProcessDefinition> runningServices;
        static List<ServiceProcessDefinition> serviceDefinitions;
        static Timer serviceWatcher;
        const int serviceWatcherPeriodMs = 250;
        const int serviceRestartPeriodMs = 15000;
        static string RootDirectory;
        static void Main(string[] args)
        {
            Logger.StartLogger();
            Console.WriteLine($"You are running MagnetHS v{Assembly.GetExecutingAssembly().GetName().Version}");
            runningServices = new Dictionary<Process, ServiceProcessDefinition>();
            RootDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            // Get root path.
            string rootConfigPath = Path.GetFullPath("./config");

            serviceDefinitions = new List<ServiceProcessDefinition>()
            {
                new ServiceProcessDefinition("MagnetHS.ClientServerAPIService.dll", "Client Service API", 1, Path.Combine(rootConfigPath, "csapi.service.yaml")),
                new ServiceProcessDefinition("MagnetHS.UserService.dll", "User Service", 1, Path.Combine(rootConfigPath, "user.service.yaml")),
                new ServiceProcessDefinition("MagnetHS.DatastoreService.dll", "Datastore Service", 1, Path.Combine(rootConfigPath, "datastore.service.yaml")),
                new ServiceProcessDefinition("MagnetHS.LogService.dll", "Log Service", 1, Path.Combine(rootConfigPath, "log.service.yaml")),
                new ServiceProcessDefinition("MagnetHS.RoomService.dll", "Room Service", 1, Path.Combine(rootConfigPath, "room.service.yaml")),
                //new ServiceProcessDefinition("MagnetHS.ConfigurationService.dll", "Configuration Service"),
            };
            // Spin up some services
            foreach (var service in serviceDefinitions)
            {
                StartProcess(service);
            }
            serviceWatcher = new Timer(Watchdog, null, serviceWatcherPeriodMs, serviceWatcherPeriodMs);
            Console.WriteLine("Press any key to kill");
            Console.ReadKey();
            serviceWatcher.Dispose();
            runningServices.Keys.ToList().ForEach((p) => p.Close());
            runningServices.Keys.ToList().ForEach((p) => p.WaitForExit());
            Environment.Exit(0);
        }

        static void Watchdog(object state)
        {
            Dictionary<Process, ServiceProcessDefinition> processesToPop = new Dictionary<Process, ServiceProcessDefinition>();
            foreach (var service in runningServices)
            {
                if(service.Key.HasExited)
                {
                    Logger.Warn($"Process {service.Value.Name} has exited!");
                    processesToPop.Add(service.Key, service.Value);
                }
            }
            foreach (var service in processesToPop)
            {
                runningServices.Remove(service.Key);
                Logger.Info($"Process {service.Value.Name} has been removed!");
                Task.Delay(serviceRestartPeriodMs).ContinueWith((t) =>
                {
                    StartProcess(service.Value, 1);
                });
            }
        }

        static void StartProcess(ServiceProcessDefinition service, int overrideCount = 0)
        {
            var startProc = new ProcessStartInfo()
            {
                FileName = "dotnet",
                Arguments = service.Dll + " "+ String.Join(' ', service.Arguments),
                WorkingDirectory = RootDirectory
            };

            for (int i = 0; i < (overrideCount == 0 ? service.Count : overrideCount); i++)
            {
                var proc = Process.Start(startProc);
                Logger.Info($"Starting new {service.Name} instance.");
                runningServices.Add(proc, service);
            }
        }
    }
}
