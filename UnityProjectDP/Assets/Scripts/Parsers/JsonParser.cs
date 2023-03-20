using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Visualization.Animation;
using Visualization.ClassDiagram;
using Visualization.ClassDiagram.ClassComponents;
using Visualization.ClassDiagram.Editors;
using Visualization.ClassDiagram.Relations;

namespace Parsers
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
            ParsedEditor.ReverseNodesGeometry();
            var serializedClasses =
                JsonConvert.SerializeObject(DiagramPool.Instance.ClassDiagram.GetClassList());
            ParsedEditor.ReverseNodesGeometry();

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
