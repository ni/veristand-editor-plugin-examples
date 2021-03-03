using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using NationalInstruments.Hmi.Core.Controls.Models;
using NationalInstruments.SourceModel;
using NationalInstruments.SourceModel.Persistence;
using NationalInstruments.VeriStand.SourceModel.Screen;

namespace NationalInstruments.VeriStand.CustomControlsExamples
{
    /// <summary>
    /// This class is an implementation of ICustomVeriStandControl. By specifying it as an Export, we are identifying
    /// it as a plugin to VeriStand.
    /// The interface implementation defines how the control should appear in the palette.
    /// </summary>
    [Export(typeof(ICustomVeriStandControl))]
    public class KeySwitchControlModelExporter : ICustomVeriStandControl
    {
        /// <summary>
        /// Mergescript which defines what to drop on the screen from the palette.  Can be used to set default values on the control
        /// </summary>
        public string Target =>
            "<pf:MergeScript xmlns:pf=\"http://www.ni.com/PlatformFramework\">" +
                "<pf:MergeItem>" +
                    "<KeySwitch xmlns=\"http://www.your-company.com/VeriStandExample\" Width=\"[float]172\" Height=\"[float]172\"/>" +
                "</pf:MergeItem>" +
            "</pf:MergeScript>";

        /// <summary>
        /// Name of the control as it will appear in the palette
        /// </summary>
        public string Name => "Key Switch";

        /// <summary>
        /// Path to the image to use in the palette
        /// </summary>
        public string ImagePath => "/NationalInstruments.VeriStand.CustomControlsExamples;component/Resources/TestIcon_32x32.png";

        /// <summary>
        /// Tool tip to display in the palette
        /// </summary>
        public string ToolTip => "Key Switch Tool Tip";

        /// <summary>
        /// Unique id for the control. The only requirement is that this doesn't overlap with existing controls or other custom controls.
        /// This is used for serialization and the context help system.
        /// </summary>
        public string UniqueId => "KeySwitch";

        /// <summary>
        /// If a folder hierarchy is desired it can be returned here.  If multiple controls should show up in the same subfolder either use the same PaletteElementCategory object in their
        /// hierarchy list or use a category with the same unique id.  Unique IDs cannot be duplicated at different hierarchy levels.
        /// </summary>
        public IList<PaletteElementCategory> PaletteHierarchy =>
            new List<PaletteElementCategory>() { new PaletteElementCategory("Fancy Switches", ImagePath, "FancySwitches", .1) };
    }

    /// <summary>
    /// This model has the business logic for the keyswitch control.  Mostly it handles all of that in the base class ChannelKnobModel.
    /// </summary>
    public class KeySwitchControlModel : ChannelKnobModel
    {
        /// <summary>
        /// The name of the control which will be used in XML.  This name in the mergescript in the Target property of the ICustomVeriStandControl interface must match this
        /// </summary>
        private const string KeySwitchName = "KeySwitch";

        /// <summary>
        /// How this model will be represented in XML.
        /// </summary>
        public override XName XmlElementName => XName.Get(KeySwitchName, PluginNamespaceSchema.ParsableNamespaceName);

        /// <summary>Creates a <see cref="KeySwitchControlModel"/>.</summary>
        /// <param name="info">An <see cref="IElementCreateInfo"/> instance.</param>
        /// <returns>A <see cref="KeySwitchControlModel"/>.</returns>
        [XmlParserFactoryMethod(KeySwitchName, PluginNamespaceSchema.ParsableNamespaceName)]
        public static KeySwitchControlModel CreateShifterControlModel(IElementCreateInfo info)
        {
            var model = new KeySwitchControlModel();
#if MUTATE2020
            model.Initialize(info);
#else
            model.Init(info);
#endif
            return model;
        }
    }
}
