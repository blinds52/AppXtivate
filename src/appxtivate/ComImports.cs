using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
// ReSharper disable All

namespace AppXtivate.ComImports
{
    public enum ActivateOptions
    {
        None = 0x00000000,
        DesignMode = 0x00000001,
        NoErrorUI = 0x00000002,
        NoSplashScreen = 0x00000004
    }

    [ComImport, Guid("2e941141-7f97-4756-ba1d-9decde894a3d"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IApplicationActivationManager
    {
        IntPtr ActivateApplication([In] string appUserModelId, [In] string arguments, [In] ActivateOptions options, [Out] out uint processId);
        IntPtr ActivateForFile([In] string appUserModelId, [In] [MarshalAs(UnmanagedType.Interface, IidParameterIndex = 2)] IShellItemArray itemArray, [In] string verb, [Out] out uint processId);
        IntPtr ActivateForProtocol([In] string appUserModelId, [In] [MarshalAs(UnmanagedType.Interface, IidParameterIndex = 2)] IShellItemArray itemArray, [Out] out uint processId);
    }

    [ComImport, Guid("45BA127D-10A8-46EA-8AB7-56EA9078943C")]
    class ApplicationActivationManager : IApplicationActivationManager
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public extern IntPtr ActivateApplication([In] string appUserModelId, [In] string arguments, [In] ActivateOptions options, [Out] out uint processId);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public extern IntPtr ActivateForFile([In] string appUserModelId, [In] [MarshalAs(UnmanagedType.Interface, IidParameterIndex = 2)] IShellItemArray itemArray, [In] string verb, [Out] out uint processId);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public extern IntPtr ActivateForProtocol([In] string appUserModelId, [In] [MarshalAs(UnmanagedType.Interface, IidParameterIndex = 2)] IShellItemArray itemArray, [Out] out uint processId);
    }

    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("43826d1e-e718-42ee-bc55-a1e261c37bfe")]
    interface IShellItem
    {
    }

    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("b63ea76d-1f85-456f-a19c-48159efa858b")]
    interface IShellItemArray
    {
    }

    static class ShellItemHelpers
    {
        [DllImport("shell32", CharSet = CharSet.Unicode, PreserveSig = false)]
        private static extern void SHCreateItemFromParsingName(
            [In] [MarshalAs(UnmanagedType.LPWStr)] string pszPath,
            [In] IntPtr pbc,
            [In] [MarshalAs(UnmanagedType.LPStruct)] Guid iIdIShellItem,
            [Out] [MarshalAs(UnmanagedType.Interface, IidParameterIndex = 2)] out IShellItem iShellItem);

        [DllImport("shell32", CharSet = CharSet.Unicode, PreserveSig = false)]
        private static extern void SHCreateShellItemArrayFromShellItem(
            [In] [MarshalAs(UnmanagedType.Interface, IidParameterIndex = 2)] IShellItem psi,
            [In] [MarshalAs(UnmanagedType.LPStruct)] Guid iIdIShellItem,
            [Out] [MarshalAs(UnmanagedType.Interface, IidParameterIndex = 2)] out IShellItemArray iShellItemArray);

        public static IShellItemArray SHCreateShellItemArrayFromParsingName(string sourceFile)
        {
            return SHCreateShellItemArrayFromShellItem(SHCreateItemFromParsingName(sourceFile));
        }

        public static IShellItem SHCreateItemFromParsingName(string sourceFile)
        {
            var iIdIShellItem = new Guid("43826d1e-e718-42ee-bc55-a1e261c37bfe");
            SHCreateItemFromParsingName(sourceFile, IntPtr.Zero, iIdIShellItem, out IShellItem iShellItem);
            return iShellItem;
        }

        public static IShellItemArray SHCreateShellItemArrayFromShellItem(IShellItem shellItem)
        {
            var iIdIShellItemArray = new Guid("b63ea76d-1f85-456f-a19c-48159efa858b");
            SHCreateShellItemArrayFromShellItem(shellItem, iIdIShellItemArray, out IShellItemArray iShellItemArray);
            return iShellItemArray;
        }
    }
}
