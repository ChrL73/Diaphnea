 #pragma once

 namespace produce_questions
 {
    class CompleteQuestion;
    class MapParameters;

    class Category
    {
    private:
        const unsigned int _weightIndex;
        const MapParameters *_mapParameters;
        int _suitabilityCounter;

    protected:
        Category(unsigned int weightIndex, const MapParameters *mapParameters) :
            _weightIndex(weightIndex), _mapParameters(mapParameters), _suitabilityCounter(0) {}

    public:
        virtual ~Category();

        unsigned int getWeightIndex(void) const { return _weightIndex; }
        const MapParameters *getMapParameters(void) const { return _mapParameters; }
        void reduceSuitability(int i) { _suitabilityCounter += i; }
        void increaseSuitability(void) { if (_suitabilityCounter > 0) --_suitabilityCounter; }
        bool isSuitable(void);

        virtual CompleteQuestion *getNewQuestion(int choiceCount, double distribParameter) const = 0;

    };
 }
