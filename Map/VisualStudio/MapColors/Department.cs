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

namespace MapColors
{
    class Department
    {
        internal string Id { get; set; }
        internal Region Region { get; set; }
        internal Dictionary<Department, int> AdjacentDepartments { get; private set; }
        internal double Latitude { get; set; }
        internal double Longitude { get; set; }
        internal ColorEnum Color { get; private set; }
        private Dictionary<ColorEnum, int> _unsuitableColors = new Dictionary<ColorEnum, int>();

        private static Dictionary<ColorEnum, int> _colorCount;

        static Department()
        {
            _colorCount = new Dictionary<ColorEnum, int>();
            _colorCount[ColorEnum.RED] = 0;
            _colorCount[ColorEnum.GREEN] = 0;
            _colorCount[ColorEnum.BLUE] = 0;
            _colorCount[ColorEnum.YELLOW] = 0;
        }

        internal Department()
        {
            Color = ColorEnum.NONE;
            AdjacentDepartments = new Dictionary<Department, int>();
        }

        internal void clear()
        {
            _unsuitableColors.Clear();
            Color = ColorEnum.NONE;
        }

        internal bool setColor()
        {
            if (_unsuitableColors.Count == 4)
            {
                if (Color != ColorEnum.NONE) --_colorCount[Color];
                Color = ColorEnum.NONE;
            }
            else
            {
                SortedDictionary<int, List<ColorEnum>> suitableColors = new SortedDictionary<int, List<ColorEnum>>();

                foreach (KeyValuePair<ColorEnum, int> pair in _colorCount)
                {
                    if (!_unsuitableColors.ContainsKey(pair.Key))
                    {
                        if (!suitableColors.ContainsKey(pair.Value)) suitableColors[pair.Value] = new List<ColorEnum>();
                        suitableColors[pair.Value].Add(pair.Key);
                    }
                }

                if (Color != ColorEnum.NONE) --_colorCount[Color];
                Color = suitableColors.First().Value[0];
                ++_colorCount[Color];
                _unsuitableColors.Add(Color, 0);
            }

            return Color != ColorEnum.NONE;
        }

        internal double distance(Department department)
        {
            double d = 0.0;

            if (department != this)
            {
                if (department.Region != Region) d += 1e8;
                if (!AdjacentDepartments.ContainsKey(department)) d += 1e4;

                double d1 = Latitude - department.Latitude;
                double d2 = Longitude - department.Longitude;
                d += d1 * d1 + d2 * d2;
            }

            return d;
        }
    }
}
