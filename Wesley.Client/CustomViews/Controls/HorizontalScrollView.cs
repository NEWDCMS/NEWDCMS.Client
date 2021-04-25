using System.Collections;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;
namespace Wesley.Client.CustomViews
{
    /// <summary>
    /// 自定义水平滚动条
    /// </summary>
    public class HorizontalScrollView : ScrollView
    {
        public List<ViewCell> Cells = new List<ViewCell>();

        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create("ItemsSource", typeof(IEnumerable), typeof(HorizontalScrollView),
                default(IEnumerable));

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly BindableProperty ItemTemplateProperty =
            BindableProperty.Create("ItemTemplate", typeof(DataTemplate), typeof(HorizontalScrollView),
                default(DataTemplate));

        public DataTemplate ItemTemplate
        {
            get => (DataTemplate)GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        public static readonly BindableProperty SelectedCommandProperty =
            BindableProperty.Create("SelectedCommand", typeof(ICommand), typeof(HorizontalScrollView), null);

        public ICommand SelectedCommand
        {
            get => (ICommand)GetValue(SelectedCommandProperty);
            set => SetValue(SelectedCommandProperty, value);
        }

        public static readonly BindableProperty SelectedCommandParameterProperty =
            BindableProperty.Create("SelectedCommandParameter", typeof(object), typeof(HorizontalScrollView), null);

        public object SelectedCommandParameter
        {
            get => GetValue(SelectedCommandParameterProperty);
            set => SetValue(SelectedCommandParameterProperty, value);
        }

        public void Render()
        {
            if (ItemTemplate == null || ItemsSource == null)
            {
                return;
            }

            var layout = new StackLayout
            {
                Orientation = Orientation == ScrollOrientation.Vertical
                    ? StackOrientation.Vertical
                    : StackOrientation.Horizontal
            };

            foreach (var item in ItemsSource)
            {
                if (!(ItemTemplate.CreateContent() is ViewCell viewCell))
                {
                    continue;
                }

                object[] items = { item, viewCell.View };
                viewCell.View.BindingContext = item;
                viewCell.View.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = SelectedCommand,
                    CommandParameter = SelectedCommandParameter ?? items,
                    NumberOfTapsRequired = 1
                });

                Cells.Add(viewCell);
                layout.Children.Add(viewCell.View);
            }

            Content = layout;
        }
    }
}