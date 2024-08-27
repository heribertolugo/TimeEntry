namespace TimeClock.Maui.Helpers
{
    public static class VisualElementExtensions
    {
        public static IEnumerable<VisualElement> Ancestors(this VisualElement element)
        {
            while (element != null && element.Parent is not ShellContent)
            {
                yield return element;
                element = (VisualElement)element.Parent;
            }
        }

        public static Point GetAbsolutePosition(this VisualElement visualElement)
        {
            var ancestors = visualElement.Ancestors();
            var x = ancestors.Sum(ancestor => ancestor.X);
            var y = ancestors.Sum(ancestor => ancestor.Y);

            return new Point(x, y);
        }

        public static Task BlinkBackground(this VisualElement visualElement, Color color)
        {
            var animation = new Animation
            {
                { 0, 0.5, new Animation(callback: v => visualElement.BackgroundColor = Color.FromRgba(color.Red, color.Green, color.Blue, v), start: 1, end: 0)} ,
                { 0.5, 1, new Animation(callback: v => visualElement.BackgroundColor = Color.FromRgba(color.Red, color.Green, color.Blue, v), start: 0, end: 1)}
            };
            
            visualElement.Animate("BlinkBg", animation, 10, 1500, Easing.Linear, null, () => true);

            return Task.CompletedTask;
        }

        public static bool BlinkBackgroundCancel(this VisualElement visualElement)
        {
            return visualElement.AbortAnimation("BlinkBg");
        }
    }
}
