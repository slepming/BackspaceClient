using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osuTK;

namespace BackspaceClient.Game.Graphics.UserInterface.Containers;

public partial class AnimatedTooltip : VisibilityContainer, ITooltip<LocalisableString>
{
    private static readonly Colour4 backgroundColour = Colour4.FromHex("#18181B");
    private static readonly Colour4 borderColour = Colour4.FromHex("#27272A");
    private static readonly Colour4 textColour = Colour4.FromHex("#F4F4F5");
    private static readonly Colour4 shadowColour = Colour4.Black.Opacity(0.45f);

    private const float text_size = 15;
    private const float corner_radius = 6;
    private const float border_thickness = 1.2f;

    private readonly Container content;
    private readonly SpriteText text;
    private bool instantMovement = true;

    /// <summary>
    /// The text to be displayed by this tooltip. This property is assigned to whenever the tooltip text changes.
    /// </summary>
    public virtual string TooltipText
    {
        set => SetContent(value);
    }

    public virtual void SetContent(LocalisableString contentText) => text.Text = contentText;

    /// <summary>
    /// Constructs a new tooltip that starts out invisible.
    /// </summary>
    public AnimatedTooltip()
    {
        Alpha = 0;
        AutoSizeAxes = Axes.Both;

        Child = content = new Container
        {
            AutoSizeAxes = Axes.Both,
            Masking = true,
            CornerRadius = corner_radius,
            BorderThickness = border_thickness,
            BorderColour = BorderColour,
            Origin = Anchor.TopLeft,
            Scale = new Vector2(0.92f),
            EdgeEffect = new EdgeEffectParameters
            {
                Type = EdgeEffectType.Shadow,
                Colour = shadowColour,
                Radius = 12,
                Offset = new Vector2(0, 4),
            },
            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = backgroundColour,
                },
                text = new SpriteText
                {
                    Font = FrameworkFont.Regular.With(size: text_size),
                    Colour = textColour,
                    Padding = new MarginPadding { Horizontal = 10, Vertical = 6 },
                }
            }
        };
    }

    public virtual void Refresh()
    {
    }

    /// <summary>
    /// Called whenever the tooltip appears.
    /// </summary>
    protected override void PopIn()
    {
        instantMovement |= !IsPresent;
        this.FadeIn(180, Easing.OutQuint);
        content.ScaleTo(1f, 250, Easing.OutBack);
    }

    /// <summary>
    /// Called whenever the tooltip disappears.
    /// </summary>
    protected override void PopOut()
    {
        this.Delay(80).FadeOut(150, Easing.OutQuint);
        content.ScaleTo(0.92f, 200, Easing.OutQuint);
    }

    /// <summary>
    /// Called whenever the position of the tooltip changes.
    /// </summary>
    /// <param name="pos">The new position of the tooltip.</param>
    public virtual void Move(Vector2 pos)
    {
        if (instantMovement)
        {
            Position = pos;
            instantMovement = false;
        }
        else
        {
            this.MoveTo(pos, 180, Easing.OutQuint);
        }
    }
}
