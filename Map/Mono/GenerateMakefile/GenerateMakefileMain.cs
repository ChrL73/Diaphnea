using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Schemas;
using System.Linq;

namespace GenerateMakefile
{
    class GenerateMakefileMain
    {
        private static List<Rule> _ruleList = new List<Rule>();
        private static Dictionary<string, string> _generationDirectories = new Dictionary<string, string>();

        private static Dictionary<string, List<string>> _cppGeneratedFileNames = new Dictionary<string, List<string>>();

        private static List<string> _subdirList = new List<string>() { "", "/Data", "/TextDisplayer", "/Requests" };

        private static void Main(string[] args)
        {
            Console.WriteLine("Map Makefile generation...");

            int result = setGenerationInfo();

            if (result == 0)
            {
                /*AddObjectFilesLink();
                addCppGeneratedFilesCompilation();*/
                addCppFilesCompilation();
                addCppFileGeneration();
                addMapDataProcessingBuild();
                addXsdGeneration();

                generateMakefile();
            }

            //Console.WriteLine("Press any key to continue...");
            //Console.ReadKey();

            System.Environment.Exit(result);
        }

        private static void addCppFilesCompilation()
        {
            Dictionary<string, Dictionary<string, int>> dependencies = new Dictionary<string, Dictionary<string, int>>();

            List<string> cppFileList = new List<string>();
            List<string> fileList = new List<string>();

            string[] cppFiles;
            string[] hFiles;

            foreach (string subdir in _subdirList)
            {
                cppFiles = Directory.GetFiles(String.Format("../../../../NodeJS/map_server2{0}", subdir), "*.cpp");
                hFiles = Directory.GetFiles(String.Format("../../../../NodeJS/map_server2{0}", subdir), "*.h");

                foreach (string file in cppFiles)
                {
                    string name = String.Format("{0}/{1}", subdir, Path.GetFileName(file));
                    cppFileList.Add(name);
                    fileList.Add(name);
                }

                foreach (string file in hFiles)
                {
                    string name = String.Format("{0}/{1}", subdir, Path.GetFileName(file));
                    fileList.Add(name);
                }
            }

            foreach (string file in fileList) dependencies.Add(file, new Dictionary<string, int>());
            foreach (string file in fileList) addIncludeDependencies(file, dependencies, new Dictionary<string, int>(), new List<string>());

            foreach (string file in cppFileList)
            {
                Rule rule = new Rule();
                rule.Target = String.Format("NodeJS/map_server2/obj/Release{0}o", file.Substring(0, file.Length - 3));
                rule.Dependencies.Add(String.Format("NodeJS/map_server2{0}", file));

                foreach (string dep in dependencies[file].Keys) rule.Dependencies.Add(String.Format("NodeJS/map_server2{0}", dep));

                rule.Commands.Add("mkdir -p NodeJS/map_server2/obj/Release");
                rule.Commands.Add(String.Format("cd NodeJS/map_server2 && g++ -I. -IRequests -IData -ITextDisplayer -Wall -fexceptions -O2 -std=c++11 -c {0} -o obj/Release{1}o",
                                                file.Substring(1), file.Substring(0, file.Length - 3)));

                _ruleList.Add(rule);
            }
        }

        private static void addIncludeDependencies(string file, Dictionary<string, Dictionary<string, int>> dependencies, Dictionary<string, int> doneFiles, List<string> fileStack)
        {
            fileStack.Add(file);
            doneFiles.Add(file, 1);

            List<string> includes = getIncludeFiles(file);
            foreach (string include in includes)
            {
                foreach (string subdir in _subdirList)
                {
                    string includeWithSubdir = String.Format("{0}/{1}", subdir, include);
                    if (dependencies.ContainsKey(includeWithSubdir))
                    {
                        foreach (string f in fileStack) dependencies[f][includeWithSubdir] = 1;
                        if (!doneFiles.ContainsKey(includeWithSubdir)) addIncludeDependencies(includeWithSubdir, dependencies, doneFiles, fileStack);
                    }
                }
            }

            fileStack.RemoveAt(fileStack.Count - 1);
        }

        private static List<string> getIncludeFiles(string file)
        {
            List<string> includeFiles = new List<string>();
            StreamReader reader = new System.IO.StreamReader(String.Format("../../../../NodeJS/map_server2/{0}", file));

            while(true)
            {
                string line = reader.ReadLine();
                if (line == null) break;

                // Note: We do not handle the case where the 'include' directive is inside a comment
                if (line.Contains("#") && line.Contains("include"))
                {
                    int index = line.IndexOf('"');
                    if (index >= 0)
                    {
                        line = line.Substring(index + 1);
                        index = line.IndexOf('"');
                        if (index >= 0)
                        {
                            line = line.Substring(0, index);
                            includeFiles.Add(line);
                        }
                    }
                }
            }

            reader.Close();

            return includeFiles;
        }

        private static void addCppFileGeneration()
        {
            foreach (KeyValuePair<string, string> pair in _generationDirectories)
            {
                foreach (string fileName in _cppGeneratedFileNames[pair.Key])
                {
                    Rule rule = new Rule();
                    rule.Target = String.Format("NodeJS/map_server2/Data/generated_code/{0}/{1}.cpp", pair.Key, fileName);
                    rule.Dependencies.Add("Mono/MapDataProcessing/bin/Debug/MapDataProcessing.exe");
                    rule.Dependencies.Add(String.Format("VisualStudio/MapDataProcessing/{0}", pair.Value));
                    rule.Commands.Add("mkdir -p NodeJS/map_server2/Data/generated_code");
                    rule.Commands.Add(String.Format(
                        "cd Mono/MapDataProcessing/bin/Debug && mono MapDataProcessing.exe ../../../../VisualStudio/MapDataProcessing/{0}",
                        pair.Value));
                    _ruleList.Add(rule);
                }

                Rule rule2 = new Rule();
                rule2.Target = String.Format("NodeJS/map_server2/Data/generated_code/{0}/PointLists.h", pair.Key);
                rule2.Dependencies.Add("Mono/MapDataProcessing/bin/Debug/MapDataProcessing.exe");
                rule2.Dependencies.Add(String.Format("VisualStudio/MapDataProcessing/{0}", pair.Value));
                rule2.Commands.Add("mkdir -p NodeJS/map_server2/Data/generated_code");
                rule2.Commands.Add(String.Format(
                    "cd Mono/MapDataProcessing/bin/Debug && mono MapDataProcessing.exe ../../../../VisualStudio/MapDataProcessing/{0}",
                    pair.Value));
                _ruleList.Add(rule2);
            }
        }

        private static int setGenerationInfo()
        {
            string[] files = Directory.GetFiles("../../../../VisualStudio/MapDataProcessing", "MapData*.xml");
            foreach (string file in files)
            {
                if (!Path.GetFileName(file).Contains("Test"))
                {
                    XmlReader reader;
                    XmlReaderSettings settings = new XmlReaderSettings();
                    settings.ValidationType = ValidationType.Schema;
                    settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
                    settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation;
                    settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessIdentityConstraints;

                    try
                    {
                        reader = XmlReader.Create(file, settings);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Fail to open file {0}", file);
                        Console.WriteLine(e.Message);
                        return -1;
                    }

                    XmlSerializer serializer = new XmlSerializer(typeof(XmlMapData));

                    XmlMapData mapData;
                    try
                    {
                        mapData = (XmlMapData)serializer.Deserialize(reader);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Fail to deserialize content of file {0}", file);
                        Console.WriteLine(e.Message);
                        if (e.InnerException != null)
                            Console.WriteLine(e.InnerException.Message);
                        return -1;
                    }

                    reader.Close();

                    _generationDirectories.Add(mapData.parameters.mapId, Path.GetFileName(file));

                    List<string> cppFileName = new List<string>()
                    {
                        "ElementNames",
                        "IntArrays",
                        "ItemData",
                        "LineElements",
                        "LineLooks",
                        "MapData",
                        "MultipointItems",
                        "NameTranslations",
                        "PointElements",
                        "PointLists",
                        "PointLooks",
                        "PolygonElements",
                        "PolygonLooks",
                        "Strings"
                    };

                    _cppGeneratedFileNames.Add(mapData.parameters.mapId, cppFileName);

                    foreach (XmlPolygonElement element in mapData.elementList.polygonElementList)
                    {
                        cppFileName.Add(String.Format("_{0}", element.id.Substring(2)));
                    }

                    try
                    {
                        string[] kmlFiles = Directory.GetFiles(String.Format("{0}/Lines", mapData.parameters.kmlDir), "*.kml");
                        foreach (string kmlFile in kmlFiles)
                        {
                            string str = Path.GetFileNameWithoutExtension(kmlFile);
                            if (!isExcluded(str, mapData.parameters.exclusionTag))
                            {
                                cppFileName.Add(String.Format("_{0}", str).Replace("#", "_").Replace("!", "_").Replace("~", "_"));
                            }
                        }
                    }
                    catch(DirectoryNotFoundException)
                    {
                    }

                    try
                    {
                        string[] kmlFiles = Directory.GetFiles(String.Format("{0}/Polygons/Polygons_Lines", mapData.parameters.kmlDir), "*.kml");
                        foreach (string kmlFile in kmlFiles)
                        {
                            string str = Path.GetFileNameWithoutExtension(kmlFile);
                            if (!isExcluded(str, mapData.parameters.exclusionTag))
                            {
                                cppFileName.Add(String.Format("_{0}", str).Replace("#", "_").Replace("!", "_").Replace("~", "_"));
                            }
                        }
                    }
                    catch(DirectoryNotFoundException)
                    {
                    }

                    try
                    {
                        string[] kmlFiles = Directory.GetFiles(String.Format("{0}/Polygons/Polygons_Polygons", mapData.parameters.kmlDir), "*.kml");
                        foreach (string kmlFile in kmlFiles)
                        {
                            string str = Path.GetFileNameWithoutExtension(kmlFile);
                            if (!isExcluded(str, mapData.parameters.exclusionTag))
                            {
                                cppFileName.Add(String.Format("_{0}", str).Replace("#", "_").Replace("!", "_").Replace("~", "_"));
                            }
                        }
                    }
                    catch(DirectoryNotFoundException)
                    {
                    }

                    // tmp debug
                    /*foreach (string x in cppFileName)
                    {
                        using (StreamWriter y = new StreamWriter(String.Format("../../../../NodeJS/map_server2/Data/generated_code/{0}/{1}.cpp_",
                            mapData.parameters.mapId, x)))
                        {
                            y.WriteLine();
                        }
                    }*/
                }
            }

            return 0;
        }

        private static bool isExcluded(string name, string exclusionTag)
        {
            if (exclusionTag == null) return false;
            string[] idList = name.Split('_');
            return idList.Contains(exclusionTag);
        }

        private static void addMapDataProcessingBuild()
        {
            Rule rule = new Rule();
            rule.Target = "Mono/MapDataProcessing/bin/Debug/MapDataProcessing.exe";
            rule.Commands.Add("cd Mono/MapDataProcessing && mdtool build MapDataProcessing.csproj");
            rule.Dependencies.Add("Mono/MapDataProcessing/MapDataProcessing.csproj");

            string[] files = Directory.GetFiles("../../../../VisualStudio/MapDataProcessing", "*.cs");
            foreach (string file in files)
            {
                rule.Dependencies.Add(file.Substring(12));
            }

            files = Directory.GetFiles("../../../../VisualStudio/MapDataProcessing", "*.kml");
            foreach (string file in files)
            {
                rule.Dependencies.Add(file.Substring(12));
            }

            _ruleList.Add(rule);
        }

        private static void addXsdGeneration()
        {
            Rule rule = new Rule();
            rule.Target = "VisualStudio/MapDataProcessing/XmlMapData.cs";
            rule.Dependencies.Add("VisualStudio/MapDataProcessing/XmlMapData.xsd");
            rule.Commands.Add("cd VisualStudio/MapDataProcessing && xsd XmlMapData.xsd /classes");
            _ruleList.Add(rule);
        }

        private static void generateMakefile()
        {
            using (StreamWriter file = new StreamWriter("../../../../Makefile", false))
            {
                foreach (Rule rule in _ruleList)
                {
                    file.Write(String.Format("{0}: ", rule.Target));
                    file.WriteLine(String.Join(" ", rule.Dependencies));
                    foreach (string command in rule.Commands) file.WriteLine("\t{0}", command);
                    file.WriteLine();
                }
            }
        }
    }
}
