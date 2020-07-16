using System;
using System.Windows;
using System.Windows.Input;
using NationalInstruments.Controls;

namespace NationalInstruments.VeriStand.CustomControlsExamples
{
    /// <summary>
    /// Callback for changing the fault state
    /// </summary>
    /// <param name="faultState">state to fault to</param>
    public delegate void ChangeFaultStateCallback(FaultState faultState);

    /// <summary>
    /// Interaction logic for PulseWidthModulationControl.xaml
    /// </summary>
    public partial class PulseWidthModulationControl
    {
        private readonly PulseWidthModulationControlViewModel _viewModel;

        /// <summary>
        /// Constructor for PulseWidthModulationControl
        /// </summary>
        /// <param name="viewModel">view model associated with this control</param>
        public PulseWidthModulationControl(PulseWidthModulationControlViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
        }

        /// <summary>
        /// Callback for setting the fault state
        /// </summary>
        public event EventHandler<FaultStateChangeEventArgs> FaultStateChanged;

        /// <summary>
        /// Raises the FaultStateChanged event. Invoked when the channel value changes.
        /// </summary>
        /// <param name="newFaultState">The new fault state.</param>
        protected virtual void OnFaultStateChanged(FaultState newFaultState)
        {
            var faultStateChangedSubscribers = FaultStateChanged;
            if (faultStateChangedSubscribers != null)
            {
                faultStateChangedSubscribers(this, new FaultStateChangeEventArgs(newFaultState));
            }
        }

        /// <summary>
        /// Event that is fired when the value on the control changes
        /// </summary>
        public event EventHandler<CustomChannelValueChangedEventArgs> ValueChanged;

       /// <summary>
        /// Raises the ChannelValueChanged event. Invoked when the channel value changes.
       /// </summary>
       /// <param name="channelValue">New channel value</param>
       /// <param name="channelName">Name of the channel that changed</param>
        protected virtual void OnValueChanged(double channelValue, string channelName)
        {
            var channelValueChangedSubscribers = ValueChanged;
            if (channelValueChangedSubscribers != null)
            {
                channelValueChangedSubscribers(this, new CustomChannelValueChangedEventArgs(channelValue, channelName));
            }
        }

        /// <summary>
        /// The value of the duty cycle of this control
        /// </summary>
        public double DutyCycleValue
        {
            get { return (double)GetValue(DutyCycleValueProperty); }
            set { SetValue(DutyCycleValueProperty, value); }
        }

        /// <summary>
        /// Dependency property for Duty Cycle for the slider
        /// </summary>
        public static readonly DependencyProperty SliderDutyCycleValueProperty = DependencyProperty.Register(
          "SliderDutyCycleValue", typeof(double), typeof(PulseWidthModulationControl), new FrameworkPropertyMetadata(0.0));

        /// <summary>
        /// The value of the duty cycle for the slider. The only reason this has a separate property is becase we supress updates from the gateway to the slider
        /// control when the slider is being draggged to keep it from jumping around
        /// </summary>
        public double SliderDutyCycleValue
        {
            get { return (double)GetValue(SliderDutyCycleValueProperty); }
            set { SetValue(SliderDutyCycleValueProperty, value); }
        }

        /// <summary>
        /// Dependency property for Duty Cycle
        /// </summary>
        public static readonly DependencyProperty DutyCycleValueProperty = DependencyProperty.Register(
          "DutyCycleValue", typeof(double), typeof(PulseWidthModulationControl), new FrameworkPropertyMetadata(0.0));

        /// <summary>
        /// The value of the frequency of this control
        /// </summary>
        public double FrequencyValue
        {
            get { return (double)GetValue(FrequencyValueProperty); }
            set { SetValue(FrequencyValueProperty, value); }
        }

        /// <summary>
        /// Dependency property for Frequency
        /// </summary>
        public static readonly DependencyProperty FrequencyValueProperty = DependencyProperty.Register(
          "FrequencyValue", typeof(double), typeof(PulseWidthModulationControl), new FrameworkPropertyMetadata(0.0));

        private void NormalButton_OnClicked(object sender, RoutedEventArgs e)
        {
            OnFaultStateChanged(FaultState.Normal);
        }

        private void FaultToGroundButton_OnClicked(object sender, RoutedEventArgs e)
        {
            OnFaultStateChanged(FaultState.FaultedLow);
        }

        private void FaultToHighButton_OnClicked(object sender, RoutedEventArgs e)
        {
            OnFaultStateChanged(FaultState.FaultedHigh);
        }

        private void FrequencyTextBox_OnValueChanged(object sender, ValueChangedEventArgs<double> e)
        {
            OnValueChanged(e.NewValue, PulseWidthModulationControlModel.FrequencyChannelName);
        }

        private void DutyCycleTextBox_OnValueChanged(object sender, ValueChangedEventArgs<double> e)
        {
            OnValueChanged(e.NewValue, PulseWidthModulationControlModel.DutyCycleChannelName);
        }

        private void DutyCycleSlider_OnValueChanged(object sender, ValueChangedEventArgs<double> e)
        {
            OnValueChanged(e.NewValue, PulseWidthModulationControlModel.DutyCycleChannelName);
        }

        /// <summary>
        /// Invoked when the left mouse button is pressed while the mouse pointer is over the slider.
        /// </summary>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        private void HandlePreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Supress value changes on the slider while the  mouse is down
            _viewModel.SuppressValueChanges = true;
        }

        /// <summary>
        /// Invoked when the left mouse button is released while the mouse pointer is over the slider
        /// </summary>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        private void HandlePreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Once the user is done manipulating the view, let updates flow.
            _viewModel.SuppressValueChanges = false;
            // send final updated channel values to gateway so we get an updated value response for all of the controls
            OnValueChanged(DutyCycleSlider.Value, PulseWidthModulationControlModel.DutyCycleChannelName);
        }
    }
}
