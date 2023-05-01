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
    public class JsonParser : Parser
    {
        private JObject _document;

        public override void LoadDiagram()
        {
            var encoding = Encoding.GetEncoding("UTF-8");
            var jsonText = System.IO.File.ReadAllText(AnimationData.Instance.GetDiagramPath(), encoding);
            _document = JObject.Parse(jsonText);
        }

        public override string SaveDiagram()
        {
            ParsedEditor.ReverseNodesGeometry();
            var classes = DiagramPool.Instance.ClassDiagram.GetClassList();
            var relations = DiagramPool.Instance.ClassDiagram.GetRelationList();
            var serializedDiagram = JsonConvert.SerializeObject(new { classes, relations });
            ParsedEditor.ReverseNodesGeometry();
            return serializedDiagram;
        }

        public override List<Class> ParseClasses()
        {
            var classes = _document["classes"];
            return classes?.ToObject<List<Class>>();
        }

        public override List<Relation> ParseRelations()
        {
            var relations = _document["relations"];
            return relations?.ToObject<List<Relation>>();
        }
    }
}
