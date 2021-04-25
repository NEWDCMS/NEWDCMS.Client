using Wesley.Client.Models;
using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
namespace Wesley.Client.Pages
{

    public partial class PopRadioButtonView : PopupPage
    {

        public PopRadioButtonView(string title, string message, Func<Task<List<PopData>>> bindData)
        {
            try
            {
                InitializeComponent();
                AnnounceBindingContext(title, message, bindData);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string> { { "Exception:", "PopRadioButtonView" } });
            }

        }

        private void AnnounceBindingContext(string title, string message, Func<Task<List<PopData>>> bindData)
        {
            var vm = (PopRadioButtonViewModel)this.BindingContext;
            if (vm != null)
            {
                vm.Title = title;
                vm.Message = message;
                if (bindData != null)
                    ((ICommand)vm.Load).Execute(bindData);
            }
        }

        public event EventHandler<PopData> Completed;

        /// <summary>
        /// 确认
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Confirm_Clicked(object sender, EventArgs e)
        {
            PopData select = null;
            var vm = (PopRadioButtonViewModel)this.BindingContext;
            if (vm != null)
            {
                select = vm.Options.Where(s => s.Selected).Select(s => s).FirstOrDefault();
            }

            if (select == null)
            {
                vm.Alert("请选择项目！");
                return;
            }

            Completed?.Invoke(this, select);
            CloseAllPopup();
        }

        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Clicked(object sender, EventArgs e)
        {
            Completed?.Invoke(this, new PopData());
            CloseAllPopup();
        }


        protected override bool OnBackgroundClicked()
        {
            CloseAllPopup();
            return false;
        }

        private async void CloseAllPopup()
        {
            await PopupNavigation.Instance.PopAllAsync();
        }

        private void RepeaterOptions_Loaded(object sender, EventArgs e)
        {
            var vm = (PopRadioButtonViewModel)this.BindingContext;
            if (vm != null)
            {
                //if (vm.Options.Count == 0)
                //    vm.IsNull = false;
            }
        }
    }

}