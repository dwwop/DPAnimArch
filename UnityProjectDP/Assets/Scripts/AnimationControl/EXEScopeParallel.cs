using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace OALProgramControl
{
    public class EXEScopeParallel : EXEScope
    {
        private List<EXEScope> Threads { get; }
        private object MyThreadEndSyncer { get; }
        private int ActiveThreadCount;
        public EXEScopeParallel() : base()
        {
            this.Threads = new List<EXEScope>();
            this.MyThreadEndSyncer = new object();
            this.ActiveThreadCount = 0;
        }
        public EXEScopeParallel(EXEScope[] Threads) : base()
        {
            this.Threads = new List<EXEScope>();
            foreach (EXEScope Thread in Threads)
            {
                this.AddThread(Thread);
            }
            this.MyThreadEndSyncer = new object();
            this.ActiveThreadCount = 0;
        }

        public void AddThread(EXEScope Thread)
        {
            Thread.SetSuperScope(this);
            this.Threads.Add(Thread);
        }
        public override Boolean SynchronizedExecute(OALProgram OALProgram, EXEScope Scope)
        {
            Boolean Success = this.Execute(OALProgram, Scope);
            return Success;
        }
        public override Boolean Execute(OALProgram OALProgram, EXEScope Scope)
        {
            this.OALProgram = OALProgram;
            this.OALProgram.ThreadSyncer.RegisterThread((uint)this.Threads.Count);
            EXEScopeParallel ParallelScope = this;
            Boolean Success = true;

            lock (this.MyThreadEndSyncer)
            {
                this.ActiveThreadCount = this.Threads.Count;
            }

            OALProgram.ThreadSyncer.UnregisterThread();

            foreach (EXEScope ThreadScope in this.Threads)
            {
                new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = false;

                    Boolean MySuccess = ThreadScope.SynchronizedExecute(OALProgram, ParallelScope);

                    OALProgram.ThreadSyncer.UnregisterThread();

                    lock (ParallelScope.MyThreadEndSyncer)
                    {
                        Success &= MySuccess;
                        ParallelScope.ActiveThreadCount--;
                        if (ParallelScope.ActiveThreadCount == 0)
                        {
                            Monitor.PulseAll(this.MyThreadEndSyncer);
                        }
                    }
                }).Start();
            }

            lock (this.MyThreadEndSyncer)
            {
                while (this.ActiveThreadCount > 0)
                {
                    Monitor.Wait(this.MyThreadEndSyncer);
                }
            }

            OALProgram.ThreadSyncer.RegisterThread(1);

            return Success;
        }
        public override Boolean PreExecute(AnimationCommandStorage ACS, OALProgram OALProgram, EXEScope Scope)
        {
            this.OALProgram = OALProgram;
            this.OALProgram.ThreadSyncer.RegisterThread((uint)this.Threads.Count);
            EXEScopeParallel ParallelScope = this;
            Boolean Success = true;

            lock (this.MyThreadEndSyncer)
            {
                this.ActiveThreadCount = this.Threads.Count;
            }

            OALProgram.ThreadSyncer.UnregisterThread();

            foreach (EXEScope ThreadScope in this.Threads)
            {
                new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = false;

                    Boolean MySuccess = ThreadScope.PreExecute(ACS, OALProgram, ParallelScope);

                    OALProgram.ThreadSyncer.UnregisterThread();

                    lock (ParallelScope.MyThreadEndSyncer)
                    {
                        Success &= MySuccess;
                        ParallelScope.ActiveThreadCount--;
                        if (ParallelScope.ActiveThreadCount == 0)
                        {
                            Monitor.PulseAll(this.MyThreadEndSyncer);
                        }
                    }
                }).Start();
            }

            lock (this.MyThreadEndSyncer)
            {
                while (this.ActiveThreadCount > 0)
                {
                    Monitor.Wait(this.MyThreadEndSyncer);
                }
            }

            OALProgram.ThreadSyncer.RegisterThread(1);

            return Success;
        }
        public override bool PropagateControlCommand(LoopControlStructure PropagatedCommand)
        {
            return false;
        }

        public override String ToCode(String Indent = "")
        {
            String Result = Indent + "par\n";
            if (this.Threads != null)
            {
                foreach (EXEScope Thread in this.Threads)
                {
                    Result += Indent + "\tthread\n";
                    foreach (EXECommand Command in Thread.Commands)
                    {
                        Result += Command.ToCode(Indent + "\t\t");
                    }
                    Result += Indent + "\tend thread;\n";
                }
            }
            Result += Indent + "end par;\n";
            return Result;
        }
    }
}