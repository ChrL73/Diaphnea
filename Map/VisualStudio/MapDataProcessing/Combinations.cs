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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapDataProcessing
{
    class Combinations
    {
        private static Dictionary<Int64, List<List<Int32>>> _combinationDictionary = new Dictionary<long, List<List<Int32>>>();

        // Returns C(n, k), the number of k-combinations from a set of n elements
        // (n >= 0 and 0 <= k <= n)
        internal static Int32 getCount(Int32 n, Int32 k)
        {
            return getCombinations(n, k).Count();
        }

        // Returns the (j+1)th element of the (i+1)th k-combination of the set {0, 1, ..., n - 1}
        // (0 <= i <= C(n, k) - 1 and 0 <= j <= k - 1)
        internal static Int32 get(Int32 n, Int32 k, Int32 i, Int32 j)
        {
            return getCombinations(n, k)[i][j];
        }

        private static List<List<Int32>> getCombinations(Int32 n, Int32 k)
        {
            Int64 key = ((Int64)n << 32) + (Int64)k;
            List<List<Int32>> combinations;
            if (_combinationDictionary.TryGetValue(key, out combinations)) return combinations;

            combinations = new List<List<int>>();
            if (n >= 0 && k >= 0 && k <= n)
            {
                if (k == 0)
                {
                    combinations.Add(new List<Int32>());
                }
                else if (k == n)
                {
                    List<Int32> combination = new List<Int32>();
                    Int32 p;
                    for (p = 0; p < n; ++p) combination.Add(p);
                    combinations.Add(combination);
                }
                else
                {
                    List<List<Int32>> combination0 = getCombinations(n - 1, k - 1);

                    Int32 p, q = combination0.Count();
                    for (p = 0; p < q; ++p)
                    {
                        List<Int32> combination = combination0[p];
                        combination.Add(n - 1);
                        combinations.Add(combination);
                    }

                    combination0 = getCombinations(n - 1, k);
                    q = combination0.Count();
                    for (p = 0; p < q; ++p)
                    {
                        combinations.Add(combination0[p]);
                    }
                }
            }

            _combinationDictionary.Add(key, combinations);
            return combinations;
        }
    }
}
