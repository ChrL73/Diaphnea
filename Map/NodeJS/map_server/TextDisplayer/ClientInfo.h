#pragma once

namespace map_server
{
	class ClientInfo
	{
	private:
		bool _stopRequested;
		int _threadCount;

	public:
		ClientInfo() : _stopRequested(false), _threadCount(0) {}

		void incrementThreadCount() { ++_threadCount; }
		void decrementThreadCount() { ++_threadCount; }
		void setStopRequested(bool stopRequested) { _stopRequested = stopRequested; }

	};
}
