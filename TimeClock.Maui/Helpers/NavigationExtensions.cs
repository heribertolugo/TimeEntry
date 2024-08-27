namespace TimeClock.Maui
{
    public static class NavigationExtensions
    {
        public static void ClearStack(this INavigation navigation)
        {
            var stack = navigation.NavigationStack.ToArray();
            for (int i = stack.Length - 1; i > 0; i--)
            {
                navigation.RemovePage(stack[i]);
            }
        }
        public static void ClearStackExceptForFirst(this INavigation navigation)
        {
            var stack = navigation.NavigationStack.ToArray();
            for (int i = stack.Length - 1; i >= 1; i--)
            {
                navigation.RemovePage(stack[i]);
            }
        }
        public static async Task ClearModalStackAsync(this INavigation navigation, bool animated = false)
        {
            var stack = navigation.ModalStack.ToArray();
            for (int i = stack.Length - 1; i > 0; i--)
            {
                await navigation.PopModalAsync(animated);
            }
        }
    }
}
