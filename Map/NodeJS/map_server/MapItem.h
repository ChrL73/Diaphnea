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

    protected:
        MapItem(int id, int resolutionCount);

        std::vector<std::string> _infoJsonVector;
        double _xMin, _xMax, _yMin, _yMax;

    public:
		virtual ~MapItem() {}

        void setCurrentLook(const ItemLook *look) { _currentLook = look; }

        int getId(void) const { return _id; }
        const ItemLook *getCurrentLook(void) const { return _currentLook; }
        const std::string& getInfoJson(unsigned int resolutionIndex) const;

        double getXMin(void) const { return _xMin; }
        double getXMax(void) const { return _xMax; }
        double getYMin(void) const { return _yMin; }
        double getYMax(void) const { return _yMax; }
    };
 }
