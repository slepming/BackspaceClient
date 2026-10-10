using osu.Framework.Graphics;

namespace BackspaceClient.Game.Extensions;

public static class DrawableExtension
{
    public static void Shake(this Drawable target, double shakeDuration = 80, float shakeMagnitude = 8, double? maximumLength = null)
    {
        if (maximumLength < shakeDuration * 2) return;

        var sequence = target.MoveToX(shakeMagnitude, shakeDuration / 2, Easing.OutSine).Then()
            .MoveToX(-shakeMagnitude, shakeDuration, Easing.InOutSine).Then();

        if (!maximumLength.HasValue || maximumLength >= shakeDuration * 4)
        {
            sequence = sequence
                       .MoveToX(shakeMagnitude, shakeDuration, Easing.InOutSine).Then()
                       .MoveToX(-shakeMagnitude, shakeDuration, Easing.InOutSine).Then();
        }

        sequence.MoveToX(0, shakeDuration / 2, Easing.InSine);
    }
}
