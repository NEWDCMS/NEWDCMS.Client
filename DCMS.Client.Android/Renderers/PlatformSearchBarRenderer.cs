using Android.Content;
using Android.Widget;
using DCMS.Client.CustomViews;
using DCMS.Client.Droid.Renderers;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using AView = Android.Views.View;
using AViewGroup = Android.Views.ViewGroup;

[assembly: ExportRenderer(handler: typeof(CrossSearchBar), target: typeof(PlatformSearchBarRenderer))]
namespace DCMS.Client.Droid.Renderers
{
    public class PlatformSearchBarRenderer : SearchBarRenderer
    {
        public PlatformSearchBarRenderer(Context context) : base(context: context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<SearchBar> e)
        {
            base.OnElementChanged(e);
            if (this.Control == null) return;


            var searchView = Control as SearchView;
            searchView.SetPadding(0, 0, 0, 0);

            //var searchView = Control as SearchView;
            //int searchPlateId = searchView.Context.Resources.GetIdentifier("android:id/search_plate", null, null);
            //var searchPlateView = searchView.FindViewById(searchPlateId);
            //searchPlateView.SetBackgroundColor(Android.Graphics.Color.Transparent);

            #region for edittext

            var editText = this.Control.GetChildrenOfType<EditText>().FirstOrDefault();
            if (editText != null)
            {
                //绘制矩形
                //var shape = new ShapeDrawable(new RectShape());
                //shape.Paint.Color = Android.Graphics.Color.Transparent;
                //shape.Paint.StrokeWidth = 0;
                //shape.Paint.SetStyle(Paint.Style.Stroke);
                //editText.Background = shape;
                editText.Background = null;
            }

            #endregion

            #region icon替换

            ////search_mag_icon为android中默认icon的id
            int svIconId = Context.Resources.GetIdentifier("android:id/search_mag_icon", null, null);
            var icon = searchView.FindViewById(svIconId);
            if (icon != null)
            {
                //icon.SetBackgroundColor(Android.Graphics.Color.ParseColor("#53a245"));
                //读取为ImageView
                var iv = (icon as ImageView);
                //更改颜色
                iv.SetColorFilter(Android.Graphics.Color.ParseColor("#53a245"));
                ////替换
                //iv.SetImageResource(Resource.Drawable.searchicon);
                ////通过padding控制图片大小
                iv.SetPadding(0, 0, 0, 0);
            }

            #endregion


            #region 边框样式

            //var gradient = new GradientDrawable();
            //gradient.SetCornerRadius(5.0f);
            //int[][] states =
            //{
            //    new[] {Android.Resource.Attribute.StateEnabled}, // enabled
            //    new[] {-Android.Resource.Attribute.StateEnabled} // disabled
            //};

            //int[] colors =
            //{
            //    Xamarin.Forms.Color.Red.ToAndroid(),
            //    Xamarin.Forms.Color.Gray.ToAndroid()
            //};
            //var stateList = new ColorStateList(states: states, colors: colors);
            //gradient.SetStroke((int)this.Context.ToPixels(1.0f), stateList);

            //this.Control.SetBackground(gradient);

            #endregion

            #region 字体设置

            //search_src_text为android默认输入文本id
            //var editTextId = Context.Resources.GetIdentifier("android:id/search_src_text", null, null);
            //var editText = sv.FindViewById(editTextId);
            //if (editText != null)
            //{
            //    var ev = (editText as EditText);
            //    //字体颜色
            //    ev.SetTextColor(Android.Graphics.Color.ParseColor("#999999"));
            //    //字体大小
            //    ev.SetTextSize(Android.Util.ComplexUnitType.Dip, 13);
            //}

            #endregion


            #region //移除默认下划线

            var plateId = Context.Resources.GetIdentifier("android:id/search_plate", null, null);
            var plate = searchView.FindViewById(plateId);
            if (plate != null)
            {
                //将背景色设为白色与整个搜索框的背景色相同
                (plate as Android.Views.View).SetBackgroundColor(Android.Graphics.Color.Transparent);
            }

            #endregion

        }
    }

    internal static class ViewGroupExtensions
    {
        internal static IEnumerable<T> GetChildrenOfType<T>(this AViewGroup self) where T : AView
        {
            for (var i = 0; i < self.ChildCount; i++)
            {
                var child = self.GetChildAt(i);
                if (child is T typedChild)
                    yield return typedChild;

                if (!(child is AViewGroup)) continue;
                var myChildren = (child as AViewGroup).GetChildrenOfType<T>();
                foreach (var nextChild in myChildren)
                    yield return nextChild;
            }
        }
    }
}