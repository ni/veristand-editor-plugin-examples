# VeriStand Editor Plugin Examples

This repo contains examples for configuring and extending the VeriStand Editor. Some of the features displayed are:
* The ability to create custom C# controls for the VeriStand Screen.
* The ability to customize the editor - removing menu items, toolbar items, tool windows and configuration pane editors.

## Getting Started

### Dependencies
* .NET - .NET 4.6.2 installed to your local machine. VeriStand is built against this version.
* Compiler - Any C# editor and compiler that supports .NET 4.6.2. These examples were created with Visual Studio 2015.

### Using these Examples
1. Clone or download this repo.
1. Update the VeriStandDir constant in the csproj files if you are using a different version of VeriStand, or a different path.

#### Custom Controls
1. Set VeriStand as your debug executable.
1. Run the project.
1. Locate the new palette in the screen document with two droppable controls.

#### Custom Editor
1. If using Visual Studio, run as an Administrator and build. This is needed for the post build step to copy the exe into the VeriStand directory. Otherwise, manually perform this step.
1. Set VeriStand.CustomApplication to start an external program, selecting the exe that has been copied to the VeriStand install directory and run it.
1. The IDE customization is not on by default - you can enable through File >> Preferences >> Example tab. This will require a restart.
1. See the IDE with a custom splash screen and most IDE menus and tool windows removed.

### Architecture
For more information on the code provided, refer to [VeriStand Plugin Architecture](VeriStandCustomControls/ARCHITECTURE.md).

## Support

This code is provided as is. The VeriStand team can provide assistance with getting started and other questions, but support will be limited. You may submit issues, documentation, and example requests.

## [License](LICENSE)
