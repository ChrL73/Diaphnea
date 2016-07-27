#pragma once

#include "mongo/client/dbclient.h"

#include <map>
#include <string>

namespace produce_questions
{
    class Level;

    class QuizData
    {
    private:
        static QuizData *_instance;
        QuizData(void);
        ~QuizData();

        int initialize(void);
        int terminate(void);
        bool _initOk;
        static bool _deleteOk;

        mongo::DBClientConnection _connection;
        std::map<std::string, Level *> _levelMap;
        void clear(void);

    public:
        static QuizData *instance(void);
        static int destroyInstance(void);

        Level *getLevel(const std::string& id);
    };
}
