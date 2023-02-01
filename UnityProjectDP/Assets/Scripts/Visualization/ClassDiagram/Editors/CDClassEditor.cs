using OALProgramControl;

namespace AnimArch.Visualization.Diagrams
{
    public class CDClassEditor
    {
        private static string CurrentClassName(string name, ref CDClass TempCdClass)
        {
            TempCdClass = null;
            var i = 0;
            var currentName = name;
            var baseName = name;
            while (TempCdClass == null)
            {
                currentName = baseName + (i == 0 ? "" : i.ToString());
                TempCdClass = OALProgram.Instance.ExecutionSpace.SpawnClass(currentName);
                i++;
                if (i > 1000)
                {
                    break;
                }
            }

            return currentName;
        }
        public static CDClass CreateNode(Class newClass)
        {
            CDClass tempCdClass = null;
            CurrentClassName(newClass.Name, ref tempCdClass);
            
            return tempCdClass;
        }
    }
}