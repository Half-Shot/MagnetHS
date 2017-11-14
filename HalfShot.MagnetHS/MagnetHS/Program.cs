using System;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
namespace HalfShot.MagnetHS.Core
{
    class Program
    {
        static Dictionary<Process, ServiceProcessDefinition> runningServices;
        static List<ServiceProcessDefinition> serviceDefinitions;
        static Timer serviceWatcher;
        const int serviceWatcherPeriodMs = 250;
        const int serviceRestartPeriodMs = 15000;
        static void Main(string[] args)
        {
            Console.WriteLine($"You are running MagnetHS v{Assembly.GetExecutingAssembly().GetName().Version}");
            runningServices = new Dictionary<Process, ServiceProcessDefinition>();
            serviceDefinitions = new List<ServiceProcessDefinition>()
            {
                new ServiceProcessDefinition("MagnetHS.ClientServerAPIService.dll", "Client Service API"),
                new ServiceProcessDefinition("MagnetHS.UserService.dll", "User Service"),
                new ServiceProcessDefinition("MagnetHS.DatastoreService.dll", "Datastore Service"),
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
                    Console.WriteLine("Process has exited!");
                    processesToPop.Add(service.Key, service.Value);
                }
            }
            foreach (var service in processesToPop)
            {
                runningServices.Remove(service.Key);
                Console.WriteLine("Process has been removed!");
                System.Threading.Tasks.Task.Delay(serviceRestartPeriodMs).ContinueWith((t) =>
                {
                    StartProcess(service.Value);
                });
            }
        }

        static void StartProcess(ServiceProcessDefinition service)
        {
            var startProc = new ProcessStartInfo()
            {
                FileName = "dotnet",
                Arguments = service.Dll,
                WorkingDirectory = Environment.CurrentDirectory
            };
            var proc = Process.Start(startProc);
            Console.WriteLine($"Starting new {service.Name} instance.");
            runningServices.Add(proc, service);
        }
    }
}
