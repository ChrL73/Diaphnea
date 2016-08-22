#pragma once

#if _WIN32
#include <WinSock2.h>
#endif

#include "ProximityCriterionTypeEnum.h"
#include "mongo/client/dbclient.h"

#include <map>
#include <string>

namespace produce_questions
{
    class Level;
    class SimpleAnswerQuestion;
    class MultipleAnswerQuestion;
    class Choice;

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
        std::map<std::string, const Level *> _levelMap;
        std::map<std::pair<std::string, int>, const SimpleAnswerQuestion *> _simpleAnswerQuestionMap;
        std::map<std::pair<std::string, int>, const MultipleAnswerQuestion *> _multipleAnswerQuestionMap;
        std::map<std::pair<std::string, int>, const Choice *> _choiceMap;

    public:
        static QuizData *instance(void);
        static int destroyInstance(void);

        void setLanguageId(const std::string& languageId) { _languageId = languageId; }

        const Level *getLevel(const std::string& id);
        const SimpleAnswerQuestion *getSimpleAnswerQuestion(const std::string& questionListId, int index, ProximityCriterionTypeEnum proximityCriterionType, const std::vector<const Choice *>& choiceVector);
        const MultipleAnswerQuestion *getMultipleAnswerQuestion(const std::string& questionListId, int index, ProximityCriterionTypeEnum proximityCriterionType, const std::vector<const Choice *>& choiceVector);
        const Choice *getChoice(const std::string& choiceListId, int index, ProximityCriterionTypeEnum criterionType);
    };
}
