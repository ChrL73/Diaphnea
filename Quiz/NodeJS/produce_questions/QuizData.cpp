#include "QuizData.h"
#include "Level.h"
#include "SimpleAnswerCategory.h"
#include "SimpleAnswerQuestion.h"
#include "MultipleAnswerCategory.h"
#include "MultipleAnswerQuestion.h"
#include "Choice.h"

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
            // Todo: Handle error
            _initOk = false;
            return;
        }

        try
        {
            _connection.connect("localhost");
        }
        catch(const mongo::DBException& e)
        {
            // Todo: Handle exception
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

        mongo::Status status = mongo::client::shutdown();
        if (!status.isOK())
        {
            // Todo: Handle error
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
                const char *excudedChoice = dbQuestion.getField("excluded_choice").Obj().getStringField(_languageId);

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

                SimpleAnswerQuestion *question = new SimpleAnswerQuestion(questionText, answer, comment, excudedChoice, proximityCriterionType, doubleCriterionValue, stringCriterionValue, pointCriterionValue, choiceVector);
                it = _simpleAnswerQuestionMap.insert(std::pair<std::pair<std::string, int>, SimpleAnswerQuestion *>(key, question)).first;
            }
            else
            {
                it = _simpleAnswerQuestionMap.insert(std::pair<std::pair<std::string, int>, SimpleAnswerQuestion *>(key, 0)).first;
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
}
