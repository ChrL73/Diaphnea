#pragma once

#include <string>

namespace produce_questions
{
    class QuestionProducer
    {
    private:
        const std::string _levelId;
        const std::string _languageId;
		const std::string _dbHost;
		const std::string _dbName;
		const std::string _dbUser;
		const std::string _dbPassword;

    public:
        QuestionProducer(const std::string& levelId, const std::string& languageId, const std::string& dbHost,
                         const std::string& dbName, const std::string& dbUser, const std::string& dbPassword) :
            _levelId(levelId), _languageId(languageId), _dbHost(dbHost), _dbName(dbName), _dbUser(dbUser), _dbPassword(dbPassword) {}

        int run(void);
    };
}
