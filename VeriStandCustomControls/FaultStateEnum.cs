namespace NationalInstruments.VeriStand.CustomControlsExamples
{
    /// <summary>
    /// Enum describing the fault state of the PWM Control
    /// </summary>
    public enum FaultState
    {
        /// <summary>
        /// No faults
        /// </summary>
        Normal,
        /// <summary>
        /// faulted to 100
        /// </summary>
        FaultedHigh,
        /// <summary>
        /// faulted to 0
        /// </summary>
        FaultedLow
    }
}
