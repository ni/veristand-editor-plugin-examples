using System;
using System.Collections.Generic;
using NationalInstruments.Core;
using NationalInstruments.Design;
using NationalInstruments.Restricted.FeatureToggles;
using NationalInstruments.Shell;
using NationalInstruments.VeriStand.Application;
using NationalInstruments.VeriStand.Design;

namespace NationalInstruments.VeriStand.CustomControlsExamples
{
    /// <summary>
    /// This is the primary plugin point for customizing the application.
    /// Unlike other plugins, we can't export it, we actually need to launch our own executable using this featureset.
    /// See the CustomVeriStand project.
    /// </summary>
    public class CustomApplicationFeatureSet : VeriStandApplicationFeatureSet
    {
        /// <summary>
        /// Our default constructor
        /// </summary>
        /// <param name="startupInfo">Startup information</param>
        internal CustomApplicationFeatureSet(CustomApplicationStartupInfo startupInfo = null)
            : base(startupInfo ?? new CustomApplicationStartupInfo(null))
        {
        }

        /// <summary>
        /// Guid for windows we want; everything else will be rejected.
        /// </summary>
        private readonly HashSet<Guid> _allowedToolWindows = new HashSet<Guid>(new Guid[]
        {
            // OutputWindowControlType.WindowId,                             // Output Window
            // StandardToolWindowIDs.ProjectExplorerWindowID,                // Project Explorer
            // new Guid(StandardToolWindowIDs.ToolLauncherWindowID),         // Tools launcher
            // StandardToolWindowIDs.MessageWindowID,                        // Errors & warnings
            VeriStandGlobalConstants.VeriStandSystemDefinitionWindowId,   // system definition palette
            // VeriStandGlobalConstants.ModelParameterManagerWindowId,       // Model parameter manager
            // VeriStandGlobalConstants.LogManagementWindowId,               // Log Management control
            // VeriStandGlobalConstants.AlarmMonitorWindowId,                // Alarm monitor control
            // VeriStandGlobalConstants.ChannelFaultManagerWindowId,         // Channel fault manager
            // VeriStandGlobalConstants.ChannelDataViewerWindowId,           // Channel Data Viewer
            // SelectionConfigurationPaneWindowType.WindowId,                // Selection Configuration Pane
            // DocumentConfigurationPaneWindowType.WindowId,                 // Document Configuration Pane
            // ManageTargetsViewModel.WindowId,                              // Manage targets
            // ModelSignalViewerViewModel.WindowId,                          // Model Signal Viewer
        });

        /// <summary>
        /// Override to specify which tool windows should be allowed.
        /// </summary>
        /// <param name="toolWindowInfo">meta data about the toolwindow in question. Generally, we just compare the GUID</param>
        /// <returns>true if it is allowed, false otherwise</returns>
        public override bool AllowToolWindow(IToolWindowTypeInfo toolWindowInfo)
        {
            if (ExamplePreferencePageController.HideEditor)
            {
                return _allowedToolWindows.Contains(new Guid(toolWindowInfo.UniqueID));
            }

            return base.AllowToolWindow(toolWindowInfo);
        }

        /// <summary>
        /// Guid for commands we want; everything else will be rejected.
        /// Note: This is not just menu items. Right click menus, toolbars and right rail property editors also go through this.
        /// </summary>
        private readonly HashSet<string> _allowedCommands = new HashSet<string>(new string[]
        {
            MenuPathCommands.FileMenu.UniqueId,
            MenuPathCommands.FileOpenMenu.UniqueId,
            ShellCommands.OpenProject.UniqueId,
            ShellCommands.Options.UniqueId,
        });

        /// <summary>
        /// Override to disable or hide any menu command. Can also change it's text.
        /// </summary>
        /// <param name="command">The command. We usually identify by the UniqueId, though they are defined all over the place.</param>
        /// <param name="sender">generally unused</param>
        /// <param name="commandParameter">The commandParameter, usually casted to ICommandParameter to do work</param>
        /// <param name="editSite">The DocumentEditSite</param>
        /// <param name="handled">set to true if you handle(prevent further processing)</param>
        /// <returns>true if the command can execute. Returning false will show it as disabled</returns>
        public override bool CanExecuteCommand(ICommandEx command, object sender, object commandParameter, DocumentEditSite editSite, ref bool handled)
        {
            if (ExamplePreferencePageController.HideEditor)
            {
                if (commandParameter is ICommandParameter parameter
                    && !_allowedCommands.Contains(command.UniqueId))
                {
                    parameter.Visible = false;
                    return false;
                }
            }

            return base.CanExecuteCommand(command, sender, commandParameter, editSite, ref handled);
        }

        /// <inheritdoc/>
        public override PlatformVisual CreateLauncherControl()
        {
            if (ExamplePreferencePageController.HideEditor)
            {
                // Command disabling kills the templates as well. Just use the open project command line flag, or double click on a project in explorer to open.
                return null;
            }

            return base.CreateLauncherControl();
        }
    }

    /// <summary>
    /// Light weight startup object. It's primary responsibility is to create the FeatureSet.
    /// </summary>
    public class CustomApplicationStartupInfo : VeriStandStartupInfo
    {
        /// <summary>
        /// Create our light weight startup object
        /// </summary>
        /// <param name="splashScreen">If a custom splash screen was shown, pass it in here</param>
        public CustomApplicationStartupInfo(ISplashScreen splashScreen)
            : base(CodeReadiness.Release, splashScreen)
        {
        }

        /// <inheritdoc />
        public override IApplicationFeatureSet CreateFeatureSet()
            => new CustomApplicationFeatureSet(this);
    }
}
