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

		void incrementThreadCount(void) { ++_threadCount; }
		void decrementThreadCount(void) { --_threadCount; }
		int getThreadCount(void) { return _threadCount; }

		void setStopRequested(bool stopRequested) { _stopRequested = stopRequested; }
		bool isStopRequested(void) const { return _stopRequested; }

	};
}
