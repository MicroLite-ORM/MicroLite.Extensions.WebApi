﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MicroLite.Extensions.WebApi {
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
    internal class Messages {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Messages() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("MicroLite.Extensions.WebApi.Messages", typeof(Messages).Assembly);
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
        ///   Looks up a localized string similar to The argument must not be null.
        /// </summary>
        internal static string ArgumentMustNotBeNull {
            get {
                return ResourceManager.GetString("ArgumentMustNotBeNull", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The type {0} does not have a property called {1}.
        /// </summary>
        internal static string InvalidPropertyName {
            get {
                return ResourceManager.GetString("InvalidPropertyName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to MicroLite: Loading the extension for ASP.NET WebApi.
        /// </summary>
        internal static string LoadingExtension {
            get {
                return ResourceManager.GetString("LoadingExtension", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to MicroLite has been configured for multiple connections, therefore the connection name must be specified in the MicroLiteSessionAttribute (e.g. [MicroLiteSession(&quot;MyConnection&quot;)])..
        /// </summary>
        internal static string NoConnectionNameMultipleSessionFactories {
            get {
                return ResourceManager.GetString("NoConnectionNameMultipleSessionFactories", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to find the session factory, check that Configure.Extensions().WithWebApi() has been called in your application start up..
        /// </summary>
        internal static string NoSessionFactoriesSet {
            get {
                return ResourceManager.GetString("NoSessionFactoriesSet", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to MicroLite has not been configured for the connection named {0}, either the connection name in the MicroLiteSessionAttribute is incorrect or Configure.Fluently().ForConnection({0}) has not been called..
        /// </summary>
        internal static string NoSessionFactoryFoundForConnectionName {
            get {
                return ResourceManager.GetString("NoSessionFactoryFoundForConnectionName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Loading a default instance of MicroLiteSessionAttribute in the specified HttpConfiguration.Filters.
        /// </summary>
        internal static string RegisteringMicroLiteSessionAttribute {
            get {
                return ResourceManager.GetString("RegisteringMicroLiteSessionAttribute", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Loading a ValidateModelNotNullAttribute in the specified HttpConfiguration.Filters.
        /// </summary>
        internal static string RegisteringValidateModelNotNullAttribute {
            get {
                return ResourceManager.GetString("RegisteringValidateModelNotNullAttribute", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Loading a ValidateModelStateAttribute in the specified HttpConfiguration.Filters.
        /// </summary>
        internal static string RegisteringValidateModelStateAttribute {
            get {
                return ResourceManager.GetString("RegisteringValidateModelStateAttribute", resourceCulture);
            }
        }
    }
}
