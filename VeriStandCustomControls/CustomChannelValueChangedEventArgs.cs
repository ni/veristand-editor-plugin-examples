using System;

namespace NationalInstruments.VeriStand.CustomControlsExamples
{
    /// <summary>
    /// Custom event arguments class we use for channel value change so we can specify both the name and the value of the channel being changed.
    /// </summary>
    public class CustomChannelValueChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Value of the channel
        /// </summary>
        public object ChannelValue { get; private set; }

        /// <summary>
        /// Name of the channel that the value change is for
        /// </summary>
        public string ChannelName { get; private set; }

        /// <summary>
        /// Constructor for CustomChannelValueChangedEventArgs
        /// </summary>
        /// <param name="channelValue">Value of the channel</param>
        /// <param name="channelName">Name of the channel</param>
        public CustomChannelValueChangedEventArgs(object channelValue, string channelName)
        {
            ChannelValue = channelValue;
            ChannelName = channelName;
        }
    }
}
