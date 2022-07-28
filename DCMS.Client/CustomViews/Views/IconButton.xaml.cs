using Wesley.Client.Enums;
using Microsoft.AppCenter.Crashes;
using System;
using Xamarin.Forms;

namespace Wesley.Client.CustomViews
{

    [Preserve(AllMembers = true)]
    public partial class IconButton : ContentView
    {
        private bool addedAnimation;

        public static readonly BindableProperty IconMarginProperty =
            BindableProperty.Create(
                "IconMargin",
                typeof(Thickness),
                typeof(IconButton),
                new Thickness(0),
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    ((IconButton)bindable).IconMargin = (Thickness)newValue;
                }
            );

        public Thickness IconMargin
        {
            get { return (Thickness)GetValue(IconMarginProperty); }
            set
            {
                SetValue(IconMarginProperty, value);
                grdIconButton.Margin = value;
            }
        }

        public static readonly BindableProperty IconHeightProperty =
            BindableProperty.Create(
                "IconHeight",
                typeof(double),
                typeof(IconButton),
                0d,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    ((IconButton)bindable).IconHeight = (double)newValue;
                });

        public double IconHeight
        {
            get { return (double)GetValue(IconHeightProperty); }
            set
            {
                SetValue(IconHeightProperty, value);
                lblIcons.HeightRequest = value;
            }
        }

        /*图标字体大小*/
        public static readonly BindableProperty IconSizeProperty =
        BindableProperty.Create(
            "FontSize",
            typeof(double),
            typeof(IconButton),
            0d,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                ((IconButton)bindable).FontSize = (double)newValue;
            }
        );
        public double FontSize
        {
            get { return (double)GetValue(IconSizeProperty); }
            set
            {
                SetValue(IconSizeProperty, value);
                lblIcons.FontSize = value;
            }
        }


        /// <summary>
        /// 描述
        /// </summary>
        public static readonly BindableProperty DescriptionPositionProperty =
            BindableProperty.Create(
                "DescriptionPosition",
                typeof(DescriptionPosition),
                typeof(IconButton),
                DescriptionPosition.None,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    ((IconButton)bindable).DescriptionPosition = (DescriptionPosition)newValue;
                }
            );

        public DescriptionPosition DescriptionPosition
        {
            get { return (DescriptionPosition)GetValue(DescriptionPositionProperty); }
            set
            {
                SetValue(DescriptionPositionProperty, value);
                setPosition(value);
            }
        }

        public static readonly BindableProperty DescriptionSizeProperty =
            BindableProperty.Create(
                "DescriptionSize",
                typeof(double),
                typeof(IconButton),
                0d,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    ((IconButton)bindable).DescriptionSize = (double)newValue;
                }
            );

        public double DescriptionSize
        {
            get { return (double)GetValue(DescriptionSizeProperty); }
            set
            {
                SetValue(DescriptionSizeProperty, value);
                lblDescription.FontSize = value;
            }
        }


        public static readonly BindableProperty DescriptionColorProperty =
            BindableProperty.Create(
                "DescriptionColor",
                typeof(Color),
                typeof(IconButton),
                Color.Black,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    ((IconButton)bindable).DescriptionColor = (Color)newValue;
                }
            );

        public Color DescriptionColor
        {
            get { return (Color)GetValue(DescriptionColorProperty); }
            set
            {
                SetValue(DescriptionColorProperty, value);
                lblDescription.TextColor = value;
            }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set
            {
                SetValue(TextProperty, value);
                //lblDescription.Text = value;
            }
        }
        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(
                "Text",
                typeof(string),
                typeof(IconButton),
                string.Empty
            );

        /*icon text*/
        public string IconText
        {
            get { return (string)GetValue(IconTextProperty); }
            set
            {
                SetValue(IconTextProperty, value);
                lblIcons.Text = value;
            }
        }

        public static readonly BindableProperty IconTextProperty =
            BindableProperty.Create(
                "IconText",
                typeof(string),
                typeof(IconButton),
                string.Empty,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    ((IconButton)bindable).IconText = (string)newValue;
                }
            );


        public static readonly BindableProperty IconColorProperty =
     BindableProperty.Create(
         "TextColor",
         typeof(Color),
         typeof(IconButton),
         Color.Black,
         propertyChanged: (bindable, oldValue, newValue) =>
         {
             ((IconButton)bindable).TextColor = (Color)newValue;
         }
     );
        public Color TextColor
        {
            get { return (Color)GetValue(IconColorProperty); }
            set
            {
                SetValue(IconColorProperty, value);
                lblIcons.TextColor = value;
            }
        }


        /*==*/
        public static readonly BindableProperty BorderColorProperty =
        BindableProperty.Create(
         "BorderColor",
         typeof(Color),
         typeof(IconButton),
         Color.Transparent,
         propertyChanged: (bindable, oldValue, newValue) =>
         {
             ((IconButton)bindable).BorderColor = (Color)newValue;
         }
        );


        public Color BorderColor
        {
            get { return (Color)GetValue(BorderColorProperty); }
            set
            {
                SetValue(BorderColorProperty, value);
                FrameWarpper.BorderColor = value;
            }
        }


        /*FrameWarpper*/
        public static readonly BindableProperty CornerRadiusProperty =
            BindableProperty.Create(nameof(CornerRadius),
                typeof(float),
                typeof(IconButton),
                defaultValue: default(float),
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    ((IconButton)bindable).CornerRadius = (float)newValue;
                });


        public float CornerRadius
        {
            get { return (float)GetValue(CornerRadiusProperty); }
            set
            {
                SetValue(CornerRadiusProperty, value);
                FrameWarpper.CornerRadius = value;
            }
        }


        public static readonly BindableProperty FrameBackgroundColorProperty =
            BindableProperty.Create(nameof(FrameBackgroundColor),
                typeof(Color),
                typeof(IconButton),
                defaultValue: Color.Transparent,
                propertyChanged: (bindable, oldValue, newValue) =>
                {
                    ((IconButton)bindable).FrameBackgroundColor = (Color)newValue;
                });

        public Color FrameBackgroundColor
        {
            get { return (Color)GetValue(FrameBackgroundColorProperty); }
            set
            {
                SetValue(FrameBackgroundColorProperty, value);
                FrameWarpper.BackgroundColor = value;
            }
        }


        public IconButton()
        {
            try
            {

                InitializeComponent();
                lblDescription.Text = Text;
                this.Content.BindingContext = this;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
        }

        private void setPosition(DescriptionPosition position)
        {
            switch (position)
            {
                case DescriptionPosition.None:
                    lblIcons.SetValue(Grid.RowProperty, 0);
                    lblDescription.SetValue(Grid.RowProperty, 0);
                    lblIcons.SetValue(Grid.ColumnProperty, 0);
                    lblDescription.SetValue(Grid.ColumnProperty, 0);
                    lblDescription.IsVisible = false;
                    break;
                case DescriptionPosition.Left:
                    lblIcons.SetValue(Grid.RowProperty, 0);
                    lblIcons.SetValue(Grid.ColumnProperty, 1);

                    lblDescription.SetValue(Grid.RowProperty, 0);
                    lblDescription.SetValue(Grid.ColumnProperty, 0);
                    break;
                case DescriptionPosition.Right:
                    lblIcons.SetValue(Grid.RowProperty, 0);
                    lblIcons.SetValue(Grid.ColumnProperty, 0);

                    lblDescription.SetValue(Grid.RowProperty, 0);
                    lblDescription.SetValue(Grid.ColumnProperty, 1);
                    break;
                default:
                    break;
            }
        }

        private void FrameWarpper_BindingContextChanged(object sender, EventArgs e)
        {
            if (addedAnimation || GestureRecognizers.Count == 0)
            {
                return;
            }

            if (!(GestureRecognizers[0] is TapGestureRecognizer tapGesture))
            {
                return;
            }

            tapGesture.Tapped += (tapsender, tape) =>
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    FrameWarpper.Opacity = 0;
                    await FrameWarpper.FadeTo(0.5, 100);
                    await FrameWarpper.FadeTo(1, 100);
                });
            };

            addedAnimation = true;
        }



        public event EventHandler Pressed;
        public event EventHandler Released;
        public event EventHandler Clicked;

        public virtual void OnPressed()
        {
            Pressed?.Invoke(this, EventArgs.Empty);
        }

        public virtual void OnReleased()
        {
            Released?.Invoke(this, EventArgs.Empty);
        }

        public virtual void OnClicked()
        {
            Clicked?.Invoke(this, EventArgs.Empty);
        }


        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (lblDescription == null)
            {
                return;
            }

            if (propertyName == TextProperty.PropertyName)
            {
                lblDescription.Text = Text;
            }
        }
    }
}