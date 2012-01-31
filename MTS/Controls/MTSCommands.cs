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

        public static RoutedUICommand LogIn = new RoutedUICommand("Log in", "LogIn", typeof(MTSCommands));

        public static RoutedUICommand LogOut = new RoutedUICommand("Log out", "LogOut", typeof(MTSCommands));

        public static RoutedUICommand ViewProfile = new RoutedUICommand("Profile", "ViewProfile", typeof(MTSCommands));

        static MTSCommands()
        {
            ViewTester.InputGestures.Add(new KeyGesture(Key.T, ModifierKeys.Alt));

            ViewSettings.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Alt));

            ViewData.InputGestures.Add(new KeyGesture(Key.D, ModifierKeys.Alt));

            LogIn.InputGestures.Add(new KeyGesture(Key.I, ModifierKeys.Alt));

            LogOut.InputGestures.Add(new KeyGesture(Key.O, ModifierKeys.Alt));

            ViewProfile.InputGestures.Add(new KeyGesture(Key.P, ModifierKeys.Alt));
        }
    }
}
