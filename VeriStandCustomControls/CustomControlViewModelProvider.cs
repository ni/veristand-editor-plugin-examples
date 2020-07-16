using System.ComponentModel.Composition;
using NationalInstruments.Composition;
using NationalInstruments.Shell;
using NationalInstruments.VeriStand.Design.Screen;

namespace NationalInstruments.VeriStand.CustomControlsExamples
{
    /// <summary>
    /// Exports the view models (and associated models) supported by the ScreenSurface.
    /// </summary>
    [PartMetadata(ExportIdentifier.RootContainerKey, "")]
    [ExportProvideViewModels(typeof(ScreenEditControl), supportedModels: "NationalInstruments.VeriStand.CustomControlsExamples")]
    public class CustomControlViewModelProvider : ViewModelProvider
    {
        /// <inheritdoc />
        /// <remarks>
        /// This method should use AddSupportedModel to specify the relationship between all models and view models exported
        /// from this assembly.
        /// </remarks>
        protected override void AddSupportedModels()
        {
            AddSupportedModel<PulseWidthModulationControlModel>(e => new PulseWidthModulationControlViewModel(e));
            AddSupportedModel<KeySwitchControlModel>(e => new KeySwitchControlViewModel(e));
        }
    }
}
