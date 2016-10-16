#pragma once

#include <vector>
#include <mutex>

namespace map_server
{
    class Request
    {
    protected:
        Request(const char *socketId, const char *requestId) :
            _socketId(socketId), _requestId(requestId) {}

        static std::mutex *_coutMutexPtr;
        const char * const _socketId;
        const char * const _requestId;

    public:
        static void setCoutMutex(std::mutex *coutMutexPtr) { _coutMutexPtr = coutMutexPtr; }
        static Request *createRequest(std::vector<const char *> tokenVector);
        virtual ~Request() {}
        virtual void execute(void) = 0;
    };
 }
