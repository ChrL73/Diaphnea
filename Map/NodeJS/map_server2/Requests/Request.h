#pragma once

#include <vector>
#include <mutex>

namespace map_server
{
    //class Map;
    class CommonData;

    class Request
    {
    protected:
        Request(const char *socketId, const char *requestId) : _socketId(socketId), _requestId(requestId) {}

        static std::mutex *_coutMutexPtr;
        const char * const _socketId;
        const char * const _requestId;
        //void flushErrors(Map *map);

    public:
        static void setCoutMutex(std::mutex *coutMutexPtr) { _coutMutexPtr = coutMutexPtr; }
        static Request *createRequest(CommonData *commonData, std::vector<const char *> tokenVector);
        virtual ~Request() {}
        virtual void execute(void) = 0;
    };
 }
