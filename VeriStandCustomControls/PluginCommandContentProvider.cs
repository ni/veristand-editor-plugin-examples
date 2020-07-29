using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Media;
using NationalInstruments.Composition;
using NationalInstruments.Controls;
using NationalInstruments.Controls.Shell;
using NationalInstruments.Core;
using NationalInstruments.DataTypes;
using NationalInstruments.Design;
using NationalInstruments.Shell;

namespace NationalInstruments.VeriStand.CustomControlsExamples
{
    /// <summary>
    /// Command content plugin for this assembly
    /// <see cref="IPushCommandContent"/> for the various entrypoints where commands can be added.
    /// </summary>
    /// <remarks>The LaunchKeyword is to ensure we are loaded directly at application startup,
    /// which is necessary if you want to add to menus.</remarks>
    [ExportPushCommandContent]
    [BindsToKeyword(DocumentEditSite.LaunchKeyword)]
    public class PluginCommandContentProvider : PushCommandContent
    {
        /// <summary>
        /// Imported composition host for accessing other plugin components
        /// </summary>
        [Import]
        public ICompositionHost Host { get; set; }

        /// <inheritdoc />
        public override void CreateCommandContentForElement(ICommandPresentationContext context)
        {
            base.CreateCommandContentForElement(context);
            // This entrypoint can be used to add commands for any selectable item in the UI.
            // In this case, we own the ViewModel, so could do it in that class, but are showing
            // here as an example and to keep these commands separate.
            if (context.IsSingleSelect<PulseWidthModulationControlViewModel>())
            {
                using (context.AddConfigurationPaneContent())
                {
                    // These are for the individual expandable sections...can make your own or use ours.
                    using (context.AddGroup(ConfigurationPaneCommands.BehaviorGroupCommand))
                    {
                        // Use a command with a UIType, or use/customize the factory yourself
                        context.Add(CheckBoxCommand);
                        ////context.Add(CheckBoxCommand, CheckBoxFactory.ForConfigurationPane);
                        ////context.Add(CheckBoxCommand, ToggleButtonFactory.ForConfigurationPane);
                        context.Add(TextBoxCommand);
                        var filters = new FileDialogFilterCollection();
                        filters.Add(new FileDialogFilter() { Extensions = new[] { "jpg", "png" }, Label = "Image Files" });
                        context.Add(PathCommand, new PathSelectorFactory() { Filters = filters });
                        ////context.Add(TextBoxCommand, TextBoxFactory.ForConfigurationPane);
                        ////context.Add(TextBoxCommand, StaticTextFactory.ForConfigurationPane);
                        context.Add(NumericCommand, new NumericTextBoxFactory(NITypes.Double));
                        context.Add(ColorCommand, new ColorBoxFactory { ColorOnly = false, ShowMoreColors = true });
                        context.AddContentFontEditor(null, true);
                    }
                }
            }
        }

        #region Commands
        private const string UniqueIdPrefix = "VeriStand.Example.Commands.";

        /// <summary>
        /// A boolean command shown as checkbox.
        /// UIType can be used for some standard types, for all others, see examples where a VisualFactory is included.
        /// when adding the command to the ICommandPresentationContext.
        /// </summary>
        public static readonly ICommandEx CheckBoxCommand = new ShellSelectionRelayCommand(HandleExecuteCommand, HandleCanExecuteCommand)
        {
            UniqueId = UniqueIdPrefix + "CheckBox",
            UIType = UITypeForCommand.CheckBox,
            LabelTitle = UITypeForCommand.CheckBox
        };

        /// <summary>
        /// A string command shown as a textbox
        /// UIType can be used for some standard types, for all others, see examples where a VisualFactory is included.
        /// when adding the command to the ICommandPresentationContext.
        /// </summary>
        public static readonly ICommandEx TextBoxCommand = new ShellSelectionRelayCommand(HandleExecuteCommand, HandleCanExecuteCommand)
        {
            UniqueId = UniqueIdPrefix + "TextBox",
            UIType = UITypeForCommand.TextBox,
            LabelTitle = UITypeForCommand.TextBox
        };

        /// <summary>
        /// A string command shown as a path
        /// </summary>
        public static readonly ICommandEx PathCommand = new ShellSelectionRelayCommand(HandleExecuteCommand, HandleCanExecuteCommand)
        {
            UniqueId = UniqueIdPrefix + "Path",
            LabelTitle = "Path"
        };

        /// <summary>
        /// A numeric command
        /// </summary>
        public static readonly ICommandEx NumericCommand = new ShellSelectionRelayCommand(HandleExecuteCommand, HandleCanExecuteCommand)
        {
            UniqueId = UniqueIdPrefix + "Numeric",
            LabelTitle = "Numeric"
        };

        /// <summary>
        /// A color command
        /// </summary>
        public static readonly ICommandEx ColorCommand = new ShellSelectionRelayCommand(HandleExecuteCommand, HandleCanExecuteCommand)
        {
            UniqueId = UniqueIdPrefix + "Color",
            LabelTitle = "Color"
        };

        /// <summary>
        /// A font command
        /// </summary>
        public static readonly ICommandEx FontCommand = new ShellSelectionRelayCommand(HandleExecuteCommand, HandleCanExecuteCommand)
        {
            UniqueId = UniqueIdPrefix + "Font",
            LabelTitle = "Font"
        };

        /// <summary>
        /// A generic CanExecute handler.
        /// The role of this handler is to take the current state in the model and transfer it to the parameter,
        /// which is used by the view to display the current value.
        /// In general, you would likely write a different one for each command that did the appropriate
        /// cast of the parameter to get/set the value.
        /// </summary>
        /// <param name="parameter">command parameter - this is where the current 'value' should be held</param>
        /// <param name="selection">user selection, for commands based on selection</param>
        /// <param name="host">composition host</param>
        /// <param name="site">for communicating with user interface APIs</param>
        /// <returns>true if it can handle</returns>
        public static bool HandleCanExecuteCommand(ICommandParameter parameter, IEnumerable<IViewModel> selection, ICompositionHost host, DocumentEditSite site)
        {
            // probably cast/OfType to your derived view model
            var viewModel = selection.OfType<ElementViewModel>().First();
            var booleanParameter = parameter as ICheckableCommandParameter;
            var choiceParameter = parameter as IChoiceCommandParameter;
            var numericParameter = parameter as IValueCommandParameter;
            var textParameter = parameter as ITextCommandParameter;
            var colorParameter = parameter as ColorCommandParameter;
            if (booleanParameter != null)
            {
                booleanParameter.IsChecked = true;
            }
            else if (choiceParameter != null)
            {
                if (choiceParameter.Choices == null)
                {
                    choiceParameter.Choices = new object[] { "one", "two", "three" }.Select(item => new ChoiceCommandParameterChoice(item, null, item.ToString())).ToArray();
                    choiceParameter.Choices.ElementAt(1).IsEnabled = false;
                }
            }
            else if (textParameter != null)
            {
                // This is for textbox and Path controls
                textParameter.Text = viewModel.Name;
            }
            else if (colorParameter != null)
            {
                // color and others are also value parameters, so we check them first.
                colorParameter.Value = SMBrush.FromBrush(Brushes.Red);
            }
            else if (numericParameter != null)
            {
                numericParameter.Value = 3.14;
            }

            return true; // or false to disable the command
        }

        /// <summary>
        /// Here we handle the user changing the given command.
        /// Our job is to take the current value in the parameter and update our model
        /// </summary>
        /// <param name="parameter">command parameter - this is where the current 'value' should be held</param>
        /// <param name="selection">user selection, for commands based on selection</param>
        /// <param name="host">composition host</param>
        /// <param name="site">for communicating with user interface APIs</param>
        private static void HandleExecuteCommand(ICommandParameter parameter, IEnumerable<IViewModel> selection, ICompositionHost host, DocumentEditSite site)
        {
            var viewModel = selection.OfType<ElementViewModel>().First();
            var booleanParameter = parameter as ICheckableCommandParameter;
            var choiceParameter = parameter as IChoiceCommandParameter;
            var numericParameter = parameter as IValueCommandParameter;
            var textParameter = parameter as ITextCommandParameter;
            var colorParameter = parameter as ColorCommandParameter;
            // Update the model based on current state. In general, changes to the model should
            // be transacted so undo redo works.
            using (var transaction = viewModel.Element.TransactionManager.BeginTransaction("update", NationalInstruments.SourceModel.TransactionPurpose.User))
            {
                // update state of the model based on type of parameter.
                // viewModel.Element.newState = parameter.newValue;
                transaction.Commit();
            }
        }
        #endregion
    }
}
