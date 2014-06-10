using System;

namespace MonoTouch.SlideMenu
{
    public class SlideMenuGesturesState
    {
        public bool PanGestureEnabled { get; set; }
        public bool TapGestureEnabled { get; set; }

        public SlideMenuGesturesState(bool panEnabled, bool tapEnabled)
        {
            PanGestureEnabled = panEnabled;
            TapGestureEnabled = tapEnabled;
        }
    }
}

