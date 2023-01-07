using System.Collections.Generic;
using System.Text;
using AnimArch.Visualization.Animating;
using AnimArch.Visualization.Diagrams;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AnimArch.Parsing
{
    public static class JsonParser
    {
        public static JObject OpenDiagram()
        {
            var encoding = Encoding.GetEncoding("UTF-8");
            var jsonText = System.IO.File.ReadAllText(AnimationData.Instance.GetDiagramPath(), encoding);
            return JObject.Parse(jsonText);
        }

        public static string SaveDiagramToJson()
        {
            ClassEditor.ReverseClassesGeometry();
            var serializedClasses =
                JsonConvert.SerializeObject(DiagramPool.Instance.ClassDiagram.GetClassList());
            ClassEditor.ReverseClassesGeometry();

            var serializedRelations = JsonConvert.SerializeObject(DiagramPool.Instance.ClassDiagram.GetRelationList());
            var serializedDiagram = "{\"classes\":" + serializedClasses + ",\"relations\":" + serializedRelations + "}";
            return serializedDiagram;
        }

        public static List<Class> ParseClasses(JObject jsonObject)
        {
            var classes = jsonObject["classes"];
            return classes.ToObject<List<Class>>();
        }

        public static List<Relation> ParseRelations(JObject jsonObject)
        {
            var relations = jsonObject["relations"];
            return relations.ToObject<List<Relation>>();
        }
    }
}