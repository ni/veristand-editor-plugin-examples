using NationalInstruments.Controls;

namespace NationalInstruments.VeriStand.CustomControlsExamples
{
    /// <summary>
    /// This is the custom control we are styling.  We aren't changing any behaviors in this control, the main point of giving this a new type is so that we can control
    /// which styles are applied to it
    /// </summary>
    public class KeySwitchControl : KnobInt32
    {
        /// <summary>
        /// Constructs a new instance of the KeySwitchControl class
        /// </summary>
        public KeySwitchControl()
        {
            // Set the default style key to the type.  The style key is used to apply themes to controls.  It lets us set an implicit style for this class in KeySwitchControlTemplate.xaml and have it be applied to this class.  If we didn't set this here the theme for the Knob would be used which we aren't currenly importing so we would end up
            // without a visual.
           DefaultStyleKey = typeof(KeySwitchControl);
        }
    }
}
