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

        private readonly Dictionary<string, StreamWriter> _streamWriterDictionary = new Dictionary<string, StreamWriter>();

        private int _currentStringOffset;
        private int _currentIntArrayOffset;
        private int _currentPointElementOffset;
        private int _currentLineElementOffset;
        private int _currentPolygonElementOffset;
        private int _currentMultipointItemOffset;
        private int _currentPointLookOffset;
        private int _currentLineLookOffset;
        private int _currentPolygonLookOffset;

        private int _pointElementCount;
        private int _lineElementCount;
        private int _polygonElementCount;

        private readonly Dictionary<string, int> _stringDictionary = new Dictionary<string, int>();

        internal CodeGenerator(string dirName)
        {
            _dirName = dirName;

            _currentStringOffset = 1;
            _currentIntArrayOffset = 1;
            _currentPointElementOffset = 0;
            _currentLineElementOffset = 0;
            _currentPolygonElementOffset = 0;
            _currentMultipointItemOffset = 0;
            _currentPointLookOffset = 0;
            _currentLineLookOffset = 0;
            _currentPolygonLookOffset = 0;

            _pointElementCount = 0;
            _lineElementCount = 0;
            _polygonElementCount = 0;

            _stringDictionary.Add("", 0);

            if (!Directory.Exists(dirName)) Directory.CreateDirectory(dirName);

            string path = String.Format("{0}/Strings.cpp", dirName);
            if (File.Exists(path)) File.Delete(path);
            append("Strings.cpp", "namespace map_server\n{\nunsigned char strings[] =\n{\n// 0: Empty string\n0");

            path = String.Format("{0}/IntArrays.cpp", dirName);
            if (File.Exists(path)) File.Delete(path);
            append("IntArrays.cpp", "namespace map_server\n{\nint intArrays[] =\n{\n0");

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
        }

        internal void close(MapData mapData)
        {
            string path = String.Format("{0}/MapData.cpp", _dirName);
            if (File.Exists(path)) File.Delete(path);

            string code = String.Format(
                "namespace map_server\n{{\n    int pointElementCount = {0};\n    int lineElementCount = {1};\n    int polygonElementCount = {2};\n    double zoomMinDistance = {3};\n    double zoomMaxDistance = {4};\n}}\n",
                _pointElementCount, _lineElementCount, _polygonElementCount, mapData.XmlMapData.parameters.zoomMinDistance,
                mapData.XmlMapData.parameters.zoomMaxDistance);

            append("MapData.cpp", code);

            append("Strings.cpp", "\n};\n}\n");
            append("IntArrays.cpp", "\n};\n}\n");
            append("PointElements.cpp", "\n};\n}\n");
            append("LineElements.cpp", "\n};\n}\n");
            append("PolygonElements.cpp", "\n};\n}\n");
            append("MultipointItems.cpp", "\n};\n}\n");
            append("PointLooks.cpp", "\n};\n}\n");
            append("LineLooks.cpp", "\n};\n}\n");
            append("PolygonLooks.cpp", "\n};\n}\n");

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
            int[] xInt = doubleToIntArray(x);
            int[] yInt = doubleToIntArray(y);

            List<int> lookOffsetList = new List<int>();
            foreach (Look look in element.Looks) lookOffsetList.Add(look.CppOffset);
            int looksOffset = 0;
            if (lookOffsetList.Count() != 0) looksOffset = getIntArrayOffset(lookOffsetList);

            int framingLevel = element.Category.XmlCategory.framingLevel;

            string code = String.Format("{0}\n// {1} \"{2}\", ...\n{3},{4},{5},{6},{7},{8},{9},{10},{11}",
                _currentPointElementOffset == 0 ? "" : ",", _currentPointElementOffset, element.Id,
                idOffset, itemId, xInt[0], xInt[1], yInt[0], yInt[1], lookOffsetList.Count(), looksOffset,
                framingLevel);

            append("PointElements.cpp", code);
            _currentPointElementOffset += 9;
            ++_pointElementCount;
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

            string code = String.Format("{0}\n// {1} \"{2}\", ...\n{3},{4},{5},{6},{7},{8}",
                _currentLineElementOffset == 0 ? "" : ",", _currentLineElementOffset, element.Id,
                idOffset, itemOffsetList.Count(), itemsOffset, lookOffsetList.Count(), looksOffset,
                framingLevel);

            append("LineElements.cpp", code);
            _currentLineElementOffset += 6;
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

            List<int> lookOffsetList = new List<int>();
            foreach (Look look in element.Looks) lookOffsetList.Add(look.CppOffset);
            int looksOffset = 0;
            if (lookOffsetList.Count() != 0) looksOffset = getIntArrayOffset(lookOffsetList);

            int framingLevel = element.Category.XmlCategory.framingLevel;

            string code = String.Format("{0}\n// {1} \"{2}\", ...\n{3},{4},{5},{6},{7},{8},{9},{10},{11}",
                _currentPolygonElementOffset == 0 ? "" : ",", _currentPolygonElementOffset, element.Id,
                idOffset, element.ContourMapItem.CppOffset, itemOffsetList.Count(), itemsOffset,
                element.CoveredElementList.Count(), coveredElementsOffset, lookOffsetList.Count(), looksOffset,
                framingLevel);

            append("PolygonElements.cpp", code);
            _currentPolygonElementOffset += 9;
            ++_polygonElementCount;
        }

        internal int addMultipointItem(double xMin, double xMax, double yMin, double yMax, int itemId, string comment)
        {
            int offset = _currentMultipointItemOffset;

            int[] xMinInt = doubleToIntArray(xMin);
            int[] xMaxInt = doubleToIntArray(xMax);
            int[] yMinInt = doubleToIntArray(yMin);
            int[] yMaxInt = doubleToIntArray(yMax);

            string code = String.Format("{0}\n// {1} ({2})\n{3},{4},{5},{6},{7},{8},{9},{10},{11}",
                offset == 0 ? "" : ",", offset, comment,
                xMinInt[0], xMinInt[1], xMaxInt[0], xMaxInt[1], yMinInt[0], yMinInt[1], yMaxInt[0], yMaxInt[1], itemId);

            append("MultipointItems.cpp", code);
            _currentMultipointItemOffset += 9;

            return offset;
        }

        internal int addPointLook(int lookId, XmlPointLook look)
        {
            int offset = _currentPointLookOffset;

            int[] pointSize = doubleToIntArray(look.pointSize);
            int[] textSize = doubleToIntArray(look.textSize);

            string code = String.Format("{0}\n// {1}\n{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15}",
                offset == 0 ? "" : ",", offset,
                lookId, (int)look.pointZIndex, (int)look.pointAlpha, (int)look.pointRed, (int)look.pointGreen, (int)look.pointBlue, pointSize[0], pointSize[1],
                (int)look.textAlpha, (int)look.textRed, (int)look.textGreen, (int)look.textBlue, textSize[0], textSize[1]);

            append("PointLooks.cpp", code);
            _currentPointLookOffset += 14;

            return offset;
        }

        internal int addLineLook(int lookId, XmlLineLook look)
        {
            int offset = _currentLineLookOffset;

            int[] lineSize = doubleToIntArray(look.lineSize);
            int[] textSize = doubleToIntArray(look.textSize);

            string code = String.Format("{0}\n// {1}\n{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15}",
                offset == 0 ? "" : ",", offset,
                lookId, (int)look.lineZIndex, (int)look.lineAlpha, (int)look.lineRed, (int)look.lineGreen, (int)look.lineBlue, lineSize[0], lineSize[1],
                (int)look.textAlpha, (int)look.textRed, (int)look.textGreen, (int)look.textBlue, textSize[0], textSize[1]);

            append("LineLooks.cpp", code);
            _currentLineLookOffset += 14;

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

            string code = String.Format("{0}\n// {1}\n{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20}",
                offset == 0 ? "" : ",", offset,
                lookId, contourZIndex, contourAlpha, contourRed, contourGreen, contourBlue, contourSizeInt[0], contourSizeInt[1],
                textAlpha, textRed, textGreen, textBlue, textSizeInt[0], textSizeInt[1],
                fillZIndex, fillAlpha, fillRed, fillGreen, fillBlue);

            append("PolygonLooks.cpp", code);
            _currentPolygonLookOffset += 19;

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
    }
}
