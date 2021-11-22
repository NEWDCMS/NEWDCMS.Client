using Wesley.Client.Models;
using Wesley.Client.Services;
using Wesley.Client.ViewModels;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using Rg.Plugins.Popup.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Wesley.Client.Pages
{

    public partial class PopRadioButtonPage : BasePopupPage<PopRadioButtonPageViewModel>
    {
        private readonly string _message;
        private readonly Func<Task<List<PopData>>> _call;

        public PopRadioButtonPage(string title, string message, Func<Task<List<PopData>>> call)
        {
            try
            {
                InitializeComponent();
                Title = title;

                _message = message;
                _call = call;

                var vm = this.ViewModel;
                if (vm != null)
                {
                    vm.Title = title;
                    vm.Message = message;
                    vm.BindDataAction = call;
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string> { { "Exception:", "PopRadioButtonView" } });
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (!(this.BindingContext is PopRadioButtonPageViewModel vm))
            {
                var navigationService = App.Resolve<INavigationService>();
                var dialogService = App.Resolve<IDialogService>();
                vm = new PopRadioButtonPageViewModel(navigationService, dialogService)
                {
                    Title = Title,
                    Message = _message,
                    BindDataAction = _call
                };
                this.ViewModel = vm;
            }

            if (vm != null && vm.BindDataAction != null)
            {
                ((ICommand)vm.Load).Execute(vm.BindDataAction);
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
            var vm = this.ViewModel;
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
            Completed?.Invoke(this, null);
            CloseAllPopup();
        }


        private async void CloseAllPopup()
        {
            if (PopupNavigation.Instance.PopupStack.Count > 0)
                await PopupNavigation.Instance.PopAllAsync();
        }


        protected override bool OnBackgroundClicked()
        {
            Completed?.Invoke(this, null);
            return true;
        }

        protected override bool OnBackButtonPressed()
        {
            Completed?.Invoke(this, null);
            return true;
        }
    }
}