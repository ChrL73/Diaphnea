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

#pragma once

namespace map_server
{
    class TextDisplayerParameters
    {
    private:
        static constexpr double _maxRepulsionRatio = 4.0;
        static constexpr int _potentialTableSize = 1000;
        static constexpr int _maxVisibleTextCount = 100;
        static constexpr double _potentialTableSizeD = static_cast<double>(_potentialTableSize);

        static constexpr double _potentialThreshold = 4.5;
        static constexpr double _pointRefPotential = 3.0;
        static constexpr double _pointRadiusCoeff = 1.0;
        static constexpr double _segmentRefPotential = 1.5;
        static constexpr double _segmentRadius1Coeff = 1.0;
        static constexpr double _segmentRadius2Coeff = 7.0;
        static constexpr double _textRadiusCoeff = 0.75;
        static constexpr double _textRefPotential = 3.5;

        static constexpr int _pointTryCount = 18;
        static constexpr int _polygonXTryCount = 5; // Must be odd
        static constexpr int _polygonYTryCount = 9; // Must be odd
        static constexpr int _lineXTryCount = 10;
        static constexpr int _lineYTryCount = 10;

        static constexpr double _edgeRefPotential = 7.0;
        static constexpr double _edgeRangeRatio = 0.02;
        static constexpr double _centeringPotential = 0.2;
        static constexpr double _computationDensityFactor = 0.1;

        static double _excludingPotentialTable[_potentialTableSize];
        static double _notExcludingPotentialTable1[_potentialTableSize];
        static double _notExcludingPotentialTable2[_potentialTableSize];
        static bool _tablesFilled;

        static void fillTables(void);

    public:
        TextDisplayerParameters();

        double getMaxRepulsionRatio(void) const { return _maxRepulsionRatio; }
        int getPotentialTableSize(void) const { return _potentialTableSize; }
        double getPotentialTableSizeD(void) const { return _potentialTableSizeD; }
        int getMaxVisibleTextCount(void) const { return _maxVisibleTextCount; }

        double getPotentialThreshold(void) const { return _potentialThreshold; }
        double getPointRefPotential(void) const { return _pointRefPotential; }
        double getPointRadiusCoeff(void) const { return _pointRadiusCoeff; }
        double getSegmentRefPotential(void) const { return _segmentRefPotential; }
        double getSegmentRadius1Coeff(void) const { return _segmentRadius1Coeff; }
        double getSegmentRadius2Coeff(void) const { return _segmentRadius2Coeff; }
        double getTextRadiusCoeff(void) const { return _textRadiusCoeff; }
        double getTextRefPotential(void) const { return _textRefPotential; }

        int getPointTryCount(void) const { return _pointTryCount; }
        int getPolygonXTryCount(void) const { return _polygonXTryCount; }
        int getPolygonYTryCount(void) const { return _polygonYTryCount; }
        int getLineXTryCount(void) const { return _lineXTryCount; }
        int getLineYTryCount(void) const { return _lineYTryCount; }

        double getEdgeRefPotential(void) const { return _edgeRefPotential; }
        double getEdgeRangeRatio(void) const { return _edgeRangeRatio; }
        double getCenteringPotential(void) const { return _centeringPotential; }
        double getComputationDensityFactor(void) const { return _computationDensityFactor; }

        double getExcludingPotential(int i) const { return _excludingPotentialTable[i]; }
        double getNotExcludingPotential1(int i) const { return _notExcludingPotentialTable1[i]; }
        double getNotExcludingPotential2(int i) const { return _notExcludingPotentialTable2[i]; }
    };
}
