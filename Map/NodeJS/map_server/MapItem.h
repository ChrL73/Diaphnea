 #pragma once

 namespace map_server
 {
    class MapItem
    {
    private:
        const int _id;

    protected:
        MapItem(int id) : _id(id) {}

    public:
		virtual ~MapItem() {}

        int getId(void) const { return _id; }

    };
 }
