#include "QuizData.h"
#include "Level.h"

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

        std::map<std::string, Level *>::iterator levelIt = _levelMap.begin();
        for (; levelIt != _levelMap.end(); ++levelIt) delete (*levelIt).second;

        mongo::Status status = mongo::client::shutdown();
        if (!status.isOK())
        {
            // Todo: Handle error
            _deleteOk = false;
        }
    }

    Level *QuizData::getLevel(const std::string& id)
    {
        std::map<std::string, Level *>::iterator it = _levelMap.find(id);
        if (it == _levelMap.end())
        {
            auto cursor = _connection.query("diaphnea.levels", MONGO_QUERY( "_id" << mongo::OID(id)), 1);
            while (cursor->more())
            {
                mongo::BSONObj dbLevel = cursor->next();
                std::cout << dbLevel.toString() << std::endl;
            }

            it = _levelMap.insert(std::pair<std::string, Level *>(id, new Level())).first;
        }


        return (*it).second;
    }
}
