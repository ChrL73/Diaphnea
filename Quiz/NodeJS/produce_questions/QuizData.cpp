#include "QuizData.h"
#include "Level.h"
#include "SimpleAnswerCategory.h"
#include "SimpleAnswerQuestion.h"
#include "MultipleAnswerCategory.h"
#include "MultipleAnswerQuestion.h"
#include "AttributeOrderCategory.h"
#include "RelationOrderCategory.h"
#include "RelationOrderQuestion.h"
#include "Choice.h"
#include "AttributeOrderChoice.h"
#include "TextAndComment.h"

namespace produce_questions
{
    QuizData *QuizData::_instance = 0;
    bool QuizData::_deleteOk = true;

    QuizData *QuizData::instance(void)
    {
        if (_instance == 0)
        {
            _instance = new QuizData();
            if (!_instance->_initOk)
            {
                delete _instance;
                _instance = 0;
            }
        }
        return _instance;
    }

    int QuizData::destroyInstance(void)
    {
        delete _instance;
        _instance = 0;
        if (!_deleteOk) return -1;
        return 0;
    }

    QuizData::QuizData(void) : _initOk(true)
    {
        mongo::Status status = mongo::client::initialize();
        if (!status.isOK())
        {
            std::cerr << "Error in mongo::client::initialize()" << std::endl;
            _initOk = false;
            return;
        }

        try
        {
            _connection.connect("localhost");
        }
        catch(const mongo::DBException& e)
        {
            std::cerr << "Exception in _connection.connect(\"localhost\")" << std::endl;
            _initOk = false;
        }
    }

    QuizData::~QuizData()
    {
        _deleteOk = true;

        std::map<std::string, const Level *>::iterator levelIt = _levelMap.begin();
        for (; levelIt != _levelMap.end(); ++levelIt) delete (*levelIt).second;

        std::map<std::pair<std::string, int>, const SimpleAnswerQuestion *>::iterator simpleAnswerQuestionIt = _simpleAnswerQuestionMap.begin();
        for (; simpleAnswerQuestionIt != _simpleAnswerQuestionMap.end(); ++simpleAnswerQuestionIt) delete (*simpleAnswerQuestionIt).second;

        std::map<std::pair<std::string, int>, const MultipleAnswerQuestion *>::iterator multipleAnswerQuestionIt = _multipleAnswerQuestionMap.begin();
        for (; multipleAnswerQuestionIt != _multipleAnswerQuestionMap.end(); ++multipleAnswerQuestionIt) delete (*multipleAnswerQuestionIt).second;

        std::map<std::pair<std::string, int>, const Choice *>::iterator choiceIt = _choiceMap.begin();
        for (; choiceIt != _choiceMap.end(); ++choiceIt) delete (*choiceIt).second;

        std::map<std::pair<std::string, int>, const AttributeOrderChoice *>::iterator attributeOrderChoiceIt = _attributeOrderChoiceMap.begin();
        for (; attributeOrderChoiceIt != _attributeOrderChoiceMap.end(); ++attributeOrderChoiceIt) delete (*attributeOrderChoiceIt).second;

        std::map<std::pair<std::string, int>, const RelationOrderQuestion *>::iterator relationOrderQuestionIt = _relationOrderQuestionMap.begin();
        for (; relationOrderQuestionIt != _relationOrderQuestionMap.end(); ++relationOrderQuestionIt) delete (*relationOrderQuestionIt).second;

        mongo::Status status = mongo::client::shutdown();
        if (!status.isOK())
        {
            std::cerr << "Error in mongo::client::shutdown()" << std::endl;
            _deleteOk = false;
        }
    }

    const Level *QuizData::getLevel(const std::string& id)
    {
        std::map<std::string, const Level *>::iterator it = _levelMap.find(id);
        if (it == _levelMap.end())
        {
            auto cursor = _connection.query("diaphnea.levels", MONGO_QUERY( "_id" << mongo::OID(id)), 1);
            if (cursor->more())
            {
                mongo::BSONObj dbLevel = cursor->next();
                int questionCount = dbLevel.getIntField("question_count");
                int choiceCount = dbLevel.getIntField("choice_count");
                int categoryCount = dbLevel.getIntField("category_count");
                int weightSum = dbLevel.getIntField("weight_sum");
                double distribParameter = dbLevel.getField("distrib_parameter").numberDouble();

                std::vector<Category *> categoryVector;
                std::vector<mongo::BSONElement> dbCategoryVector = dbLevel.getField("categories").Array();
                int i;
                for (i = 0; i < categoryCount; ++i)
                {
                    mongo::BSONObj dbCategory = dbCategoryVector[i].Obj();
                    const char *categoryType = dbCategory.getStringField("type");
                    unsigned int weightIndex = dbCategory.getIntField("weight_index");

                    if (strcmp(categoryType, "SimpleAnswer") == 0)
                    {
                        int categoryQuestionCount = dbCategory.getIntField("question_count");
                        std::string questionListId = dbCategory.getField("question_list").OID().toString();
                        int categoryChoiceCount = dbCategory.getIntField("choice_count");
                        std::string choiceListId = dbCategory.getField("choice_list").OID().toString();
                        double distribParameterCorrection = dbCategory.getField("distrib_parameter_correction").numberDouble();

                        ProximityCriterionTypeEnum proximityCriterionType = produce_questions::NONE;
                        const char *criterion = dbCategory.getStringField("proximity_criterion_type");
                        if (criterion[0] == 's') proximityCriterionType = produce_questions::STRING;
                        else if (criterion[1] == 'u') proximityCriterionType = produce_questions::NUMBER;
                        else if (criterion[0] == '3') proximityCriterionType = produce_questions::POINT_3D;

                        SimpleAnswerCategory *simpleAnswerCategory = new SimpleAnswerCategory(weightIndex, categoryQuestionCount, questionListId, categoryChoiceCount,
                                                                                              choiceListId, distribParameterCorrection, proximityCriterionType);

                        categoryVector.push_back(simpleAnswerCategory);
                    }
                    else if (strcmp(categoryType, "MultipleAnswer") == 0)
                    {
                        int categoryQuestionCount = dbCategory.getIntField("question_count");
                        std::string questionListId = dbCategory.getField("question_list").OID().toString();
                        int categoryChoiceCount = dbCategory.getIntField("choice_count");
                        std::string choiceListId = dbCategory.getField("choice_list").OID().toString();
                        double distribParameterCorrection = dbCategory.getField("distrib_parameter_correction").numberDouble();

                        ProximityCriterionTypeEnum proximityCriterionType = produce_questions::NONE;
                        const char *criterion = dbCategory.getStringField("proximity_criterion_type");
                        if (criterion[0] == '3') proximityCriterionType = produce_questions::POINT_3D;

                        MultipleAnswerCategory *multipleAnswerCategory = new MultipleAnswerCategory(weightIndex, categoryQuestionCount, questionListId, categoryChoiceCount,
                                                                                                    choiceListId, distribParameterCorrection, proximityCriterionType);

                        categoryVector.push_back(multipleAnswerCategory);
                    }
                    else if (strcmp(categoryType, "AttributeOrder") == 0)
                    {
                        const char *questionText = dbCategory.getField("question").Obj().getStringField(_languageId);
                        int categoryChoiceCount = dbCategory.getIntField("choice_count");
                        std::string choiceListId = dbCategory.getField("choice_list").OID().toString();
                        double distribParameterCorrection = dbCategory.getField("distrib_parameter_correction").numberDouble();
                        int maxIndex = dbCategory.getIntField("max_index");

                        AttributeOrderCategory *attributeOrderCategory = new AttributeOrderCategory(weightIndex, questionText, categoryChoiceCount, choiceListId, distribParameterCorrection, maxIndex);

                        categoryVector.push_back(attributeOrderCategory);
                    }
                    else if  (strcmp(categoryType, "RelationOrder") == 0)
                    {
                        int categoryQuestionCount = dbCategory.getIntField("question_count");
                        std::string questionListId = dbCategory.getField("question_list").OID().toString();
                        double distribParameterCorrection = dbCategory.getField("distrib_parameter_correction").numberDouble();

                        RelationOrderCategory *relationOrderCategory = new RelationOrderCategory(weightIndex, categoryQuestionCount, questionListId, distribParameterCorrection);

                        categoryVector.push_back(relationOrderCategory);
                    }
                }

                Level *level = new Level(questionCount, choiceCount, categoryCount, weightSum, distribParameter, categoryVector);
                it = _levelMap.insert(std::pair<std::string, Level *>(id, level)).first;
            }
            else
            {
                it = _levelMap.insert(std::pair<std::string, Level *>(id, 0)).first;
            }
        }

        return (*it).second;
    }

    const SimpleAnswerQuestion *QuizData::getSimpleAnswerQuestion(const std::string& questionListId, int index, ProximityCriterionTypeEnum proximityCriterionType, const std::vector<const Choice *>& choiceVector)
    {
        std::pair<std::string, int> key(questionListId, index);
        std::map<std::pair<std::string, int>, const SimpleAnswerQuestion *>::iterator it = _simpleAnswerQuestionMap.find(key);

        if (it == _simpleAnswerQuestionMap.end())
        {
            char projectionStr[64];
            sprintf(projectionStr, "{ questions: { $slice: [%d, 1] } }", index);
            mongo::BSONObj projection = mongo::fromjson(projectionStr);
            auto cursor = _connection.query("diaphnea.question_lists", MONGO_QUERY( "_id" << mongo::OID(questionListId)), 1, 0, &projection);

            if (cursor->more())
            {
                mongo::BSONObj dbList = cursor->next();
                mongo::BSONObj dbQuestion = dbList.getField("questions").Array()[0].Obj();

                const char *questionText = dbQuestion.getField("question").Obj().getStringField(_languageId);
                const char *answer = dbQuestion.getField("answer").Obj().getStringField(_languageId);
                const char *comment = dbQuestion.getField("comment").Obj().getStringField(_languageId);
                const char *excludedChoice = dbQuestion.getField("excluded_choice").Obj().getStringField(_languageId);
                const char *mapId = dbQuestion.getStringField("map_id");

                double doubleCriterionValue = 0.0;
                std::string stringCriterionValue;
                const Point *pointCriterionValue = 0;

                if (proximityCriterionType == produce_questions::STRING)
                {
                    stringCriterionValue = dbQuestion.getStringField("proximity_criterion_value");
                }
                else if (proximityCriterionType == produce_questions::NUMBER)
                {
                    doubleCriterionValue = dbQuestion.getField("proximity_criterion_value").Number();
                }
                else if (proximityCriterionType == produce_questions::POINT_3D)
                {
                    mongo::BSONObj criterionValue = dbQuestion.getField("proximity_criterion_value").Obj();
                    double x = criterionValue.getField("x").Number();
                    double y = criterionValue.getField("y").Number();
                    double z = criterionValue.getField("z").Number();
                    pointCriterionValue = new Point(x, y, z);
                }

                SimpleAnswerQuestion *question = new SimpleAnswerQuestion(questionText, answer, comment, mapId, excludedChoice, proximityCriterionType,
                                                                          doubleCriterionValue, stringCriterionValue, pointCriterionValue, choiceVector);
                it = _simpleAnswerQuestionMap.insert(std::pair<std::pair<std::string, int>, SimpleAnswerQuestion *>(key, question)).first;
            }
            else
            {
                it = _simpleAnswerQuestionMap.insert(std::pair<std::pair<std::string, int>, SimpleAnswerQuestion *>(key, 0)).first;
            }
        }

        return (*it).second;
    }

    const MultipleAnswerQuestion *QuizData::getMultipleAnswerQuestion(const std::string& questionListId, int index, ProximityCriterionTypeEnum proximityCriterionType, const std::vector<const Choice *>& choiceVector)
    {
        std::pair<std::string, int> key(questionListId, index);
        std::map<std::pair<std::string, int>, const MultipleAnswerQuestion *>::iterator it = _multipleAnswerQuestionMap.find(key);

        if (it == _multipleAnswerQuestionMap.end())
        {
            char projectionStr[64];
            sprintf(projectionStr, "{ questions: { $slice: [%d, 1] } }", index);
            mongo::BSONObj projection = mongo::fromjson(projectionStr);
            auto cursor = _connection.query("diaphnea.question_lists", MONGO_QUERY("_id" << mongo::OID(questionListId)), 1, 0, &projection);

            if (cursor->more())
            {
                mongo::BSONObj dbList = cursor->next();
                mongo::BSONObj dbQuestion = dbList.getField("questions").Array()[0].Obj();

                const char *questionText = dbQuestion.getField("question").Obj().getStringField(_languageId);

                std::vector<const TextAndComment *> answerVector;
                int i, answerCount = dbQuestion.getIntField("answer_count");
                for (i = 0; i < answerCount; ++i)
                {
                    mongo::BSONObj dbAnswer = dbQuestion.getField("answers").Array()[i].Obj(); // Todo: Call 'Array()' once, before the loop
                    const char *answer = dbAnswer.getField("answer").Obj().getStringField(_languageId);
                    const char *comment = dbAnswer.getField("comment").Obj().getStringField(_languageId);
                    answerVector.push_back(new TextAndComment(answer, comment));
                }

                const char *excludedChoice = dbQuestion.getField("excluded_choice").Obj().getStringField(_languageId);

                const Point *pointCriterionValue = 0;
                if (proximityCriterionType == produce_questions::POINT_3D)
                {
                    mongo::BSONObj criterionValue = dbQuestion.getField("proximity_criterion_value").Obj();
                    double x = criterionValue.getField("x").Number();
                    double y = criterionValue.getField("y").Number();
                    double z = criterionValue.getField("z").Number();
                    pointCriterionValue = new Point(x, y, z);
                }

                MultipleAnswerQuestion *question = new MultipleAnswerQuestion(questionText, answerVector, excludedChoice, proximityCriterionType, pointCriterionValue, choiceVector);
                it = _multipleAnswerQuestionMap.insert(std::pair<std::pair<std::string, int>, MultipleAnswerQuestion *>(key, question)).first;
            }
            else
            {
                it = _multipleAnswerQuestionMap.insert(std::pair<std::pair<std::string, int>, MultipleAnswerQuestion *>(key, 0)).first;
            }
        }

        return (*it).second;
    }

    const Choice *QuizData::getChoice(const std::string& choiceListId, int index, ProximityCriterionTypeEnum criterionType)
    {
        std::pair<std::string, int> key(choiceListId, index);
        std::map<std::pair<std::string, int>, const Choice *>::iterator it = _choiceMap.find(key);

        if (it == _choiceMap.end())
        {
            char projectionStr[64];
            sprintf(projectionStr, "{ choices: { $slice: [%d, 1] } }", index);
            mongo::BSONObj projection = mongo::fromjson(projectionStr);
            auto cursor = _connection.query("diaphnea.choice_lists", MONGO_QUERY( "_id" << mongo::OID(choiceListId)), 1, 0, &projection);

            if (cursor->more())
            {
                mongo::BSONObj dbList = cursor->next();
                mongo::BSONObj dbChoice = dbList.getField("choices").Array()[0].Obj();

                const char *choiceText = dbChoice.getField("choice").Obj().getStringField(_languageId);
                const char *comment = dbChoice.getField("comment").Obj().getStringField(_languageId);

                std::vector<std::string> stringCriterionVector;
                std::vector<double> doubleCriterionVector;
                std::vector<Point> pointCriterionVector;

                if (criterionType != produce_questions::NONE)
                {
                    std::vector<mongo::BSONElement> criterionVector = dbChoice.getField("proximity_criterion_values").Array();
                    int i, n = criterionVector.size();

                    for (i = 0; i < n; ++i)
                    {
                        if (criterionType == produce_questions::STRING)
                        {
                            stringCriterionVector.push_back(criterionVector[i].String());
                        }
                        else if (criterionType == produce_questions::NUMBER)
                        {
                            doubleCriterionVector.push_back(criterionVector[i].Number());
                        }
                        else if (criterionType == produce_questions::POINT_3D)
                        {
                            double x = criterionVector[i].Obj().getField("x").Number();
                            double y = criterionVector[i].Obj().getField("y").Number();
                            double z = criterionVector[i].Obj().getField("z").Number();
                            pointCriterionVector.push_back(Point(x, y, z));
                        }
                    }
                }

                Choice *choice = new Choice(choiceText, comment, doubleCriterionVector, stringCriterionVector, pointCriterionVector);
                it = _choiceMap.insert(std::pair<std::pair<std::string, int>, Choice *>(key, choice)).first;
            }
            else
            {
                it = _choiceMap.insert(std::pair<std::pair<std::string, int>, Choice *>(key, 0)).first;
            }
        }

        return (*it).second;
    }

    const AttributeOrderChoice *QuizData::getAttributeOrderChoice(const std::string& choiceListId, int index)
    {
        std::pair<std::string, int> key(choiceListId, index);
        std::map<std::pair<std::string, int>, const AttributeOrderChoice *>::iterator it = _attributeOrderChoiceMap.find(key);

        if (it == _attributeOrderChoiceMap.end())
        {
            char projectionStr[64];
            sprintf(projectionStr, "{ choices: { $slice: [%d, 1] } }", index);
            mongo::BSONObj projection = mongo::fromjson(projectionStr);
            auto cursor = _connection.query("diaphnea.choice_lists", MONGO_QUERY("_id" << mongo::OID(choiceListId)), 1, 0, &projection);

            if (cursor->more())
            {
                mongo::BSONObj dbList = cursor->next();
                mongo::BSONObj dbChoice = dbList.getField("choices").Array()[0].Obj();

                const char *choiceText = dbChoice.getField("choice").Obj().getStringField(_languageId);
                const char *comment = dbChoice.getStringField("comment");
                int minIndex = dbChoice.getIntField("min_index");

                AttributeOrderChoice *choice = new AttributeOrderChoice(choiceText, comment, minIndex);
                it = _attributeOrderChoiceMap.insert(std::pair<std::pair<std::string, int>, AttributeOrderChoice *>(key, choice)).first;
            }
            else
            {
                it = _attributeOrderChoiceMap.insert(std::pair<std::pair<std::string, int>, AttributeOrderChoice *>(key, 0)).first;
            }
        }

        return (*it).second;
    }

    const RelationOrderQuestion *QuizData::getRelationOrderQuestion(const std::string& questionListId, int index)
    {
        std::pair<std::string, int> key(questionListId, index);
        std::map<std::pair<std::string, int>, const RelationOrderQuestion *>::iterator it = _relationOrderQuestionMap.find(key);

        if (it == _relationOrderQuestionMap.end())
        {
            char projectionStr[64];
            sprintf(projectionStr, "{ questions: { $slice: [%d, 1] } }", index);
            mongo::BSONObj projection = mongo::fromjson(projectionStr);
            auto cursor = _connection.query("diaphnea.question_lists", MONGO_QUERY( "_id" << mongo::OID(questionListId)), 1, 0, &projection);

            if (cursor->more())
            {
                mongo::BSONObj dbList = cursor->next();
                mongo::BSONObj dbQuestion = dbList.getField("questions").Array()[0].Obj();

                const char *questionText = dbQuestion.getField("question").Obj().getStringField(_languageId);
                int choiceCount = dbQuestion.getIntField("choice_count");
                std::string choiceListId = dbQuestion.getField("choice_list").OID().toString();

                RelationOrderQuestion *question = new RelationOrderQuestion(questionText, choiceCount, choiceListId);
                it = _relationOrderQuestionMap.insert(std::pair<std::pair<std::string, int>, RelationOrderQuestion *>(key, question)).first;
            }
            else
            {
                it = _relationOrderQuestionMap.insert(std::pair<std::pair<std::string, int>, RelationOrderQuestion *>(key, 0)).first;
            }
        }

        return (*it).second;
    }

    const std::string& QuizData::getRelationOrderChoice(const std::string& choiceListId, int index)
    {
        std::pair<std::string, int> key(choiceListId, index);
        std::map<std::pair<std::string, int>, std::string>::iterator it = _relationOrderChoiceMap.find(key);

        if (it == _relationOrderChoiceMap.end())
        {
            char projectionStr[64];
            sprintf(projectionStr, "{ choices: { $slice: [%d, 1] } }", index);
            mongo::BSONObj projection = mongo::fromjson(projectionStr);
            auto cursor = _connection.query("diaphnea.choice_lists", MONGO_QUERY( "_id" << mongo::OID(choiceListId)), 1, 0, &projection);

            if (cursor->more())
            {
                mongo::BSONObj dbList = cursor->next();
                mongo::BSONObj dbChoice = dbList.getField("choices").Array()[0].Obj();
                const char *choiceText = dbChoice.getField("choice").Obj().getStringField(_languageId);

                it = _relationOrderChoiceMap.insert(std::pair<std::pair<std::string, int>, std::string>(key, choiceText)).first;
            }
            else
            {
                it = _relationOrderChoiceMap.insert(std::pair<std::pair<std::string, int>, std::string>(key, std::string())).first;
            }
        }

        return (*it).second;
    }
}
