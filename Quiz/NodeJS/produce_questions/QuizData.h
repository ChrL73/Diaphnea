#pragma once

#if _WIN32
#include <WinSock2.h>
#endif

#include "ProximityCriterionTypeEnum.h"

#ifdef __GNUC__
// Avoid this warning: ‘template<class> class std::auto_ptr’ is deprecated
#pragma GCC diagnostic ignored "-Wdeprecated-declarations"
#endif
#include "mongo/client/dbclient.h"
#ifdef __GNUC__
#pragma GCC diagnostic warning "-Wdeprecated-declarations"
#endif

#include <map>
#include <string>

namespace produce_questions
{
    class Level;
    class SimpleAnswerQuestion;
    class MultipleAnswerQuestion;
    class Choice;
    class AttributeOrderChoice;
    class RelationOrderQuestion;
    class MapParameters;
    class MapSubParameters;
    class RelationOrderChoice;

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
        std::map<std::pair<std::string, int>, const AttributeOrderChoice *> _attributeOrderChoiceMap;
        std::map<std::pair<std::string, int>, const RelationOrderQuestion *> _relationOrderQuestionMap;
        std::map<std::pair<std::string, int>, const RelationOrderChoice *> _relationOrderChoiceMap;

    public:
        static QuizData *instance(void);
        static int destroyInstance(void);

        void setLanguageId(const std::string& languageId) { _languageId = languageId; }

        const Level *getLevel(const std::string& id);
        const SimpleAnswerQuestion *getSimpleAnswerQuestion(const std::string& questionListId, int index, ProximityCriterionTypeEnum proximityCriterionType, const std::vector<const Choice *>& choiceVector);
        const MultipleAnswerQuestion *getMultipleAnswerQuestion(const std::string& questionListId, int index, ProximityCriterionTypeEnum proximityCriterionType, const std::vector<const Choice *>& choiceVector);
        const Choice *getChoice(const std::string& choiceListId, int index, ProximityCriterionTypeEnum criterionType);
        const AttributeOrderChoice *getAttributeOrderChoice(const std::string& choiceListId, int index);
        const RelationOrderQuestion *getRelationOrderQuestion(const std::string& questionListId, int index);
        const RelationOrderChoice *getRelationOrderChoice(const std::string& choiceListId, int index);
        const MapParameters *getMapParameters(mongo::BSONObj dbCategory);
        const MapSubParameters *getMapSubParameters(mongo::BSONObj dbMapParameters, const char *fieldName);
    };
}
