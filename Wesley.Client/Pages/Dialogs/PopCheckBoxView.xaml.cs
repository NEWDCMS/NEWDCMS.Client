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


    public partial class PopCheckBoxView : PopupPage
    {

        public PopCheckBoxView(string title, string message, Func<Task<List<PopData>>> bindData)
        {
            try
            {
                InitializeComponent();
                AnnounceBindingContext(title, message, bindData);
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }

            //var closeTGR = new TapGestureRecognizer();
            //var confirmTGR = new TapGestureRecognizer();
            //closeTGR.Tapped += (s, e) => Cancel_Clicked(s, (TappedEventArgs)e);
            //confirmTGR.Tapped += (s, e) => Confirm_Clicked(s, (TappedEventArgs)e);
            //CalcleBtn.GestureRecognizers.Add(closeTGR);
            //ConfirmBtn.GestureRecognizers.Add(confirmTGR);
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
        }

        private void AnnounceBindingContext(string title, string message, Func<Task<List<PopData>>> bindData)
        {
            var vm = (PopCheckBoxViewModel)this.BindingContext;
            if (vm != null)
            {
                vm.Title = title;
                vm.Message = message;
                ((ICommand)vm.Load).Execute(bindData);
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
            var vm = (PopCheckBoxViewModel)this.BindingContext;
            if (vm != null)
            {
                selects = vm.Options.Where(s => s.Selected).Select(s => s).ToList();
            }
            Completed?.Invoke(this, selects);
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

    }
}