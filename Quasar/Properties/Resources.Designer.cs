﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Quasar.Properties {
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
    public class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Quasar.Properties.Resources", typeof(Resources).Assembly);
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
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Assignments.
        /// </summary>
        public static string MainUI_AssignmentTabHeader {
            get {
                return ResourceManager.GetString("MainUI_AssignmentTabHeader", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to File Manager.
        /// </summary>
        public static string MainUI_FileManagerTabHeader {
            get {
                return ResourceManager.GetString("MainUI_FileManagerTabHeader", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Library.
        /// </summary>
        public static string MainUI_LibraryTabHeader {
            get {
                return ResourceManager.GetString("MainUI_LibraryTabHeader", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Settings.
        /// </summary>
        public static string MainUI_SettingsTabHeader {
            get {
                return ResourceManager.GetString("MainUI_SettingsTabHeader", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Delete this mod.
        /// </summary>
        public static string ModListItem_DeleteModAction {
            get {
                return ResourceManager.GetString("ModListItem_DeleteModAction", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Quasar.
        /// </summary>
        public static string Quasar_WindowTitle {
            get {
                return ResourceManager.GetString("Quasar_WindowTitle", resourceCulture);
            }
        }
    }
}
