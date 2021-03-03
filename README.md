# VeriStand Editor Plugin Examples

This repo contains example code that configures and extends the VeriStand Editor. These examples demonstrate how to:
* Create custom C# screen controls.
* Customize the IDE with menu items, toolbar items, tool windows, and configuration pane editors.

## Getting Started

### Dependencies
* .NET - .NET 4.6.2 installed to your local machine. VeriStand is built against this version.
* Compiler - Any C# editor and compiler that supports .NET 4.6.2. These examples were created with Visual Studio 2015.
Note: some Visual Studio options can cause the app to crash. If using Visual Studio, it is recommended to turn off options for:
    * Enable XAML Hot Reload
    * Enable UI Debugging Tools for XAML (older version of the above option)

### Using these Examples
1. Clone or download this repo.
1. If you are using a different version of VeriStand or a different file path location, update the *VeriStandDir* constant in the *.csproj* files.

#### Custom Controls
1. Set VeriStand as your debug executable.
1. Run the project.
1. Locate the new palette in the screen document with two droppable controls.

#### Custom Editor
1. Run the editor as an administrator and build. This will allow the post build step to copy the executable into the VeriStand directory. Otherwise, you will have to manually copy and move the executable.
1. Set *VeriStand.CustomApplication* to start an external program.
1. Select the copied executable in the VeriStand install directory and run it.
1. Enable IDE customization by selecting **File** > **Preferences** > **Example**.
1. Restart VeriStand.

The IDE will now have a custom splash screen with most menus and tool windows removed.

### Architecture
For more information on the code provided, refer to [VeriStand Plugin Architecture](VeriStandCustomControls/ARCHITECTURE.md).

## Support

This code is provided as is. The VeriStand team can provide assistance with getting started and other questions, but support will be limited. You may submit issues, documentation, and example requests.

## [Contributing](CONTRIBUTING.md)

## [License](LICENSE)
