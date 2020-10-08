using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OALProgramControl
{
    public class EXETestGUIImplementation : EXETestGUI
    {
        Dictionary<string, int> HighLightedClasses = new Dictionary<string, int>();
        Dictionary<string, int> HighLightedRelationships = new Dictionary<string, int>();
        private readonly object Syncer = new object();

        public bool HighlightRelationship(string RelationshipName)
        {
            lock (Syncer)
            {
                if (!this.HighLightedRelationships.ContainsKey(RelationshipName))
                {
                    this.HighLightedRelationships.Add(RelationshipName, 0);
                }
                this.HighLightedRelationships[RelationshipName]++;
            }

            this.PrintState();
            return true;
        }

        public bool HighlightClass(string ClassName)
        {
            lock (Syncer)
            {
                if (!this.HighLightedClasses.ContainsKey(ClassName))
                {
                    this.HighLightedClasses.Add(ClassName, 0);
                }
                this.HighLightedClasses[ClassName]++;
            }

            this.PrintState();
            return true;
        }

        public bool UnHighlightRelationship(string RelationshipName)
        {
            lock (Syncer)
            {
                if (!this.HighLightedRelationships.ContainsKey(RelationshipName))
                {
                    return false;
                }
                this.HighLightedRelationships[RelationshipName]--;
                if (this.HighLightedRelationships[RelationshipName] == 0)
                {
                    this.HighLightedRelationships.Remove(RelationshipName);
                }
            }

            this.PrintState();
            return true;
        }

        public bool UnHighlightClass(string ClassName)
        {
            lock (Syncer)
            {
                if (!this.HighLightedClasses.ContainsKey(ClassName))
                {
                    return false;
                }
                this.HighLightedClasses[ClassName]--;
                if (this.HighLightedClasses[ClassName] == 0)
                {
                    this.HighLightedClasses.Remove(ClassName);
                }
            }

            this.PrintState();
            return true;
        }

        private void PrintState()
        {
            String Relationships;
            String Classes;
            lock (Syncer)
            {
                Relationships = String.Join(", ", this.HighLightedRelationships.ToArray());
                Classes = String.Join(", ", this.HighLightedClasses.ToArray());
            }

            Console.Write("Classes: " + Classes + "\n" + "Relationships: " + Relationships + "\n" + "-------------------------------------------------\n");
        }
    }
}
