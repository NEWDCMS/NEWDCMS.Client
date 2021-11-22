using Android.App;
using Android.Content;
using Android.Widget;
using Wesley.Client.CustomViews;
using Wesley.Client.Droid.Renderers;
using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(NullableDatePicker), typeof(NullableDatePickerRenderer))]
namespace Wesley.Client.Droid.Renderers
{
    /// <summary>
    /// 可为空的时间选择器
    /// </summary>
    public class NullableDatePickerRenderer : ViewRenderer<NullableDatePicker, EditText>
    {
        private DatePickerDialog _dialog;

        public NullableDatePickerRenderer(Context context) : base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<NullableDatePicker> e)
        {
            base.OnElementChanged(e);

            this.SetNativeControl(new Android.Widget.EditText(Context));
            if (Control == null || e.NewElement == null)
                return;

            var entry = (NullableDatePicker)this.Element;


            this.Control.Click += OnPickerClick;
            this.Control.Text = !entry.NullableDate.HasValue ? entry.PlaceHolder : Element.Date.ToString(Element.Format);
            this.Control.KeyListener = null;
            this.Control.FocusChange += OnPickerFocusChange;
            this.Control.Enabled = Element.IsEnabled;
            this.Control.Background = null;
            this.Control.SetTextSize(Android.Util.ComplexUnitType.Dip, !entry.NullableDate.HasValue ? (float)entry.FontSize : 12f);
            this.Control.SetTextColor(Color.Gray.ToAndroid());
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Xamarin.Forms.DatePicker.DateProperty.PropertyName || e.PropertyName == Xamarin.Forms.DatePicker.FormatProperty.PropertyName)
            {
                var entry = (NullableDatePicker)this.Element;

                if (this.Element.Format == entry.PlaceHolder)
                {
                    this.Control.Text = entry.PlaceHolder;
                    return;
                }
            }

            base.OnElementPropertyChanged(sender, e);
        }

        private void OnPickerFocusChange(object sender, Android.Views.View.FocusChangeEventArgs e)
        {
            if (e.HasFocus)
            {
                ShowDatePicker();
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (Control != null)
                {
                    this.Control.Click -= OnPickerClick;
                    this.Control.FocusChange -= OnPickerFocusChange;

                    if (_dialog != null)
                    {
                        ///_dialog.Hide();
                        _dialog.Dispose();
                        _dialog = null;
                    }
                }
            }
            catch (Exception) { }

            base.Dispose(disposing);
        }

        private void OnPickerClick(object sender, EventArgs e)
        {
            ShowDatePicker();
        }

        private void SetDate(DateTime date)
        {
            this.Control.Text = date.ToString(Element.Format);
            Element.Date = date;
        }

        private void ShowDatePicker()
        {
            CreateDatePickerDialog(this.Element.Date.Year, this.Element.Date.Month - 1, this.Element.Date.Day);
            _dialog.Show();
        }

        private void CreateDatePickerDialog(int year, int month, int day)
        {
            NullableDatePicker view = Element;
            _dialog = new DatePickerDialog(Context, (o, e) =>
            {
                view.Date = e.Date;
                ((IElementController)view).SetValueFromRenderer(VisualElement.IsFocusedProperty, false);
                Control.ClearFocus();

                _dialog = null;
            }, year, month, day);

            _dialog.SetButton("确定", (sender, e) =>
            {
                this.Element.Format = this.Element._originalFormat;
                SetDate(_dialog.DatePicker.DateTime);
                this.Element.AssignValue();
            });
            _dialog.SetButton2("清除", (sender, e) =>
            {
                this.Element.CleanDate();
                Control.Text = this.Element.Format;
            });
        }
    }
}