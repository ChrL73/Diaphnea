#pragma once

#include <vector>
#include <string>

namespace produce_questions
{
    class CompleteQuestion;
    class RelationOrderChoice;
    class MapParameters;

    class RelationOrderQuestion
    {
    private:
        const std::string _question;
        const int _choiceCount;
        const std::string _choiceListId;
        const std::string _mapId;

        std::vector<const RelationOrderChoice *> _choiceVector;

    public:
        RelationOrderQuestion(const std::string& question, int choiceCount, const std::string& choiceListId, const std::string& mapId);

        const std::string& getMapId(void) const { return _mapId; }
        CompleteQuestion *getNewQuestion(int choiceCount, double distribParameter, const MapParameters *mapParameters) const;

    };
}
