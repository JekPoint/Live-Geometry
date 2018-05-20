using DynamicGeometry.UI.CommandSystem;

namespace DynamicGeometry.UI
{
    public partial class DrawingControl
    {
        public Command CommandUndo { get; set; }
        public Command CommandRedo { get; set; }
    }
}
