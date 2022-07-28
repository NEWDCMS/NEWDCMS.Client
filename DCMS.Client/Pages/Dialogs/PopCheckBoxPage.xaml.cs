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


    public partial class PopCheckBoxPage : BasePopupPage<PopCheckBoxPageViewModel>
    {
        private readonly string _message;
        private readonly Func<Task<List<PopData>>> _call;

        public PopCheckBoxPage(string title, string message, Func<Task<List<PopData>>> call)
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
                Crashes.TrackError(ex);
            }
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (!(this.BindingContext is PopCheckBoxPageViewModel vm))
            {
                var navigationService = App.Resolve<INavigationService>();
                var dialogService = App.Resolve<IDialogService>();
                //var connectivity = App.Resolve<IConnectivity>();
                vm = new PopCheckBoxPageViewModel(navigationService, dialogService)
                {
                    Title = Title,
                    Message = _message,
                    BindDataAction = _call
                };
                this.ViewModel = vm;
            }

            if (vm != null)
            {
                ((ICommand)vm.Load).Execute(vm.BindDataAction);
            }
        }

        public event EventHandler<IList<PopData>> Completed;

        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="args"></param>
        public void Cancel_Clicked(Object Sender, EventArgs args)
        {
            Completed?.Invoke(this, new List<PopData>());
            CloseAllPopup();
        }

        /// <summary>
        /// 确认
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="args"></param>
        public void Confirm_Clicked(Object Sender, EventArgs args)
        {
            var selects = new List<PopData>();
            var vm = this.ViewModel;
            if (vm != null)
            {
                selects = vm.Options.Where(s => s.Selected).Select(s => s).ToList();
            }
            Completed?.Invoke(this, selects);
            CloseAllPopup();
        }

        private async void CloseAllPopup()
        {
            if (PopupNavigation.Instance.PopupStack.Count > 0)
                await PopupNavigation.Instance.PopAllAsync();
        }

        protected override bool OnBackgroundClicked()
        {
            Completed?.Invoke(this, new List<PopData>());
            return true;
        }

        protected override bool OnBackButtonPressed()
        {
            Completed?.Invoke(this, new List<PopData>());
            return true;
        }
    }
}