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
        static Dictionary<string, Department> _departements = new Dictionary<string, Department>();

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

                if (result == 0) Console.WriteLine("MapColors terminated successfully for file {0}", args[0]);
                else Console.WriteLine("MapColors terminated with errors for file {0}", args[0]);
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
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

                    foreach (XmlRelation relation in xmlElement.relationList)
                    {
                        if (relation.type == "rNN_Departement_DepartementsLimitrophes")
                        {

                        }
                    }

                    _departements.Add(xmlElement.id, departement);
                }
            }

            foreach (XmlElement xmlElement in _quizData.elementList)
            {
                if (xmlElement.type == "et_Departement")
                {
                    Department department1 = _departements[xmlElement.id];

                    foreach (XmlRelation relation in xmlElement.relationList)
                    {
                        if (relation.type == "rNN_Departement_DepartementsLimitrophes")
                        {
                            Department department2 = _departements[relation.linkedElement];
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
                            Department department = _departements[relation.linkedElement];

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

            foreach(Department department in _departements.Values)
            {
                if (department.Region == null)
                {
                    Console.WriteLine(String.Format("Error: Department {0} has no region", department.Id));
                    return -1;
                }
            }

            return 0;
        }
    }
}
