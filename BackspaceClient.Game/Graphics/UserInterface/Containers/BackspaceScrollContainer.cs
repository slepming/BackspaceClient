using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace BackspaceClient.Game.Graphics.UserInterface.Containers;

public partial class BackspaceScrollContainer : BasicScrollContainer
{
    public ColourInfo BarColour = Colour4.FromHex("#27272A");

    protected override ScrollbarContainer CreateScrollbar(Direction direction) => new BackspaceScrollbar(direction) { BarColour = BarColour };

    protected partial class BackspaceScrollbar : ScrollbarContainer
    {
        private const float dim_size = 8;

        public ColourInfo BarColour = Colour4.DarkViolet;

        private Box scrollSquare;

        public BackspaceScrollbar(Direction direction)
            : base(direction)
        {
            Child = scrollSquare = new Box
            {
                RelativeSizeAxes = Axes.Both,
            };
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            scrollSquare.Colour = BarColour;
        }

        public override void ResizeTo(float val, int duration = 0, Easing easing = Easing.None)
        {
            Vector2 size = new Vector2(dim_size)
            {
                [(int)ScrollDirection] = val
            };
            this.ResizeTo(size, duration, easing);
        }
    }
}
