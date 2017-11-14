using System;

namespace Edo.Win32.Native
{
    /// <summary>
    /// Wraps names for various kernel privileges in the windows api
    /// </summary>
    public static class PrivilegeName
    {
        /// <summary>
        /// Gives a process right to open low level service processes for debugging
        /// </summary>
        public const String Debug = "SeDebugPrivilege";

        /// <summary>
        /// Allows a process to authenticate like a user and thus gain access to the same resources as a user
        /// </summary>
        public const String AuthenticateAsUser = "SeTcbPrivilege";
    }
}