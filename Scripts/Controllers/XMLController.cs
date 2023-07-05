using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;
using System.Text;

public static class XMLController
{
    public struct XmlNodeAttributes
    {
        public string title;
        public string value;

        public XmlNodeAttributes(string _title, string _value) { title = _title; value = _value; }
    }

    //Private
    private static XmlDocument xmlDoc;
    private static string saveSlot = "";
    private static XmlElement rootNode = null;
    private static string xmlPath = @"C://Users//Ryan Santos//Desktop//Unity Project Learning//Projects//Fear of The Dark//teste.xml";

    static void InitializeXmlDoc()
    {
        xmlDoc = new XmlDocument();
        //TODO: not fixed path
        if (!File.Exists(xmlPath))
        {
            File.Create(xmlPath);
        }
        xmlDoc.Load(xmlPath);
        rootNode = xmlDoc.DocumentElement;

        if (rootNode != null)
        {
            XmlNode temporaryNode = rootNode.SelectSingleNode("Temporary");

            if (temporaryNode == null)
            {
                temporaryNode = xmlDoc.CreateElement("Temporary");
                rootNode.AppendChild(temporaryNode);
            }
            else
            {
                temporaryNode.RemoveAll();
            }

            xmlDoc.Save(xmlPath);
        }
    }

    public static void SetSaveSlot(string _slot)
    {
        saveSlot = _slot;
    }

    public static string GetSaveSlot()
    {
        return saveSlot;
    }

    public static string GetSavedSceneName()
    {
        XmlNode slot = rootNode.SelectSingleNode(saveSlot);
        return slot.Attributes["SavedSceneName"].Value;
    }

    public static void SetSavedSceneName(string _sceneName)
    {
        XmlNode root = xmlDoc.SelectSingleNode(saveSlot);
        root.Attributes["SavedSceneName"].Value = _sceneName;
    }

    public static List<XmlNode> LoadSceneObjects(string _sceneName, string _parentNode, string _objectName)
    {
        if (xmlDoc == null) { InitializeXmlDoc(); }

        List<XmlNode> attributesList = new List<XmlNode>();
        XmlNode temporaryNode = rootNode.SelectSingleNode("Temporary");
        XmlNode sceneNode = temporaryNode.SelectSingleNode(_sceneName);
        XmlNode parentNode = null;

        if (sceneNode != null)
        {
            parentNode = sceneNode.SelectSingleNode(_parentNode);

            if (parentNode != null)
            {
                foreach (XmlNode node in parentNode)
                {
                    if (node.Attributes != null && node.Name == _objectName)
                    {
                        attributesList.Add(node);
                    }
                }
            }
        }

        return attributesList;
    }

    public static List<XmlNode> LoadSlotObjects(string _parentNode, string _objectName)
    {
        if (xmlDoc == null) { InitializeXmlDoc(); }

        List<XmlNode> objectsList = new List<XmlNode>();
        XmlNode temporaryNode = rootNode.SelectSingleNode("Temporary");

        if (temporaryNode != null)
        {
            foreach (XmlNode sceneNode in temporaryNode)
            {
                XmlNode parentNode = sceneNode.SelectSingleNode(_parentNode);

                if (parentNode != null)
                {
                    foreach (XmlNode childNode in parentNode)
                    {
                        if (childNode.Name.Equals(_objectName))
                        {
                            objectsList.Add(childNode);
                        }
                    }
                }
            }
        }

        return objectsList;
    }

    public static List<XmlNodeAttributes> LoadObjectState(string _sceneName, string _parentNode, string _objectName, string _objectID)
    {
        if (xmlDoc == null) { InitializeXmlDoc(); }

        List<XmlNodeAttributes> attributesList = new List<XmlNodeAttributes>();
        XmlNode temporaryNode = rootNode.SelectSingleNode("Temporary");
        XmlNode sceneNode = temporaryNode.SelectSingleNode(_sceneName);

        if (sceneNode != null)
        {
            XmlNode parentNode = sceneNode.SelectSingleNode(_parentNode);

            foreach (XmlNode node in parentNode)
            {
                if (node.Attributes != null && node.Attributes["id"] != null)
                {
                    if (node.Attributes["id"].Value.Equals(_objectID))
                    {
                        foreach (XmlAttribute attribute in node.Attributes)
                        {
                            attributesList.Add(new XmlNodeAttributes(attribute.Name, attribute.Value));
                        }
                    }
                }
            }
        }

        return attributesList;
    }

    public static void ClearTemporaryScene(string _sceneName)
    {
        if (xmlDoc == null) { InitializeXmlDoc(); }

        XmlNode temporaryNode = rootNode.SelectSingleNode("Temporary");
        XmlNode sceneNode = temporaryNode.SelectSingleNode(_sceneName);

        if (sceneNode != null)
        {
            sceneNode.RemoveAll();
        }
    }

    public static void SaveObjectState(string _sceneName, string _parentNode, string _objectName, string _objectID, List<XmlNodeAttributes> _attributesList)
    {
        if (xmlDoc == null) { InitializeXmlDoc(); }

        XmlNode temporaryNode = rootNode.SelectSingleNode("Temporary");
        XmlNode sceneNode = temporaryNode.SelectSingleNode(_sceneName);
        XmlNode parentNode = null;
        XmlNode oldChild = null;
        XmlElement newChild = null;

        //If the scene node exists we try to get the parent node
        if (sceneNode != null)
        {
            parentNode = sceneNode.SelectSingleNode(_parentNode);
        }
        if (sceneNode == null)
        {//If the scene do not exists we need to create it together with the parent node for the object

            sceneNode = xmlDoc.CreateElement(_sceneName);
            temporaryNode.AppendChild(sceneNode);
        }

        if (parentNode == null)
        {
            parentNode = xmlDoc.CreateElement(_parentNode);
            sceneNode.AppendChild(parentNode);
        }

        foreach (XmlNode node in parentNode)
        {
            if (node.Attributes != null && node.Attributes["id"] != null)
            {
                if (node.Attributes["id"].Value == _objectID)
                {
                    oldChild = node;
                }
            }
        }

        newChild = xmlDoc.CreateElement(_objectName);

        foreach (XmlNodeAttributes attribute in _attributesList)
        {
            newChild.SetAttribute(attribute.title, attribute.value);
        }

        //If the object exists we just update it, else we create
        if (oldChild == null)
        {   
            parentNode.AppendChild(newChild);
        }
        else
        {
            parentNode.ReplaceChild(newChild, oldChild);
        }

        xmlDoc.Save(xmlPath);
    }

    public static void SaveGame(string _sceneName, string _savedText)
    {
        if (xmlDoc == null) { InitializeXmlDoc(); }

        XmlNode temporaryNode = rootNode.SelectSingleNode("Temporary");
        XmlNode savedSlotNode = rootNode.SelectSingleNode(saveSlot);

        if (savedSlotNode == null)
        {
            XmlElement saveSlotNode = xmlDoc.CreateElement(saveSlot);
            saveSlotNode.SetAttribute("SavedSceneName", _sceneName);
            saveSlotNode.SetAttribute("SavedText", _savedText);
            rootNode.AppendChild(saveSlotNode);
            savedSlotNode = saveSlotNode;
        }
        else
        {
            foreach (XmlNode node in savedSlotNode)
            {
                savedSlotNode.RemoveChild(node);
            }
            savedSlotNode.Attributes["SavedSceneName"].Value = _sceneName;
        }

        foreach (XmlNode node in temporaryNode.ChildNodes)
        {
            savedSlotNode.AppendChild(node.Clone());
        }

        temporaryNode.RemoveAll();

        xmlDoc.Save(xmlPath);
    }

    public static void AddSceneNode(string _sceneName)
    {
        if (xmlDoc == null) { InitializeXmlDoc(); }

        XmlNode parentNode = xmlDoc.SelectSingleNode("Temporary");
        XmlNode sceneNode = parentNode.SelectSingleNode(_sceneName);

        if (sceneNode == null)
        {
            XmlNode newNode = xmlDoc.CreateElement(_sceneName);
            parentNode.AppendChild(newNode);
        }
    }

    public static void LoadGame()
    {
        if (xmlDoc == null){ InitializeXmlDoc(); }

        XmlNode temporaryNode = rootNode.SelectSingleNode("Temporary");
        XmlNode savedNode = rootNode.SelectSingleNode(saveSlot);

        temporaryNode.RemoveAll();

        foreach (XmlNode node in savedNode)
        {
            temporaryNode.AppendChild(node.Clone());
        }

        xmlDoc.Save(xmlPath);
    }

    public static string GetSavedSlotText(string _slotToLoad)
    {
        if (xmlDoc == null) { InitializeXmlDoc(); }

        XmlNode savedNode = rootNode.SelectSingleNode(_slotToLoad);

        if (savedNode != null)
        {
            return savedNode.Attributes["SavedText"].Value;
        }
        else
        {
            return "Empty";
        }
    }

    public static string GetSceneFromSlot()
    {
        if (xmlDoc == null) { InitializeXmlDoc(); }

        XmlNode savedNode = rootNode.SelectSingleNode(saveSlot);

        if (savedNode != null)
        {
            return savedNode.Attributes["SavedSceneName"].Value;
        }
        else
        {
            return "";
        }
    }
}
