namespace Warranty.JdeImport
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Importers;

    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 1 && args[0] == "-help")
            {
                PrintUsage();
                return;
            }

            Console.Out.WriteLine("Retrieving from JDE and writing to SQL Server...");
            Console.WriteLine("Started at {0}", DateTime.Now);

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var options = args.ToList();

            if (options.Count == 0)
            {
                options = new List<string>
                {
                    "-wp",
                    "-p",
                    "-j",
                    "-js"
                };
            }

            if (options.Contains("-wp")) new WarrantyPaymentImporter().CustomImport();
            if (options.Contains("-p")) new PaymentImporter().CustomImport();
            if (options.Contains("-j")) new JobImporter().CustomImport();
            if (options.Contains("-js")) new JobStageImporter().CustomImport();

            stopWatch.Stop();
            Console.WriteLine("\n\nTOTAL TIME: {0}\n", stopWatch.Elapsed);
            Console.WriteLine("Finished at {0}", DateTime.Now);
        }

        private static void PrintUsage()
        {
            Console.WriteLine("\nRunning the importer with no options will run all importers.");
            Console.WriteLine("Running the importer with -help will show this message.");
            Console.WriteLine("\nEach option needs to be separated by a space and prefixed with a -:");
            Console.WriteLine("-wp\tWarranty Payments");
            Console.WriteLine("-p\tPayments");
            Console.WriteLine("-j\tJobs");
            Console.WriteLine("-js\tJob Stage History");
        }
    }
}
