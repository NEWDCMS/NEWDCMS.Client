using PanCardView;
using PanCardView.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace DCMS.Behaviors
{
    [Preserve(AllMembers = true)]
    public class CardViewBehavior : Behavior<PanCardView.CarouselView>
    {
        #region Fields

        private int previousIndex;

        #endregion

        #region Methods

        /// <summary>
        /// 在将动画初始化为视图时调用
        /// </summary>
        /// <param name="rotator">The SfRotator</param>
        /// <param name="selectedIndex">Selected Index</param>
        public void Animation(PanCardView.CarouselView rotator, double selectedIndex)
        {
            try
            {
                if (rotator != null && rotator.ItemsSource != null && rotator.ItemsCount > 0)
                {
                    //System.Diagnostics.Debug.WriteLine($"selectedIndex:-------------------->{selectedIndex.ToString()}");

                    //[0:] Styles: Style TargetType PanCardView.Controls.IndicatorsControl is not compatible with element target type PanCardView.Controls.IndicatorItemView
                    //** Java.Lang.IllegalArgumentException:** 'Cannot set 'scaleX' to Float.NaN'

                    //int itemsCount = rotator.ItemsCount;
                    int.TryParse(selectedIndex.ToString(), out int index);

                    //var viewModel = rotator.BindingContext as BoardingPageViewModel;
                    //if (selectedIndex == itemsCount - 1)
                    //{
                    //    viewModel.NextButtonText = "DONE";
                    //    viewModel.IsSkipButtonVisible = false;
                    //}
                    //else
                    //{
                    //    viewModel.NextButtonText = "NEXT";
                    //    viewModel.IsSkipButtonVisible = true;
                    //}

                    if (Device.RuntimePlatform != Device.UWP)
                    {
                        var items = (rotator.ItemsSource as IEnumerable<object>).ToList();

                      
                        var currentItem = items[index];

                        //if (currentItem is Boarding boarding)
                        //{
                        //    if (boarding.RotatorItem is ContentView contentView)
                        //    {
                        //        if (contentView.Children.Count > 0)
                        //        {
                        //            var childElement = (contentView.Children[0] as StackLayout)?.Children?.ToList();
                        //            if (childElement != null && childElement.Count > 0)
                        //            {
                        //                this.StartAnimation(childElement, currentItem as Boarding);
                        //            }
                        //        }
                               
                        //    }
                        //}
                       

                        // Set default value to previous view.
                        if (index != this.previousIndex)
                        {
                            try
                            {
                                var previousItem = items[this.previousIndex];
                                //if (previousItem is Boarding pboarding)
                                //{
                                //    if (pboarding.RotatorItem is ContentView contentView)
                                //    {
                                //        if (contentView.Children.Count > 0)
                                //        {
                                //            var previousChildElement = (contentView.Children[0] as StackLayout)?.Children?.ToList();
                                //            if (previousChildElement != null && previousChildElement.Count > 0)
                                //            {
                                //                previousChildElement[0].FadeTo(0, 250);
                                //                previousChildElement[1].FadeTo(0, 250);
                                //                previousChildElement[1].TranslateTo(0, 80, 250);
                                //                previousChildElement[1].ScaleTo(1, 0);
                                //                previousChildElement[2].FadeTo(0, 250);
                                //                previousChildElement[2].TranslateTo(0, 80, 250);
                                //            }
                                //        }
                                //    }
                                //}
                            }
                            catch (Exception)
                            {
                            }
                        }

                        this.previousIndex = index;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Animation -> {ex.Message}");
            }
        }

        ///// <summary>
        ///// 开始动画视图
        ///// </summary>
        ///// <param name="childElement">子元素</param>
        ///// <param name="item"></param>
        //public async void StartAnimation(List<View> childElement, Boarding item)
        //{
        //    try
        //    {
        //        var fadeAnimationImage = childElement[0]?.FadeTo(1, 250);
        //        var fadeAnimationtaskTitleTime = childElement[1]?.FadeTo(1, 1000);

        //        var translateAnimation = childElement[1]?.TranslateTo(0, 0, 500);
        //        var scaleAnimationTitle = childElement[1]?.ScaleTo(1.5, 500, Easing.SinIn);

        //        var fadeAnimationTaskDescriptionTime = childElement[2]?.FadeTo(1, 1000);
        //        var translateDescriptionAnimation = childElement[2]?.TranslateTo(0, 0, 500);

        //        var animation = new Animation();
        //        var scaleDownAnimation = new Animation(v =>
        //        {
        //            if (childElement[0] != null)
        //                childElement[0].Scale = v;

        //        }, 0.5, 1, Easing.SinIn);

        //        animation.Add(0, 1, scaleDownAnimation);
        //        animation.Commit((item as Boarding).RotatorItem as ContentView, "animation", 16, 500);

        //        await Task.WhenAll(fadeAnimationTaskDescriptionTime,
        //            fadeAnimationtaskTitleTime,
        //            translateAnimation,
        //            scaleAnimationTitle,
        //            translateDescriptionAnimation);
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine($"StartAnimation -> {ex.Message}");
        //    }
        //}

        /// <summary>
        /// 当预览视图时调用
        /// </summary>
        /// <param name="rotator">The Rotator</param>
        protected override void OnAttachedTo(PanCardView.CarouselView rotator)
        {
            base.OnAttachedTo(rotator);
            //rotator.ItemSwiped += Rotator_ItemSwiped;
            rotator.ItemAppearing += Rotator_ItemAppearing;
            rotator.BindingContextChanged += this.Rotator_BindingContextChanged;
        }

        /// <summary>
        /// 当退出视图时调用
        /// </summary>
        /// <param name="rotator"></param>
        protected override void OnDetachingFrom(PanCardView.CarouselView rotator)
        {
            base.OnDetachingFrom(rotator);
            //rotator.ItemSwiped -= this.Rotator_ItemSwiped;
            rotator.ItemAppearing -= Rotator_ItemAppearing;
            rotator.BindingContextChanged -= this.Rotator_BindingContextChanged;
        }


        /// <summary>
        /// 当rotator绑定上下文更改时调用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Rotator_BindingContextChanged(object sender, EventArgs e)
        {
            try
            {
                if (sender != null)
                    Task.Delay(500).ContinueWith(t => this.Animation(sender as PanCardView.CarouselView, 0));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Rotator_BindingContextChanged -> {ex.Message}");
            }
        }

        private void Rotator_ItemAppearing(CardsView view, ItemAppearingEventArgs e)
        {
            try
            {
                if (view is PanCardView.CarouselView rotator)
                    this.Animation(rotator, e.Index);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Rotator_ItemAppearing -> {ex.Message}");
            }
        }

        //private void Rotator_ItemSwiped(object sender, ItemSwipedEventArgs e)
        //{
        //    PanCardView.CarouselView rotator = sender as PanCardView.CarouselView;
        //    this.Animation(rotator, e.Index);
        //}

        ///// <summary>
        ///// Invoked when selected index is changed.
        ///// </summary>
        ///// <param name="sender">The rotator</param>
        ///// <param name="e">The selection changed event args</param>
        //private void Rotator_SelectedIndexChanged(object sender, SelectedIndexChangedEventArgs e)
        //{
        //    PanCardView.CarouselView rotator = sender as PanCardView.CarouselView;
        //    this.Animation(rotator, e.Index);
        //}
    }

    #endregion
}
