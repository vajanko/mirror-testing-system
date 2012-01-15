using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace MTS.Controls
{
    static public class MTSCommands
    {
        public static RoutedUICommand ViewTester = new RoutedUICommand("Tester", "ViewTester", typeof(MTSCommands));

        public static RoutedUICommand ViewSettings = new RoutedUICommand("Settings", "ViewSettings", typeof(MTSCommands));

        public static RoutedUICommand ViewData = new RoutedUICommand("Data", "ViewData", typeof(MTSCommands));

        static MTSCommands()
        {
            ViewTester.InputGestures.Add(new KeyGesture(Key.T, ModifierKeys.Alt));

            ViewSettings.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Alt));

            ViewData.InputGestures.Add(new KeyGesture(Key.D, ModifierKeys.Alt));
        }
    }
}
