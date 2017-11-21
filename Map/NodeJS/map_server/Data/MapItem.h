 #pragma once

 #include <string>
 #include <vector>
 #include <map>

 namespace map_server
 {
    class ItemLook;
    class ElementName;
    class Category;

    class MapItem
    {
    private:
        const int _id;
        std::vector<const ItemLook *> _currentLooks;
		std::vector<const ItemLook *> _currentTextLooks;
		const Category *_currentCategory;
        std::map<std::string, std::vector<ElementName *> > *_nameMapPtr;
        std::string _elementIdForText;
        double _importance;
        const std::string _emptyString;

    protected:
        MapItem(int id, int resolutionCount);

        std::vector<std::string> _infoJsonVector;
        double _xMin, _xMax, _yMin, _yMax;

    public:
		virtual ~MapItem() {}

        void setCurrentLooks(const std::vector<const ItemLook *>& looks) { _currentLooks = looks; }
		void setCurrentTextLooks(const std::vector<const ItemLook *>& looks) { _currentTextLooks = looks; }
		void setCurrentCategory(const Category *category) { _currentCategory = category; }

		void setNameMap(std::map<std::string, std::vector<ElementName *> > *nameMapPtr) { _nameMapPtr = nameMapPtr; }
		void setElementIdForText(const std::string& elementId) { _elementIdForText = elementId; }
		void setImportance(double importance) { _importance = importance; }
		int getTextCount(const std::string& languageId) const;
		const ElementName *getText(const std::string& languageId, int i) const;
		const std::string& getElementIdForText(void) const { return _elementIdForText; }
		double getImportance(void) const { return _importance; }

        int getId(void) const { return _id; }
        const ItemLook *getCurrentLook(int i) const;
		const ItemLook *getCurrentTextLook(int i) const;
		const Category *getCurrentCategory(void) const { return _currentCategory; }
        const std::string& getInfoJson(unsigned int resolutionIndex) const;
        virtual bool hasResolution(void) const = 0;

        double getXMin(void) const { return _xMin; }
        double getXMax(void) const { return _xMax; }
        double getYMin(void) const { return _yMin; }
        double getYMax(void) const { return _yMax; }
    };
 }
