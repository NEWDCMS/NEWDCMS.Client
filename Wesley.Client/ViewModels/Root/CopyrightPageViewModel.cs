using Wesley.Client.Models;
using Wesley.Client.Services;

using Prism.Navigation;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;


namespace Wesley.Client.ViewModels
{
    public class CopyrightPageViewModel : ViewModelBase
    {
        [Reactive] public IList<Copyright> CopyrightSeries { get; set; } = new ObservableCollection<Copyright>();
        public CopyrightPageViewModel(INavigationService navigationService,
            IDialogService dialogService) : base(navigationService, dialogService)
        {

            Title = "版权信息";

            this.Load = ReactiveCommand.Create(() =>
            {
                var lists = new List<Copyright>()
               {
                    new Copyright() { Name = "Acr.UserDialogs" ,Version="7.1.0.442" , Licence = "MIT", IsChenged = "否" },
                    new Copyright() { Name = "akavache" ,Version="6.10.20" , Licence = "MIT", IsChenged = "否" },
                    new Copyright() { Name = "EasyNetQ" ,Version="5.2.0" , Licence = "MIT", IsChenged = "否" },
                    new Copyright() { Name = "Microsoft.CognitiveServices.Speech", Version="1.12.1", Licence = "MIT", IsChenged = "否" },
                    new Copyright() { Name = "Microsoft.CSharp" ,Version="4.7.0", Licence = "MIT", IsChenged = "否" },
                    new Copyright() { Name = "NETStandard.Library" ,Version="2.0.3" , Licence = "MIT", IsChenged = "否" },
                    new Copyright() { Name = "Newtonsoft.Json", Version="12.0.3", Licence = "MIT", IsChenged = "否" },
                    new Copyright() { Name = "NLog" ,Version="4.7.2" , Licence = "MIT", IsChenged = "否" },
                    new Copyright() { Name = "Prism.DryIoc.Forms", Version="7.2.0.1422", Licence = "MIT", IsChenged = "否" },
                    new Copyright() { Name = "Prism.Plugin.Popups", Version="7.2.0.1046" , Licence = "MIT", IsChenged = "否" },
                    new Copyright() { Name = "ReactiveUI.Fody", Version="11.4.17" , Licence = "MIT", IsChenged = "否" },
                    new Copyright() { Name = "ReactiveUI.Validation", Version="1.4.15" , Licence = "MIT", IsChenged = "否" },
                    new Copyright() { Name = "Refractored.XamForms.PullToRefresh" ,Version="2.4.1" , Licence = "MIT", IsChenged = "否" },
                    new Copyright() { Name = "SkiaSharp.Views.Forms" ,Version="1.68.3" , Licence = "MIT", IsChenged = "否" },
                    new Copyright() { Name = "System.Reactive" ,Version="4.4.1" , Licence = "MIT", IsChenged = "否" },
                    new Copyright() { Name = "System.Reactive.Linq" ,Version="4.4.1" , Licence = "MIT", IsChenged = "否" },
                    new Copyright() { Name = "System.ValueTuple" ,Version="4.5.0", Licence = "MIT", IsChenged = "否" },
                    new Copyright() { Name = "Xam.Plugin.Connectivity" ,Version="3.2.0" , Licence = "MIT", IsChenged = "否" },
                    new Copyright() { Name = "Xam.Plugin.Iconize.FontAwesome", Version="3.5.0.114" , Licence = "MIT", IsChenged = "否" },
                    new Copyright() { Name = "Xam.Plugin.Media" ,Version="5.0.1" , Licence = "MIT", IsChenged = "否" },
                    new Copyright() { Name = "Xam.Plugins.Forms.ImageCircle" ,Version="3.0.0.5" , Licence = "MIT", IsChenged = "否" },
                    new Copyright() { Name = "Xam.Plugins.Settings", Version="3.1.1" , Licence = "MIT", IsChenged = "否" },
                    new Copyright() { Name = "Xamarin.Essentials", Version="1.5.3.2" , Licence = "MIT", IsChenged = "否" },
                    new Copyright() { Name = "Xamarin.FFImageLoading.Forms", Version="2.4.11.982" , Licence = "MIT", IsChenged = "否" },
                    new Copyright() { Name = "Xamarin.FFImageLoading.Svg.Forms", Version="2.4.11.982" , Licence = "MIT", IsChenged = "否" },
                    new Copyright() { Name = "Xamarin.FFImageLoading.Transformations", Version="2.4.11.982", Licence = "MIT", IsChenged = "否" },
                    new Copyright() { Name = "Xamarin.Forms", Version="4.6.0.967" , Licence = "MIT", IsChenged = "否" },
                    new Copyright() { Name = "Xamarin.Forms.PancakeView", Version="1.4.2" , Licence = "MIT", IsChenged = "否" },
                    new Copyright() { Name = "ZXing.Net.Mobile.Forms", Version="2.4.1" , Licence = "MIT", IsChenged = "否" },
               };
                CopyrightSeries = new ObservableCollection<Copyright>(lists);
            });
            BindBusyCommand(Load);
            this.ExceptionsSubscribe();
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            ((ICommand)Load)?.Execute(null);
        }
    }
}
