using System;
using System.Collections.Generic;

namespace Wesley.SlideOverKit
{
    public interface IPopupContainerPage
    {
        Dictionary<string, SlidePopupView> PopupViews { get; set; }

        Action<string>  ShowPopupAction { get; set; }

        Action HidePopupAction { get; set; }
    }
}

