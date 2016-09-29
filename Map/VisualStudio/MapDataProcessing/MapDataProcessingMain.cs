using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapDataProcessing
{
    class MapDataProcessingMain
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("usage: MapDataProcessing.exe path1 [path2 ...]");
            }
            else
            {
                foreach (string path in args)
                {
                    MapDataProcessor mapDataProcessor = new MapDataProcessor(path);
                    int result = mapDataProcessor.run();

                    if (result == 0) Console.WriteLine("Map data processing terminated successfully for file {0}", path);
                    else Console.WriteLine("Map data processing terminated with errors for file {0}", path);

                    Console.WriteLine();
                }
            }
    
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
