# VeriStand Plugin Architecture

This document provides a high level overview of the components required to plugin with a VeriStand screen.

## Dependencies
### MEF
VeriStand uses the [MEF Framework](https://docs.microsoft.com/en-us/dotnet/framework/mef/) as the basis of its plugin model. To use a plugin with VeriStand:
1. Place the plugin in a known directory. The *.csproj* file copies the built output to the plugin directory. If your file path is different, update the [project file](CustomControlsExamples.csproj).
>     <PostBuildEvent>xcopy /y "$(TargetPath)" "C:\Users\Public\Documents\National Instruments\NI VeriStand 2021\Custom UI Manager Controls\"</PostBuildEvent>

2. Use attributes to identify individual plugin classes.
> [Export(typeof(TYPENAME))] // Or this may be a derived attribute.

### WPF
VeriStand is built using [WPF](https://en.wikipedia.org/wiki/Windows_Presentation_Foundation#:~:text=Windows%20Presentation%20Foundation%20(WPF)%20is,NET%20Framework%203.0%20in%202006.) as the UI stack. You can use other technologies if they can be hosted within a WPF control. Some platforms are easier to use than others. For example, WinForms is simple to implement while HTML is more difficult.

If you are creating a WPF control, use Microsoft resources to understand their best practices. The VeriStand team cannot provide support for learning WPF.

## Adding a Plugin to VeriStand

### Versioning
Any plugin that participates in serialization must define a *NamespaceSchema*. This schema allows the plugin to define its version and perform an XML mutation between versions. For an example schema, see the [PluginNamespaceSchema](PluginNamespaceSchema.cs).

### Adding a Control
VeriStand uses the [MVVM (Model-View-ViewModel) pattern](https://en.wikipedia.org/wiki/Model%E2%80%93view%E2%80%93viewmodel).
To define a control, you need:
1. An export to define the plugin. For an example export, refer to the [KeySwitchControlModelExporter](KeySwitchControlModel.cs).
1. A model to manage the control state through transactions (undo/redo) and serialization.
1. A ViewModel to instantiate when a UI is needed to provide commands (such as a context menu, right rail, menus, etc.) and create the View.
1. A View as the actual control.

## Recommendations
The VeriStand team recommends writing your control independent of the framework first and then creating the appropriate plugin pieces to expose the control within the VeriStand IDE.

## Customizing the Application

### Add, Remove, or Rename Commands
The term *command* generally refers to menus, toolbar items, context menus, and properties pane configuration. These items can be customized through a plugin derived from *IPushCommandContent*. The example provides such a plugin in the [PluginCommandContentProvider](PluginCommandContentProvider.cs) class.

Customization options include:
* Hiding a command.
* Disabling a command.
* Changing the text associated with a command.
* Overriding the behavior of a command with another behavior.
* Adding a command to the menus, toolbar, context menus, and properties pane. This includes those for a selected item.

### Advanced Customization
Full customization of the application is restricted to the *IApplicationFeatureSet*. There can only be one *IApplicationFeatureSet*, and it must be provided by the executable through the *VeriStand.CustomApplication* project and the [CustomApplicationFeatureSet](CustomApplicationFeatureSet.cs) class.

The feature set can customize more than *IPushCommandContent*. Other examples include:
* Filtering out any plugin from the environment. The example demonstrates how to filter unwanted tool windows.
* Providing a different splash screen and application name.
* Defining what *docking*, or the user ability to drag windows, is allowed.
* Providing a custom welcome screen (see *CreateLauncherControl*) when the application is launched.
* Removing the existing welcome screen.
