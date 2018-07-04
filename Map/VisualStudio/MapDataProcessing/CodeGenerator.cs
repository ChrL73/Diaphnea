/* --------------------------------------------------------------------
 *
 * Copyright (C) 2018
 *
 * This file is part of the Diaphnea project.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License
 * version 3 as published by the Free Software Foundation
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 * -------------------------------------------------------------------- */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;
using Schemas;

namespace MapDataProcessing
{
    class CodeGenerator
    {
        private readonly string _dirName;
        private readonly MapData _mapData;

        private readonly Dictionary<string, StreamWriter> _streamWriterDictionary = new Dictionary<string, StreamWriter>();

        private int _currentStringOffset;
        private int _currentIntArrayOffset;
        //private int _currentDoubleArrayOffset;
        private int _currentPointElementOffset;
        private int _currentLineElementOffset;
        private int _currentPolygonElementOffset;
        private int _currentMultipointItemOffset;
        private int _currentPointLookOffset;
        private int _currentLineLookOffset;
        private int _currentPolygonLookOffset;
        private int _currentNameTranslationOffset;
        private int _currentElementNameOffset;
        private int _currentPointListOffset;

        private int _pointElementCount;
        private int _lineElementCount;
        private int _polygonElementCount;

        private readonly Dictionary<string, int> _stringDictionary = new Dictionary<string, int>();
        private readonly string[] _itemDataArray;
        private readonly CultureInfo _cultureInfo = CultureInfo.CreateSpecificCulture("en-US");

        internal CodeGenerator(string dirName, MapData mapData)
        {
            if (!Directory.Exists(dirName)) Directory.CreateDirectory(dirName);

            _dirName = dirName;
            _mapData = mapData;

            DirectoryInfo dirInfo = new DirectoryInfo(dirName);
            foreach(FileInfo file in dirInfo.GetFiles()) file.Delete();

            _currentStringOffset = 1;
            _currentIntArrayOffset = 1;
            //_currentDoubleArrayOffset = 0;
            _currentPointElementOffset = 0;
            _currentLineElementOffset = 0;
            _currentPolygonElementOffset = 0;
            _currentMultipointItemOffset = 0;
            _currentPointLookOffset = 0;
            _currentLineLookOffset = 0;
            _currentPolygonLookOffset = 0;
            _currentNameTranslationOffset = 0;
            _currentElementNameOffset = 0;
            _currentPointListOffset = 0;

            _pointElementCount = 0;
            _lineElementCount = 0;
            _polygonElementCount = 0;

            _stringDictionary.Add("", 0);

            _itemDataArray = new string[(ItemId.MaxValue + 1) * mapData.XmlMapData.resolutionList.Length];

            string path = String.Format("{0}/Strings.cpp", dirName);
            if (File.Exists(path)) File.Delete(path);
            append("Strings.cpp", "namespace map_server\n{\nunsigned char strings[] =\n{\n// 0: Empty string\n0");

            path = String.Format("{0}/IntArrays.cpp", dirName);
            if (File.Exists(path)) File.Delete(path);
            append("IntArrays.cpp", "namespace map_server\n{\nint intArrays[] =\n{\n0");

            /*path = String.Format("{0}/DoubleArrays.cpp", dirName);
            if (File.Exists(path)) File.Delete(path);
            append("DoubleArrays.cpp", "namespace map_server\n{\ndouble doubleArrays[] =\n{");*/

            path = String.Format("{0}/PointElements.cpp", dirName);
            if (File.Exists(path)) File.Delete(path);
            append("PointElements.cpp", "namespace map_server\n{\nint pointElements[] =\n{");

            path = String.Format("{0}/LineElements.cpp", dirName);
            if (File.Exists(path)) File.Delete(path);
            append("LineElements.cpp", "namespace map_server\n{\nint lineElements[] =\n{");

            path = String.Format("{0}/PolygonElements.cpp", dirName);
            if (File.Exists(path)) File.Delete(path);
            append("PolygonElements.cpp", "namespace map_server\n{\nint polygonElements[] =\n{");

            path = String.Format("{0}/MultipointItems.cpp", dirName);
            if (File.Exists(path)) File.Delete(path);
            append("MultipointItems.cpp", "namespace map_server\n{\nint multipointItems[] =\n{");

            path = String.Format("{0}/PointLooks.cpp", dirName);
            if (File.Exists(path)) File.Delete(path);
            append("PointLooks.cpp", "namespace map_server\n{\nint pointLooks[] =\n{");

            path = String.Format("{0}/LineLooks.cpp", dirName);
            if (File.Exists(path)) File.Delete(path);
            append("LineLooks.cpp", "namespace map_server\n{\nint lineLooks[] =\n{");

            path = String.Format("{0}/PolygonLooks.cpp", dirName);
            if (File.Exists(path)) File.Delete(path);
            append("PolygonLooks.cpp", "namespace map_server\n{\nint polygonLooks[] =\n{");

            path = String.Format("{0}/NameTranslations.cpp", dirName);
            if (File.Exists(path)) File.Delete(path);
            append("NameTranslations.cpp", "namespace map_server\n{\nint nameTranslations[] =\n{");

            path = String.Format("{0}/ElementNames.cpp", dirName);
            if (File.Exists(path)) File.Delete(path);
            append("ElementNames.cpp", "namespace map_server\n{\nint elementNames[] =\n{");

            path = String.Format("{0}/PointLists.cpp", dirName);
            if (File.Exists(path)) File.Delete(path);
            append("PointLists.cpp", "#include \"PointLists.h\"\n\nnamespace map_server\n{\n\ntypedef struct { int a; int b; double *c; } pList;\n\npList pointLists[] =\n{");

            path = String.Format("{0}/PointLists.h", dirName);
            if (File.Exists(path)) File.Delete(path);
            append("PointLists.h", "#pragma once\n\nnamespace map_server\n{\n");

            path = String.Format("{0}/ItemData.cpp", dirName);
            if (File.Exists(path)) File.Delete(path);
             append("ItemData.cpp", "namespace map_server\n{\n");
        }

        internal void close(string mapInfo)
        {
            string path = String.Format("{0}/MapData.cpp", _dirName);
            if (File.Exists(path)) File.Delete(path);

            string code = String.Format(
                "namespace map_server\n{{\n    int pointElementCount = {0};\n    int lineElementCount = {1};\n    int polygonElementCount = {2};\n",
                _pointElementCount, _lineElementCount, _polygonElementCount);
            append("MapData.cpp", code);

            int mapInfoOffset = getStringOffset(mapInfo);
            XmlParameters parameters = _mapData.XmlMapData.parameters;
            code = String.Format(
                "    double zoomMinDistance = {0};\n    double zoomMaxDistance = {1};\n    double resolutionThreshold = {2};\n    double sizeParameter1 = {3};\n    double sizeParameter2 = {4};\n    int mapInfo = {5};\n",
                parameters.zoomMinDistance.ToString(_cultureInfo), parameters.zoomMaxDistance.ToString(_cultureInfo),
                parameters.resolutionThreshold.ToString(_cultureInfo), parameters.sizeParameter1.ToString(_cultureInfo),
                parameters.sizeParameter2.ToString(_cultureInfo), mapInfoOffset);
            append("MapData.cpp", code);

            List<string> sampleLengthList = new List<string>();
            int i, n = _mapData.XmlMapData.resolutionList.Length;
            for (i = 0; i < n; ++i)
            {
                XmlResolution resolution = _mapData.XmlMapData.resolutionList[i];
                double sampleLength = resolution.sampleLength1 * Double.Parse(resolution.sampleRatio);
                sampleLengthList.Add(sampleLength.ToString(_cultureInfo));
            }
            code = String.Format("    int sampleLengthCount = {0};\n    double sampleLengths[] = {{{1}}};\n", n, String.Join(", ", sampleLengthList));
            append("MapData.cpp", code);

            List<int> languageIdOffsetList = new List<int>();
            foreach (XmlLanguage language in _mapData.XmlMapData.parameters.languageList)
            {
                languageIdOffsetList.Add(getStringOffset(language.id.ToString()));
            }
            code = String.Format("    int languageCount = {0};\n    int languageIds[] = {{{1}}};\n    int itemCount = {2};\n}}\n",
                                 _mapData.XmlMapData.parameters.languageList.Count(), String.Join(", ", languageIdOffsetList), ItemId.MaxValue + 1);
            append("MapData.cpp", code);

            append("ItemData.cpp", "unsigned char *itemData[] = {\n");
            int j;
            for (j = 0; j <= ItemId.MaxValue; ++j)
            {
                List<string> data = new List<string>();
                for (i = 0; i < n; ++i)
                {
                    data.Add(_itemDataArray[j * n + i]);
                }
                append("ItemData.cpp", String.Join(",", data));
                if (j != ItemId.MaxValue) append("ItemData.cpp", ",");
                append("ItemData.cpp", "\n");
            }
            append("ItemData.cpp", "};\n");

            append("Strings.cpp", "\n};\n}\n");
            append("IntArrays.cpp", "\n};\n}\n");
            //append("DoubleArrays.cpp", "\n};\n}\n");
            append("PointElements.cpp", "\n};\n}\n");
            append("LineElements.cpp", "\n};\n}\n");
            append("PolygonElements.cpp", "\n};\n}\n");
            append("MultipointItems.cpp", "\n};\n}\n");
            append("PointLooks.cpp", "\n};\n}\n");
            append("LineLooks.cpp", "\n};\n}\n");
            append("PolygonLooks.cpp", "\n};\n}\n");
            append("NameTranslations.cpp", "\n};\n}\n");
            append("ElementNames.cpp", "\n};\n}\n");
            append("PointLists.cpp", "\n};\n}\n");
            append("PointLists.h", "\n}\n");
            append("ItemData.cpp", "\n}\n");

            foreach (StreamWriter file in _streamWriterDictionary.Values) file.Close();
        }

        private int getStringOffset(string str)
        {
            int offset;
            if (!_stringDictionary.TryGetValue(str, out offset))
            {
                offset = _currentStringOffset;
                _stringDictionary.Add(str, _currentStringOffset);

                string code = String.Format(",\n// {0} \"{1}\"\n", _currentStringOffset, str);

                byte[] bytes = Encoding.UTF8.GetBytes(str);
                code += String.Join(",", bytes);
                code += ",0";

                _currentStringOffset += bytes.Count() + 1;

                append("Strings.cpp", code);
            }

            return offset;
        }

        internal void addPointElement(PointMapElement element, XmlProjectionEnum projection)
        {
            int idOffset = getStringOffset(element.Id);
            int itemId = element.ItemId.Value;

            double x = 0.0, y = 0.0;
            element.Point.getProjection(projection, out x, out y);
            y = -y;
            int[] xInt = doubleToIntArray(x);
            int[] yInt = doubleToIntArray(y);

            int[] importance = doubleToIntArray(element.Importance);

            List<int> lookOffsetList = new List<int>();
            foreach (Look look in element.Looks) lookOffsetList.Add(look.CppOffset);
            int looksOffset = 0;
            if (lookOffsetList.Count() != 0) looksOffset = getIntArrayOffset(lookOffsetList);

            int framingLevel = element.Category.XmlCategory.framingLevel;
                
            int translationsOffset = addElementName(element.NameDictionary);
            int infoJsonOffset = getStringOffset(element.getInfoJson());

            string code = String.Format("{0}\n// {1} \"{2}\", ...\n{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15}",
                _currentPointElementOffset == 0 ? "" : ",", _currentPointElementOffset, element.Id,
                idOffset, itemId, xInt[0], xInt[1], yInt[0], yInt[1], importance[0], importance[1], lookOffsetList.Count(), looksOffset,
                framingLevel, translationsOffset, infoJsonOffset);

            append("PointElements.cpp", code);
            _currentPointElementOffset += 13;
            ++_pointElementCount;

            string infoJson = String.Format("{{\"type\":\"point\",\"x\":{0},\"y\":{1}}}", x.ToString("G6", _cultureInfo), y.ToString("G6", _cultureInfo));
            byte[] bytes = Encoding.UTF8.GetBytes(infoJson);
            append("ItemData.cpp", String.Format("unsigned char _{0}__j[] = {{{1},0}};\n", element.Id, String.Join(",", bytes)));

            int i, n = _mapData.XmlMapData.resolutionList.Length;
            for (i = 0; i < n; ++i) _itemDataArray[itemId * n + i] = String.Format("_{0}__j", element.Id);
        }

        internal void addLineElement(LineMapElement element)
        {
            int idOffset = getStringOffset(element.Id);

            List<int> itemOffsetList = new List<int>();
            foreach (LineLinePart part in element.PartList) itemOffsetList.Add(part.MapItemCppOffset);
            int itemsOffset = 0;
            if (itemOffsetList.Count() != 0) itemsOffset = getIntArrayOffset(itemOffsetList);

            List<int> lookOffsetList = new List<int>();
            foreach (Look look in element.Looks) lookOffsetList.Add(look.CppOffset);
            int looksOffset = 0;
            if (lookOffsetList.Count() != 0) looksOffset = getIntArrayOffset(lookOffsetList);

            int framingLevel = element.Category.XmlCategory.framingLevel;

            int[] importance = doubleToIntArray(element.Importance);

            int translationsOffset = addElementName(element.NameDictionary);
            int infoJsonOffset = getStringOffset(element.getInfoJson());

            string code = String.Format("{0}\n// {1} \"{2}\", ...\n{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}",
                _currentLineElementOffset == 0 ? "" : ",", _currentLineElementOffset, element.Id,
                idOffset, itemOffsetList.Count(), itemsOffset, lookOffsetList.Count(), looksOffset,
                framingLevel, importance[0], importance[1], translationsOffset, infoJsonOffset);

            append("LineElements.cpp", code);
            _currentLineElementOffset += 10;
            ++_lineElementCount;
        }

        internal void addPolygonElement(PolygonMapElement element)
        {
            List<int> itemOffsetList = new List<int>();
            foreach (OrientedLineList list in element.CompoundPolygonList)
            {
                foreach (OrientedPolygonLinePart linePart in list.LineList)
                {
                    itemOffsetList.Add(linePart.MapItemCppOffset);
                }
            }
            foreach (PolygonPolygonPart part in element.PolygonPartList)
            {
                itemOffsetList.Add(part.MapItemCppOffset);
            }

            int idOffset = getStringOffset(element.Id);

            int itemsOffset = 0;
            if (itemOffsetList.Count() != 0)
            {
                itemsOffset = getIntArrayOffset(itemOffsetList);
            }

            int coveredElementsOffset = 0;
            if (element.CoveredElementList.Count() != 0)
            {
                List<int> stringOffsets = new List<int>();
                foreach (string str in element.CoveredElementList) stringOffsets.Add(getStringOffset(str));
                coveredElementsOffset = getIntArrayOffset(stringOffsets);
            }

            int[] importance = doubleToIntArray(element.Importance);

            List<int> lookOffsetList = new List<int>();
            foreach (Look look in element.Looks) lookOffsetList.Add(look.CppOffset);
            int looksOffset = 0;
            if (lookOffsetList.Count() != 0) looksOffset = getIntArrayOffset(lookOffsetList);

            int framingLevel = element.Category.XmlCategory.framingLevel;

            int translationsOffset = addElementName(element.NameDictionary);
            int infoJsonOffset = getStringOffset(element.getInfoJson());

            string code = String.Format("{0}\n// {1} \"{2}\", ...\n{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15}",
                _currentPolygonElementOffset == 0 ? "" : ",", _currentPolygonElementOffset, element.Id,
                idOffset, element.ContourMapItem.CppOffset, itemOffsetList.Count(), itemsOffset,
                element.CoveredElementList.Count(), coveredElementsOffset, importance[0], importance[1],
                lookOffsetList.Count(), looksOffset, framingLevel, translationsOffset, infoJsonOffset);

            append("PolygonElements.cpp", code);
            _currentPolygonElementOffset += 13;
            ++_polygonElementCount;
        }

        private int addElementName(Dictionary<string, List<List<string>>> nameDictionary)
        {
            List<int> translationOffset = new List<int>();

            int i, n = _mapData.XmlMapData.parameters.languageList.Count();
            for (i = 0; i < n; ++i)
            {
                string languageId = _mapData.XmlMapData.parameters.languageList[i].id.ToString();

                if (nameDictionary.ContainsKey(languageId))
                {
                    List<List<string>> translation = nameDictionary[languageId];
                    List<int> nameOffset = new List<int>();

                    foreach (List<string> name in translation)
                    {
                        List<int> lineOffset = new List<int>();
                        foreach (string line in name)
                        {
                            lineOffset.Add(getStringOffset(line));
                        }

                        int linesOffset = 0;
                        if (lineOffset.Count() != 0) linesOffset = getIntArrayOffset(lineOffset);

                        string code1 = String.Format("{0}\n// {1}\n{2},{3}", _currentElementNameOffset == 0 ? "" : ",", _currentElementNameOffset, lineOffset.Count(), linesOffset);
                        append("ElementNames.cpp", code1);
                        nameOffset.Add(_currentElementNameOffset);
                        _currentElementNameOffset += 2;
                    }

                    int namesOffset = 0;
                    if (nameOffset.Count() != 0) namesOffset = getIntArrayOffset(nameOffset);

                    string code2 = String.Format("{0}\n// {1}\n{2},{3}", _currentNameTranslationOffset == 0 ? "" : ",", _currentNameTranslationOffset, nameOffset.Count(), namesOffset);
                    append("NameTranslations.cpp", code2);
                    translationOffset.Add(_currentNameTranslationOffset);
                    _currentNameTranslationOffset += 2;
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            if (translationOffset.Count() != 0) return getIntArrayOffset(translationOffset);
            return 0;
        }

        internal int addMultipointItem(double xMin, double xMax, double yMin, double yMax, int itemId, string itemName, List<List<double>> lineList, bool cap1Round, bool cap2Round, string type)
        {
            int offset = _currentMultipointItemOffset;

            int[] xMinInt = doubleToIntArray(xMin);
            int[] xMaxInt = doubleToIntArray(xMax);
            int[] yMinInt = doubleToIntArray(-yMax);
            int[] yMaxInt = doubleToIntArray(-yMin);

            int i = 0;
            List<int> pointListOffsets = new List<int>();
            StringBuilder fileContent = new StringBuilder();
            fileContent.Append("namespace map_server\n{\n");
            foreach (List<double> pointList in lineList)
            {
                pointListOffsets.Add(addPointList(pointList, itemName, itemId, fileContent, i, type));
                ++i;
            }
            fileContent.Append("\n}\n");
            writeAndClose(String.Format("_{0}.cpp", itemName.Replace("#", "_").Replace("!", "_").Replace("~", "_")), fileContent.ToString());

            int pointArrayOffset = getIntArrayOffset(pointListOffsets);

            string code = String.Format("{0}\n// {1} ({2})\n{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14}",
                offset == 0 ? "" : ",", offset, itemName,
                xMinInt[0], xMinInt[1], xMaxInt[0], xMaxInt[1], yMinInt[0], yMinInt[1], yMaxInt[0], yMaxInt[1], itemId, pointArrayOffset,
                cap1Round ? 1 : 0, cap2Round ? 1 : 0);

            append("MultipointItems.cpp", code);
            _currentMultipointItemOffset += 12;

            return offset;
        }

        private int addPointList(List<double> pointList, string itemName, int itemId, StringBuilder fileContent, int i, string type)
        {
            // Todo: Return error if several 'itemCppId' are identical
            // Todo: Handle other incorrect chars than '_', '!' and '~' or return error if 'itemName' contains such chars
            string itemCppId = String.Format("_{0}_{1}", itemName.Replace("#", "_").Replace("!", "_").Replace("~", "_"), i);

            append("PointLists.h", String.Format("extern double {0}[];\n", itemCppId));

            int offset = _currentPointListOffset;

            //int arrayOffset = getDoubleArrayOffset(pointList);
            fileContent.Append(String.Format("double {0}[] =\n{{{1}}};\n",
                itemCppId, String.Join(",", pointList.Select(v => v.ToString("G", _cultureInfo)))));

            List<string> pointJsonList = new List<string>();
            int j, n = pointList.Count() / 6;
            for (j = 0; j < n; ++j)
            {
                double x = pointList[j * 6 + 4];
                double y = pointList[j * 6 + 5];
                string pointJson = String.Format("{{\"x\":{0},\"y\":{1}}}", x.ToString("G6", _cultureInfo), y.ToString("G6", _cultureInfo));
                pointJsonList.Add(pointJson);
            }
            string infoJson = String.Format("{{\"type\":\"{0}\",\"points\":[{1}]}}", type, String.Join(",", pointJsonList));

            byte[] bytes = Encoding.UTF8.GetBytes(infoJson);
            fileContent.Append(String.Format("unsigned char {0}__j[] = {{{1},0}};\n", itemCppId, String.Join(",", bytes)));

            append("ItemData.cpp", String.Format("extern unsigned char {0}__j[];\n", itemCppId));
            _itemDataArray[itemId * _mapData.XmlMapData.resolutionList.Length + i] = String.Format("{0}__j", itemCppId);

            string code = String.Format("{0}\n// {1}\n{2},0,{3}",
                offset == 0 ? "" : ",", offset, pointList.Count() / 6, itemCppId);

            append("PointLists.cpp", code);
            _currentPointListOffset += 1;

            return offset;
        }

        internal int addPointLook(int lookId, XmlPointLook look)
        {
            int offset = _currentPointLookOffset;

            int[] pointSize = doubleToIntArray(look.pointSize);
            int[] textSize = doubleToIntArray(look.textSize);
            int[] pointOpacity = doubleToIntArray((double)look.pointAlpha / 255.0);
            int[] textOpacity = doubleToIntArray((double)look.textAlpha / 255.0);
            int pointColorOffset = getStringOffset(String.Format("#{0:x2}{1:x2}{2:x2}", look.pointRed, look.pointGreen, look.pointBlue));
            int textColorOffset = getStringOffset(String.Format("#{0:x2}{1:x2}{2:x2}", look.textRed, look.textGreen, look.textBlue));

            string code = String.Format("{0}\n// {1}\n{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13}",
                offset == 0 ? "" : ",", offset,
                lookId, (int)look.pointZIndex, pointSize[0], pointSize[1], pointOpacity[0], pointOpacity[1], pointColorOffset,
                textColorOffset, textSize[0], textSize[1], textOpacity[0], textOpacity[1]);

            append("PointLooks.cpp", code);
            _currentPointLookOffset += 12;

            return offset;
        }

        internal int addLineLook(int lookId, XmlLineLook look)
        {
            int offset = _currentLineLookOffset;

            int[] lineSize = doubleToIntArray(look.lineSize);
            int[] textSize = doubleToIntArray(look.textSize);
            int[] lineOpacity = doubleToIntArray((double)look.lineAlpha / 255.0);
            int[] textOpacity = doubleToIntArray((double)look.textAlpha / 255.0);
            int lineColorOffset = getStringOffset(String.Format("#{0:x2}{1:x2}{2:x2}", look.lineRed, look.lineGreen, look.lineBlue));
            int textColorOffset = getStringOffset(String.Format("#{0:x2}{1:x2}{2:x2}", look.textRed, look.textGreen, look.textBlue));

            string code = String.Format("{0}\n// {1}\n{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13}",
                offset == 0 ? "" : ",", offset,
                lookId, (int)look.lineZIndex, lineSize[0], lineSize[1], lineOpacity[0], lineOpacity[1], lineColorOffset,
                textColorOffset, textSize[0], textSize[1], textOpacity[0], textOpacity[1]);

            append("LineLooks.cpp", code);
            _currentLineLookOffset += 12;

            return offset;
        }

        internal int addPolygonLook(int lookId, XmlPolygonLook1 look1, XmlPolygonLook2 look2)
        {
            int contourZIndex, contourAlpha, contourRed, contourGreen, contourBlue;
            int fillZIndex, fillAlpha, fillRed, fillGreen, fillBlue;
            int textAlpha, textRed, textGreen, textBlue;
            double contourSize, textSize;

            if (look2 == null)
            {
                contourZIndex = look1.contourZIndex;
                contourAlpha = (int)look1.contourAlpha;
                contourRed = (int)look1.contourRed;
                contourGreen = (int)look1.contourGreen;
                contourBlue = (int)look1.contourBlue;
                contourSize = look1.contourSize;
                fillZIndex = look1.fillZIndex;
                fillAlpha = (int)look1.contourAlpha;
                fillRed = 255 - (int)((double)(255 - look1.contourRed) * look1.fillLightRatio);
                fillGreen = 255 - (int)((double)(255 - look1.contourGreen) * look1.fillLightRatio);
                fillBlue = 255 - (int)((double)(255 - look1.contourBlue) * look1.fillLightRatio);
                textAlpha = (int)look1.textAlpha;
                textRed = (int)look1.textRed;
                textGreen = (int)look1.textGreen;
                textBlue = (int)look1.textBlue;
                textSize = look1.textSize;
            }
            else
            {
                contourZIndex = look2.contourZIndex;
                contourAlpha = (int)look2.contourAlpha;
                contourRed = (int)look2.contourRed;
                contourGreen = (int)look2.contourGreen;
                contourBlue = (int)look2.contourBlue;
                contourSize = look2.contourSize;
                fillZIndex = look2.fillZIndex;
                fillAlpha = (int)look2.contourAlpha;
                fillRed = (int)look2.fillRed;
                fillGreen = (int)look2.fillGreen;
                fillBlue = (int)look2.fillBlue;
                textAlpha = (int)look2.textAlpha;
                textRed = (int)look2.textRed;
                textGreen = (int)look2.textGreen;
                textBlue = (int)look2.textBlue;
                textSize = look2.textSize;
            }

            int offset = _currentPolygonLookOffset;

            int[] contourSizeInt = doubleToIntArray(contourSize);
            int[] textSizeInt = doubleToIntArray(textSize);
            int[] contourOpacity = doubleToIntArray((double)contourAlpha / 255.0);
            int[] textOpacity = doubleToIntArray((double)textAlpha / 255.0);
            int[] fillOpacity = doubleToIntArray((double)fillAlpha / 255.0);
            int contourColorOffset = getStringOffset(String.Format("#{0:x2}{1:x2}{2:x2}", contourRed, contourGreen, contourBlue));
            int textColorOffset = getStringOffset(String.Format("#{0:x2}{1:x2}{2:x2}", textRed, textGreen, textBlue));
            int fillColorOffset = getStringOffset(String.Format("#{0:x2}{1:x2}{2:x2}", fillRed, fillGreen, fillBlue));

            string code = String.Format("{0}\n// {1}\n{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17}",
                offset == 0 ? "" : ",", offset,
                lookId, contourZIndex, contourSizeInt[0], contourSizeInt[1], contourOpacity[0], contourOpacity[1], contourColorOffset,
                textColorOffset, textSizeInt[0], textSizeInt[1], textOpacity[0], textOpacity[1],
                fillZIndex, fillColorOffset, fillOpacity[0], fillOpacity[1]);

            append("PolygonLooks.cpp", code);
            _currentPolygonLookOffset += 16;

            return offset;
        }

        private int getIntArrayOffset(IEnumerable<int> values)
        {
            int offset = _currentIntArrayOffset;

            string code = String.Format(",\n// {0}\n{1}", offset, String.Join(",", values));
            append("IntArrays.cpp", code);
            _currentIntArrayOffset += values.Count();

            return offset;
        }

        /*private int getDoubleArrayOffset(IEnumerable<double> values)
        {
            int offset = _currentDoubleArrayOffset;

            string code = String.Format("{0}\n// {1}\n{2}",
                offset == 0 ? "" : ",", offset, String.Join(",", values.Select(v => v.ToString("G", _cultureInfo))));
            append("DoubleArrays.cpp", code);
            _currentDoubleArrayOffset += values.Count();

            return offset;
        }*/

        private int[] doubleToIntArray(double d)
        {
            byte[] b = BitConverter.GetBytes(d);
            int[] r = new int[2];
            r[0] = (((int)b[3]) << 24) | (((int)b[2]) << 16) | (((int)b[1]) << 8) | ((int)b[0]);
            r[1] = (((int)b[7]) << 24) | (((int)b[6]) << 16) | (((int)b[5]) << 8) | ((int)b[4]);
            return r;
        }

        private void append(string fileName, string text)
        {
            string path = String.Format("{0}/{1}", _dirName, fileName);

            StreamWriter file;
            if (!_streamWriterDictionary.TryGetValue(path, out file))
            {
                file = new StreamWriter(path, true);
                _streamWriterDictionary.Add(path, file);
            }

            file.Write(text);
        }

        private void writeAndClose(string fileName, string text)
        {
            string path = String.Format("{0}/{1}", _dirName, fileName);
            StreamWriter file = new StreamWriter(path, false);
            file.Write(text);
            file.Close();
        }
    }
}
