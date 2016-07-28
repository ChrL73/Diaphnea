 #pragma once

 namespace produce_questions
 {
    class CompleteQuestion;

    class Category
    {
    private:
        const unsigned int _weightIndex;
        int _suitabilityCounter;

    protected:
        Category(unsigned int weightIndex) : _weightIndex(weightIndex), _suitabilityCounter(0) {}

    public:
        virtual ~Category() {}

        unsigned int getWeightIndex(void) const { return _weightIndex; }
        void reduceSuitability(int i) { _suitabilityCounter += i; }
        void increaseSuitability(void) { if (_suitabilityCounter > 0) --_suitabilityCounter; }
        bool isSuitable(void);

        virtual const CompleteQuestion *getNewQuestion(int choiceCount, double distribParameter) const = 0;

    };
 }
