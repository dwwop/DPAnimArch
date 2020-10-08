using System.Collections;
using System.Collections.Generic;
using System;
using System.Xml;
using System.Xml.XPath;
using UnityEngine;
using System.Text;

public static class XMIParser
{
    static String path = @"C:/AnimArch/exportedXMI.xml";
    //static String path = "C:/TPFIIT/visitor_v2_s_client_metodami.xml";
    static String currDiagramIDPath = @"C:/AnimArch/currDiagramID.txt";
    public static List<String> parseCurrentDiagramElementsIDs()
    {
        //Document
        XmlDocument xmlDoc = new XmlDocument(); // Create an XML document object
        var encoding = Encoding.GetEncoding("UTF-8");
        //System.IO.StreamReader reader = new System.IO.StreamReader(path, System.Text.Encoding.GetEncoding("Windows-1252"), true);
        string xmlText= System.IO.File.ReadAllText(path, encoding);
        // XmlTextReader xmlReader = new XmlTextReader(reader);
        xmlDoc.LoadXml(xmlText); // Load the XML document from the specified file

        string currDiagramID = System.IO.File.ReadAllText(currDiagramIDPath, encoding);

        //pridaj vsetky elementy patriace current otvorenemu diagramu
        List<string> currDiagramElements = new List<string>();
        XmlNodeList diagrams = xmlDoc.GetElementsByTagName("diagrams");
        XmlNodeList d = diagrams[0].ChildNodes;

        foreach (XmlNode diagram in d)
        {
            XmlNodeList diagramNodes = diagram.ChildNodes;

            foreach (XmlNode node in diagramNodes)
            {
                if (node.Name == "model")
                {
                    if ((node.Attributes["localID"].Value == currDiagramID) == false) break;
                }

                if (node.Name == "elements")
                {
                    XmlNodeList diagramElements = node.ChildNodes;
                    foreach (XmlNode diagramElement in diagramElements)
                    {
                        currDiagramElements.Add(diagramElement.Attributes["subject"].Value);
                    }
                }

            }

        }

        return currDiagramElements;
    }

    public static List<Class> ParseClasses()
    {
        List<Class> XMIClassList = new List<Class>();
        XmlDocument xmlDoc = new XmlDocument(); // Create an XML document object
        // Load the XML document from the specified file
        string xml = System.IO.File.ReadAllText(path);
        xmlDoc.LoadXml(xml);
        string currDiagramID = System.IO.File.ReadAllText(currDiagramIDPath);
        //string currDiagramID = System.IO.File.ReadAllText(currDiagramIDPath);
        List<String> currDiagramElements = parseCurrentDiagramElementsIDs();

        // Get elements
        XmlNodeList classNodeList = xmlDoc.GetElementsByTagName("UML:Class");
        XmlNodeList classIndices = xmlDoc.GetElementsByTagName("UML:DiagramElement");
        XmlNodeList elementClass = xmlDoc.GetElementsByTagName("elements");

        
        XmlNodeList elementsClass = elementClass[0].ChildNodes;
        //XmlNodeList geometryElements = elementClass[1].ChildNodes; //todo fix for current diagram
        XmlNodeList geometryElements = null;

        for(int i = 1; i < elementClass.Count; i++)
        {
            XmlNode parentDiagram = elementClass[i].ParentNode;
            XmlNodeList parentDiagramNodes = parentDiagram.ChildNodes;
            foreach (XmlNode node in parentDiagramNodes)
            {
                if(node.Name == "model")
                {
                    if(node.Attributes["localID"].Value == currDiagramID)
                    {
                        geometryElements = elementClass[i].ChildNodes;
                        break;
                    }
                }
            }
        }


        for (int i = 1; i < elementsClass.Count; i++)
        {
            Class XMIClass = new Class();
            XMIClass.Type = elementsClass[i].Attributes["xmi:type"].Value;
            try
            {
                XMIClass.Name = elementsClass[i].Attributes["name"].Value;
            }
            catch { }
            XMIClass.XmiId = elementsClass[i].Attributes["xmi:idref"].Value;
            //  XMIClassList.Add(XMIClass);
            if (!(XMIClass.Type.Equals("uml:Interface") || XMIClass.Type.Equals("uml:Class"))) continue;

            if (elementsClass[i].HasChildNodes)
            {
                XmlNodeList test = elementsClass[i].ChildNodes;
                foreach (XmlNode node in test)
                {
                    if (node.Name == "attributes")
                    {
                        XmlNodeList attributes = node.ChildNodes;
                        XMIClass.Attributes = new List<Attribute>();

                        foreach (XmlNode attribute in attributes)
                        {
                            string type = "";
                            string id = attribute.Attributes["xmi:idref"].Value;
                            string name = attribute.Attributes["name"].Value;

                            XmlNodeList attributeAttributes = attribute.ChildNodes;
                            foreach (XmlNode attributeAttribute in attributeAttributes)
                            {
                                if (attributeAttribute.Name == "properties")
                                {

                                    type = attributeAttribute.Attributes["type"].Value;

                                }
                            }

                            Attribute attr = new Attribute(id, name, type);
                            XMIClass.Attributes.Add(attr);
                        }
                    }
                    else if (node.Name == "operations")
                    {
                        XmlNodeList operations = node.ChildNodes;
                        XMIClass.Methods = new List<Method>();

                        foreach (XmlNode operation in operations)
                        {
                            string name = operation.Attributes["name"].Value;
                            string id = operation.Attributes["xmi:idref"].Value;
                            string returnType = "";
                            List<string> arguments = new List<string>();

                            Method oper = new Method();
                            oper.Name = name;
                            oper.Id = id;


                            XmlNodeList operationProperties = operation.ChildNodes;

                            foreach (XmlNode operationProperty in operationProperties)
                            {
                                //TODO
                                if (operationProperty.Name == "parameters")
                                {

                                    XmlNodeList parameters = operationProperty.ChildNodes;

                                    int count = 0;
                                    foreach (XmlNode parameter in parameters)
                                    {
                                        if (count++ > 0)
                                        {
                                            var nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
                                            nsmgr.AddNamespace("xmi", "http://schema.omg.org/spec/XMI/2.1");
                                            var xmi_id = parameter.Attributes["xmi:idref"].Value;
                                            var refnode = xmlDoc.SelectSingleNode("//*[@xmi:id='"+xmi_id+"']", nsmgr);
                                           // XmlNode refnode = xmlDoc.SelectSingleNode("//*[@xmi:id='EAID_855A1E81_E810_4e59_B919_A02E42179E4F']", nsmgr);
    
                                          
                                            var argname = refnode.Attributes["name"].Value;
                                            

                                            XmlNodeList parameterProperties = parameter.ChildNodes;

                                            foreach (XmlNode parameterProperty in parameter)
                                            {
                                                if (parameterProperty.Name == "properties")
                                                {
                                                    //arguments.Add(parameterProperty.Attributes["type"].Value);
                                                    var argpom = parameterProperty.Attributes["type"].Value;
                                                    arguments.Add(argpom + " " + argname);
                                                }
                                            }
                                        }
                                    }
                                    //Method oper = new Method(name, id, returnType);   
                                    oper.arguments = arguments;

                                    /*Debug.Log(oper.Name);
                                    Debug.Log(oper.ReturnValue);
                                    if(oper.arguments.Count > 0)
                                        Debug.Log(oper.arguments[0]);*/

                                }

                                if (operationProperty.Name == "type")
                                {
                                    returnType = operationProperty.Attributes["type"].Value;
                                    /*Method oper = new Method(name, id, returnType);
                                    XMIClass.Methods.Add(oper);*/
                                    oper.ReturnValue = returnType;

                                    XMIClass.Methods.Add(oper);
                                }

                            }

                        }
                    }



                }
            }
            if(currDiagramElements.Contains(XMIClass.XmiId)) XMIClassList.Add(XMIClass);
        }

        for (int i = 0; i < geometryElements.Count; i++)
        {
            string subject = geometryElements[i].Attributes["subject"].Value;
            foreach (var item in XMIClassList)
            {
                if (item.XmiId == subject)
                {
                    item.Geometry = geometryElements[i].Attributes["geometry"].Value;

                    string[] words = item.Geometry.Split(';');
                    foreach (string word in words)
                    {
                        //aby som nedostal IndexOutOfRangeException... 
                        if (String.IsNullOrEmpty(word)) break;
                        string[] values = word.Split('=');
                        switch (values[0])
                        {
                            case "Left":
                                item.Left = int.Parse(values[1]);
                                break;
                            case "Top":
                                item.Top = int.Parse(values[1]);
                                break;
                            case "Right":
                                item.Right = int.Parse(values[1]);
                                break;
                            case "Bottom":
                                item.Bottom = int.Parse(values[1]);
                                break;
                        }
                    }
                }
            }
        }
        return XMIClassList;
    }

    public static List<Relation> ParseRelations()
    {
        List<Relation> connetorClassesList = new List<Relation>();

        XmlDocument xmlDoc = new XmlDocument(); // Create an XML document object
        var encoding = Encoding.GetEncoding("UTF-8");
        string xmlText = System.IO.File.ReadAllText(path, encoding);
        // XmlTextReader xmlReader = new XmlTextReader(reader);
        xmlDoc.LoadXml(xmlText); // Load the XML document from the specified file
        List<String> currDiagramElements = parseCurrentDiagramElementsIDs();

        XmlNodeList connectorClass = xmlDoc.GetElementsByTagName("connectors");


        foreach (XmlNode connector in connectorClass)
        {


            if (connector.HasChildNodes)
            {
                XmlNodeList childNodeList = connector.ChildNodes;
                //prejde vsetky <connector>
                foreach (XmlNode childNode in childNodeList)
                {
                    Relation xmiConnetorClass = new Relation();
                    xmiConnetorClass.ConnectorXmiId = childNode.Attributes["xmi:idref"].Value;

                    if (childNode.HasChildNodes)
                    {
                        XmlNodeList childNodeNextList = childNode.ChildNodes;

                        foreach (XmlNode nodeNext in childNodeNextList)
                        {
                            String name = nodeNext.Name;

                            switch (name)
                            {
                                case "source":
                                    xmiConnetorClass.SourceXmiId = nodeNext.Attributes["xmi:idref"].Value;
                                    xmiConnetorClass.SourceName = nodeNext.Name; //wtf?

                                    if (nodeNext.HasChildNodes)
                                    {
                                        XmlNodeList childNodeSource = nodeNext.ChildNodes;
                                        foreach (XmlNode sourceNode in childNodeSource)
                                        {
                                            //inside <source>
                                            String innerName = sourceNode.Name;
                                            switch (innerName)
                                            {
                                                case "model":
                                                    xmiConnetorClass.SourceModelEaLocalId = Int32.Parse(sourceNode.Attributes["ea_localid"].Value);
                                                    xmiConnetorClass.SourceModelType = sourceNode.Attributes["type"].Value; //!!!
                                                    try
                                                    {
                                                        xmiConnetorClass.SourceModelName = sourceNode.Attributes["name"].Value;
                                                    }
                                                    catch {}
                                                    break;
                                                case "role":
                                                    xmiConnetorClass.SourceRoleVisibility = sourceNode.Attributes["visibility"].Value;
                                                    xmiConnetorClass.SourceRoleTargetScope = sourceNode.Attributes["targetScope"].Value;
                                                    break;
                                                case "type":
                                                    xmiConnetorClass.SourceTypeContainment = sourceNode.Attributes["containment"].Value;
                                                    try
                                                    {
                                                        xmiConnetorClass.SourceMultiplicity = sourceNode.Attributes["multiplicity"].Value;
                                                    }
                                                    catch {}
                                                    break;
                                                case "modifiers":
                                                    xmiConnetorClass.SourceModifiersIsOrdered = bool.Parse(sourceNode.Attributes["isOrdered"].Value);
                                                    xmiConnetorClass.SourceModifiersChangeable = sourceNode.Attributes["changeable"].Value;
                                                    xmiConnetorClass.SourceModifiersisNavigablee = bool.Parse(sourceNode.Attributes["isNavigable"].Value);
                                                    break;
                                                case "style":
                                                    xmiConnetorClass.SourceStyleValue = sourceNode.Attributes["value"].Value;
                                                    break;
                                            }
                                        }

                                    }
                                    break;
                                case "target":
                                    xmiConnetorClass.TargetXmiId = nodeNext.Attributes["xmi:idref"].Value;
                                    xmiConnetorClass.TargetName = nodeNext.Name;

                                    if (nodeNext.HasChildNodes)
                                    {
                                        XmlNodeList childNodeTarget = nodeNext.ChildNodes;
                                        foreach (XmlNode targetNode in childNodeTarget)
                                        {
                                            //inside <target>
                                            String innerName = targetNode.Name;
                                            switch (innerName)
                                            {
                                                case "model":
                                                    xmiConnetorClass.TargetModelEaLocalId = Int32.Parse(targetNode.Attributes["ea_localid"].Value);
                                                    xmiConnetorClass.TargetModelType = targetNode.Attributes["type"].Value; //!!!
                                                    try
                                                    {
                                                        xmiConnetorClass.TargetModelName = targetNode.Attributes["name"].Value;
                                                    }
                                                    catch
                                                    {

                                                    }
                                                    break;
                                                case "role":
                                                    xmiConnetorClass.TargetRoleVisibility = targetNode.Attributes["visibility"].Value;
                                                    xmiConnetorClass.TargetRoleTargetScope = targetNode.Attributes["targetScope"].Value;
                                                    break;
                                                case "type":
                                                    xmiConnetorClass.TargetTypeAggregation = targetNode.Attributes["aggregation"].Value;
                                                    xmiConnetorClass.TargetTypeContainment = targetNode.Attributes["containment"].Value;
                                                    try
                                                    {
                                                        xmiConnetorClass.TargetMultiplicity = targetNode.Attributes["multiplicity"].Value;
                                                    }
                                                    catch {}
                                                    break;
                                                case "modifiers":
                                                    xmiConnetorClass.TargetModifiersIsOrdered = bool.Parse(targetNode.Attributes["isOrdered"].Value);
                                                    xmiConnetorClass.TargetModifiersChangeable = targetNode.Attributes["changeable"].Value;
                                                    xmiConnetorClass.TargetModifiersisNavigablee = bool.Parse(targetNode.Attributes["isNavigable"].Value);
                                                    break;
                                                case "style":
                                                    xmiConnetorClass.TargetStyleValue = targetNode.Attributes["value"].Value;
                                                    break;
                                            }
                                        }

                                    }
                                    break;
                                case "model":
                                    xmiConnetorClass.ModelEaLocalId = int.Parse(nodeNext.Attributes["ea_localid"].Value);
                                    break;
                                case "properties":
                                    xmiConnetorClass.PropertiesEa_type = nodeNext.Attributes["ea_type"].Value;
                                    xmiConnetorClass.ProperitesDirection = nodeNext.Attributes["direction"].Value;
                                    break;
                                case "modifiers":
                                    xmiConnetorClass.ModifiersIsRoot = bool.Parse(nodeNext.Attributes["isRoot"].Value);
                                    xmiConnetorClass.ModifiersIsLeaf = bool.Parse(nodeNext.Attributes["isLeaf"].Value);
                                    break;
                                case "appearance":
                                    xmiConnetorClass.AppearanceLinemode = nodeNext.Attributes["linemode"].Value;
                                    xmiConnetorClass.AppearanceLineColor = nodeNext.Attributes["linecolor"].Value;
                                    xmiConnetorClass.AppearanceLinewidth = nodeNext.Attributes["linewidth"].Value;
                                    xmiConnetorClass.AppearanceSeqno = nodeNext.Attributes["seqno"].Value;
                                    xmiConnetorClass.AppearanceHeadStyle = nodeNext.Attributes["headStyle"].Value;
                                    xmiConnetorClass.AppearanceLineStyle = nodeNext.Attributes["lineStyle"].Value;
                                    break;
                                case "labels":
                                    try
                                    {
                                        xmiConnetorClass.Label = nodeNext.Attributes["mt"].Value;
                                    }
                                    catch {}
                                    break;
                                case "extendedProperties":
                                    break;
                            }
                        }
                    }
                    if ((xmiConnetorClass.SourceModelType.Equals("Class") || xmiConnetorClass.SourceModelType.Equals("Interface")) &&
                        (xmiConnetorClass.TargetModelType.Equals("Interface") || xmiConnetorClass.TargetModelType.Equals("Class")))
                    {
                        if (currDiagramElements.Contains(xmiConnetorClass.ConnectorXmiId))
                        {
                            connetorClassesList.Add(xmiConnetorClass);
                        }
                    }
                }
            }
        }
        return connetorClassesList;
    }
}
