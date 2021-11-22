using DCMS.Client.ViewModels;
using Xamarin.Forms;

namespace DCMS.Client.Pages
{
	public partial class LazyTestView : ContentView
	{
		public LazyTestView()
		{
			InitializeComponent();

			Build();
			NormalTestViewModel.Current.LoadedViews += "LazyView Loaded \n";
		}

		void Build()
		{
			for (var i = 0; i < 117; i++)
			{
				var box = new BoxView
				{
					BackgroundColor = i % 2 == 0 ? Color.Blue : Color.Fuchsia
				};

				uniformGrid.Children.Add(box);
			}
		}
	}
}