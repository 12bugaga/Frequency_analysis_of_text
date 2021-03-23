using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Triplet
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, int> allTriplet = new Dictionary<string, int>();
            var serviceProvider = new ServiceCollection()
                .AddSingleton<IWorkWithConsole, WorkWithConsole>()
                .AddSingleton<IMainLogic, MainLogic>()
                .AddSingleton<IStartApp, StartApp>()
            .BuildServiceProvider();

            Console.OutputEncoding = Encoding.UTF8;
            var start = serviceProvider.GetService<IStartApp>();
            CancellationTokenSource cts = new CancellationTokenSource();
            
            start.StartProgram(cts);
        }
    }
}
