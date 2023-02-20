using AnimArch.Extensions;
using AnimArch.Visualization.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AnimArch.Visualization.Diagrams
{
    public class VisualEditorClient : VisualEditor
    {
        public void AddAttribute(string attributeName, string attributeText, GameObject parentClass)
        {
            var attributeLayoutGroup = GetAttributeLayoutGroup(parentClass);
            var instance = Object.Instantiate(DiagramPool.Instance.classAttributePrefab, attributeLayoutGroup, false);
            instance.name = attributeName;
            instance.transform.Find("AttributeText").GetComponent<TextMeshProUGUI>().text += attributeText;
            instance.GetComponent<AttributeManager>().classTxt =
                GetNodeHeader(parentClass).GetComponent<TextMeshProUGUI>();
            if (UIEditorManager.Instance.active)
                instance.GetComponentsInChildren<Button>(true)
                    .ForEach(x => x.gameObject.SetActive(true));
        }

        public void UpdateAttribute(string oldAttributeName, string newAttributeName, string attributeText, GameObject parentClass)
        {
            var attribute = GetAttributeLayoutGroup(parentClass).Find(oldAttributeName);
            attribute.name = newAttributeName;
            attribute.Find("AttributeText").GetComponent<TextMeshProUGUI>().text = attributeText;
        }

        public void AddMethod(string methodName, string methodText, GameObject parentClass)
        {
            var methodLayoutGroup = GetMethodLayoutGroup(parentClass);
            var instance = Object.Instantiate(DiagramPool.Instance.classMethodPrefab, methodLayoutGroup, false);
            instance.name = methodName;
            instance.transform.Find("MethodText").GetComponent<TextMeshProUGUI>().text += methodText;
            instance.GetComponent<MethodManager>().classTxt =
                GetNodeHeader(parentClass).GetComponent<TextMeshProUGUI>();

            if (UIEditorManager.Instance.active)
                instance.GetComponentsInChildren<Button>(true)
                    .ForEach(x => x.gameObject.SetActive(true));
        }
        public void UpdateMethod(string oldMethodName, string newMethodName, string methodText, GameObject parentClass)
        {
            var method = GetMethodLayoutGroup(parentClass).Find(oldMethodName);
            method.name = newMethodName;
            method.Find("MethodText").GetComponent<TextMeshProUGUI>().text = methodText;
        }


    }
}
