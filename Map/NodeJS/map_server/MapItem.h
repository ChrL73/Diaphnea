 #pragma once

 #include <string>

 namespace map_server
 {
    class ItemLook;

    class MapItem
    {
    private:
        const int _id;
        const ItemLook *_currentLook;

    protected:
        MapItem(int id) : _id(id), _currentLook(0) {}

        std::string _infoJson;

    public:
		virtual ~MapItem() {}

        void setCurrentLook(const ItemLook *look) { _currentLook = look; }

        int getId(void) const { return _id; }
        const ItemLook *getCurrentLook(void) const { return _currentLook; }
        const std::string& getInfoJson(void) const { return _infoJson; }

    };
 }
