using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OALProgramControl
{
    class EXEInstanceIDSeed
    {
        private static EXEInstanceIDSeed Instance;
        private int Seed;
        private readonly Object Lock;
        private EXEInstanceIDSeed()
        {
            EXEInstanceIDSeed.Instance = null;
            this.Seed = 0;
            this.Lock = new Object();
        }

        public static EXEInstanceIDSeed GetInstance()
        {
            if (EXEInstanceIDSeed.Instance == null)
            {
                EXEInstanceIDSeed.Instance = new EXEInstanceIDSeed();
            }

            return EXEInstanceIDSeed.Instance;
        }

        public int GenerateID()
        {
            int Result;
            lock (EXEInstanceIDSeed.Instance.Lock) {
                Result = ++this.Seed;
            }
            return Result;
        }
        public void ResetID()
        {
            lock (EXEInstanceIDSeed.Instance.Lock)
            {
                this.Seed = 0;
            }
        }
    }
}
