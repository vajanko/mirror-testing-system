﻿#pragma checksum "..\..\..\WindowMain.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "C29A097C0B741B7AC35F6D201CCAB865"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3053
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using AvalonDock;
using MTS;
using MTS.Controls;
using MTS.EditorModule;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace MTS {
    
    
    /// <summary>
    /// WindowMain
    /// </summary>
    public partial class WindowMain : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 2 "..\..\..\WindowMain.xaml"
        internal MTS.WindowMain MainWindow;
        
        #line default
        #line hidden
        
        
        #line 99 "..\..\..\WindowMain.xaml"
        internal AvalonDock.DockingManager dockManager;
        
        #line default
        #line hidden
        
        
        #line 108 "..\..\..\WindowMain.xaml"
        internal AvalonDock.DocumentPane filePane;
        
        #line default
        #line hidden
        
        
        #line 116 "..\..\..\WindowMain.xaml"
        internal System.Windows.Controls.DockPanel outputPanel;
        
        #line default
        #line hidden
        
        
        #line 117 "..\..\..\WindowMain.xaml"
        internal System.Windows.Controls.TextBox outputConsole;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/MTS;component/windowmain.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\WindowMain.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.MainWindow = ((MTS.WindowMain)(target));
            return;
            case 2:
            
            #line 15 "..\..\..\WindowMain.xaml"
            ((System.Windows.Input.CommandBinding)(target)).CanExecute += new System.Windows.Input.CanExecuteRoutedEventHandler(this.newCanExecute);
            
            #line default
            #line hidden
            
            #line 15 "..\..\..\WindowMain.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.newExecuted);
            
            #line default
            #line hidden
            return;
            case 3:
            
            #line 16 "..\..\..\WindowMain.xaml"
            ((System.Windows.Input.CommandBinding)(target)).CanExecute += new System.Windows.Input.CanExecuteRoutedEventHandler(this.openCanExecute);
            
            #line default
            #line hidden
            
            #line 16 "..\..\..\WindowMain.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.openExecuted);
            
            #line default
            #line hidden
            return;
            case 4:
            
            #line 17 "..\..\..\WindowMain.xaml"
            ((System.Windows.Input.CommandBinding)(target)).CanExecute += new System.Windows.Input.CanExecuteRoutedEventHandler(this.saveCanExecute);
            
            #line default
            #line hidden
            
            #line 17 "..\..\..\WindowMain.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.saveExecuted);
            
            #line default
            #line hidden
            return;
            case 5:
            
            #line 18 "..\..\..\WindowMain.xaml"
            ((System.Windows.Input.CommandBinding)(target)).CanExecute += new System.Windows.Input.CanExecuteRoutedEventHandler(this.saveAsCanExecute);
            
            #line default
            #line hidden
            
            #line 18 "..\..\..\WindowMain.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.saveAsExecuted);
            
            #line default
            #line hidden
            return;
            case 6:
            
            #line 19 "..\..\..\WindowMain.xaml"
            ((System.Windows.Input.CommandBinding)(target)).CanExecute += new System.Windows.Input.CanExecuteRoutedEventHandler(this.closeCanExecute);
            
            #line default
            #line hidden
            
            #line 19 "..\..\..\WindowMain.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.closeExecuted);
            
            #line default
            #line hidden
            return;
            case 7:
            
            #line 21 "..\..\..\WindowMain.xaml"
            ((System.Windows.Input.CommandBinding)(target)).CanExecute += new System.Windows.Input.CanExecuteRoutedEventHandler(this.viewTesterCanExecute);
            
            #line default
            #line hidden
            
            #line 21 "..\..\..\WindowMain.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.viewTesterExecuted);
            
            #line default
            #line hidden
            return;
            case 8:
            
            #line 43 "..\..\..\WindowMain.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.menuClick_Exit);
            
            #line default
            #line hidden
            return;
            case 9:
            this.dockManager = ((AvalonDock.DockingManager)(target));
            return;
            case 10:
            this.filePane = ((AvalonDock.DocumentPane)(target));
            return;
            case 11:
            this.outputPanel = ((System.Windows.Controls.DockPanel)(target));
            return;
            case 12:
            this.outputConsole = ((System.Windows.Controls.TextBox)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}
