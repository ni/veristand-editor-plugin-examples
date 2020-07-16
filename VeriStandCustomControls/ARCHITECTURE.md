This document is intended to provide high level explanation of the components that are required to plugin to the VeriStand screen.

# Dependencies
## MEF
VeriStand uses the [MEF Framework](https://docs.microsoft.com/en-us/dotnet/framework/mef/) as the basis of its plugin model. To plugin to VeriStand, you need to:
* Put the plugin in a known directory. The csproj copies the built output to the plugin directory. If your path is different, make sure to update the [project file](CustomControlsExamples.csproj)
>     <PostBuildEvent>xcopy /y "$(TargetPath)" "C:\Users\Public\Documents\National Instruments\NI VeriStand 2020\Custom UI Manager Controls\"</PostBuildEvent>

* The assembly must identify itself as providing plugin components. This is done by adding the following attribute to the assembly.cs file.
> [assembly: NationalInstruments.Composition.ParticipatesInComposition]
* Individual plugin classes are identified by an attribute
> [Export(TYPENAME)] // Or this may be a derived attribute.
## WPF
VeriStand is built using [WPF](https://en.wikipedia.org/wiki/Windows_Presentation_Foundation#:~:text=Windows%20Presentation%20Foundation%20(WPF)%20is,NET%20Framework%203.0%20in%202006.) as our UI stack, though you can use other technologies as long as they can be appropriately hosted within a WPF control (WinForms is fairly simple, HTML a bit harder). If you are creating a WPF control, make sure to take advantage of Microsoft resources to understand best practices, as the VeriStand team cannot support you as you get up to speed with WPF.

# Plugging into VeriStand
## Versioning
Any plugin that participates in serialization must define a NamespaceSchema. This allows the plugin to define its version and do XML mutation between versions. For this example see [PluginNamespaceSchema](PluginNamespaceSchema.cs)
## Adding a Control
VeriStand uses the [MVVM (Model-View-ViewModel) pattern](https://en.wikipedia.org/wiki/Model%E2%80%93view%E2%80%93viewmodel).
To define a control, we need:
1) The export defining the plugin (see KeySwitchControlModelExporter)
2) A Model - this manages the state of your control through transactions (undo/redo) and serialization.
3) A ViewModel - instantiated when UI is needed to provide commands (context menu, right rail, menus, etc) and create the View.
4) A View - actual control.
# Recommendations
We recommend writing your control independent of our framework first, and then create the appropriate plugin pieces to expose the control within the VeriStand IDE.
# Known Issues
1) Palette Icons - We internally deprecated use of PNGs in our palettes in favor of a vector rendering technology that is not easy to share externally. As such, the new controls in the palette will not show a PNG, instead just showing a ? icon. We will look to add this support back for VeriStand 2020 R3.