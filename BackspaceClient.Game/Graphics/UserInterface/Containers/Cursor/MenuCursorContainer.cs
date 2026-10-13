using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Sample;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Input.Events;
using osu.Framework.Utils;
using osuTK;

namespace BackspaceClient.Game.Graphics.UserInterface.Containers.Cursor;

public partial class MenuCursorContainer : CursorContainer
{
    private Sample tapSample;
    private Cursor activeCursor;

    protected override Drawable CreateCursor() => activeCursor = new Cursor();

    [BackgroundDependencyLoader]
    private void load(AudioManager audio)
    {
        tapSample = audio.Samples.Get("UI/cursor-tap");
    }

    private void playTapSample(double baseFrequency = 1f)
    {
        const float random_range = 0.02f;
        SampleChannel channel = tapSample.GetChannel();

        // Scale to [-0.75, 0.75] so that the sample isn't fully panned left or right (sounds weird)
        channel.Balance.Value = (activeCursor.X / DrawWidth * 2 - 1) * 1;
        channel.Frequency.Value = baseFrequency - (random_range / 2f) + RNG.NextDouble(random_range);
        channel.Volume.Value = baseFrequency;

        channel.Play();
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        if (State.Value == Visibility.Visible)
        {
            activeCursor.Scale = new Vector2(1);
            activeCursor.ScaleTo(0.90f, 800, Easing.OutQuint);

            activeCursor.AdditiveLayer.Alpha = 0;
            activeCursor.AdditiveLayer.FadeInFromZero(800, Easing.OutQuint);

            playTapSample();
        }

        return base.OnMouseDown(e);
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        if (!e.HasAnyButtonPressed)
        {
            activeCursor.AdditiveLayer.FadeOutFromOne(500, Easing.OutQuint);
            activeCursor.ScaleTo(1, 500, Easing.OutElastic);

            if (State.Value == Visibility.Visible)
                playTapSample(0.6);
        }

        base.OnMouseUp(e);
    }

    public partial class Cursor : Container
    {
        private Container cursorContainer;
        private Bindable<float> cursorScale = new(1.0f);
        private const float base_scale = 0.15f;

        public Sprite AdditiveLayer = null!;

        public Cursor()
        {
            AutoSizeAxes = Axes.Both;
        }

        [BackgroundDependencyLoader]
        private void load(TextureStore textures)
        {
            Children =
            [
                cursorContainer = new Container
                {
                    AutoSizeAxes = Axes.Both,
                    Children =
                    [
                        new Sprite
                        {
                            Texture = textures.Get(@"Cursor/menu-cursor"),
                        },
                        AdditiveLayer = new Sprite
                        {
                            Blending = BlendingParameters.Additive,
                            Colour = Colour4.DarkGray,
                            Alpha = 0,
                            Texture = textures.Get(@"Cursor/menu-cursor-additive"),
                        }
                    ]
                }
            ];

            cursorScale.BindValueChanged(scale => cursorContainer.Scale = new Vector2(scale.NewValue * base_scale), true);
        }
    }

    private partial class MouseInputDetector : Component
    {
        /// <summary>
        /// Whether the last input applied to the game is sourced from mouse.
        /// </summary>
        public IBindable<bool> LastInputWasMouseSource => lastInputWasMouseSource;

        private readonly Bindable<bool> lastInputWasMouseSource = new Bindable<bool>();

        public MouseInputDetector()
        {
            RelativeSizeAxes = Axes.Both;
        }

        protected override bool Handle(UIEvent e)
        {
            switch (e)
            {
                case MouseDownEvent:
                case MouseMoveEvent:
                    lastInputWasMouseSource.Value = true;
                    return false;

                case KeyDownEvent keyDown when !keyDown.Repeat:
                case JoystickPressEvent:
                case MidiDownEvent:
                    lastInputWasMouseSource.Value = false;
                    return false;
            }

            return false;
        }
    }

    private enum DragRotationState
    {
        NotDragging,
        DragStarted,
        Rotating,
    }
}
