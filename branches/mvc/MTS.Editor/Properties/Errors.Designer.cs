﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.431
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MTS.Editor.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Errors {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Errors() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("MTS.Editor.Properties.Errors", typeof(Errors).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        internal static System.Drawing.Bitmap cancel {
            get {
                object obj = ResourceManager.GetObject("cancel", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Template file {0} is corrupted.
        /// </summary>
        internal static string ConfigCorruptedMsg {
            get {
                return ResourceManager.GetString("ConfigCorruptedMsg", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Template file {0} was not found. Check application settings or contact application administrator.
        /// </summary>
        internal static string ConfigNotFoundMsg {
            get {
                return ResourceManager.GetString("ConfigNotFoundMsg", resourceCulture);
            }
        }
        
        internal static System.Drawing.Bitmap cross {
            get {
                object obj = ResourceManager.GetObject("cross", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        internal static System.Drawing.Bitmap ErrorIcon {
            get {
                object obj = ResourceManager.GetObject("ErrorIcon", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error.
        /// </summary>
        internal static string ErrorTitle {
            get {
                return ResourceManager.GetString("ErrorTitle", resourceCulture);
            }
        }
        
        internal static System.Drawing.Bitmap exclamation {
            get {
                object obj = ResourceManager.GetObject("exclamation", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        internal static System.Drawing.Bitmap FileErrorIcon {
            get {
                object obj = ResourceManager.GetObject("FileErrorIcon", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to File error.
        /// </summary>
        internal static string FileErrorTitle {
            get {
                return ResourceManager.GetString("FileErrorTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to File {0} may be corrupted and could not be read.
        /// </summary>
        internal static string FileFormatErrorMsg {
            get {
                return ResourceManager.GetString("FileFormatErrorMsg", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to File {0} could not be found.
        /// </summary>
        internal static string FileNotFoundMsg {
            get {
                return ResourceManager.GetString("FileNotFoundMsg", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Parameter {0} could not be found in test {1}.
        /// </summary>
        internal static string ParamNotFoundMsg {
            get {
                return ResourceManager.GetString("ParamNotFoundMsg", resourceCulture);
            }
        }
    }
}
