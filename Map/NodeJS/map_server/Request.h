 #pragma once

 #include <vector>

 namespace map_server
 {
    class Request
    {
    private:
        const char * const _socketId;
        const char * const _requestId;

    protected:
        Request(const char *socketId, const char *requestId) :
            _socketId(socketId), _requestId(requestId) {}

    public:
        static Request *createRequest(std::vector<const char *> tokenVector);

        virtual ~Request() {}

        virtual void execute(void) = 0;
    };
 }
