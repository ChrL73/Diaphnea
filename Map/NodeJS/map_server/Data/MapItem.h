 #pragma once

 #include <string>
 #include <vector>

 namespace map_server
 {
    class ItemLook;

    class MapItem
    {
    private:
        const int _id;
        const ItemLook *_currentLook;
		const ItemLook *_currentTextLook;

    protected:
        MapItem(int id, int resolutionCount);

        std::vector<std::string> _infoJsonVector;
        double _xMin, _xMax, _yMin, _yMax;

    public:
		virtual ~MapItem() {}

        void setCurrentLook(const ItemLook *look) { _currentLook = look; }
		void setCurrentTextLook(const ItemLook *look) { _currentTextLook = look; }

        int getId(void) const { return _id; }
        const ItemLook *getCurrentLook(void) const { return _currentLook; }
		const ItemLook *getCurrentTextLook(void) const { return _currentTextLook; }
        const std::string& getInfoJson(unsigned int resolutionIndex) const;
        virtual bool hasResolution(void) const = 0;

        double getXMin(void) const { return _xMin; }
        double getXMax(void) const { return _xMax; }
        double getYMin(void) const { return _yMin; }
        double getYMax(void) const { return _yMax; }
    };
 }
