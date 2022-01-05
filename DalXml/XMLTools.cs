using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.IO;
using DalApi;
using DO;
namespace Dal
{
    class XMLTools
    {
        static string dir = @"xml\";
        static XMLTools()
        {
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }
        #region SaveLoadWithXElement
        //save a specific xml file according the name- throw exception in case of problems..
        //for the using with XElement..
        public static void SaveListToXMLElement(XElement rootElem, string filePath)
        {
            try
            {
                rootElem.Save(dir+filePath);
            }
            catch (Exception ex)
            {
                throw new XMLFileLoadCreateException("fail to create xml file:"+ filePath, ex);
            }
        }
        //load a specific xml file according the name- throw exception in case of problems..
        //for the using with XElement..
        public static XElement LoadListFromXMLElement(string filePath)
        {
            try
            {
                if (File.Exists(dir+filePath))
                {
                    return XElement.Load(dir+ filePath);
                }
                else
                {
                    FileStream file = new FileStream(dir + filePath, FileMode.Open);                   
                    XElement rootElem = new XElement(dir+filePath);
                    rootElem.Save(dir + filePath);
                    file.Close();
                    return rootElem;
                }
            }
            catch (Exception ex)
            {
                        throw new XMLFileLoadCreateException("fail to load xml file:"+ filePath, ex);
            }
        }
        #endregion

        #region SaveLoadWithXMLSerializer
        //save a complete listin a specific file- throw exception in case of problems..
        //for the using with XMLSerializer..
        public static void SaveToXMLSerializer<T>(List<T> list, string filePath)
        {
            try
            {
                FileStream file = new FileStream(dir + filePath, FileMode.Create);
                XmlSerializer x = new XmlSerializer(list.GetType());
                x.Serialize(file, list);
                file.Close();
            }
            catch(Exception ex)
            {
                throw new XMLFileLoadCreateException("fail to create xml file:" + filePath, ex);
            }
        }
        //load a complete list from a specific file- throw exception in case of problems..
        //for the using with XMLSerializer..
        public static List<T> LoadListFromXMLSerializer<T>(string filePath)
        {
            try
            {
                if (File.Exists(dir + filePath))
                {
                    List<T> list;
                    XmlSerializer x = new XmlSerializer(typeof(List<T>));
                    FileStream file = new FileStream(dir + filePath, FileMode.Open);
                    list = (List<T>)x.Deserialize(file);
                    file.Close();
                    return list;
                }
                else
                    return new List<T>();
            }
            catch (Exception ex)
            {
                throw new XMLFileLoadCreateException("fail to load xml file:" + filePath, ex);
            }
        }
        #endregion
    }
}
