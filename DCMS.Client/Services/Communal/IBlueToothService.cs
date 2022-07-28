using DCMS.Client.Models;
using DCMS.Client.Models.Global;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DCMS.Client.Services
{
    /// <summary>
    /// 用于表示蓝牙服务
    /// </summary>
    public interface IBlueToothService
    {
        Task<bool> ConnectDevice(Printer printer);
        void Disconnect();
        void PrintStop();
        void Print(AbstractBill bill, int type = 58, int repeatPrintNum = 1);
        ObservableRangeCollection<Printer> PairedDevices();
        Task<bool> GetPermissionsAsync();
    }
}
