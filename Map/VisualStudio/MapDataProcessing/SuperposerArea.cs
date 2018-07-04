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
    class SuperposerArea
    {
        private readonly List<SuperposerSegment> _segmentList = new List<SuperposerSegment>();

        internal void addSegment(SuperposerSegment segment) { _segmentList.Add(segment); }

        internal SuperposerPoint getNearestPoint(GeoPoint point, double threshold)
        {
            double dMin = threshold;
            int bestI = -1;

            int i, n = _segmentList.Count();
            for (i = 0; i < n; ++i)
            {
                double d = _segmentList[i].getDistance(point);
                if (d < dMin)
                {
                    dMin = d;
                    bestI = i;
                }
            }

            if (bestI != -1) return _segmentList[bestI].getNearestExtremity(point);
            return null;
        }
    }
}
