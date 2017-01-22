#include "Combinations.h"

namespace map_server
{
	std::map<std::pair<int, int>, std::vector<std::vector<int> > > Combinations::_combinationMap;

	int Combinations::getCount(int n, int k)
	{
		return (*getIterator(n, k)).second.size();
	}

	int Combinations::get(int n, int k, int i, int j)
	{
		return (*getIterator(n, k)).second[i][j];
	}

	std::map<std::pair<int, int>, std::vector<std::vector<int> > >::iterator Combinations::getIterator(int n, int k)
	{
		std::pair<int, int> pair(n, k);
		std::map<std::pair<int, int>, std::vector<std::vector<int> > >::iterator it = _combinationMap.find(pair);
		if (it != _combinationMap.end()) return it;

		std::vector<std::vector<int> > combinations;
		if (n >= 0 && k >= 0 && k <= n)
		{
			if (k == 0)
			{
				std::vector<int> combination;
				combinations.push_back(combination);
			}
			else if (k == n)
			{
				std::vector<int> combination;
				int p;
				for (p = 0; p < n; ++p) combination.push_back(p);
				combinations.push_back(combination);
			}
			else
			{
				std::map<std::pair<int, int>, std::vector<std::vector<int> > >::iterator it = getIterator(n - 1, k - 1);
				int p;
				for (p = 0; p < (*it).second.size(); ++p)
				{
					std::vector<int> combination = (*it).second[p];
					combination.push_back(n - 1);
					combinations.push_back(combination);
				}

				it = getIterator(n - 1, k);
				for (p = 0; p < (*it).second.size(); ++p)
				{
					combinations.push_back((*it).second[p]);
				}
			}
		}

		return _combinationMap.insert(std::pair<std::pair<int, int>, std::vector<std::vector<int> > >(pair, combinations)).first;
	}
}
