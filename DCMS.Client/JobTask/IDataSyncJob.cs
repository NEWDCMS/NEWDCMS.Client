using System;
using System.Collections.Generic;
using System.Text;

namespace Wesley.Client
{
    //public interface IDataSync
    //{
    //    void Run();
    //}

    public interface IDataSyncListenableWorkerCall
    {
        void Call();
    }
}
