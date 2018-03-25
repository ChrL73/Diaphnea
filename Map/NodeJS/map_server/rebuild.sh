#!/bin/bash -v
set -e
mkdir -p obj/Release/Data
mkdir -p obj/Release/Requests
mkdir -p obj/Release/TextDisplayer
mkdir -p bin/Release
flags='-Wall -fexceptions -O2 -std=c++11 -DBUILD_WITHOUT_PNGPP -I../../../mongo-cxx-driver/include -I. -IData -IRequests -ITextDisplayer -I../../../../freetype-2.7/include'
g++ $flags -c Data/BezierInfo.cpp -o obj/Release/Data/BezierInfo.o
g++ $flags -c Data/Category.cpp -o obj/Release/Data/Category.o
g++ $flags -c Data/Combinations.cpp -o obj/Release/Data/Combinations.o
g++ $flags -c Data/DatabaseError.cpp -o obj/Release/Data/DatabaseError.o
g++ $flags -c Data/ElementName.cpp -o obj/Release/Data/ElementName.o
g++ $flags -c Data/FilledPolygonItem.cpp -o obj/Release/Data/FilledPolygonItem.o
g++ $flags -c Data/ItemLook.cpp -o obj/Release/Data/ItemLook.o
g++ $flags -c Data/LineElement.cpp -o obj/Release/Data/LineElement.o
g++ $flags -c Data/LineItem.cpp -o obj/Release/Data/LineItem.o
g++ $flags -c Data/LineLook.cpp -o obj/Release/Data/LineLook.o
g++ $flags -c Data/Look.cpp -o obj/Release/Data/Look.o
g++ $flags -c Data/Map.cpp -o obj/Release/Data/Map.o
g++ $flags -c Data/MapData.cpp -o obj/Release/Data/MapData.o
g++ $flags -c Data/MapElement.cpp -o obj/Release/Data/MapElement.o
g++ $flags -c Data/MapItem.cpp -o obj/Release/Data/MapItem.o
g++ $flags -c Data/MultipointsItem.cpp -o obj/Release/Data/MultipointsItem.o
g++ $flags -c Data/Point.cpp -o obj/Release/Data/Point.o
g++ $flags -c Data/PointElement.cpp -o obj/Release/Data/PointElement.o
g++ $flags -c Data/PointItem.cpp -o obj/Release/Data/PointItem.o
g++ $flags -c Data/PointLook.cpp -o obj/Release/Data/PointLook.o
g++ $flags -c Data/PointVector.cpp -o obj/Release/Data/PointVector.o
g++ $flags -c Data/PolygonElement.cpp -o obj/Release/Data/PolygonElement.o
g++ $flags -c Data/PolygonLook.cpp -o obj/Release/Data/PolygonLook.o
g++ $flags -c map_server.cpp -o obj/Release/map_server.o
g++ $flags -c MapServer.cpp -o obj/Release/MapServer.o
g++ $flags -c Requests/ElementInfoRequest.cpp -o obj/Release/Requests/ElementInfoRequest.o
g++ $flags -c Requests/ElementsInfoRequest.cpp -o obj/Release/Requests/ElementsInfoRequest.o
g++ $flags -c Requests/ItemDataRequest.cpp -o obj/Release/Requests/ItemDataRequest.o
g++ $flags -c Requests/LookRequest.cpp -o obj/Release/Requests/LookRequest.o
g++ $flags -c Requests/MapIdsRequest.cpp -o obj/Release/Requests/MapIdsRequest.o
g++ $flags -c Requests/MapInfoRequest.cpp -o obj/Release/Requests/MapInfoRequest.o
g++ $flags -c Requests/RenderRequest.cpp -o obj/Release/Requests/RenderRequest.o
g++ $flags -c Requests/Request.cpp -o obj/Release/Requests/Request.o
g++ $flags -c Requests/SvgCreator.cpp -o obj/Release/Requests/SvgCreator.o
g++ $flags -c Requests/SvgCurveInfo.cpp -o obj/Release/Requests/SvgCurveInfo.o
g++ $flags -c Requests/SvgCustomColor.cpp -o obj/Release/Requests/SvgCustomColor.o
g++ $flags -c Requests/SvgInfo.cpp -o obj/Release/Requests/SvgInfo.o
g++ $flags -c Requests/SvgItemInfo.cpp -o obj/Release/Requests/SvgItemInfo.o
g++ $flags -c Requests/SvgLineInfo.cpp -o obj/Release/Requests/SvgLineInfo.o
g++ $flags -c Requests/SvgTextInfo.cpp -o obj/Release/Requests/SvgTextInfo.o
g++ $flags -c TextDisplayer/FilledPolygonItemCopy.cpp -o obj/Release/TextDisplayer/FilledPolygonItemCopy.o
g++ $flags -c TextDisplayer/Interval.cpp -o obj/Release/TextDisplayer/Interval.o
g++ $flags -c TextDisplayer/ItemCopy.cpp -o obj/Release/TextDisplayer/ItemCopy.o
g++ $flags -c TextDisplayer/ItemCopyBuilder.cpp -o obj/Release/TextDisplayer/ItemCopyBuilder.o
g++ $flags -c TextDisplayer/LineItemCopy.cpp -o obj/Release/TextDisplayer/LineItemCopy.o
g++ $flags -c TextDisplayer/PngImage.cpp -o obj/Release/TextDisplayer/PngImage.o
g++ $flags -c TextDisplayer/PointItemCopy.cpp -o obj/Release/TextDisplayer/PointItemCopy.o
g++ $flags -c TextDisplayer/Potential.cpp -o obj/Release/TextDisplayer/Potential.o
g++ $flags -c TextDisplayer/RepulsiveCenter.cpp -o obj/Release/TextDisplayer/RepulsiveCenter.o
g++ $flags -c TextDisplayer/TextDisplayer.cpp -o obj/Release/TextDisplayer/TextDisplayer.o
g++ $flags -c TextDisplayer/TextDisplayerParameters.cpp -o obj/Release/TextDisplayer/TextDisplayerParameters.o
g++ $flags -c TextDisplayer/TextInfo.cpp -o obj/Release/TextDisplayer/TextInfo.o
g++ $flags -c TextDisplayer/TextInfoLine.cpp -o obj/Release/TextDisplayer/TextInfoLine.o
g++ $flags -c TextDisplayer/TextPotential.cpp -o obj/Release/TextDisplayer/TextPotential.o
g++ $flags -c ThreadInfo.cpp -o obj/Release/ThreadInfo.o
g++ -o bin/Release/map_server obj/Release/Data/BezierInfo.o obj/Release/Data/Category.o obj/Release/Data/Combinations.o obj/Release/Data/DatabaseError.o obj/Release/Data/ElementName.o obj/Release/Data/FilledPolygonItem.o obj/Release/Data/ItemLook.o obj/Release/Data/LineElement.o obj/Release/Data/LineItem.o obj/Release/Data/LineLook.o obj/Release/Data/Look.o obj/Release/Data/Map.o obj/Release/Data/MapData.o obj/Release/Data/MapElement.o obj/Release/Data/MapItem.o obj/Release/Data/MultipointsItem.o obj/Release/Data/Point.o obj/Release/Data/PointElement.o obj/Release/Data/PointItem.o obj/Release/Data/PointLook.o obj/Release/Data/PointVector.o obj/Release/Data/PolygonElement.o obj/Release/Data/PolygonLook.o obj/Release/map_server.o obj/Release/MapServer.o obj/Release/Requests/ElementInfoRequest.o obj/Release/Requests/ElementsInfoRequest.o obj/Release/Requests/ItemDataRequest.o obj/Release/Requests/LookRequest.o obj/Release/Requests/MapIdsRequest.o obj/Release/Requests/MapInfoRequest.o obj/Release/Requests/RenderRequest.o obj/Release/Requests/Request.o obj/Release/Requests/SvgCreator.o obj/Release/Requests/SvgCurveInfo.o obj/Release/Requests/SvgCustomColor.o obj/Release/Requests/SvgInfo.o obj/Release/Requests/SvgItemInfo.o obj/Release/Requests/SvgLineInfo.o obj/Release/Requests/SvgTextInfo.o obj/Release/TextDisplayer/FilledPolygonItemCopy.o obj/Release/TextDisplayer/Interval.o obj/Release/TextDisplayer/ItemCopy.o obj/Release/TextDisplayer/ItemCopyBuilder.o obj/Release/TextDisplayer/LineItemCopy.o obj/Release/TextDisplayer/PngImage.o obj/Release/TextDisplayer/PointItemCopy.o obj/Release/TextDisplayer/Potential.o obj/Release/TextDisplayer/RepulsiveCenter.o obj/Release/TextDisplayer/TextDisplayer.o obj/Release/TextDisplayer/TextDisplayerParameters.o obj/Release/TextDisplayer/TextInfo.o obj/Release/TextDisplayer/TextInfoLine.o obj/Release/TextDisplayer/TextPotential.o obj/Release/ThreadInfo.o  -s -pthread ../../../mongo-cxx-driver/lib/libmongoclient.a -lboost_system -lboost_regex -lboost_thread -lfreetype -lssl -lcrypto
cp ./bin/Release/map_server ./../map_server.exe