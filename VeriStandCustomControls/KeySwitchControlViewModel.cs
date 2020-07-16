using NationalInstruments.Hmi.Core.Controls.ViewModels;

namespace NationalInstruments.VeriStand.CustomControlsExamples
{
    /// <summary>
    /// The view model for the keyswitch control. The only thing this does different from the channelknobviewmodel is that we return a custom view.
    /// Since the view we have inherits from knob it will be compatible with the existing models and view models.
    /// </summary>
    public class KeySwitchControlViewModel : ChannelKnobViewModel
    {
        /// <summary>
        /// Constructs a new instance of the KeySwitchControlViewModel class
        /// </summary>
        /// <param name="model">The KeySwitchControlModel assocaited with this view model.</param>
        public KeySwitchControlViewModel(KeySwitchControlModel model) : base(model)
        {
        }

       /// <summary>
       /// Returns a custom view for associated with this view model
       /// </summary>
       /// <returns>keyswitch view</returns>
        public override object CreateView()
        {
            var view = new KeySwitchControl();
            // numeric controls have a helper which needs to get attached to the view that helps setting values of different types to the control. Since we only have I32
            // for our control type we just hard code that when creating the helper
            Helper = CreateHelper(typeof(int), this, view);
            return view;
        }
    }
}
