 #pragma once

 namespace map_server
 {
    class ItemLook;

    class MapItem
    {
    private:
        const int _id;
        ItemLook *_currentLook;

    protected:
        MapItem(int id) : _id(id), _currentLook(0) {}

    public:
		virtual ~MapItem() {}

        void setCurrentLook(ItemLook *look);

        int getId(void) const { return _id; }

    };
 }
