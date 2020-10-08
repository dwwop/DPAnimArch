using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace OALProgramControl
{
    public class BooleanSyncer
    {
        public object Syncer;
        private bool Value;

        public BooleanSyncer()
        {
            this.Syncer = new object();
            this.Value = false;
        }

        public void SetValue(bool Value)
        {
            lock (this.Syncer)
            {
                this.Value = Value;
                Monitor.PulseAll(this.Syncer);
            }
        }

        public void WaitForTrue()
        {
            lock (this.Syncer)
            {
                while (!this.Value)
                {
                    Monitor.Wait(this.Syncer);
                }
            }
        }
    }
}
