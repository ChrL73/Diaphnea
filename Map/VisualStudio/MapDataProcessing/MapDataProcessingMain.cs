/* --------------------------------------------------------------------
 *
 * Copyright (C) 2018
 *
 * This file is part of the Diaphnea project.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License
 * version 3 as published by the Free Software Foundation
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 * -------------------------------------------------------------------- */

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
            int result = 0;

            if (args.Length < 1)
            {
                Console.WriteLine("usage: MapDataProcessing.exe path1 [path2 ...]");
                result = -1;
            }
            else
            {
                foreach (string path in args)
                {
                    MapDataProcessor mapDataProcessor = new MapDataProcessor(path);
                    result = mapDataProcessor.run();

                    if (result != 0)
                    {
                        Console.WriteLine("Map data processing terminated with errors for file {0}", path);
                        break;
                    }

                    Console.WriteLine("Map data processing terminated successfully for file {0}", path);
                    Console.WriteLine();
                }
            }
    
            //Console.WriteLine("Press any key to continue...");
            //Console.ReadKey();

            System.Environment.Exit(result);
        }
    }
}
