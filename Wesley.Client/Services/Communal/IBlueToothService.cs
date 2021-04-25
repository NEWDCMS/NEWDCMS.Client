using Wesley.Client.Models;
using Wesley.Client.Models.Global;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Wesley.Client.Services
{
    /// <summary>
    /// 用于表示蓝牙服务
    /// </summary>
    public interface IBlueToothService
    {
        void Connect(string name);
        void Disconnect();
        void SendData(AbstractBill data);
        ObservableRangeCollection<Printer> PairedDevices();
        void StartDiscovery();
        Task<bool> GetPermissionsAsync();
    }
}
