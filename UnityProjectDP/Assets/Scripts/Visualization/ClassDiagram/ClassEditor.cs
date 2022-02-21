using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using AnimArch.Visualization.UI;

namespace AnimArch.Visualization.ClassDiagrams
{
    public class ClassEditor : Singleton<ClassEditor>
    {
        Graph graph;
        int id = 0;
        bool active = false;
        GameObject node1;
        GameObject node2;

        public GameObject methodMenu;
        public GameObject attributeMenu;
        public AttributeMenu atrMenu;
        public MethodMenu mtdMenu;
        public void InitializeCreation()
        {
            graph = ClassDiagram.Instance.CreateGraph();
            active = true;

        }
        public void CreateNode()
        {
            var node = graph.AddNode();
            node.name = "NewClass " + id;
            var background = node.transform.Find("Background");
            var header = background.Find("Header");
            header.GetComponent<TMP_Text>().text = node.name;
            var attributes = background.Find("Attributes");
            var methods = background.Find("Methods");
            RectTransform rc = node.GetComponent<RectTransform>();
            rc.position = new Vector3(100f, 200f, 1);
            id++;

        }
        public void SelectNode(GameObject selected)
        {
            if (active)
            {
                if (node1 == null)
                {
                    node1 = selected;
                    Debug.Log("node 1 added");
                }
                else if (node2 == null)
                {
                    node2 = selected;
                    Debug.Log("node 2 added");
                }
                else
                {
                    node2 = node1;
                    node1 = selected;
                }
            }


        }
        public void DrawRelation()
        {
            if (node1 != null && node2 != null)
            {
                ClassDiagram.Instance.CreateRelationEdge(node1, node2);
                node1 = null;
                node2 = null;
                graph.UpdateGraph();
            }

        }
    }
}