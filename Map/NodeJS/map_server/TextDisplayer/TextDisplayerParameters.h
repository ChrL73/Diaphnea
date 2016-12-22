#pragma once

namespace map_server
{
    class Potential;

    class TextDisplayerParameters
    {
	private:
		static constexpr double _maxRepulsionRatio = 5.0;
		static constexpr int _potentialTableSize = 1000;
		static constexpr int _maxVisibleTextCount = 100;
		static constexpr double _potentialTableSizeD = static_cast<double>(_potentialTableSize);

		static constexpr double _softThresholdExcludingTerm = 1.0;
		static constexpr double _softThresholdNotExcludingTerm = 8.0;
		static constexpr double _hardThresholdExcludingTerm = 1.0;
		static constexpr double _hardThresholdNotExcludingTerm = 0.75;

		static constexpr double _pointRefPotential = 2.0;
		static constexpr double _pointRadiusCoeff = 2.0;
		static constexpr double _segmentRefPotential = 1.5;
		static constexpr double _segmentRadius1Coeff = 0.9;
		static constexpr double _segmentRadius2Coeff = 2.5;

		static double _excludingPotentialTable[_potentialTableSize];
		static double _notExcludingPotentialTable[_potentialTableSize];
		static bool _tablesFilled;

		double _excludingPotentialTableCopy[_potentialTableSize];
		double _notExcludingPotentialTableCopy[_potentialTableSize];

		void fillTables(void);

    public:
		TextDisplayerParameters();

		double getMaxRepulsionRatio(void) const { return _maxRepulsionRatio; }
		int getPotentialTableSize(void) const { return _potentialTableSize; }
		int getPotentialTableSizeD(void) const { return _potentialTableSizeD; }
		int getMaxVisibleTextCount(void) const { return _maxVisibleTextCount; }

		double getSoftThresholdExcludingTerm(void) const { return _softThresholdExcludingTerm; }
		double getSoftThresholdNotExcludingTerm(void) const { return _softThresholdNotExcludingTerm; }
		double getHardThresholdExcludingTerm(void) const { return _hardThresholdExcludingTerm; }
		double getHardThresholdNotExcludingTerm(void) const { return _hardThresholdNotExcludingTerm; }

		double getPointRefPotential(void) const { return _pointRefPotential; }
		double getPointRadiusCoeff(void) const { return _pointRadiusCoeff; }
		double getSegmentRefPotential(void) const { return _segmentRefPotential; }
		double getSegmentRadius1Coeff(void) const { return _segmentRadius1Coeff; }
		double getSegmentRadius2Coeff(void) const { return _segmentRadius2Coeff; }

		double getExcludingPotential(int i) const { return _excludingPotentialTableCopy[i]; }
		double getNotExcludingPotential(int i) const { return _notExcludingPotentialTableCopy[i]; }
    };
}
