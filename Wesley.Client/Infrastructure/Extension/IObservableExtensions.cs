using System;
using System.Reactive.Disposables;
namespace Wesley.Client.Extension
{
    public static class IObservableExtensions
    {
        public static TDisposable DisposeWith<TDisposable>(this TDisposable observable, CompositeDisposable disposables) where TDisposable : class, IDisposable
        {
            if (observable != null)
            {
                disposables.Add(observable);
            }

            return observable;
        }
    }

    //public static class ViewModelLocationProvider
    //{

    //    public static Func<Type, Type> DefaultViewTypeToViewModelTypeResolver =
    //       viewType =>
    //       {
    //           var viewName = viewType.FullName;
    //           viewName = viewName.Replace(".Views.", ".ViewModels.");
    //           var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
    //           var suffix = viewName.EndsWith("View") ? "Model" : "ViewModel";
    //           var viewModelName = String.Format(CultureInfo.InvariantCulture, "{0}{1}, {2}", viewName, suffix, viewAssemblyName);
    //           return Type.GetType(viewModelName);
    //       };


    //    public Type GetModel()
    //    {
    //        var vm = ViewModelLocationProvider.DefaultViewTypeToViewModelTypeResolver = viewType =>
    //        {
    //            var viewModelTypeName = viewType.FullName.Replace("Page", "ViewModel");
    //            var viewModelType = Type.GetType(viewModelTypeName);
    //            return viewModelType;
    //        };
    //        var ytt = vm.Invoke();
    //    }
    //}
}
