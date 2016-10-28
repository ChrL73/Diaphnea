 #pragma once

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

    public:
		virtual ~MapItem() {}

        void setCurrentLook(const ItemLook *look) { _currentLook = look; }

        int getId(void) const { return _id; }
        const ItemLook *getCurrentLook(void) { return _currentLook; }

    };
 }
