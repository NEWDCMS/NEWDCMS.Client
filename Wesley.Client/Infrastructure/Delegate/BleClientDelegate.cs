using Shiny;
using Shiny.BluetoothLE;
using System.Threading.Tasks;

namespace DCMS.Client
{
    public class BleClientDelegate : IBleDelegate
    {
        readonly CoreDelegateServices services;
        public BleClientDelegate(CoreDelegateServices services) => this.services = services;

        public async Task OnAdapterStateChanged(AccessState state)
        {
            if (state == AccessState.Disabled && this.services.AppSettings.UseNotificationsBle)
                await this.services.SendNotification("BLE State", "已经打开蓝牙");
        }

        public Task OnConnected(IPeripheral peripheral) => Task.WhenAll(
            this.services.SendNotification("蓝牙设备已连接", $"{peripheral.Name} 已连接",
                x => x.UseNotificationsBle
            )
        );
    }
}
