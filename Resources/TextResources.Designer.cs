﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SetonixUpdater.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class TextResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal TextResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("SetonixUpdater.Resources.TextResources", typeof(TextResources).Assembly);
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
        
        /// <summary>
        ///   Looks up a localized string similar to The update was not performed. Please try again later..
        /// </summary>
        internal static string Abort {
            get {
                return ResourceManager.GetString("Abort", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The update request was invalid:
        ///{0}.
        /// </summary>
        internal static string ArgumentsMissing {
            get {
                return ResourceManager.GetString("ArgumentsMissing", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to (no command line arguments).
        /// </summary>
        internal static string EmptyCommandLine {
            get {
                return ResourceManager.GetString("EmptyCommandLine", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Update.
        /// </summary>
        internal static string ErrorMsgBoxTitle {
            get {
                return ResourceManager.GetString("ErrorMsgBoxTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The update cannot be performed due to an invalid manifest file..
        /// </summary>
        internal static string InvalidManifest {
            get {
                return ResourceManager.GetString("InvalidManifest", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error performing the update: The file {0} can not be found in temporary path {1}..
        /// </summary>
        internal static string NewFileNotFound {
            get {
                return ResourceManager.GetString("NewFileNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The update cannot be performed while {0} is running. Please close all windows and press &quot;Retry.&quot;.
        /// </summary>
        internal static string PleaseClose {
            get {
                return ResourceManager.GetString("PleaseClose", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Preparing....
        /// </summary>
        internal static string Preparing {
            get {
                return ResourceManager.GetString("Preparing", resourceCulture);
            }
        }
    }
}
