using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MapColors
{
    class MapColorsMain
    {
        static XmlQuizData _quizData;
        static Dictionary<string, Department> _departments = new Dictionary<string, Department>();
        static List<Department> _departmentsAlgoOrder = new List<Department>();
        static List<Department> _departmentsXmlOrder = new List<Department>();
        static SortedDictionary<string, Department> _unsortedDepartments = new SortedDictionary<string, Department>();

        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("usage: MapColors.exe path");
            }
            else
            {
                int result = 0;

                if (result == 0) result = loadData(args[0]);
                if (result == 0) result = sortDepartments();
                if (result == 0) result = color();
                if (result == 0) result = printResults();

                if (result == 0) Console.WriteLine("MapColors terminated successfully for file {0}", args[0]);
                else Console.WriteLine("MapColors terminated with errors for file {0}", args[0]);
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static int printResults()
        {
            int i;
            int n = _departmentsXmlOrder.Count;
            for (i = 0; i < n; ++i)
            {
                Console.WriteLine(String.Format("{0} {1}", _departmentsXmlOrder[i].Id, (int)_departmentsXmlOrder[i].Color));
            }
            Console.WriteLine();

            int[] colorCounts = new int[5];
            int[] colorCounts0 = new int[5];

            for (i = 0; i < 5; ++i)
            {
                colorCounts[i] = 0;
                colorCounts0[i] = 0;
            }

            n = _departmentsAlgoOrder.Count;
            for (i = 0; i < n; ++i)
            {
                Department dep = _departmentsAlgoOrder[i];

                Console.WriteLine(String.Format("{0} {1}", dep.Id, dep.Color));
                ++colorCounts[(int)dep.Color];

                if (i == n - 1 || dep.Region != _departmentsAlgoOrder[i + 1].Region)
                {
                    Console.WriteLine();
                    Console.WriteLine(_departmentsAlgoOrder[i].Region.Id);
                    //Console.WriteLine("NONE: {0}", colorCounts[0] - colorCounts0[0]);
                    Console.WriteLine("RED: {0}", colorCounts[1] - colorCounts0[1]);
                    Console.WriteLine("GREEN: {0}", colorCounts[2] - colorCounts0[2]);
                    Console.WriteLine("BLUE: {0}", colorCounts[3] - colorCounts0[3]);
                    Console.WriteLine("YELLOW: {0}", colorCounts[4] - colorCounts0[4]);
                    Console.WriteLine();
                    int j;
                    for (j = 0; j < 5; ++j) colorCounts0[j] = colorCounts[j];
                }
            }

            Console.WriteLine();
            Console.WriteLine("NONE: {0}", colorCounts[0]);
            Console.WriteLine("RED: {0}", colorCounts[1]);
            Console.WriteLine("GREEN: {0}", colorCounts[2]);
            Console.WriteLine("BLUE: {0}", colorCounts[3]);
            Console.WriteLine("YELLOW: {0}", colorCounts[4]);

            if (colorCounts[0] != 0)
            {
                Console.WriteLine("Error in algorithm implementation: All departments should have a color");
                return -1;
            }

            return 0;
        }

        static int color()
        {
            int index = 0;
            int k = 0;

            while (index != _departments.Count)
            {
                if (k % 1000000 == 0) Console.WriteLine(index);
                ++k;

                Department department = _departmentsAlgoOrder[index];

                bool colorOk = false;

                while (!colorOk)
                {
                    colorOk = department.setColor();

                    if (colorOk)
                    {
                        colorOk = checkColor(department);
                        if (colorOk) ++index;
                    }
                    else
                    {
                        colorOk = true;
                        --index;
                        department.clear();

                        if (index == 0)
                        {
                            Console.WriteLine("The coloring algorithm failed");
                            return -1;
                        }
                    }
                }
            }

            return 0;
        }

        static bool checkColor(Department department)
        {
            foreach(Department department2 in department.AdjacentDepartments.Keys)
            {
                if (department2.Color == department.Color) return false;
            }

            int[] colorCounts = new int[4];
            int i;
            for (i = 0; i < 4; ++i) colorCounts[i] = 0;

            foreach (Department dep in department.Region.Departments)
            {
                if (dep.Color == ColorEnum.NONE) return true;
                if (dep.Color == ColorEnum.RED) ++colorCounts[0];
                if (dep.Color == ColorEnum.GREEN) ++colorCounts[1];
                if (dep.Color == ColorEnum.BLUE) ++colorCounts[2];
                if (dep.Color == ColorEnum.YELLOW) ++colorCounts[3];
            }

            int min = 10000, max = 0;
            for (i = 0; i < 4; ++i)
            {
                if (colorCounts[i] < min) min = colorCounts[i];
                if (colorCounts[i] > max) max = colorCounts[i];
            }

            return max - min <= 1;
        }

        static int sortDepartments()
        {
            Department department = getNearestUnsortedDepartment(null);
            
            while (department != null)
            {
                _departmentsAlgoOrder.Add(department);
                _unsortedDepartments.Remove(department.Id);
                department = getNearestUnsortedDepartment(department);
            }

            return 0;
        }

        static Department getNearestUnsortedDepartment(Department department)
        {
            if (department == null) return _departments["el_Calvados"];

            double dMin = 1e20;
            Department nearestDepartment = null;

            foreach(Department department2 in _unsortedDepartments.Values)
            {
                if (department2 != department)
                {
                    double d = department.distance(department2);
                    if (d < dMin)
                    {
                        dMin = d;
                        nearestDepartment = department2;
                    }
                }
            }

            return nearestDepartment;
        }

        static int loadData(string path)
        {
            XmlReader reader;
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessIdentityConstraints;

            try
            {
                reader = XmlReader.Create(path, settings);
            }
            catch (Exception e)
            {
                Console.WriteLine("Fail to open file {0}", path);
                Console.WriteLine(e.Message);
                return -1;
            }

            Console.WriteLine("Reading configuration file " + path + " ...");

            XmlSerializer serializer = new XmlSerializer(typeof(XmlQuizData));

            try
            {
                _quizData = (XmlQuizData)serializer.Deserialize(reader);
            }
            catch (Exception e)
            {
                Console.WriteLine("Fail to deserialize content of file {0}", path);
                Console.WriteLine(e.Message);
                if (e.InnerException != null) Console.WriteLine(e.InnerException.Message);
                return -1;
            }

            reader.Close();

            foreach (XmlElement xmlElement in _quizData.elementList)
            {
                if (xmlElement.type == "et_Departement")
                {
                    Department departement = new Department();
                    departement.Id = xmlElement.id;

                    double latitude = 1000.0;
                    double longitude = 1000.0;

                    foreach (XmlNumericalAttribute attribute in xmlElement.numericalAttributeList)
                    {
                        if (attribute.type == "nt_Latitude") latitude = attribute.value;
                        if (attribute.type == "nt_Longitude") longitude = attribute.value;
                    }

                    if (latitude == 1000.0)
                    {
                        Console.WriteLine(String.Format("Error: Department {0} has no latitude", xmlElement.id));
                        return -1;
                    }

                    if (longitude == 1000.0)
                    {
                        Console.WriteLine(String.Format("Error: Department {0} has no longitude", xmlElement.id));
                        return -1;
                    }

                    departement.Latitude = latitude;
                    departement.Longitude = longitude;

                    _departmentsXmlOrder.Add(departement);
                    _departments.Add(xmlElement.id, departement);
                    _unsortedDepartments.Add(xmlElement.id, departement);
                }
            }

            foreach (XmlElement xmlElement in _quizData.elementList)
            {
                if (xmlElement.type == "et_Departement")
                {
                    Department department1 = _departments[xmlElement.id];

                    foreach (XmlRelation relation in xmlElement.relationList)
                    {
                        if (relation.type == "rNN_Departement_DepartementsLimitrophes")
                        {
                            Department department2 = _departments[relation.linkedElement];
                            department1.AdjacentDepartments.Add(department2, 0);
                        }
                    }
                }
            }

            foreach (XmlElement xmlElement in _quizData.elementList)
            {
                if (xmlElement.type == "et_Region")
                {
                    Region region = new Region();
                    region.Id = xmlElement.id;

                    foreach (XmlRelation relation in xmlElement.relationList)
                    {
                        if (relation.type == "r1N_Region_Departements")
                        {
                            Department department = _departments[relation.linkedElement];

                            if (department.Region != null)
                            {
                                Console.WriteLine(String.Format("Error: Department {0} has several regions ({1} and {2})", department.Id, department.Region.Id, region.Id));
                                return -1;
                            }

                            department.Region = region;
                            region.Departments.Add(department);
                        }
                    }
                }

            }

            foreach(Department department in _departments.Values)
            {
                if (department.Region == null)
                {
                    Console.WriteLine(String.Format("Error: Department {0} has no region", department.Id));
                    return -1;
                }
            }

            _departments["el_MoselleDepartement"].AdjacentDepartments.Add(_departments["el_Vosges"], 0);
            _departments["el_Vosges"].AdjacentDepartments.Add(_departments["el_MoselleDepartement"], 0);
            _departments["el_MayenneDepartement"].AdjacentDepartments.Add(_departments["el_LoireAtlantique"], 0);
            _departments["el_LoireAtlantique"].AdjacentDepartments.Add(_departments["el_MayenneDepartement"], 0);
            _departments["el_Calvados"].AdjacentDepartments.Add(_departments["el_SeineMaritime"], 0);
            _departments["el_SeineMaritime"].AdjacentDepartments.Add(_departments["el_Calvados"], 0);
            _departments["el_BouchesDuRhone"].AdjacentDepartments.Add(_departments["el_AlpesHauteProvence"], 0);
            _departments["el_AlpesHauteProvence"].AdjacentDepartments.Add(_departments["el_BouchesDuRhone"], 0);
            _departments["el_HautRhin"].AdjacentDepartments.Add(_departments["el_HauteSaone"], 0);
            _departments["el_HauteSaone"].AdjacentDepartments.Add(_departments["el_HautRhin"], 0);

            return 0;
        }
    }
}
