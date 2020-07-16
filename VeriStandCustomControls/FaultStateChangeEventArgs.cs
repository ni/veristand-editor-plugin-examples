using System;

namespace NationalInstruments.VeriStand.CustomControlsExamples
{
    /// <summary>
    /// The event arguments for fault state change events.
    /// </summary>
    public class FaultStateChangeEventArgs : EventArgs
    {
        /// <summary>
        /// Creates a new instance of the FaultStateChangeEventArgs class
        /// </summary>
        /// <param name="newFaultState">The new fault state.</param>
        public FaultStateChangeEventArgs(FaultState newFaultState)
        {
            FaultState = newFaultState;
        }

        /// <summary>
        /// Gets the new fault state.
        /// </summary>
        public FaultState FaultState { get; private set; }
    }
}
