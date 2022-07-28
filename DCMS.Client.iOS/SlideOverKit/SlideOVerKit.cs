using System;

namespace DCMS.SlideOverKit.iOS
{
    public class SlideOverKit
    {
        public static void Init()
        {
            //需要这样做，否则在release版中不起作用
            var mc = new MenuContainerPageiOSRenderer (); 
        }
    }
}
