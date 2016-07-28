#pragma once

#include "mongo/client/dbclient.h"

#include <map>
#include <string>

namespace produce_questions
{
    class Level;
    class SimpleAnswerQuestion;

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

        std::string _languageId;
        std::map<std::string, Level *> _levelMap;
        std::map<std::pair<std::string, int>, SimpleAnswerQuestion *> _simpleAnswerQuestionMap;

    public:
        static QuizData *instance(void);
        static int destroyInstance(void);

        void setLanguageId(const std::string& languageId) { _languageId = languageId; }

        Level *getLevel(const std::string& id);
        SimpleAnswerQuestion *getSimpleAnswerQuestion(const std::string& questionListId, int index);
    };
}
