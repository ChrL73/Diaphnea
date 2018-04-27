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
        }

        internal void close()
        {
            string path = String.Format("{0}/MapData.cpp", _dirName);
            if (File.Exists(path)) File.Delete(path);

            string code = String.Format(
                "namespace map_server\n{{\n    int pointElementCount = {0};\n    int lineElementCount = {1};\n    int polygonElementCount = {2};\n}}\n",
                _pointElementCount, _lineElementCount, _polygonElementCount);

            append("MapData.cpp", code);

            append("Strings.cpp", "\n};\n}\n");
            append("IntArrays.cpp", "\n};\n}\n");
            append("PointElements.cpp", "\n};\n}\n");
            append("LineElements.cpp", "\n};\n}\n");
            append("PolygonElements.cpp", "\n};\n}\n");
            append("MultipointItems.cpp", "\n};\n}\n");

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

            string code = String.Format("{0}\n// {1} \"{2}\", ...\n{3},{4},{5},{6},{7},{8}", _currentPointElementOffset == 0 ? "" : ",", _currentPointElementOffset, element.Id,
                idOffset, itemId, xInt[0], xInt[1], yInt[0], yInt[1]);

            append("PointElements.cpp", code);
            _currentPointElementOffset += 1;
            ++_pointElementCount;
        }

        internal void addLineElement(LineMapElement element)
        {
            List<int> itemOffsetList = new List<int>();
            foreach (LineLinePart part in element.PartList) itemOffsetList.Add(part.MapItemCppOffset);

            int idOffset = getStringOffset(element.Id);

            int itemsOffset = 0;
            if (itemOffsetList.Count() != 0)
            {
                itemsOffset = getIntArrayOffset(itemOffsetList);
            }

            string code = String.Format("{0}\n// {1} \"{2}\", ...\n{3},{4},{5}", _currentLineElementOffset == 0 ? "" : ",", _currentLineElementOffset, element.Id,
                idOffset, itemOffsetList.Count(), itemsOffset);

            append("LineElements.cpp", code);
            _currentLineElementOffset += 3;
            ++_lineElementCount;
        }

        internal void addPolygonElement(PolygonMapElement element)
        {
            int idOffset = getStringOffset(element.Id);

            string code = String.Format("{0}\n// {1} \"{2}\", ...\n{3}", _currentPolygonElementOffset == 0 ? "" : ",", _currentPolygonElementOffset, element.Id,
                idOffset);

            append("PolygonElements.cpp", code);
            _currentPolygonElementOffset += 1;
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
