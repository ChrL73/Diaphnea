using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.IO;
using MongoDB.Driver;
using MongoDB.Bson;
using Schemas;

namespace MapDataProcessing
{
    class MapDataProcessor
    {
        private readonly String _configFile;
        private MapData _mapData;
        private readonly Dictionary<String, MapElement> _elementDictionary = new Dictionary<string, MapElement>();
        private readonly Dictionary<String, List<LineMapElement>> _attachedLineElementDictionary = new Dictionary<string, List<LineMapElement>>();
        private readonly Dictionary<String, List<LineMapElement>> _attachedPolygonElementDictionary = new Dictionary<string, List<LineMapElement>>();
        private readonly Dictionary<String, int> _fileNameDictionary = new Dictionary<string, int>();
        private readonly ElementLinker _elementLinker = new ElementLinker();

        internal MapDataProcessor(String configFile)
        {
            _configFile = configFile;
        }

        internal int run()
        {
            int result = 0;

            PolygonLinePart.clearAll();
            PolygonPolygonPart.clearAll();
            LineLinePart.clearAll();
            GeoPoint.reset();
            ElementName.reset();
            ItemId.reset();
            Look.reset();
            Category.reset();
            
            if (result == 0)
            {
                MessageLogger.addMessage(XmlLogLevelEnum.MESSAGE, String.Format("Loading configurationfile {0} ...", _configFile));
                result = loadData();
            }

            if (result == 0)
            {
                MessageLogger.addMessage(XmlLogLevelEnum.MESSAGE, "Creating elements...");
                result = createElements();
            }

            if (result == 0)
            {
                MessageLogger.addMessage(XmlLogLevelEnum.MESSAGE, "Linking line elements...");
                result = attachElements();
            }

            if (result == 0)
            {
                MessageLogger.addMessage(XmlLogLevelEnum.MESSAGE, "Adding KML files...");
                result = addKmlFiles(_mapData.XmlMapData.parameters.kmlDir);
            }

            if (result == 0)
            {
                MessageLogger.addMessage(XmlLogLevelEnum.MESSAGE, "Forming elements parts...");
                result = formParts();
            }

            if (result == 0)
            {
                MessageLogger.addMessage(XmlLogLevelEnum.MESSAGE, "Smoothing line parts of polygon elements...");
                result = PolygonLinePart.smoothAll(_mapData);
            }

            if (result == 0)
            {
                MessageLogger.addMessage(XmlLogLevelEnum.MESSAGE, "Smoothing polygon parts of polygon elements...");
                result = PolygonPolygonPart.smoothAll(_mapData);
            }

            if (result == 0)
            {
                MessageLogger.addMessage(XmlLogLevelEnum.MESSAGE, "Classifying segment for superposing algorithm...");
                foreach (XmlResolution xmlResolution in _mapData.XmlMapData.resolutionList)
                {
                    _mapData.LineSuperposerDictionary[xmlResolution].classifySegments(_mapData.XmlMapData.parameters.superposingThreshold);
                }
            }

            if (result == 0)
            {
                MessageLogger.addMessage(XmlLogLevelEnum.MESSAGE, "Smoothing line parts of line elements...");
                result = LineLinePart.smoothAll(_mapData);
            }

            if (result == 0)
            {
                MessageLogger.addMessage(XmlLogLevelEnum.MESSAGE, "Forming contours of polygon elements...");
                result = formContours();
            }

            if (result == 0)
            {
                MessageLogger.addMessage(XmlLogLevelEnum.MESSAGE, "Linking elements...");
                result = linkElements();
            }

            if (result == 0)
            {
                MessageLogger.addMessage(XmlLogLevelEnum.MESSAGE, "Filling database...");
                //result = fillDatabase();
            }

            if (result == 0)
            {
                MessageLogger.addMessage(XmlLogLevelEnum.MESSAGE, "C++ code generation...");
                result = generateCode();
            }

            return result;
        }

        private int loadData()
        {
            XmlReader reader;
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessIdentityConstraints;

            try
            {
                reader = XmlReader.Create(_configFile, settings);
            }
            catch (Exception e)
            {
                Console.WriteLine("Fail to open file {0}", _configFile);
                Console.WriteLine(e.Message);
                return -1;
            }

            try
            {
                while (reader.Read());
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in file {0}", _configFile);
                Console.WriteLine(e.Message);
                if (e.InnerException != null) Console.WriteLine(e.InnerException.Message);
                return -1;
            }

            try
            {
                reader = XmlReader.Create(_configFile, settings);
            }
            catch (Exception e)
            {
                Console.WriteLine("Fail to open file {0}", _configFile);
                Console.WriteLine(e.Message);
                return -1;
            }

            XmlSerializer serializer = new XmlSerializer(typeof(XmlMapData));
            XmlMapData mapData = null;

            try
            {
                mapData = (XmlMapData)serializer.Deserialize(reader);
            }
            catch (Exception e)
            {
                Console.WriteLine("Fail to deserialize content of file {0}", _configFile);
                Console.WriteLine(e.Message);
                if (e.InnerException != null) Console.WriteLine(e.InnerException.Message);
                return -1;
            }

            reader.Close();

            _mapData = new MapData(_configFile, mapData);

            MessageLogger.LogFileName = mapData.parameters.logFileName;
            MessageLogger.FileLogLevel = mapData.parameters.fileLogLevel;
            MessageLogger.ConsoleLogLevel = mapData.parameters.consoleLogLevel;

            return 0;
        }

        private int createElements()
        {
            foreach (XmlPolygonElement xmlPolygonElement in _mapData.XmlMapData.elementList.polygonElementList)
            {
                String id = xmlPolygonElement.id.Substring(2);

                List<string> coveredElementList = new List<string>();
                foreach (XmlCoveredElement coveredElement in xmlPolygonElement.coveredElementList) coveredElementList.Add(coveredElement.id.Substring(2));

                List<string> lookIds = new List<string>();
                foreach (XmlPolygonLookRef lookRef in xmlPolygonElement.looks) lookIds.Add(lookRef.id);

                double importance = _mapData.getCategory(xmlPolygonElement.category).Importance + xmlPolygonElement.importance;

                PolygonMapElement polygonMapElement = new PolygonMapElement(id, _mapData, xmlPolygonElement.name, xmlPolygonElement.shortName,
                                                                            importance, lookIds, xmlPolygonElement.category, coveredElementList);
                _elementDictionary.Add(id, polygonMapElement);
                _elementLinker.addElement(id, polygonMapElement);
            }

            foreach (XmlLineElement xmlLineElement in _mapData.XmlMapData.elementList.lineElementList)
            {
                String id = xmlLineElement.id.Substring(2);

                List<string> lookIds = new List<string>();
                foreach (XmlLineLookRef lookRef in xmlLineElement.looks) lookIds.Add(lookRef.id);

                double importance = _mapData.getCategory(xmlLineElement.category).Importance + xmlLineElement.importance;

                LineMapElement lineMapElement = new LineMapElement(id, _mapData, xmlLineElement.name, xmlLineElement.shortName, importance, lookIds, xmlLineElement.category);
                _elementDictionary.Add(id, lineMapElement);
                _elementLinker.addElement(id, lineMapElement);
            }

            foreach (XmlPointElement xmlPointElement in _mapData.XmlMapData.elementList.pointElementList)
            {
                String id = xmlPointElement.id.Substring(2);

                List<string> lookIds = new List<string>();
                foreach (XmlPointLookRef lookRef in xmlPointElement.looks) lookIds.Add(lookRef.id);

                double importance = _mapData.getCategory(xmlPointElement.category).Importance + xmlPointElement.importance;

                PointMapElement pointMapElement = new PointMapElement(id, _mapData, xmlPointElement.name, xmlPointElement.shortName, importance, lookIds, xmlPointElement.category);
                _elementDictionary.Add(id, pointMapElement);
                _elementLinker.addElement(id, pointMapElement);
            }

            return 0;
        }

        private int attachElements()
        {
            foreach (XmlLineElement xmlLineElement in _mapData.XmlMapData.elementList.lineElementList)
            {
                String id = xmlLineElement.id.Substring(2);
                LineMapElement lineMapElement = (LineMapElement)_elementDictionary[id];

                foreach(XmlAttachedElement xmlAttachedElement in xmlLineElement.attachmentList)
                {
                    string attachedElementId = xmlAttachedElement.id.Substring(2);
                    MapElement attachedElement = _elementDictionary[attachedElementId];
                    bool onlyForLinker = xmlAttachedElement.onlyForLinkerSpecified && xmlAttachedElement.onlyForLinkerSpecified;

                    if (attachedElement is LineMapElement)
                    {
                        if (!onlyForLinker)
                        {
                            if (!_attachedLineElementDictionary.ContainsKey(attachedElementId)) _attachedLineElementDictionary.Add(attachedElementId, new List<LineMapElement>());
                            _attachedLineElementDictionary[attachedElementId].Add(lineMapElement);
                        }
                        _elementLinker.addAttachment(lineMapElement, (LineMapElement)attachedElement);
                    }
                    else if (attachedElement is PolygonMapElement)
                    {
                        if (!onlyForLinker)
                        {
                            if (!_attachedPolygonElementDictionary.ContainsKey(attachedElementId)) _attachedPolygonElementDictionary.Add(attachedElementId, new List<LineMapElement>());
                            _attachedPolygonElementDictionary[attachedElementId].Add(lineMapElement);
                        }
                    }
                    else
                    {
                        MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format("Element '{0}' can not be attached to element '{1}': Only line elements and polygon elements can be attached",
                                                 attachedElementId, id));
                        return -1;
                    }
                }
            }

            return 0;
        }

        private int addKmlFiles(String path)
        {
            FileAttributes attr;
            try
            {
                attr = File.GetAttributes(path);
            }
            catch (FileNotFoundException)
            {
                MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format("Kml path {0} not found", path));
                return -1;
            }

            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                //MessageLogger.addMessage(XmlLogLevelEnum.MESSAGE, String.Format("Browsing Kml path {0} ...", path));
                List<String> entries = new List<String>(Directory.EnumerateFileSystemEntries(path));
                foreach (String entry in entries)
                {                
                    if (addKmlFiles(entry) != 0) return -1;
                }
            }
            else
            {
                if (String.Equals(Path.GetExtension(path), ".kml", StringComparison.OrdinalIgnoreCase))
                {
                    if (addKmlFile(path) != 0) return -1;
                }
            }

            return 0;
        }

        private int addKmlFile(string path)
        {
            string name = Path.GetFileNameWithoutExtension(path);
            if (_fileNameDictionary.ContainsKey(name))
            {
                MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format("Several KMl files have the same name '{0}'", name));
                return -1;
            }

            _fileNameDictionary.Add(name, 0);
            string[] idList = name.Split('_');
            string exclusionTag = _mapData.XmlMapData.parameters.exclusionTag;

            if (exclusionTag == null || !idList.Contains(exclusionTag))
            {
                int i, n = idList.Length;
                for (i = 0; i < n; ++i)
                {
                    string id = idList[i];

                    MapElement element;
                    if (_elementDictionary.TryGetValue(id, out element))
                    {
                        if (element.addKmlFile(path) != 0) return -1;

                        if (element is PolygonMapElement)
                        {
                            KmlFileData data = KmlFileData.getData(path);
                            if (data.Type == KmlFileTypeEnum.LINE)
                            {
                                int j;
                                for (j = i + 1; j < n; ++j)
                                {
                                    string id2 = idList[j];
                                    MapElement element2;
                                    if (_elementDictionary.TryGetValue(id2, out element2))
                                    {
                                        PolygonMapElement polygon2 = element2 as PolygonMapElement;
                                        if (polygon2 != null && element.Category.Id == polygon2.Category.Id)
                                        {
                                            _elementLinker.addNeighbor((PolygonMapElement)element, polygon2);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    List<LineMapElement> lineElementList;
                    if (_attachedLineElementDictionary.TryGetValue(id, out lineElementList))
                    {
                        foreach (LineMapElement lineElement in lineElementList)
                        {
                            if (lineElement.attachLineLineKmlFile(path) != 0) return -1;
                        }
                    }

                    if (_attachedPolygonElementDictionary.TryGetValue(id, out lineElementList))
                    {
                        foreach (LineMapElement lineElement in lineElementList)
                        {
                            if (lineElement.attachPolygonLineKmlFile(path) != 0) return -1;
                        }
                    }
                }
            }

            return 0;
        }

        private int formParts()
        {
            foreach (MapElement element in _elementDictionary.Values)
            {
                if (element.formParts1() != 0) return -1;
            }

            foreach (MapElement element in _elementDictionary.Values)
            {
                if (element.formParts2() != 0) return -1;
            }

            return 0;
        }

        private int formContours()
        {
            foreach (PolygonMapElement element in _elementDictionary.Values.OfType<PolygonMapElement>())
            {
                if (element.formContours() != 0) return -1;
            }

            return 0;
        }

        private int linkElements()
        {
            XmlResolution bestResolution = null;
            foreach(XmlResolution resolution in _mapData.XmlMapData.resolutionList)
            {
                if (bestResolution == null || resolution.sampleLength1 * Double.Parse(resolution.sampleRatio) < bestResolution.sampleLength1 * Double.Parse(bestResolution.sampleRatio))
                {
                    bestResolution = resolution;
                }
            }

            _elementLinker.Resolution = bestResolution;
            _elementLinker.Projection = _mapData.XmlMapData.parameters.projection;
            _elementLinker.LinePointLinkThreshold = _mapData.XmlMapData.parameters.linePointLinkThreshold;

            foreach (PolygonMapElement element in _elementDictionary.Values.OfType<PolygonMapElement>())
            {
                if (_elementLinker.addPolygon(element) != 0) return -1;
            }

            _elementLinker.classifySegments();

            foreach (MapElement element in _elementDictionary.Values)
            {
                Dictionary<MapElement, int> linked1Elements = _elementLinker.getLinked1Elements(element);
                if (linked1Elements == null) return -1;

                foreach(MapElement linked1Element in linked1Elements.Keys)
                {
                    if (!element.LinkedElements1.ContainsKey(linked1Element)) element.LinkedElements1.Add(linked1Element, 0);
                    if (!linked1Element.LinkedElements1.ContainsKey(element)) linked1Element.LinkedElements1.Add(element, 0);
                }

                Dictionary<MapElement, int> linked2Elements = _elementLinker.getLinked2Elements(element);
                if (linked2Elements == null) return -1;

                foreach (MapElement linked2Element in linked2Elements.Keys)
                {
                    if (!element.LinkedElements2.ContainsKey(linked2Element)) element.LinkedElements2.Add(linked2Element, 0);
                    if (!linked2Element.LinkedElements2.ContainsKey(element)) linked2Element.LinkedElements2.Add(element, 0);
                }
            }

            foreach (MapElement element in _elementDictionary.Values)
            {
                Dictionary<MapElement, int> elementsToLink = new Dictionary<MapElement, int>();

                {
                    // If 'element' is a river, all rivers along the stream to the sea (or ocean) must be linked to 'element'
                    Dictionary<MapElement, int> unsymetricalLinked1Elements = _elementLinker.getUnsymetricalLinked1Elements(element);
                    if (unsymetricalLinked1Elements == null) return -1;
                    foreach (MapElement unsymetricalLinked1Element in unsymetricalLinked1Elements.Keys)
                    {
                        if (!elementsToLink.ContainsKey(unsymetricalLinked1Element)) elementsToLink.Add(unsymetricalLinked1Element, 0);
                    }
                }

                foreach (MapElement linkedElement in element.LinkedElements1.Keys)
                {
                    // If 'linkedElement' is a river, all rivers along the stream to the sea (or ocean) must be linked to 'element'
                    Dictionary<MapElement, int> unsymetricalLinked1Elements = _elementLinker.getUnsymetricalLinked1Elements(linkedElement);
                    if (unsymetricalLinked1Elements == null) return -1;
                    foreach (MapElement unsymetricalLinked1Element in unsymetricalLinked1Elements.Keys)
                    {
                        if (unsymetricalLinked1Element != element && !elementsToLink.ContainsKey(unsymetricalLinked1Element)) elementsToLink.Add(unsymetricalLinked1Element, 0);
                    }
                }

                foreach (MapElement elementToLink in elementsToLink.Keys)
                {
                    if (!element.LinkedElements1.ContainsKey(elementToLink)) element.LinkedElements1.Add(elementToLink, 0);
                }
            }

            foreach (MapElement element in _elementDictionary.Values)
            {
                foreach(MapElement linkedElement1 in element.LinkedElements1.Keys)
                {
                    if (element.LinkedElements2.ContainsKey(linkedElement1)) element.LinkedElements2.Remove(linkedElement1);
                }

                if (element.LinkedElements1.ContainsKey(element)) element.LinkedElements1.Remove(element);
                if (element.LinkedElements2.ContainsKey(element)) element.LinkedElements2.Remove(element); 
            }

            return 0;
        }

        private int fillDatabase()
        {
            MongoClient mongoClient = new MongoClient();
            IMongoDatabase database = mongoClient.GetDatabase(_mapData.XmlMapData.parameters.databaseName);

            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("map", _mapData.XmlMapData.parameters.mapId);
            IMongoCollection<BsonDocument> pointListCollection = database.GetCollection<BsonDocument>("point_lists");
            pointListCollection.DeleteMany(filter);
            IMongoCollection<BsonDocument> mapCollection = database.GetCollection<BsonDocument>("maps");
            mapCollection.DeleteMany(filter);
            IMongoCollection<BsonDocument> polygonElementCollection = database.GetCollection<BsonDocument>("polygon_elements");
            polygonElementCollection.DeleteMany(filter);
            IMongoCollection<BsonDocument> pointElementCollection = database.GetCollection<BsonDocument>("point_elements");
            pointElementCollection.DeleteMany(filter);
            IMongoCollection<BsonDocument> lineElementCollection = database.GetCollection<BsonDocument>("line_elements");
            lineElementCollection.DeleteMany(filter);
            IMongoCollection<BsonDocument> itemCollection = database.GetCollection<BsonDocument>("items");
            itemCollection.DeleteMany(filter);

            BsonArray languagesArray = new BsonArray();
            foreach (XmlLanguage language in _mapData.XmlMapData.parameters.languageList)
            {
                BsonDocument languageDocument = new BsonDocument()
                {
                    { "id", language.id.ToString() },
                    { "name", language.name }
                };
                languagesArray.Add(languageDocument);
            }

            BsonArray resolutionArray = new BsonArray();
            int i, n = _mapData.XmlMapData.resolutionList.Length;
            for (i = 0; i < n; ++i)
            {
                XmlResolution resolution = _mapData.XmlMapData.resolutionList[i];
                double sample_length = resolution.sampleLength1 * Double.Parse(resolution.sampleRatio);

                BsonDocument resolutionDocument = new BsonDocument()
                {
                    { "index", i},
                    { "sample_length", sample_length}
                };

                resolutionArray.Add(resolutionDocument);
            }

            BsonArray lookArray = new BsonArray();
            foreach (Look look in _mapData.LookList)
            {
                lookArray.Add(look.getBsonDocument());
            }

            BsonArray categoryArray = new BsonArray();
            foreach (Category category in _mapData.CategoryList)
            {
                categoryArray.Add(category.getBsonDocument());
            }

            BsonDocument mapDocument = new BsonDocument()
            {
                { "map", _mapData.XmlMapData.parameters.mapId },
                { "name", new ElementName(_mapData.XmlMapData.parameters.mapName).getBsonDocument() },
                { "languages", languagesArray },
                { "resolutions", resolutionArray },
                { "looks", lookArray },
                { "categories", categoryArray },
                { "zoom_min_distance", _mapData.XmlMapData.parameters.zoomMinDistance },
                { "zoom_max_distance", _mapData.XmlMapData.parameters.zoomMaxDistance },
                { "resolution_threshold", _mapData.XmlMapData.parameters.resolutionThreshold },
                { "size_parameter1", _mapData.XmlMapData.parameters.sizeParameter1 },
                { "size_parameter2", _mapData.XmlMapData.parameters.sizeParameter2 }
            };

            mapCollection.InsertOne(mapDocument);

            if (PolygonLinePart.fillDatabase(database, _mapData) != 0) return -1;
            if (PolygonPolygonPart.fillDatabase(database, _mapData) != 0) return -1;
            if (LineLinePart.fillDatabase(database, _mapData) != 0) return -1;

            foreach (MapElement element in _elementDictionary.Values)
            {
                if (element.fillDatabase(database) != 0) return -1;
            }

            return 0;
        }

        private int generateCode()
        {
            if (!Directory.Exists(_mapData.XmlMapData.parameters.cppGenerationDir))
            {
                MessageLogger.addMessage(XmlLogLevelEnum.ERROR, String.Format("Generation folder does not exist ({0})", _mapData.XmlMapData.parameters.cppGenerationDir));
                return -1;
            }

            CodeGenerator codeGenerator = new CodeGenerator(String.Format("{0}/{1}", _mapData.XmlMapData.parameters.cppGenerationDir, _mapData.XmlMapData.parameters.mapId), _mapData);

            foreach (Look look in _mapData.LookList)
            {
                if (look.generateCode(codeGenerator) != 0) return -1;
            }

            if (PolygonLinePart.generateCode(codeGenerator, _mapData) != 0) return -1;
            if (PolygonPolygonPart.generateCode(codeGenerator, _mapData) != 0) return -1;
            if (LineLinePart.generateCode(codeGenerator, _mapData) != 0) return -1;

            foreach (MapElement element in _elementDictionary.Values)
            {
                if (element.generateCode(codeGenerator) != 0) return -1;
            }

            string mapInfo = getMapInfoJson();

            codeGenerator.close(mapInfo);

            return 0;
        }

        private string getMapInfoJson()
        {
            StringBuilder mapInfo = new StringBuilder();

            mapInfo.Append("{\"elementIds\":[\"");

            List<string> elementIdList = new List<string>();
            foreach (XmlPointElement xmlPointElement in _mapData.XmlMapData.elementList.pointElementList) elementIdList.Add(xmlPointElement.id.Substring(2));
            foreach (XmlLineElement xmlLineElement in _mapData.XmlMapData.elementList.lineElementList) elementIdList.Add(xmlLineElement.id.Substring(2));
            foreach (XmlPolygonElement xmlPolygonElement in _mapData.XmlMapData.elementList.polygonElementList) elementIdList.Add(xmlPolygonElement.id.Substring(2));
            mapInfo.Append(String.Join("\",\"", elementIdList));

            mapInfo.Append("\"],\"languages\":[");

            List<string> languageList = new List<string>();
            foreach (XmlLanguage xmlLanguage in _mapData.XmlMapData.parameters.languageList)
            {
                languageList.Add(String.Format("{{\"id\":\"{0}\",\"name\":\"{1}\"}}", xmlLanguage.id, xmlLanguage.name));
            }
            mapInfo.Append(String.Join(",", languageList));

            mapInfo.Append("],\"names\":{");
            List<string> nameList = new List<string>();
            foreach (XmlName xmlName in _mapData.XmlMapData.parameters.mapName)
            {
                nameList.Add(String.Format("\"{0}\":\"{1}\"", xmlName.language, xmlName.text));
            }
            mapInfo.Append(String.Join(",", nameList));

            mapInfo.Append("},\"categories\":[");

            List<string> categoryList = new List<string>();
            foreach(XmlCategory xmlCategory in _mapData.XmlMapData.categoryList)
            {
                StringBuilder categoryInfo = new StringBuilder();
                categoryInfo.Append("{");
                List<string> categoryNameList = new List<string>();
                foreach(XmlName xmlName in xmlCategory.name)
                {
                    categoryNameList.Add(String.Format("\"{0}\":\"{1}\"", xmlName.language, xmlName.text));
                }
                categoryInfo.Append(String.Join(",", categoryNameList));
                categoryInfo.Append("}");
                categoryList.Add(categoryInfo.ToString());
            }
            mapInfo.Append(String.Join(",", categoryList));

            mapInfo.Append("],\"looks\":[");

            List<string> lookList = new List<string>();
            foreach(Look look in _mapData.LookList) lookList.Add(look.getJson());
            mapInfo.Append(String.Join(",", lookList));

            mapInfo.Append("],\"zoomMinDistance\":");
            mapInfo.Append(_mapData.XmlMapData.parameters.zoomMinDistance.ToString("G5", CultureInfo.CreateSpecificCulture("en-US")));
            mapInfo.Append(",\"zoomMaxDistance\":");
            mapInfo.Append(_mapData.XmlMapData.parameters.zoomMaxDistance.ToString("G5", CultureInfo.CreateSpecificCulture("en-US")));
            mapInfo.Append(",\"sizeParameter1\":");
            mapInfo.Append(_mapData.XmlMapData.parameters.sizeParameter1.ToString("G5", CultureInfo.CreateSpecificCulture("en-US")));
            mapInfo.Append(",\"sizeParameter2\":");
            mapInfo.Append(_mapData.XmlMapData.parameters.sizeParameter2.ToString("G5", CultureInfo.CreateSpecificCulture("en-US")));

            mapInfo.Append("}");

            return mapInfo.ToString();
        }
    }
}
