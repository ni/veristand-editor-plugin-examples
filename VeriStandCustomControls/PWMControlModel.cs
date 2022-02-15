using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Xml.Linq;
using NationalInstruments.CBSCommon;
using NationalInstruments.CommonModel;
using NationalInstruments.Core;
using NationalInstruments.DynamicProperties;
using NationalInstruments.Hmi.Core.Controls.Models;
using NationalInstruments.Hmi.Core.Services;
using NationalInstruments.SourceModel;
using NationalInstruments.SourceModel.Persistence;
using NationalInstruments.VeriStand.ServiceModel;
using NationalInstruments.VeriStand.SourceModel;
using NationalInstruments.VeriStand.SourceModel.Screen;
using NationalInstruments.VeriStand.SystemStorage;
using static NationalInstruments.Core.ExceptionHelper;

namespace NationalInstruments.VeriStand.CustomControlsExamples
{
    /// <summary>
    /// This class is an implementation of ICustomVeriStandControl. By specifying it as an Export, we are identifying
    /// it as a plugin to VeriStand.
    /// The interface implementation defines how the control should appear in the palette.
    /// </summary>
    [Export(typeof(ICustomVeriStandControl))]
    public class PulseWidthModulationControlModelExporter : ICustomVeriStandControl
    {
        /// <summary>
        /// MergeScript which defines what to drop on the screen from the palette.  Can be used to set default values on the control
        /// </summary>
        public string Target =>
            "<pf:MergeScript xmlns:pf=\"http://www.ni.com/PlatformFramework\">" +
                "<pf:MergeItem>" +
                    "<PWMControl xmlns=\"http://www.your-company.com/VeriStandExample\" Width=\"[float]220\" Height=\"[float]175\"/>" +
                "</pf:MergeItem>" +
            "</pf:MergeScript>";

        /// <summary>
        /// Name of the control as it will appear in the palette
        /// </summary>
        public string Name => "PWM Control";

        /// <summary>
        /// Path to the image to use in the palette
        /// </summary>
        public string ImagePath => "/NationalInstruments.VeriStand.CustomControlsExamples;component/Resources/TestIcon_32x32.png";

        /// <summary>
        /// Tool tip to display in the palette
        /// </summary>
        public string ToolTip => "PWM Control which maps to a duty cycle and frequency channel.";

        /// <summary>
        /// Unique id for the control. The only requirement is that this doesn't overlap with existing controls or other custom controls.
        /// This is used for serialization and the context help system.
        /// </summary>
        public string UniqueId => "PWMControl";

        /// <summary>
        /// Returns the palette hierarchy for this element. Returning null tells VeriStand to put this in the top level custom controls directory.
        /// </summary>
        public IList<PaletteElementCategory> PaletteHierarchy => null;
    }

    /// <summary>
    /// Model class which defines the business logic for the PWM Control.
    /// </summary>
    public class PulseWidthModulationControlModel :
        VisualModel,
#if MUTATE2020R4
        IDataEngineStateChangeObserver
#else
        ISubscribeProviderStatusUpdates
#endif
    {
        /// <summary>
        /// The name to use for serialization of this model.  This name must match the name used in the Target xml in the ICustomVeriStandControl interface
        /// </summary>
        private const string NumericTextName = "PWMControl";

        /// <summary>
        /// String used to put errors from this control in their own bucket so code from this model doesn't interfere with the rest of the error
        /// reporting behavior in VeriStand
        /// </summary>
        private const string PwmControlModelErrorString = "PWMControlModelErrors";

        /// <summary>
        /// Specifies the name of the frequency channel
        /// </summary>
        public const string FrequencyChannelName = "FrequencyChannel";

        /// <summary>
        /// Specifies the PropertySymbol for the first registered channel.  Any custom attribute that needs to serialized so that it is saved needs to be a property symbol.
        /// </summary>
        public static readonly PropertySymbol FrequencyChannelSymbol =
            ExposePropertySymbol<PulseWidthModulationControlModel>(FrequencyChannelName, string.Empty);

        /// <summary>
        /// Specifies the name of the duty cycle channel property
        /// </summary>
        public const string DutyCycleChannelName = "DutyCycleChannel";

        /// <summary>
        /// Specifies the PropertySymbol for the duty cycle channel
        /// </summary>
        public static readonly PropertySymbol DutyCycleChannelSymbol =
            ExposePropertySymbol<PulseWidthModulationControlModel>(DutyCycleChannelName, string.Empty);

        /// <summary>
        /// Provide a xaml generation helper. This is used to help generate xaml for the properties on this control.
        /// </summary>
        public override XamlGenerationHelper XamlGenerationHelper
        {
            get { return new PwmControlXamlHelper(); }
        }

        /// <summary>
        /// Gets the type of the specified property.  This must be implemented for any new properties that get added that need to be serialized.
        /// </summary>
        /// <param name="identifier">The property to get the type of.</param>
        /// <returns>The exact runtime type of the specified property.</returns>
        public override Type GetPropertyType(PropertySymbol identifier)
        {
            switch (identifier.Name)
            {
                case FrequencyChannelName:
                    return typeof(string);
                case DutyCycleChannelName:
                    return typeof(string);
                default:
                    return base.GetPropertyType(identifier);
            }
        }

        /// <summary>
        /// Gets the default value of the specified property.  This must be implemented for any new properties that get added that need to be serialized.
        /// </summary>
        /// <param name="identifier">The property to get the default value of.</param>
        /// <returns>The default value of the specified property.</returns>
        public override object DefaultValue(PropertySymbol identifier)
        {
            switch (identifier.Name)
            {
                case FrequencyChannelName:
                    return string.Empty;
                case DutyCycleChannelName:
                    return string.Empty;
                default:
                    return base.DefaultValue(identifier);
            }
        }

        /// <summary>
        /// XML element name, including full namespace, for universal persistence.
        /// </summary>
        public override XName XmlElementName
        {
            get { return XName.Get(NumericTextName, PluginNamespaceSchema.ParsableNamespaceName); }
        }

        /// <summary>
        /// Factory method for creating a new PWMControlModel
        /// </summary>
        /// <param name="info">Information required to create the model, such as the parser.</param>
        /// <returns>A constructed and initialized PulseWidthModulationControlModel instance.</returns>
        [XmlParserFactoryMethod(NumericTextName, PluginNamespaceSchema.ParsableNamespaceName)]
        public static PulseWidthModulationControlModel Create(IElementCreateInfo info)
        {
            var model = new PulseWidthModulationControlModel();
#if MUTATE2020
            model.Initialize(info);
#else
            model.Init(info);
#endif
            return model;
        }

        /// <summary>
        /// Private class which helps with xaml generation for this model.  For most custom models this should just need to override the control type from the generic XamlGenerationHelper
        /// </summary>
        private class PwmControlXamlHelper : XamlGenerationHelper
        {
            public override Type ControlType => typeof(PulseWidthModulationControl);
        }

        /// <summary>
        ///   Called when VeriStand connects to the gateway. This control should register for the channel value change
        /// events it is interested in when this happens.
        /// </summary>
        /// <returns>Task which can be awaited</returns>
        public async Task OnConnectedAsync()
        {
            // use Host.BeginInvoke to clear error messages when connecting to the gateway.  The error message collection must be interacted with by the UI thread
            // which is why we must use BeginInvoke since OnConnectToGateway is not guaranteed to be called by the UI thread
            Host.BeginInvoke(AsyncTaskPriority.WorkerHigh, () =>
                {
                    MessageScope?.AllMessages.ClearMessageByCategoryAndReportingElement(PwmControlModelErrorString, this);
                });
            if (!string.IsNullOrEmpty(FrequencyChannel))
            {
                try
                {
                    await Host.GetRunTimeService<ITagService>().RegisterTagAsync(FrequencyChannel, OnFrequencyChannelValueChange);
                }
                catch (Exception ex) when (ShouldExceptionBeCaught(ex))
                {
                    ReportErrorToModel(ex);
                }
            }
            if (!string.IsNullOrEmpty(DutyCycleChannel))
            {
                try
                {
                    await Host.GetRunTimeService<ITagService>().RegisterTagAsync(DutyCycleChannel, OnDutyCycleChannelValueChange);
                }
                catch (Exception ex) when (ShouldExceptionBeCaught(ex))
                {
                    ReportErrorToModel(ex);
                }
            }
        }

        private void ReportErrorToModel(Exception ex)
        {
            // Clear any existing errors and then report a new error message.  Use Host.BeginInvoke here since this must occur on the UI thread
            Host.BeginInvoke(AsyncTaskPriority.WorkerHigh, () =>
            {
                MessageScope?.AllMessages.ClearMessageByCategoryAndReportingElement(
                    PwmControlModelErrorString,
                    this);
#if MUTATE2021
                this.SafeReportError(PwmControlModelErrorString, null, MessageDescriptor.Empty, ex);
#else
                this.ReportError(PwmControlModelErrorString, null, MessageDescriptor.Empty, ex);
#endif
            });
        }

        /// <summary>
        /// Called when VeriStand is connecting to the gateway.
        /// </summary>
        /// <returns>awaitable task</returns>
        public Task OnConnectingAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Called when VeriStand is disconnecting from the VeriStand gateway or the control is removed from the application.
        /// The control should unregister from channel value change events when this happens.
        /// </summary>
        /// <returns>Task which can be awaited</returns>
        public async Task OnDisconnectingAsync()
        {
            // use Host.BeginInvoke to clear error messages when connecting to the gateway.  The error message collection must be interacted with by the UI thread
            // which is why we must use BeginInvoke since OnConnectToGateway is not guaranteed to be called by the UI thread
            Host.BeginInvoke(
                AsyncTaskPriority.WorkerHigh,
                () =>
                    MessageScope?.AllMessages.ClearMessageByCategoryAndReportingElement(
                        PwmControlModelErrorString,
                        this));
            if (!string.IsNullOrEmpty(FrequencyChannel))
            {
                try
                {
                    await Host.GetRunTimeService<ITagService>().UnregisterTagAsync(FrequencyChannel, OnFrequencyChannelValueChange);
                }
                catch (Exception ex) when (ShouldExceptionBeCaught(ex))
                {
                    ReportErrorToModel(ex);
                }
            }
            if (!string.IsNullOrEmpty(DutyCycleChannel))
            {
                try
                {
                    await Host.GetRunTimeService<ITagService>().UnregisterTagAsync(DutyCycleChannel, OnDutyCycleChannelValueChange);
                }
                catch (Exception ex) when (ShouldExceptionBeCaught(ex))
                {
                    ReportErrorToModel(ex);
                }
            }
        }

        /// <summary>
        /// Called when VeriStand becomes disconnected from the gateway.
        /// </summary>
        /// <returns>awaitable task</returns>
        public Task OnDisconnectedAsync()
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task OnStartAsync()
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task OnShutdownAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Called when the view model observes a change in a model property.
        /// Used to reflect model property changes on the VeriStand gateway (when necessary).
        /// </summary>
        /// <param name="modelElement">The model whose property changed. Unused by this method.</param>
        /// <param name="propertyName">The name of the property that changed.</param>
        /// <param name="transactionItem">The transaction item associated with the property change.</param>
        public void PropertyChanged(Element modelElement, string propertyName, TransactionItem transactionItem)
        {
            ScreenModel owningScreen = ScreenModel.GetScreen(this);
            switch (propertyName)
            {
                case DutyCycleChannelName:
                    HandleChannelChangeAsync(transactionItem, owningScreen, OnDutyCycleChannelValueChange).IgnoreAwait();
                    break;
                case FrequencyChannelName:
                    HandleChannelChangeAsync(transactionItem, owningScreen, OnFrequencyChannelValueChange).IgnoreAwait();
                    break;
            }
        }

        /// <summary>
        /// This method is called when the channel itself is changed
        /// </summary>
        /// <param name="transactionItem">Information about the change in the model</param>
        /// <param name="owningScreen">Screen which owns this control</param>
        /// <param name="channelChangeHandler">The handler to use for registering with the gateway. Since we have two different channels
        /// we must make sure to provide the gateway with the correct handler</param>
        private async Task HandleChannelChangeAsync(
            TransactionItem transactionItem,
            ScreenModel owningScreen,
            Action<ITagValue> channelChangeHandler)
        {
            var propertyExpressionTransactionItem =
                transactionItem as PropertyExpressionTransactionItem;
            // Lots of conditions where we ignore the channel change
            if (propertyExpressionTransactionItem == null
                || Host.ActiveRunTimeServiceProvider().Status != RunTimeProviderStatus.Connected
                || Equals(propertyExpressionTransactionItem.OldValue, propertyExpressionTransactionItem.NewValue)
                || owningScreen == null)
            {
                return;
            }

            // The oldValue can be thought of as what the channel value currently is and the newValue is what it will become.
            string oldValue, newValue;

            // transaction state being rolled back means that we are undoing something. for our purposes this reverses the logic for which item in the transaction is the old value and which is the new value
            if (transactionItem.State == TransactionState.RolledBack)
            {
                oldValue = propertyExpressionTransactionItem.NewValue as string;
                newValue = propertyExpressionTransactionItem.OldValue as string;
            }
            else
            {
                oldValue = propertyExpressionTransactionItem.OldValue as string;
                newValue = propertyExpressionTransactionItem.NewValue as string;
            }

            if (CheckForVectorChannels(!string.IsNullOrEmpty(newValue) ? newValue : oldValue))
            {
                return;
            }

            // Depending on whether there are values for old value and new value, either register, unregister, or rebind (which is unregister and register). Rebind is used to avoid
            // some race conditions with registration when the same control is being unregistered and re-registered to something else
            if (!string.IsNullOrEmpty(oldValue))
            {
                try
                {
                    await Host.GetRunTimeService<ITagService>().UnregisterTagAsync(oldValue, channelChangeHandler);
                }
                catch (Exception ex) when (ShouldExceptionBeCaught(ex))
                {
                    ReportErrorToModel(ex);
                }
            }
            if (!string.IsNullOrEmpty(newValue))
            {
                try
                {
                    await Host.GetRunTimeService<ITagService>().UnregisterTagAsync(newValue, channelChangeHandler);
                }
                catch (Exception ex) when (ShouldExceptionBeCaught(ex))
                {
                    ReportErrorToModel(ex);
                }
            }
        }

        /// <summary>
        /// Acquires a read lock on the entire model that the associated element is a part of.
        /// the model includes all elements which share a transaction manager.
        /// </summary>
        /// <returns>Disposable to dispose of to release the read lock.</returns>
        protected IDisposable AcquireReadLock()
        {
            return AcquireModelReadLock();
        }

        private bool CheckForVectorChannels(string channelName)
        {
            BaseNodeType node;
            Host.GetSharedExportedValue<VeriStandHelper>()
                .ActiveSystemDefinition.Root.BaseNodeType.FindNode(
                    StringArrayToNodePathUtils.NodePathToStringArray(channelName), out node);
            ChannelType channel = null;
            if (node is AliasType)
            {
                channel = (node as AliasType).ResolveAliasReference as ChannelType;
            }
            else if (node is ChannelType)
            {
                channel = node as ChannelType;
            }
            if (channel != null && !(channel.RowDim == 1 && channel.ColDim == 1))
            {
                ReportError(PwmControlModelErrorString, null, MessageDescriptor.Empty, "Cannot Register Vector Channels");
                return true;
            }
            return false;
        }

        /// <summary>
        /// Occurs when the model has been updated with a new channel value by the VeriStand gateway.
        /// </summary>
        public event EventHandler<ChannelValueChangedEventArgs> FrequencyChannelValueChangedEvent;

        /// <summary>
        /// Occurs when the model has been updated with a new channel value by the VeriStand gateway.
        /// </summary>
        public event EventHandler<ChannelValueChangedEventArgs> DutyCycleChannelValueChangedEvent;

        /// <summary>
        /// Raises the FrequencyChannelValueChangedEvent. Invoked when the channel value changes.
        /// </summary>
        protected virtual void OnFrequencyChannelValueChangedEvent()
        {
            EventHandler<ChannelValueChangedEventArgs> channelValueChangedSubscribers =
                FrequencyChannelValueChangedEvent;
            if (channelValueChangedSubscribers != null)
            {
                channelValueChangedSubscribers(this, new ChannelValueChangedEventArgs(FrequencyChannelValue));
            }
        }

        /// <summary>
        /// Raises the DutyCycleChannelValueChangedEvent. Invoked when the channel value changes.
        /// </summary>
        protected virtual void OnDutyCycleChannelValueChangedEvent()
        {
            EventHandler<ChannelValueChangedEventArgs> channelValueChangedSubscribers =
                DutyCycleChannelValueChangedEvent;
            if (channelValueChangedSubscribers != null)
            {
                channelValueChangedSubscribers(this, new ChannelValueChangedEventArgs(DutyCycleChannelValue));
            }
        }

        /// <summary>
        /// These objects are used as owners for the gateway collator.  The gateway collators only stores one action per owner at a time and sends it to/receives from  the gateway periodically.   This
        /// is limit the rate at which things are sent/received from the gateway to avoid flooding the WCF pipe or falling behind in time.  Since this control has two buckets of things to collate against each other (frequency updates,
        /// and duty cycle updates, we need two owners to keep one controls updates from overwriting the others updates in the collator
        /// </summary>
        private readonly object _frequencyCollatorOwner = new object();
        private readonly object _dutyCycleCollatorOwner = new object();

        /// <summary>
        /// Fired when frequency channel value changes
        /// </summary>
        /// <param name="value">new tag value</param>
        private void OnFrequencyChannelValueChange(ITagValue value)
        {
            double newChannelValue = (double)value.Value;
            using (AcquireReadLock())
            {
                // The visual parent is null if the item is deleted, this is not null for models that are within a container
                // and are not directly the children of the screen surface.
                if (VisualParent == null)
                {
                    return;
                }
                ScreenModel screenModel = ScreenModel.GetScreen(this);
                // add an action to the collator.  the collator will limit the number of actions coming from the gateway and only
                // process the most recent action. This keeps us from falling behind in time if we can't process the gateway updates as fast as they are received.
                screenModel.FromGatewayActionCollator.AddAction(
                    _frequencyCollatorOwner,
                    () =>
                    {
                        using (AcquireReadLock())
                        {
                            // The item could get deleted after the action has been dispatched.
                            if (VisualParent != null)
                            {
                                if (!Equals(FrequencyChannelValue, newChannelValue))
                                {
                                    FrequencyChannelValue = newChannelValue;
                                    OnFrequencyChannelValueChangedEvent();
                                }
                            }
                        }
                    });
            }
        }

        /// <summary>
        /// Fired when the duty cyle channel value changes
        /// </summary>
        /// <param name="value">new tag value</param>
        private void OnDutyCycleChannelValueChange(ITagValue value)
        {
            double newChannelValue = (double)value.Value;
            using (AcquireReadLock())
            {
                // The visual parent is null if the item is deleted, this is not null for models that are within a container
                // and are not directly the children of the screen surface.
                if (VisualParent == null)
                {
                    return;
                }
                ScreenModel screenModel = ScreenModel.GetScreen(this);
                screenModel.FromGatewayActionCollator.AddAction(
                    _dutyCycleCollatorOwner,
                    () =>
                    {
                        using (AcquireReadLock())
                        {
                            // The item could get deleted after the action has been dispatched.
                            if (VisualParent != null)
                            {
                                DutyCycleChannelValue = newChannelValue;
                                OnDutyCycleChannelValueChangedEvent();
                            }
                        }
                    });
            }
        }

        /// <summary>
        /// Gets the frequency channel value
        /// </summary>
        public double FrequencyChannelValue { get; protected set; }

        /// <summary>
        /// Gets the duty cycle channel value
        /// </summary>
        public double DutyCycleChannelValue { get; protected set; }

        /// <summary>
        /// Gets or sets the path to the VeriStand channel associated with this control models frequency
        /// </summary>
        public string FrequencyChannel
        {
            get { return ImmediateValueOrDefault<string>(FrequencyChannelSymbol); }
            set { SetOrReplaceImmediateValue(FrequencyChannelSymbol, value); }
        }

        /// <summary>
        /// Gets or sets the path to the VeriStand channel associated with this control models duty cycle
        /// </summary>
        public string DutyCycleChannel
        {
            get { return ImmediateValueOrDefault<string>(DutyCycleChannelSymbol); }
            set { SetOrReplaceImmediateValue(DutyCycleChannelSymbol, value); }
        }

        /// <summary>
        /// Called by the view or view model to set the fault state on the model
        /// </summary>
        /// <param name="faultState">The state to fault to</param>
        /// <returns>A task which completes when set fault is done </returns>
        public async Task SetFaultStateAsync(FaultState faultState)
        {
            var faultService = Host.GetRunTimeService<IFaultService>();
            if (Host.IsConnectedToRunningGateway() && !string.IsNullOrEmpty(DutyCycleChannel) && faultService != null)
            {
                try
                {
                    switch (faultState)
                    {
                        case FaultState.Normal:
                            await faultService.ClearFaultAsync(DutyCycleChannel);
                            break;
                        case FaultState.FaultedHigh:
                            await faultService.SetFaultValueAsync(DutyCycleChannel, 100.0);
                            break;
                        case FaultState.FaultedLow:
                            await faultService.SetFaultValueAsync(DutyCycleChannel, 0.0);
                            break;
                    }
                }
                catch (CodedException e)
                {
                    FaultCompleteFailure(e);
                }
            }
        }

        private void FaultCompleteFailure(CodedException error)
        {
            Host.Dispatcher.InvokeIfNecessary(
                  PlatformDispatcher.AsyncOperationAlwaysValid,
                  () => ReportError(
                      PwmControlModelErrorString,
                      null,
                      MessageDescriptor.Empty,
                   error.ResolvedErrorMessage));
        }

        /// <summary>
        /// Called by the view when a value change occurs.  The view fires this for both duty cycle and frequency value changes and the event args let us
        /// tell which one was fired
        /// </summary>
        /// <param name="channelName">The name of the channel to set the value on.</param>
        /// <param name="channelValue">The new channel value.</param>
        public void SetChannelValue(string channelName, double channelValue)
        {
            // set the collator owner to be different for the different channel value change operations so a value change for one of the controls doesn't
            // erase the value change for the other one
            var collatorOwner = channelName == PulseWidthModulationControlModel.DutyCycleChannelName ? _dutyCycleCollatorOwner : _frequencyCollatorOwner;
            if (Host.ActiveRunTimeServiceProvider().Status == RunTimeProviderStatus.Connected)
            {
                ScreenModel screenModel = ScreenModel.GetScreen(this);
                if (screenModel != null)
                {
                    // Use the action collator to make sure we are not generating more set channel value calls than we can handle.
                    screenModel.ToGatewayActionCollator.AddAction(collatorOwner, async () =>
                    {
                        try
                        {
                            if (channelName == PulseWidthModulationControlModel.DutyCycleChannelName && !string.IsNullOrEmpty(DutyCycleChannel))
                            {
                                // set the channel value on the gateway, we are passing in empty labda expressions to the success and failure callbacks in this case
                                // if we wanted to report errors to the user we could add some handling code for the failure case
                                await Host.GetRunTimeService<ITagService>().SetTagValueAsync(DutyCycleChannel, TagFactory.CreateTag(channelValue));
                            }
                            if (channelName == PulseWidthModulationControlModel.FrequencyChannelName && !string.IsNullOrEmpty(FrequencyChannel))
                            {
                                await Host.GetRunTimeService<ITagService>().SetTagValueAsync(FrequencyChannel, TagFactory.CreateTag(channelValue));
                            }
                        }
                        catch (VeriStandException e)
                        {
                            Host.Dispatcher.InvokeIfNecessary(
                                PlatformDispatcher.AsyncOperationAlwaysValid,
#if MUTATE2021
                                () => this.SafeReportError(
#else
                                () => this.ReportError(
#endif
                                    PwmControlModelErrorString,
                                    null,
                                    MessageDescriptor.Empty,
                                    e));
                        }
                    });
                }
            }
        }
    }
}
