using System.Runtime.InteropServices;

namespace TimeClock.Maui.Helpers
{
    /// <summary>
    /// This prevents unhandled win32 exception from being thrown when exiting application on Windows platform. 
    /// A actual fix was implemented by Microsoft which invloves a newer version of Microsoft.WindowsAppSDK. 
    /// However, adding the nuget package breaks compile for all other platforms. 
    /// The proper fix implementation may be to update the SDK on the actual machine, 
    /// which I am not ready to take that leap of faith.
    /// See any of the following links for more information:
    /// <para>
    /// <see href="https://github.com/dotnet/maui/issues/7317"/>
    /// </para>
    /// <para>
    /// <see href="https://github.com/microsoft/microsoft-ui-xaml/issues/7260#issuecomment-1231314776"/>
    /// </para>
    /// </summary>
    internal static class CleanExit
    {
        /// <summary>
        /// Applies the workaround created by namazso for the Microsoft bug
        /// </summary>
        public static void Apply()
        {
            string name = "Windows.System.Threading.ThreadPoolTimer";
            _ = WindowsCreateString(name, name.Length, out var str);
            Guid guid = new(0x1a8a9d02, 0xe482, 0x461b, 0xB8, 0xC7, 0x8E, 0xFA, 0xD1, 0xCC, 0xE5, 0x90);
            _ = RoGetActivationFactory(str, ref guid, out _);
            _ = RoGetActivationFactory(str, ref guid, out _);
            _ = RoGetActivationFactory(str, ref guid, out _);
        }

        [DllImport("api-ms-win-core-winrt-l1-1-0.dll")]
        private static extern int RoGetActivationFactory(IntPtr activatableClassId, ref Guid iid, out IntPtr factory);

        [DllImport("api-ms-win-core-winrt-string-l1-1-0.dll")]
        private static extern int WindowsCreateString(
            [MarshalAs(UnmanagedType.LPWStr)] string sourceString, int length, out IntPtr str);
    }
}
