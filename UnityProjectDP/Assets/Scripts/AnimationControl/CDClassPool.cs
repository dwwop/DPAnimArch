using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OALProgramControl
{
    public class CDClassPool
    {
        private long ClassIDPrefix;
        public List<CDClass> ClassPool { get; }

        public CDClassPool()
        {
            this.ClassPool = new List<CDClass>();
        }

        public CDClassInstance GetClassInstanceById(String ClassName, long Id)
        {
            CDClassInstance Result = null;
            foreach (CDClass Class in this.ClassPool)
            {
                if (String.Equals(Class.Name, ClassName))
                {
                    Result = Class.GetInstanceByID(Id);
                    break;
                }
            }

            return Result; 
        }
        public CDClass SpawnClass(String Name)
        {
            CDClass NewClass = null;
            if (!ClassExists(Name))
            {
                NewClass = new CDClass(Name);
                this.ClassPool.Add(NewClass);
            }
            return NewClass;
        }

        public Boolean ClassesExist(List<String> ClassNames)
        {
            Boolean Result = true;

            foreach (String ClassName in ClassNames)
            {
                if (!ClassExists(ClassName))
                {
                    Result = false;
                }
            }

            return Result;
        }

        public Boolean ClassExists(String ClassName)
        {
            Boolean Result = false;

            foreach (CDClass Class in this.ClassPool)
            {
                if (Class.Name.Equals(ClassName))
                {
                    Result = true;
                    break;
                }
            }

            return Result;
        }

        public CDClass getClassByName(String ClassName)
        {
            CDClass Result = null;

            foreach (CDClass Class in this.ClassPool)
            {
                if (Class.Name.Equals(ClassName))
                {
                    Result = Class;
                    break;
                }
            }

            return Result;
        }

        public Boolean MethodExists(String ClassName, String MethodName)
        {
            if (!ClassExists(ClassName))
            {
                return false;
            }

            CDClass SearchedClass = null;

            foreach (CDClass Class in this.ClassPool)
            {
                if (Class.Name.Equals(ClassName))
                {
                    SearchedClass = Class;
                    break;
                }
            }

            if (SearchedClass == null)
            {
                return false;
            }

            return SearchedClass.MethodExists(MethodName);
        }

        public bool DestroyInstance(String ClassName, long InstanceId)
        {
            bool Result = false;
            CDClass Class = this.getClassByName(ClassName);
            if (Class != null)
            {
                Result = Class.DestroyInstance(InstanceId);
            }

            return Result;
        }

        public Dictionary<String, long> ProduceInstanceDatabase()
        {
            Dictionary<String, long> InstanceDatabase = new Dictionary<String, long>();
            foreach (CDClass Class in this.ClassPool)
            {
                Class.AppendToInstanceDatabase(InstanceDatabase);
            }
            return InstanceDatabase;
        }

        public Dictionary<String, int> ProduceInstanceHistogram()
        {
            Dictionary<String, int> InstanceHistogram = new Dictionary<String, int>();
            foreach (CDClass Class in this.ClassPool)
            {
                InstanceHistogram.Add(Class.Name, Class.InstanceCount());
            }
            return InstanceHistogram;
        }
    }
}