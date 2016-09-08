using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CleanKml
{
    class CleanKmlMain
    {
        static string _pointTemplatePath;
        static string _lineTemplatePath;
        static string _polygonTemplatePath;
        static List<string> fileList = new List<string>();

        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("usage: CleanKml.exe path");
            }
            else
            {
                browse(args[0]);

                if (_pointTemplatePath == null) Console.WriteLine("!!! Point template not found");
                if (_lineTemplatePath == null) Console.WriteLine("!!! Line template not found");
                if (_polygonTemplatePath == null) Console.WriteLine("!!! Polygon template not found");

                if (_pointTemplatePath != null && _lineTemplatePath != null && _polygonTemplatePath != null)
                {
                    foreach (string path in fileList) cleanFile(path);
                }
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void browse(string path)
        {
            FileAttributes attr;

            try
            {
                attr = File.GetAttributes(path);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine(path + " not found");
                return;
            }

            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                Console.WriteLine("Browsing " + path);
                List<string> entries = new List<string>(Directory.EnumerateFileSystemEntries(path));
                foreach (var entry in entries) browse(entry);
            }
            else
            {
                string fileName = Path.GetFileName(path);
                if (String.Equals(fileName, "pointtemplate.kml", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Point template: " + path);
                    _pointTemplatePath = path;
                }
                else if (String.Equals(fileName, "linetemplate.kml", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Line template: " + path);
                    _lineTemplatePath = path;
                }
                else if (String.Equals(fileName, "polygontemplate.kml", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Polygon template: " + path);
                    _polygonTemplatePath = path;
                }
                else if (String.Equals(Path.GetExtension(path), ".kml", StringComparison.OrdinalIgnoreCase))
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo("git.exe");
                    startInfo.UseShellExecute = false;
                    startInfo.RedirectStandardInput = true;
                    startInfo.RedirectStandardOutput = true;
                    startInfo.Arguments = "status --porcelain " + path;

                    Process process = new Process();
                    process.StartInfo = startInfo;
                    process.Start();

                    string fileStatus = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();

                    // Todo: test 'fileStatus' value to decide if file must be added

                    Console.WriteLine("Adding " + path);
                    fileList.Add(path);
                }
                else
                {
                    Console.WriteLine("Extension is not '.kml': " + path + " ignored");
                }
            }
        }

        static void cleanFile(string path)
        {
            Console.WriteLine("Cleaning " + path);

            XmlDocument inputDocument = new XmlDocument();
            try
            {
                inputDocument.Load(path);
            }
            catch (Exception)
            {
                Console.WriteLine("!!! Invalid file, " + path + " ignored");
                return;
            }

            XmlNodeList coordinatesList = inputDocument.GetElementsByTagName("coordinates");

            if (coordinatesList.Count == 0)
            {
                Console.WriteLine("!!! No 'coordinates' tags found, file " + path + " ignored");
                return;
            }

            if (coordinatesList.Count > 1) Console.WriteLine("!!! More than one 'coordinates' tags in file " + path + ". Only the first one is taken into account");

            String templatePath = null;
            XmlNode coodinatesNode = coordinatesList.Item(0);
            XmlNode parent = coodinatesNode.ParentNode;
            while (parent != null)
            {
                if (parent.Name == "Point")
                {
                    templatePath = _pointTemplatePath;
                    break;
                }
                else if (parent.Name == "LineString")
                {
                    templatePath = _lineTemplatePath;
                    break;
                }
                else if (parent.Name == "Polygon")
                {
                    templatePath = _polygonTemplatePath;
                    break;
                }
                parent = parent.ParentNode;
            }

            if (templatePath == null)
            {
                Console.WriteLine("!!! 'coordinates' tag is not incuded in either 'Point' or 'LineString' or 'Polygon' tag. " + path + " ignored");
                return;
            }

            XmlDocument outputDocument = new XmlDocument();
            try
            {
                outputDocument.Load(templatePath);
            }
            catch (Exception)
            {
                Console.WriteLine("!!! Invalid template file " + templatePath + ". File" + path + " ignored");
                return;
            }
        }
    }
}
