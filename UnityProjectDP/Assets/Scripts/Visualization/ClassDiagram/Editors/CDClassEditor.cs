using OALProgramControl;

namespace AnimArch.Visualization.Diagrams
{
    public static class CDClassEditor
    {
        public static CDClass CreateNode(Class newClass)
        {
            CDClass cdClass = null;
            var i = 0;
            var baseName = newClass.Name;
            while (cdClass == null)
            {
                newClass.Name = baseName + (i == 0 ? "" : i.ToString());
                cdClass = OALProgram.Instance.ExecutionSpace.SpawnClass(newClass.Name);
                i++;
                if (i > 1000)
                {
                    break;
                }
            }

            return cdClass;
        }
    }
}