using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.IO;
using MongoDB.Driver;
using MongoDB.Bson;

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
                result = fillDatabase();
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

                PolygonMapElement polygonMapElement = new PolygonMapElement(id, _mapData, xmlPolygonElement.name, xmlPolygonElement.shortName,
                                                                            xmlPolygonElement.importance, lookIds, xmlPolygonElement.category, coveredElementList);
                _elementDictionary.Add(id, polygonMapElement);
                _elementLinker.addElement(id, polygonMapElement);
            }

            foreach (XmlLineElement xmlLineElement in _mapData.XmlMapData.elementList.lineElementList)
            {
                String id = xmlLineElement.id.Substring(2);

                List<string> lookIds = new List<string>();
                foreach (XmlLineLookRef lookRef in xmlLineElement.looks) lookIds.Add(lookRef.id);

                LineMapElement lineMapElement = new LineMapElement(id, _mapData, xmlLineElement.name, xmlLineElement.shortName, xmlLineElement.importance, lookIds, xmlLineElement.category);
                _elementDictionary.Add(id, lineMapElement);
                _elementLinker.addElement(id, lineMapElement);
            }

            foreach (XmlPointElement xmlPointElement in _mapData.XmlMapData.elementList.pointElementList)
            {
                String id = xmlPointElement.id.Substring(2);

                List<string> lookIds = new List<string>();
                foreach (XmlPointLookRef lookRef in xmlPointElement.looks) lookIds.Add(lookRef.id);

                PointMapElement pointMapElement = new PointMapElement(id, _mapData, xmlPointElement.name, xmlPointElement.shortName, xmlPointElement.importance, lookIds, xmlPointElement.category);
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

                    if (attachedElement is LineMapElement)
                    {
                        if (!_attachedLineElementDictionary.ContainsKey(attachedElementId)) _attachedLineElementDictionary.Add(attachedElementId, new List<LineMapElement>());
                        _attachedLineElementDictionary[attachedElementId].Add(lineMapElement);
                    }
                    else if (attachedElement is PolygonMapElement)
                    {
                        if (!_attachedPolygonElementDictionary.ContainsKey(attachedElementId)) _attachedPolygonElementDictionary.Add(attachedElementId, new List<LineMapElement>());
                        _attachedPolygonElementDictionary[attachedElementId].Add(lineMapElement);
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
                foreach (String id in idList)
                {
                    MapElement element;
                    if (_elementDictionary.TryGetValue(id, out element))
                    {
                        if (element.addKmlFile(path) != 0) return -1;
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
                List<MapElement> linked1Elements = _elementLinker.getLinked1Elements(element);
                if (linked1Elements == null) return -1;

                foreach(MapElement linked1Element in linked1Elements)
                {
                    element.LinkedElements1.Add(linked1Element);
                    linked1Element.LinkedElements1.Add(element);
                }

                List<MapElement> linked2Elements = _elementLinker.getLinked2Elements(element);
                if (linked2Elements == null) return -1;

                foreach (MapElement linked2Element in linked2Elements)
                {
                    element.LinkedElements2.Add(linked2Element);
                    linked2Element.LinkedElements2.Add(element);
                }
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
    }
}
