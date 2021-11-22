using Android.Views;
using Wesley.Client.Droid.Effects;
using Wesley.Client.Droid.Effects.GestureCollectors;
using Wesley.Client.Effects;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using View = Android.Views.View;

[assembly: ExportEffect(typeof(CommandsPlatform), nameof(EffectCommands))]
[assembly: ResolutionGroupName("Wesley.Client")]
namespace Wesley.Client.Droid.Effects
{
    public class CommandsPlatform : PlatformEffect
    {
        public View View => Control ?? Container;
        public bool IsDisposed => (Container as IVisualElementRenderer)?.Element == null;

        private DateTime _tapTime;
        private readonly Android.Graphics.Rect _rect = new Android.Graphics.Rect();
        private readonly int[] _location = new int[2];


        protected override void OnAttached()
        {
            View.Clickable = true;
            View.LongClickable = true;
            View.SoundEffectsEnabled = true;

            TouchCollector.Add(View, OnTouch);
        }

        private void OnTouch(View.TouchEventArgs args)
        {
            switch (args.Event.Action)
            {
                case MotionEventActions.Down:
                    _tapTime = DateTime.Now;
                    break;

                case MotionEventActions.Up:
                    if (IsViewInBounds((int)args.Event.RawX, (int)args.Event.RawY))
                    {
                        var range = (DateTime.Now - _tapTime).TotalMilliseconds;
                        if (range > 800)
                            LongClickHandler();
                        else
                            ClickHandler();
                    }
                    break;
            }
        }

        private bool IsViewInBounds(int x, int y)
        {
            View.GetDrawingRect(_rect);
            View.GetLocationOnScreen(_location);
            _rect.Offset(_location[0], _location[1]);
            return _rect.Contains(x, y);
        }

        private void ClickHandler()
        {
            var cmd = EffectCommands.GetTap(Element);
            var param = EffectCommands.GetTapParameter(Element);
            if (cmd?.CanExecute(param) ?? false)
                cmd.Execute(param);
        }

        private void LongClickHandler()
        {
            var cmd = EffectCommands.GetLongTap(Element);

            if (cmd == null)
            {
                ClickHandler();
                return;
            }

            var param = EffectCommands.GetLongTapParameter(Element);
            if (cmd.CanExecute(param))
                cmd.Execute(param);
        }

        protected override void OnDetached()
        {
            if (IsDisposed) return;
            TouchCollector.Delete(View, OnTouch);
        }
    }
}




