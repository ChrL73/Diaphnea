#pragma once

#include <map>
#include <vector>

namespace map_server
{
	class Combinations
	{
	private:
		static std::map<std::pair<int, int>, std::vector<std::vector<int> > > _combinationMap;
		static std::map<std::pair<int, int>, std::vector<std::vector<int> > >::iterator getIterator(int n, int k);

	public:
		// Returns C(n, k), the number of k-combinations from a set of n elements
		// (n >= 0 and 0 <= k <= n)
		static int getCount(int n, int k);

		// Returns the (j+1)th element of the (i+1)th k-combination of the set {0, 1, ..., n - 1}
		// (0 <= i <= C(n, k) - 1 and 0 <= j <= k - 1)
		static int get(int n, int k, int i, int j);
	};
}