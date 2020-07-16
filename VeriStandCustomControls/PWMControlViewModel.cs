using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using NationalInstruments.Composition;
using NationalInstruments.Controls.Shell;
using NationalInstruments.Core;
using NationalInstruments.Design;
using NationalInstruments.Hmi.Core.Controls.Models;
using NationalInstruments.Hmi.Core.Screen;
using NationalInstruments.Shell;
using NationalInstruments.SourceModel;
using NationalInstruments.VeriStand.ServiceModel;
using NationalInstruments.VeriStand.Shell;
using NationalInstruments.VeriStand.Tools;

namespace NationalInstruments.VeriStand.CustomControlsExamples
{
    /// <summary>
    /// The view model which controls how changes on the view are propagated to the model.
    /// This ViewModel extends directly from VisualViewModel which is the base view model for all PF control view models
    /// This class implementes IControlContextMenuHelper which gives it the ability to provide custom right click menus
    /// </summary>
    public class PulseWidthModulationControlViewModel : VisualViewModel, IControlContextMenuHelper
    {
        /// <summary>
        /// Constructs a new instance of the PulseWidthModulationControlViewModel class
        /// </summary>
        /// <param name="model">The PulseWidthModulationControlModel associated with this view model.</param>
        public PulseWidthModulationControlViewModel(PulseWidthModulationControlModel model)
            : base(model)
        {
            // Register for channel value change events on the model.  The weak event manager is used here since it helps prevent memory leaks associated
            // with registering for events and lets us be less careful about unregistering for these events at a later time.
            WeakEventManager<PulseWidthModulationControlModel, ChannelValueChangedEventArgs>.AddHandler(model, "DutyCycleChannelValueChangedEvent", DutyCycleValueChangedEventHandler);
            WeakEventManager<PulseWidthModulationControlModel, ChannelValueChangedEventArgs>.AddHandler(model, "FrequencyChannelValueChangedEvent", FrequencyValueChangedEventHandler);
        }

        /// <summary>
        /// Override resize behavior so the control cannot be resized.  This is done because with composite controls it is a lot of work to get all the individual components
        /// to scale reasonably with respect to each other
        /// </summary>
        public override ResizeDirections ResizeDirections
        {
            get
            {
                return ResizeDirections.None;
            }
        }

        private void DutyCycleValueChangedEventHandler(object sender, ChannelValueChangedEventArgs e)
        {
            var pwmControl = View.Children.FirstOrDefault().AsFrameworkElement as PulseWidthModulationControl;
            if (pwmControl != null)
            {
                pwmControl.DutyCycleValue = (double)e.ChannelValue;
                // don't pass updates to the slider value if we are supressing value updates
                if (!SuppressValueChanges)
                {
                    pwmControl.SliderDutyCycleValue = (double)e.ChannelValue;
                }
            }
        }

        private void FrequencyValueChangedEventHandler(object sender, ChannelValueChangedEventArgs e)
        {
            var pwmControl = View.Children.FirstOrDefault().AsFrameworkElement as PulseWidthModulationControl;
            if (pwmControl != null)
            {
                pwmControl.FrequencyValue = (double)e.ChannelValue;
            }
        }

        /// <summary>
        /// Gets or sets if model value changes should be suppressed on the view.
        /// </summary>
        internal bool SuppressValueChanges { get; set; }

        /// <summary>
        /// Creates the view associated with this view model by initializing a new instance of our custom control class PWMControl
        /// This is an opportunity to provide callbacks to the view and to hook up event handlers.  In this case we add a value changed event handler so we can
        /// react when the view changes value.
        /// </summary>
        /// <returns>pwmcontrol view</returns>
        public override object CreateView()
        {
            var view = new PulseWidthModulationControl(this);
            WeakEventManager<PulseWidthModulationControl, FaultStateChangeEventArgs>.AddHandler(view, "FaultStateChanged", FaultStateChanged);
            WeakEventManager<PulseWidthModulationControl, CustomChannelValueChangedEventArgs>.AddHandler(view, "ValueChanged", SetChannelValue);
            return view;
        }

        private void FaultStateChanged(object sender, FaultStateChangeEventArgs e)
        {
            ((PulseWidthModulationControlModel)Model).SetFaultStateAsync(e.FaultState).IgnoreAwait();
        }

        /// <summary>
        /// Called when a property of the model associated with this view model has changed.
        /// ChannelControlViewModel forwards this change to the associated model so that the model can respond to property changes
        /// that require interactions with the VeriStand gateway
        /// </summary>
        /// <param name="modelElement">The model that changed.</param>
        /// <param name="propertyName">The name of the property that changed.</param>
        /// <param name="transactionItem">The transaction item associated with the change.</param>
        public override void ModelPropertyChanged(Element modelElement, string propertyName, TransactionItem transactionItem)
        {
            base.ModelPropertyChanged(modelElement, propertyName, transactionItem);
            ((PulseWidthModulationControlModel)Model).PropertyChanged(modelElement, propertyName, transactionItem);
        }

        /// <summary>
        ///  Creates configuration pane content for this control. See comments on
        ///  <see cref="IProvideCommandContent"/> for more information about correct usage of this function.
        /// </summary>
        /// <param name="context">The current display context</param>
        public override void CreateCommandContent(ICommandPresentationContext context)
        {
            base.CreateCommandContent(context);
            // specify that we are adding things to the configuration pane
            using (context.AddConfigurationPaneContent())
            {
                // First add the group command which lets us know what top level configuration pane group to put the child commands in
                using (context.AddGroup(ConfigurationPaneCommands.BehaviorGroupCommand))
                {
                    // add child commands whose visuals will show up in the specified parent group.
                    context.Add(FrequencyChannelBrowseCommand);
                    context.Add(DutyCycleChannelBrowseCommand);
                }
            }
        }

        private static ChannelPopup _uiSdfBrowsePopup;

        private static IEnumerable<IViewModel> _currentSelection;

        private static bool _settingFrequency;

        /// <summary>
        /// Command to launch the channel browser.
        /// </summary>
        public static readonly ISelectionCommand FrequencyChannelBrowseCommand = new ShellSelectionRelayCommand(LaunchDutyCycleChannelBrowser, CanLaunchChannelBrowser)
        {
            LabelTitle = "Duty Cycle Channel",
            LargeImageSource = ResourceHelpers.LoadImage(typeof(PulseWidthModulationControlViewModel), "Resources/Browse.png"),
            SmallImageSource = ResourceHelpers.LoadImage(typeof(PulseWidthModulationControlViewModel), "Resources/Browse_16x16.png"),
            UniqueId = "NI.ChannelCommands:BrowseForFrequencyChannelCommand",
            UIType = UITypeForCommand.Button
        };

        private static void LaunchDutyCycleChannelBrowser(ICommandParameter parameter, IEnumerable<IViewModel> selection, ICompositionHost host, DocumentEditSite site)
        {
            _settingFrequency = false;
           LaunchChannelBrowser(parameter, selection, host, site);
        }

        /// <summary>
        /// Command to launch the channel browser.
        /// </summary>
        public static readonly ISelectionCommand DutyCycleChannelBrowseCommand = new ShellSelectionRelayCommand(LaunchFrequencyChannelBrowser, CanLaunchChannelBrowser)
        {
            LabelTitle = "Frequency Channel",
            LargeImageSource = ResourceHelpers.LoadImage(typeof(PulseWidthModulationControlViewModel), "Resources/Browse.png"),
            SmallImageSource = ResourceHelpers.LoadImage(typeof(PulseWidthModulationControlViewModel), "Resources/Browse_16x16.png"),
            UniqueId = "NI.ChannelCommands:BrowseForFrequencyChannelCommand",
            UIType = UITypeForCommand.Button
        };

        private static void LaunchFrequencyChannelBrowser(ICommandParameter parameter, IEnumerable<IViewModel> selection, ICompositionHost host, DocumentEditSite site)
        {
            _settingFrequency = true;
            LaunchChannelBrowser(parameter, selection, host, site);
        }

        /// <summary>
        /// This can execute method is run periodically by the command to determine whether it should be enabled or disabled.
        /// </summary>
        private static bool CanLaunchChannelBrowser(ICommandParameter parameter, IEnumerable<IViewModel> selection, ICompositionHost host, DocumentEditSite site)
        {
            return selection.All(s => s.Model is PulseWidthModulationControlModel);
        }

        private static void LaunchChannelBrowser(ICommandParameter parameter, IEnumerable<IViewModel> selection, ICompositionHost host, DocumentEditSite site)
        {
            _uiSdfBrowsePopup = new ChannelPopup
            {
                ShowWaveforms = false,
                Name = "UI_SDFBrowsePopup",
                IsOpen = false,
                Placement = Placement.BelowCenter
            };
            // Register the property changed callback for the popup window
            WeakEventManager<ChannelPopup, PropertyChangedEventArgs>.AddHandler(_uiSdfBrowsePopup, "PropertyChanged", ChannelNamePropertyChanged);
            // Show Popup window with Channels.
            _currentSelection = selection.ToList();
            _uiSdfBrowsePopup.PlacementTarget = (UIElement)parameter.AssociatedVisual;
            _uiSdfBrowsePopup.Channel = _settingFrequency ? ((PulseWidthModulationControlModel)_currentSelection.First().Model).FrequencyChannel : ((PulseWidthModulationControlModel)_currentSelection.First().Model).DutyCycleChannel;
            _uiSdfBrowsePopup.ShowSdfBrowser(host, true, false);
        }

        private static void ChannelNamePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == ChannelControlModelPropertyNames.ChannelName)
            {
                foreach (IViewModel selectedViewModel in _currentSelection)
                {
                    var uiModel = (UIModel)selectedViewModel.Model;
                    // we are setting values on the model so start a new transaction. set the purpose of the transaction to user so that it can be undone
                    using (var transaction = uiModel.TransactionManager.BeginTransaction("Set channel", TransactionPurpose.User))
                    {
                        var pwmControlModel = uiModel as PulseWidthModulationControlModel;
                        if (pwmControlModel != null)
                        {
                            if (_settingFrequency)
                            {
                                pwmControlModel.FrequencyChannel = _uiSdfBrowsePopup.Channel;
                            }
                            else
                            {
                                pwmControlModel.DutyCycleChannel = _uiSdfBrowsePopup.Channel;
                            }
                        }
                        transaction.Commit();
                    }
                }
            }
        }

        /// <summary>
        /// Gets the adorners used with this control during a hard selection (left-click).
        /// Currently used to create an adorner that allows browsing to two channel paths
        /// </summary>
        /// <returns>An enumerable collection of hard select adorners.</returns>
        public override IEnumerable<Adorner> GetHardSelectAdorners()
        {
            var adorners = new Collection<Adorner>();
            var toolbar = new FloatingToolBar();

            // if there is no model then don't return adorners
            if (Model == null)
            {
                return adorners;
            }
            // create a new instance of our adorner class and add it to the floating toolbar which is used as the container for the adorner
            var control = new TwoChannelAdorner(DesignerNodeHelpers.GetVisualForViewModel(this));
            toolbar.ToolBar = control;
            adorners.Add(new ControlAdorner(DesignerNodeHelpers.GetVisualForViewModel(this), toolbar, Placement.BelowCenter));
            return adorners;
        }

        /// <summary>
        /// Creates and returns a list of context menu commands for this view model
        /// </summary>
        /// <returns>List of context menu commands for this view model</returns>
        public virtual IEnumerable<ShellCommandInstance> CreateContextMenuCommands()
        {
            var commands = new List<ShellCommandInstance>();
            commands.Add(
                new ShellCommandInstance(SelectChannelsCommand)
                {
                    LabelTitle = "Select Channels In Tree"
                });
            return commands;
        }

        /// <summary>
        /// Gets Unique IDs to be filtered from context menu commands
        /// </summary>
        public virtual IEnumerable<string> FilterContextMenuCommands()
        {
            return Enumerable.Empty<string>();
        }

        /// <summary>
        /// Select the current control channel in the system definition tree
        /// </summary>
        public static readonly ShellSelectionRelayCommand SelectChannelsCommand = new ShellSelectionRelayCommand(SelectChannels, CanSelectChannel)
        {
            LabelTitle = "Select Channels",
            UniqueId = "NI.ChannelCommands:SelectChannelsInSystemDefinitionTree",
            UIType = UITypeForCommand.Button
        };

        private static bool CanSelectChannel(ICommandParameter parameter, IEnumerable<IViewModel> selection, ICompositionHost host, DocumentEditSite site)
        {
            return host.GetSharedExportedValue<VeriStandHelper>().IsSystemDefinitionValid;
        }

        private static void SelectChannels(ICommandParameter parameter, IEnumerable<IViewModel> selection, ICompositionHost host, DocumentEditSite site)
        {
            IEnumerable<string> frequencyChannels = selection.Select(item => item.Model).OfType<PulseWidthModulationControlModel>().Select(model => model.FrequencyChannel).ToList();
            IEnumerable<string> dutyCycleChannels = selection.Select(item => item.Model).OfType<PulseWidthModulationControlModel>().Select(model => model.DutyCycleChannel).ToList();
            host.GetSharedExportedValue<SystemDefinitionPaletteControl>().SelectNodes(dutyCycleChannels.Concat(frequencyChannels));
        }

        /// <summary>
        /// Called by the view when a value change occurs.  The view fires this for both duty cycle and frequency value changes and the event args let us
        /// tell which one was fired
        /// </summary>
        /// <param name="sender">sending object - not used</param>
        /// <param name="eventArgs">custom event information telling us which channel changed and what its value is</param>
        private void SetChannelValue(object sender, CustomChannelValueChangedEventArgs eventArgs)
        {
            ((PulseWidthModulationControlModel)Model).SetChannelValue(eventArgs.ChannelName, (double)eventArgs.ChannelValue);
        }
    }
}
