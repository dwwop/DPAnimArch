using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OALProgramControl
{
    public interface EXETestGUI
    {
        bool HighlightClass(String ClassName);
        bool UnHighlightClass(String ClassName);

        bool HighlightRelationship(String RelationshipName);
        bool UnHighlightRelationship(String RelationshipName);
    }
}
