﻿#pragma checksum "..\..\..\..\..\Modules\TesterModule\TestWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "EB4E1871BDF344A8A89DDD6A6DA0B844"
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
using MTS.Controls;
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


namespace MTS.TesterModule {
    
    
    /// <summary>
    /// TestWindow
    /// </summary>
    public partial class TestWindow : AvalonDock.DocumentContent, System.Windows.Markup.IComponentConnector {
        
        
        #line 6 "..\..\..\..\..\Modules\TesterModule\TestWindow.xaml"
        internal MTS.TesterModule.TestWindow root;
        
        #line default
        #line hidden
        
        
        #line 31 "..\..\..\..\..\Modules\TesterModule\TestWindow.xaml"
        internal System.Windows.Controls.Label parametersStatus;
        
        #line default
        #line hidden
        
        
        #line 34 "..\..\..\..\..\Modules\TesterModule\TestWindow.xaml"
        internal System.Windows.Controls.Label partNumber;
        
        #line default
        #line hidden
        
        
        #line 37 "..\..\..\..\..\Modules\TesterModule\TestWindow.xaml"
        internal System.Windows.Controls.Label supplierName;
        
        #line default
        #line hidden
        
        
        #line 40 "..\..\..\..\..\Modules\TesterModule\TestWindow.xaml"
        internal System.Windows.Controls.Label description;
        
        #line default
        #line hidden
        
        
        #line 44 "..\..\..\..\..\Modules\TesterModule\TestWindow.xaml"
        internal System.Windows.Controls.Label paramFile;
        
        #line default
        #line hidden
        
        
        #line 48 "..\..\..\..\..\Modules\TesterModule\TestWindow.xaml"
        internal System.Windows.Controls.Label shiftStatus;
        
        #line default
        #line hidden
        
        
        #line 83 "..\..\..\..\..\Modules\TesterModule\TestWindow.xaml"
        internal System.Windows.Controls.Label powerSupplyStatus;
        
        #line default
        #line hidden
        
        
        #line 88 "..\..\..\..\..\Modules\TesterModule\TestWindow.xaml"
        internal MTS.Controls.MirrorView mirrorView;
        
        #line default
        #line hidden
        
        
        #line 101 "..\..\..\..\..\Modules\TesterModule\TestWindow.xaml"
        internal MTS.Controls.FlowControl spiralCurrent;
        
        #line default
        #line hidden
        
        
        #line 102 "..\..\..\..\..\Modules\TesterModule\TestWindow.xaml"
        internal MTS.Controls.FlowControl blinkerCurrent;
        
        #line default
        #line hidden
        
        
        #line 103 "..\..\..\..\..\Modules\TesterModule\TestWindow.xaml"
        internal MTS.Controls.FlowControl actuatorACurrent;
        
        #line default
        #line hidden
        
        
        #line 104 "..\..\..\..\..\Modules\TesterModule\TestWindow.xaml"
        internal MTS.Controls.FlowControl actuatorBCurrent;
        
        #line default
        #line hidden
        
        
        #line 105 "..\..\..\..\..\Modules\TesterModule\TestWindow.xaml"
        internal MTS.Controls.FlowControl powerfoldCurrent;
        
        #line default
        #line hidden
        
        
        #line 106 "..\..\..\..\..\Modules\TesterModule\TestWindow.xaml"
        internal MTS.Controls.FlowControl powerSupplyVoltage1;
        
        #line default
        #line hidden
        
        
        #line 107 "..\..\..\..\..\Modules\TesterModule\TestWindow.xaml"
        internal MTS.Controls.FlowControl powerSupplyVoltage2;
        
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
            System.Uri resourceLocater = new System.Uri("/MTS;component/modules/testermodule/testwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\Modules\TesterModule\TestWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler) {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.root = ((MTS.TesterModule.TestWindow)(target));
            return;
            case 2:
            this.parametersStatus = ((System.Windows.Controls.Label)(target));
            return;
            case 3:
            this.partNumber = ((System.Windows.Controls.Label)(target));
            return;
            case 4:
            this.supplierName = ((System.Windows.Controls.Label)(target));
            return;
            case 5:
            this.description = ((System.Windows.Controls.Label)(target));
            return;
            case 6:
            
            #line 42 "..\..\..\..\..\Modules\TesterModule\TestWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.loadParameters);
            
            #line default
            #line hidden
            return;
            case 7:
            this.paramFile = ((System.Windows.Controls.Label)(target));
            return;
            case 8:
            this.shiftStatus = ((System.Windows.Controls.Label)(target));
            return;
            case 9:
            
            #line 65 "..\..\..\..\..\Modules\TesterModule\TestWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.connectClick);
            
            #line default
            #line hidden
            return;
            case 10:
            
            #line 67 "..\..\..\..\..\Modules\TesterModule\TestWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.disconnectClick);
            
            #line default
            #line hidden
            return;
            case 11:
            
            #line 69 "..\..\..\..\..\Modules\TesterModule\TestWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.startClick);
            
            #line default
            #line hidden
            return;
            case 12:
            
            #line 72 "..\..\..\..\..\Modules\TesterModule\TestWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.stopClick);
            
            #line default
            #line hidden
            return;
            case 13:
            
            #line 75 "..\..\..\..\..\Modules\TesterModule\TestWindow.xaml"
            ((System.Windows.Controls.Button)(target)).PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.startButtonMouseLeftButtonDown);
            
            #line default
            #line hidden
            
            #line 76 "..\..\..\..\..\Modules\TesterModule\TestWindow.xaml"
            ((System.Windows.Controls.Button)(target)).PreviewMouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.startButtonMouseLeftButtonUp);
            
            #line default
            #line hidden
            return;
            case 14:
            this.powerSupplyStatus = ((System.Windows.Controls.Label)(target));
            return;
            case 15:
            this.mirrorView = ((MTS.Controls.MirrorView)(target));
            return;
            case 16:
            this.spiralCurrent = ((MTS.Controls.FlowControl)(target));
            return;
            case 17:
            this.blinkerCurrent = ((MTS.Controls.FlowControl)(target));
            return;
            case 18:
            this.actuatorACurrent = ((MTS.Controls.FlowControl)(target));
            return;
            case 19:
            this.actuatorBCurrent = ((MTS.Controls.FlowControl)(target));
            return;
            case 20:
            this.powerfoldCurrent = ((MTS.Controls.FlowControl)(target));
            return;
            case 21:
            this.powerSupplyVoltage1 = ((MTS.Controls.FlowControl)(target));
            return;
            case 22:
            this.powerSupplyVoltage2 = ((MTS.Controls.FlowControl)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}
