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

		static constexpr double _softThreshold = 8.0;
		static constexpr double _hardThresholdExcludingTerm = 5.0;
		static constexpr double _hardThresholdNotExcludingTerm = 1.5;

		static constexpr double _pointRefPotential = 3.0;
		static constexpr double _pointRadiusCoeff = 0.5;
		static constexpr double _segmentRefPotential = 1.5;
		static constexpr double _segmentRadius1Coeff = 1.0;
		static constexpr double _segmentRadius2Coeff = 2.0;

		static constexpr double _edgeRefPotential = 2.0;
		static constexpr double _edgeRangeRatio = 0.05;

		static double _excludingPotentialTable[_potentialTableSize];
		static double _notExcludingPotentialTable1[_potentialTableSize];
		static double _notExcludingPotentialTable2[_potentialTableSize];
		static bool _tablesFilled;

		double _excludingPotentialTableCopy[_potentialTableSize];
		double _notExcludingPotentialTable1Copy[_potentialTableSize];
		double _notExcludingPotentialTable2Copy[_potentialTableSize];

		void fillTables(void);

    public:
		TextDisplayerParameters();

		double getMaxRepulsionRatio(void) const { return _maxRepulsionRatio; }
		int getPotentialTableSize(void) const { return _potentialTableSize; }
		int getPotentialTableSizeD(void) const { return _potentialTableSizeD; }
		int getMaxVisibleTextCount(void) const { return _maxVisibleTextCount; }

		double getSoftThreshold(void) const { return _softThreshold; }
		double getHardThresholdExcludingTerm(void) const { return _hardThresholdExcludingTerm; }
		double getHardThresholdNotExcludingTerm(void) const { return _hardThresholdNotExcludingTerm; }

		double getPointRefPotential(void) const { return _pointRefPotential; }
		double getPointRadiusCoeff(void) const { return _pointRadiusCoeff; }
		double getSegmentRefPotential(void) const { return _segmentRefPotential; }
		double getSegmentRadius1Coeff(void) const { return _segmentRadius1Coeff; }
		double getSegmentRadius2Coeff(void) const { return _segmentRadius2Coeff; }

		double getEdgeRefPotential(void) const { return _edgeRefPotential; }
		double getEdgeRangeRatio(void) const { return _edgeRangeRatio; }

		double getExcludingPotential(int i) const { return _excludingPotentialTableCopy[i]; }
		double getNotExcludingPotential1(int i) const { return _notExcludingPotentialTable1Copy[i]; }
		double getNotExcludingPotential2(int i) const { return _notExcludingPotentialTable2Copy[i]; }
    };
}
