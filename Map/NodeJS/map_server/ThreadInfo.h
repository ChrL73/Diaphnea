#pragma once

#include <thread>
#include <future>
#include <string>

namespace map_server
{
    class ThreadInfo
    {
    private:
        std::thread _t;
        std::future<void> _f;
        std::string _str;

    public:
        ThreadInfo(std::thread&& t, std::future<void>&& f, const std::string&& str) :
            _t(std::move(t)), _f(std::move(f)), _str(std::move(str)) {}

        std::thread& getT(void) { return _t; }
        const std::future<void>& getF(void) const { return _f; }
        const std::string& getStr(void) const { return _str; }
    };
}
