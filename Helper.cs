using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;


namespace ConsoleApplication4
{

    public class Helper
    {

        #region " SystemRestarter "

        /// <summary>
        /// Performs different Shutdown system's operations on a local or remote machine.
        /// </summary>
        public class SystemRestarter
        {

            #region " P/Invoke "

            #region " Methods "

            /// <summary>
            /// WinAPI methods used by the Main Class.
            /// </summary>
            private class NativeMethods
            {

                /// <summary>
                /// Logs off the interactive user, shuts down the system, or shuts down and restarts the system. 
                /// It sends the 'WM_QUERYENDSESSION' message to all applications to determine if they can be terminated.
                /// </summary>
                /// <param name="uFlags">
                /// Indicates the shutdown type.
                /// </param>
                /// <param name="dwReason">
                /// Indicates the reason for initiating the shutdown.
                /// </param>
                /// <returns>
                /// If the function succeeds, the return value is 'True'. 
                /// The function executes asynchronously so a 'True' return value indicates that the shutdown has been initiated. 
                /// It does not indicate whether the shutdown will succeed. 
                /// It is possible that the system, the user, or another application will abort the shutdown.
                /// If the function fails, the return value is 'False'. 
                /// </returns>
                [DllImport("user32.dll", SetLastError = true)]
                [return: MarshalAs(UnmanagedType.Bool)]
                static internal extern bool ExitWindowsEx(Enums.ExitwindowsEx_Flags uFlags, Enums.ShutdownReason dwReason);

                /// <summary>
                /// Initiates a shutdown and restart of the specified computer, 
                /// and restarts any applications that have been registered for restart.
                /// </summary>
                /// <param name="lpMachineName">
                /// The name of the computer to be shut down. 
                /// If the value of this parameter is 'NULL', the local computer is shut down.
                /// This parameter can be an addres, for example: '127.0.0.1'
                /// </param>
                /// <param name="lpMessage">
                /// The message to be displayed in the interactive shutdown dialog box.
                /// </param>
                /// <param name="dwGracePeriod">
                /// The number of seconds to wait before shutting down the computer. 
                /// If the value of this parameter is zero, the computer is shut down immediately. 
                /// This value is limited to 'MAX_SHUTDOWN_TIMEOUT'.
                /// If the value of this parameter is greater than zero, and the 'dwShutdownFlags' parameter 
                /// specifies the flag 'GRACE_OVERRIDE', the function fails and returns the error code 'ERROR_BAD_ARGUMENTS'.
                /// </param>
                /// <param name="dwShutdownFlags">
                /// Specifies options for the shutdown.
                /// </param>
                /// <param name="dwReason">
                /// The reason for initiating the shutdown. 
                /// If this parameter is zero, 
                /// the default is an undefined shutdown that is logged as "No title for this reason could be found". 
                /// By default, it is also an 'unplanned' shutdown.
                /// </param>
                /// <returns>UInt32.</returns>
                [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
                static internal extern uint InitiateShutdown(string lpMachineName, string lpMessage, uint dwGracePeriod, Enums.InitiateShutdown_Flags dwShutdownFlags, Enums.ShutdownReason dwReason);

                /// <summary>
                /// Aborts a system shutdown that has been initiated.
                /// </summary>
                /// <param name="lpMachineName">
                /// The network name of the computer where the shutdown is to be stopped. 
                /// If this parameter is 'NULL' or an empty string, the function aborts the shutdown on the local computer.
                /// </param>
                /// <returns><c>True</c> if the function succeeds, <c>False</c> otherwise.</returns>
                [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
                [return: MarshalAs(UnmanagedType.Bool)]
                static internal extern bool AbortSystemShutdown(string lpMachineName = "127.0.0.1");

                /// <summary>
                /// Opens the access token associated with a process.
                /// </summary>
                /// <param name="ProcessHandle">
                /// A handle to the process whose access token is opened. 
                /// The process must have the 'PROCESS_QUERY_INFORMATION' access permission.
                /// </param>
                /// <param name="DesiredAccess">
                /// Specifies an access mask that specifies the requested types of access to the access token. 
                /// These requested access types are compared with the discretionary access control list (DACL) 
                /// of the token to determine which accesses are granted or denied.
                /// </param>
                /// <param name="TokenHandle">
                /// A pointer to a handle that identifies the newly opened access token when the function returns.
                /// </param>
                /// <returns>System.Int32.</returns>
                [DllImport("advapi32.dll")]
                static internal extern int OpenProcessToken(IntPtr ProcessHandle, Enums.AccessRights DesiredAccess, ref IntPtr TokenHandle);

                /// <summary>
                /// Enables or disables privileges in the specified access token.
                /// Enabling or disabling privileges in an access token requires 'TOKEN_ADJUST_PRIVILEGES' access.
                /// </summary>
                /// <param name="TokenHandle">
                /// A handle to the access token that contains the privileges to be modified. 
                /// The handle must have 'TOKEN_ADJUST_PRIVILEGES' access to the token. 
                /// If the 'PreviousState' parameter is not NULL, the handle must also have 'TOKEN_QUERY' access.
                /// </param>
                /// <param name="DisableAllPrivileges">
                /// Specifies whether the function disables all of the token's privileges. 
                /// If this value is 'TRUE', the function disables all privileges and ignores the 'NewState' parameter. 
                /// If it is 'FALSE', the function modifies privileges based on the information pointed to by the NewState parameter.
                /// </param>
                /// <param name="NewState">
                /// A pointer to a 'TOKEN_PRIVILEGES' structure that specifies an array of privileges and their attributes. 
                /// If the 'DisableAllPrivileges' parameter is 'FALSE', 
                /// the 'AdjustTokenPrivileges' function enables, disables, or removes these privileges for the token. 
                /// </param>
                /// <param name="BufferLength">
                /// Specifies the size, in bytes, of the buffer pointed to by the 'PreviousState' parameter. 
                /// This parameter can be zero if the PreviousState parameter is 'NULL'.
                /// </param>
                /// <param name="PreviousState">
                /// A pointer to a buffer that the function fills with a 'TOKEN_PRIVILEGES' structure
                /// that contains the previous state of any privileges that the function modifies. 
                /// That is, if a privilege has been modified by this function, 
                /// the privilege and its previous state are contained in the 'TOKEN_PRIVILEGES' structure 
                /// referenced by 'PreviousState'. 
                /// If the 'PrivilegeCount' member of 'TOKEN_PRIVILEGES' is zero,
                /// then no privileges have been changed by this function. 
                /// This parameter can be 'NULL'.</param>
                /// <param name="ReturnLength">
                /// A pointer to a variable that receives the required size, in bytes, 
                /// of the buffer pointed to by the 'PreviousState' parameter. 
                /// This parameter can be 'NULL' if 'PreviousState' is 'NULL'.
                /// </param>
                /// <returns>
                /// If the function succeeds, the return value is nonzero, otherwise, zero.
                /// To determine whether the function adjusted all of the specified privileges, call 'GetLastError'.</returns>
                [DllImport("advapi32.dll", SetLastError = true)]
                [return: MarshalAs(UnmanagedType.Bool)]
                static internal extern bool AdjustTokenPrivileges(IntPtr TokenHandle, [MarshalAs(UnmanagedType.Bool)] bool DisableAllPrivileges, ref Structures.TOKEN_PRIVILEGES NewState, UInt32 BufferLength, IntPtr PreviousState, IntPtr ReturnLength);

                /// <summary>
                /// Retrieves the locally unique identifier (LUID) used on a specified system, 
                /// to locally represent the specified privilege name.
                /// </summary>
                /// <param name="lpSystemName">
                /// A pointer to a null-terminated string that specifies the name of the system 
                /// on which the privilege name is retrieved. 
                /// If a null string is specified, the function attempts to find the privilege name on the local system
                /// </param>
                /// <param name="lpName">
                /// A pointer to a null-terminated string that specifies the name of the privilege, 
                /// as defined in the Winnt.h header file.
                /// For example, this parameter could specify the constant, 'SE_SECURITY_NAME', 
                /// or its corresponding string, "SeSecurityPrivilege".
                /// </param>
                /// <param name="lpLuid">
                /// A pointer to a variable that receives the LUID by which the privilege is known on
                /// the system specified by the lpSystemName parameter.
                /// </param>
                /// <returns>System.Int32.</returns>
                [DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
                static internal extern int LookupPrivilegeValue(string lpSystemName, string lpName, ref Structures.LUID lpLuid);

            }

            #endregion

            #region " Read-Only Properties (Privileges) "

            /// <summary>
            /// Privileges determine the type of system operations that a user account can perform. 
            /// An administrator assigns privileges to user and group accounts. 
            /// Each user's privileges include those granted to the user and to the groups to which the user belongs.
            /// </summary>
            private class Privileges
            {

                /// <summary>
                /// Privilege required to shut down a system using a network request.
                /// User Right: Force shutdown from a remote system.
                /// </summary>
                /// For more Info see:
                /// http://msdn.microsoft.com/en-us/library/windows/desktop/bb530716%28v=vs.85%29.aspx
                public static string SE_SHUTDOWN_NAME
                {
                    get { return "SeShutdownPrivilege"; }
                }

                /// <summary>
                /// Privilege required to shut down a local system.
                /// User Right: Shut down the system.
                /// </summary>
                /// For more Info see:
                /// http://msdn.microsoft.com/en-us/library/windows/desktop/bb530716%28v=vs.85%29.aspx
                public static string SE_REMOTE_SHUTDOWN_NAME
                {
                    get { return "SeRemoteShutdownPrivilege"; }
                }

            }

            #endregion

            #region " Enumerations "

            /// <summary>
            /// Enumerations used by the Main Class.
            /// </summary>
            public class Enums
            {

                /// <summary>
                /// Indicates the shutdown type.
                /// </summary>
                [Description("Enum used in the 'uFlags' parameter of 'ExitWindowsEx' Function.")]
                [Flags()]
                public enum ExitwindowsEx_Flags : uint
                {

                    //******' 
                    // NOTE '
                    //******'     
                    // This Enumeration is partially defined.

                    /// <summary>
                    /// Shuts down all processes running in the logon session of the current process. 
                    /// Then it logs the user off.
                    /// This flag can be used only by processes running in an interactive user's logon session.
                    /// </summary>
                    LogOff = 0x0u

                }

                /// <summary>
                /// Indicates the forcing type.
                /// </summary>
                [Description("Enum used in combination with the 'uFlags' parameter of 'ExitWindowsEx' Function.")]
                public enum ExitwindowsEx_Force : uint
                {

                    /// <summary>
                    /// Don't force the system to close the applications.
                    /// This is the default parameter.
                    /// </summary>
                    Wait = 0x0u,

                    /// <summary>
                    /// This flag has no effect if terminal services is enabled. 
                    /// Otherwise, the system does not send the 'WM_QUERYENDSESSIO'N message. 
                    /// This can cause applications to lose data. 
                    /// Therefore, you should only use this flag in an emergency.
                    /// </summary>
                    Force = 0x4u,

                    /// <summary>
                    /// Forces processes to terminate if they do not respond to the 'WM_QUERYENDSESSION',
                    /// or 'WM_ENDSESSION' message within the timeout interval.
                    /// </summary>
                    ForceIfHung = 0x10u

                }

                /// <summary>
                /// Indicates the shutdown type.
                /// </summary>
                [Description("Enum used in the 'dwShutdownFlags' parameter of 'InitiateShutdown' Function.")]
                [Flags()]
                public enum InitiateShutdown_Flags : uint
                {

                    /// <summary>
                    /// Overrides the grace period so that the computer is shut down immediately.
                    /// </summary>
                    GraceOverride = 0x20u,

                    /// <summary>
                    /// Only for Windows 8/8.1
                    /// Prepares the system for a faster startup by combining 
                    /// the 'HybridShutDown' flag with the 'ShutDown' flag. 
                    /// 'InitiateShutdown' always initiate a full system shutdown if the 'HybridShutdown' flag is not set. 
                    /// </summary>
                    HybridShutdown = 0x200u,

                    /// <summary>
                    /// The computer installs any updates before starting the shutdown.
                    /// </summary>
                    InstallUpdates = 0x40u,

                    /// <summary>
                    /// The computer is shut down but is not powered down or restarted.
                    /// </summary>
                    Shutdown = 0x10u,

                    /// <summary>
                    /// The computer is shut down and powered down.
                    /// </summary>
                    PowerOff = 0x8u,

                    /// <summary>
                    /// The computer is shut down and restarted.
                    /// </summary>
                    Restart = 0x4u,

                    /// <summary>
                    /// The system is restarted using the 'ExitWindowsEx' function with the 'RESTARTAPPS' flag. 
                    /// This restarts any applications that have been registered for restart 
                    /// using the 'RegisterApplicationRestart' function.
                    /// </summary>
                    RestartApps = 0x80u

                }

                /// <summary>
                /// Indicates the forced shutdown type.
                /// </summary>
                [Description("Enum used in combination with the 'uFlags' parameter of 'InitiateShutdown' Function.")]
                public enum InitiateShutdown_Force : uint
                {

                    /// <summary>
                    /// Don't force the system to close the applications.
                    /// This is the default parameter.
                    /// </summary>
                    Wait = 0x0u,

                    /// <summary>
                    /// All sessions are forcefully logged off. 
                    /// If this flag is not set and users other than the current user are logged on to the computer 
                    /// specified by the 'lpMachineName' parameter.
                    /// </summary>
                    ForceOthers = 0x1u,

                    /// <summary>
                    /// Specifies that the originating session is logged off forcefully. 
                    /// If this flag is not set, the originating session is shut down interactively, 
                    /// so a shutdown is not guaranteed even if the function returns successfully.
                    /// </summary>
                    ForceSelf = 0x2u

                }

                /// <summary>
                /// Indicates the shutdown reason codes. 
                /// You can specify any minor reason in combination with any major reason, but some combinations do not make sense.
                /// </summary>
                /// For more info see here:
                /// http://msdn.microsoft.com/en-us/library/windows/desktop/aa376885%28v=vs.85%29.aspx
                [Description("Enum used in the 'dwReason' parameter of 'ExitWindowsEx' Function.")]
                [Flags()]
                public enum ShutdownReason : uint
                {

                    /// <summary>
                    /// Application issue.
                    /// </summary>
                    MajorApplication = 0x40000,

                    /// <summary>
                    /// Hardware issue.
                    /// </summary>
                    MajorHardware = 0x10000,

                    /// <summary>
                    /// The 'InitiateSystemShutdown' function was used instead of 'InitiateSystemShutdownEx'.
                    /// </summary>
                    MajorLegacyApi = 0x70000,

                    /// <summary>
                    /// Operating system issue.
                    /// </summary>
                    MajorOperatingSystem = 0x20000,

                    /// <summary>
                    /// Other issue.
                    /// </summary>
                    MajorOther = 0x0,

                    /// <summary>
                    /// Power failure.
                    /// </summary>
                    MajorPower = 0x60000,

                    /// <summary>
                    /// Software issue.
                    /// </summary>
                    MajorSoftware = 0x30000,

                    /// <summary>
                    /// System failure..
                    /// </summary>
                    MajorSystem = 0x50000,

                    /// <summary>
                    /// Blue screen crash event.
                    /// </summary>
                    MinorBlueScreen = 0xf,

                    /// <summary>
                    /// Unplugged.
                    /// </summary>
                    MinorCordUnplugged = 0xb,

                    /// <summary>
                    /// Disk.
                    /// </summary>
                    MinorDisk = 0x7,

                    /// <summary>
                    /// Environment.
                    /// </summary>
                    MinorEnvironment = 0xc,

                    /// <summary>
                    /// Driver.
                    /// </summary>
                    MinorHardwareDriver = 0xd,

                    /// <summary>
                    /// Hot fix.
                    /// </summary>
                    MinorHotfix = 0x11,

                    /// <summary>
                    /// Hot fix uninstallation.
                    /// </summary>
                    MinorHotfixUninstall = 0x17,

                    /// <summary>
                    /// Unresponsive.
                    /// </summary>
                    MinorHung = 0x5,

                    /// <summary>
                    /// Installation.
                    /// </summary>
                    MinorInstallation = 0x2,

                    /// <summary>
                    /// Maintenance.
                    /// </summary>
                    MinorMaintenance = 0x1,

                    /// <summary>
                    /// MMC issue.
                    /// </summary>
                    MinorMMC = 0x19,

                    /// <summary>
                    /// Network connectivity.
                    /// </summary>
                    MinorNetworkConnectivity = 0x14,

                    /// <summary>
                    /// Network card.
                    /// </summary>
                    MinorNetworkCard = 0x9,

                    /// <summary>
                    /// Other issue.
                    /// </summary>
                    MinorOther = 0x0,

                    /// <summary>
                    /// Other driver event.
                    /// </summary>
                    MinorOtherDriver = 0xe,

                    /// <summary>
                    /// Power supply.
                    /// </summary>
                    MinorPowerSupply = 0xa,

                    /// <summary>
                    /// Processor.
                    /// </summary>
                    MinorProcessor = 0x8,

                    /// <summary>
                    /// Reconfigure.
                    /// </summary>
                    MinorReconfig = 0x4,

                    /// <summary>
                    /// Security issue.
                    /// </summary>
                    MinorSecurity = 0x13,

                    /// <summary>
                    /// Security patch.
                    /// </summary>
                    MinorSecurityFix = 0x12,

                    /// <summary>
                    /// Security patch uninstallation.
                    /// </summary>
                    MinorSecurityFixUninstall = 0x18,

                    /// <summary>
                    /// Service pack.
                    /// </summary>
                    MinorServicePack = 0x10,

                    /// <summary>
                    /// Service pack uninstallation.
                    /// </summary>
                    MinorServicePackUninstall = 0x16,

                    /// <summary>
                    /// Terminal Services.
                    /// </summary>
                    MinorTermSrv = 0x20,

                    /// <summary>
                    /// Unstable.
                    /// </summary>
                    MinorUnstable = 0x6,

                    /// <summary>
                    /// Upgrade.
                    /// </summary>
                    MinorUpgrade = 0x3,

                    /// <summary>
                    /// WMI issue.
                    /// </summary>
                    MinorWMI = 0x15

                }

                /// <summary>
                /// Indicates the shutdown reason planning.
                /// </summary>
                /// For more info see here:
                /// http://msdn.microsoft.com/en-us/library/windows/desktop/aa376885%28v=vs.85%29.aspx
                [Description("Enum used in combination with the 'dwReason' parameter of 'ExitWindowsEx' and 'InitiateShutdown' Functions.")]
                public enum ShutdownPlanning : uint
                {

                    /// <summary>
                    /// The shutdown was unplanned.
                    /// This is the default parameter.
                    /// </summary>
                    Unplanned = 0x0u,

                    /// <summary>
                    /// The reason code is defined by the user. 
                    /// For more information, see Defining a Custom Reason Code.
                    /// If this flag is not present, the reason code is defined by the system.
                    /// </summary>
                    UserDefined = 0x40000000u,

                    /// <summary>
                    /// The shutdown was planned. 
                    /// The system generates a System State Data (SSD) file. 
                    /// This file contains system state information such as the processes, threads, memory usage, and configuration.
                    /// If this flag is not present, the shutdown was unplanned.
                    /// </summary>
                    Planned = 0x80000000u

                }

                /// <summary>
                /// The attributes of a privilege.
                /// </summary>
                [Description("Enum used in the 'Privileges' parameter of 'TOKEN_PRIVILEGES' structure.")]
                [Flags()]
                public enum TOKEN_PRIVILEGES_FLAGS : uint
                {

                    /// <summary>
                    /// The privilege is enabled by default.
                    /// </summary>
                    SE_PRIVILEGE_ENABLED_BY_DEFAULT = 0x1u,

                    /// <summary>
                    /// The privilege is enabled.
                    /// </summary>
                    SE_PRIVILEGE_ENABLED = 0x2u,

                    /// <summary>
                    /// Used to remove a privilege.
                    /// </summary>
                    SE_PRIVILEGE_REMOVED = 0x4u,

                    /// <summary>
                    /// The privilege was used to gain access to an object or service. 
                    /// This flag is used to identify the relevant privileges 
                    /// in a set passed by a client application that may contain unnecessary privileges
                    /// </summary>
                    SE_PRIVILEGE_USED_FOR_ACCESS = 0x80000000u

                }

                /// <summary>
                /// An application cannot change the access control list of an object unless the application has the rights to do so.
                /// These rights are controlled by a security descriptor in the access token for the object. 
                /// </summary>
                /// For more info see here:
                /// http://msdn.microsoft.com/en-us/library/windows/desktop/aa374905%28v=vs.85%29.aspx
                [Description("Enum used in the 'DesiredAccess' parameter of 'OpenProcessToken' Function.")]
                [Flags()]
                public enum AccessRights : uint
                {

                    //******' 
                    // NOTE '
                    //******'     
                    // This Enumeration is partially defined.

                    /// <summary>
                    /// Required to enable or disable the privileges in an access token
                    /// </summary>
                    TOKEN_ADJUST_PRIVILEGES = 0x32u,

                    /// <summary>
                    /// Required to query an access token
                    /// </summary>
                    TOKEN_QUERY = 0x8u

                }

            }

            #endregion

            #region " Structures "

            /// <summary>
            /// Structures used by the Main Class.
            /// </summary>
            private class Structures
            {

                /// <summary>
                /// An 'LUID' is a 64-bit value guaranteed to be unique only on the system on which it was generated. 
                /// The uniqueness of a locally unique identifier (LUID) is guaranteed only until the system is restarted.
                /// </summary>
                internal struct LUID
                {

                    /// <summary>
                    /// The Low-order bits.
                    /// </summary>

                    public int LowPart;
                    /// <summary>
                    /// The High-order bits.
                    /// </summary>

                    public int HighPart;
                }

                /// <summary>
                /// Represents a locally unique identifier (LUID) and its attributes.
                /// </summary>
                internal struct LUID_AND_ATTRIBUTES
                {

                    /// <summary>
                    /// Specifies an 'LUID' value.
                    /// </summary>

                    public LUID pLuid;
                    /// <summary>
                    /// Specifies attributes of the 'LUID'. 
                    /// This value contains up to 32 one-bit flags.
                    /// Its meaning is dependent on the definition and use of the 'LUID'.
                    /// </summary>

                    public Enums.TOKEN_PRIVILEGES_FLAGS Attributes;
                }

                /// <summary>
                /// Contains information about a set of privileges for an access token.
                /// </summary>
                internal struct TOKEN_PRIVILEGES
                {

                    /// <summary>
                    /// This must be set to the number of entries in the Privileges array
                    /// </summary>

                    public int PrivilegeCount;
                    /// <summary>
                    /// Specifies an array of 'LUID_AND_ATTRIBUTES' structures. 
                    /// Each structure contains the 'LUID' and attributes of a privilege. 
                    /// To get the name of the privilege associated with a 'LUID', call the 'LookupPrivilegeName' function, 
                    /// passing the address of the 'LUID' as the value of the 'lpLuid' parameter.
                    /// </summary>

                    public LUID_AND_ATTRIBUTES Privileges;
                }

            }

            #endregion

            #endregion

            #region " Private Methods "

            /// <summary>
            /// Gets the necessary shutdown privileges to perform a local shutdown operation.
            /// </summary>
            /// <param name="Computer">
            /// Indicates the computer where to set the privileges.
            /// If a null string is specified, the function attempts to find the privilege name on the local system
            /// </param>

            private static void GetLocalShutdownPrivileges(string Computer)
            {
                IntPtr hToken = default(IntPtr);
                Structures.TOKEN_PRIVILEGES tkp = default(Structures.TOKEN_PRIVILEGES);

                NativeMethods.OpenProcessToken(Process.GetCurrentProcess().Handle, Enums.AccessRights.TOKEN_ADJUST_PRIVILEGES | Enums.AccessRights.TOKEN_QUERY, ref hToken);

                var _with1 = tkp;
                _with1.PrivilegeCount = 1;
                _with1.Privileges.Attributes = Enums.TOKEN_PRIVILEGES_FLAGS.SE_PRIVILEGE_ENABLED;

                NativeMethods.LookupPrivilegeValue(Computer, Privileges.SE_SHUTDOWN_NAME, ref tkp.Privileges.pLuid);

                NativeMethods.AdjustTokenPrivileges(hToken, false, ref tkp, 0u, IntPtr.Zero, IntPtr.Zero);

            }

            /// <summary>
            /// Gets the necessary shutdown privileges to perform a remote shutdown operation.
            /// </summary>
            /// <param name="Computer">
            /// Indicates the computer where to set the privileges.
            /// If a null string is specified, the function attempts to find the privilege name on the local system
            /// </param>

            private static void GetRemoteShutdownPrivileges(string Computer)
            {
                IntPtr hToken = default(IntPtr);
                Structures.TOKEN_PRIVILEGES tkp = default(Structures.TOKEN_PRIVILEGES);

                NativeMethods.OpenProcessToken(Process.GetCurrentProcess().Handle, Enums.AccessRights.TOKEN_ADJUST_PRIVILEGES | Enums.AccessRights.TOKEN_QUERY, ref hToken);

                var _with2 = tkp;
                _with2.PrivilegeCount = 1;
                _with2.Privileges.Attributes = Enums.TOKEN_PRIVILEGES_FLAGS.SE_PRIVILEGE_ENABLED;

                NativeMethods.LookupPrivilegeValue(Computer, Privileges.SE_REMOTE_SHUTDOWN_NAME, ref tkp.Privileges.pLuid);

                NativeMethods.AdjustTokenPrivileges(hToken, false, ref tkp, 0u, IntPtr.Zero, IntPtr.Zero);

            }

            #endregion

            #region " Public Methods "

            /// <summary>
            /// Aborts a system shutdown operation that has been initiated (unless a LogOff).
            /// </summary>
            /// <param name="Computer">
            /// The network name of the computer where the shutdown is to be stopped. 
            /// If this parameter is 'NULL' or an empty string, the function aborts the shutdown on the local computer.
            /// </param>
            /// <returns><c>True</c> if the function succeeds, <c>False</c> otherwise.</returns>
            public static bool Abort(string Computer = null)
            {

                return NativeMethods.AbortSystemShutdown(Computer);

            }

            //        /// <summary>
            //        /// Shuts down all processes running in the logon session and then logs off the interactive user. 
            //        /// </summary>
            //        /// <param name="Force">
            //        /// Indicates whether to force the logoff.
            //        /// </param>
            //        /// <param name="Reason">
            //        /// Indicates the reason for initiating the shutdown.
            //        /// </param>
            //        /// <returns>
            //        /// If the function succeeds, the return value is 'True'. 
            //        /// The function executes asynchronously so a 'True' return value indicates that the shutdown has been initiated. 
            //        /// It does not indicate whether the shutdown will succeed. 
            //        /// It is possible that the system, the user, or another application will abort the shutdown.
            //        /// If the function fails, the return value is 'False'. 
            //        /// </returns>
            //        public static bool LogOff(Enums.ExitwindowsEx_Force Force = Enums.ExitwindowsEx_Force.Wait, Enums.ShutdownReason Reason = Enums.ShutdownReason.MajorOther)
            //        {

            //            GetLocalShutdownPrivileges(null);
            //            GetRemoteShutdownPrivileges(null);

            //            return NativeMethods.ExitWindowsEx(Enums.ExitwindowsEx_Flags.LogOff | Force, Reason);

            //        }

            //        /// <summary>
            //        /// Shutdowns the specified computer and begins powered down.
            //        /// </summary>
            //        /// <param name="Computer">
            //        /// The name of the computer to poweroff.
            //        /// If the value of this parameter is 'NULL', the local computer is shut down.
            //        /// This parameter can be an addres, for example: '127.0.0.1'
            //        /// </param>
            //        /// <param name="Message">
            //        /// The message to be displayed in the interactive poweroff dialog box.
            //        /// </param>
            //        /// <param name="Timeout">
            //        /// The number of seconds to wait before shutting down the computer.
            //        /// If the value of this parameter is zero, the computer is poweroff immediately.
            //        /// This value is limited to 'MAX_SHUTDOWN_TIMEOUT'.
            //        /// If the value of this parameter is greater than zero, and the 'dwShutdownFlags' parameter
            //        /// specifies the flag 'GRACE_OVERRIDE', the function fails and returns the error code 'ERROR_BAD_ARGUMENTS'.
            //        /// </param>
            //        /// <param name="Force">
            //        /// Indicates whether to force the PowerOff.
            //        /// </param>
            //        /// <param name="Reason">
            //        /// The reason for initiating the poweroff.
            //        /// If this parameter is zero,
            //        /// the default is an undefined poweroff that is logged as "No title for this reason could be found".
            //        /// By default, it is also an 'unplanned' poweroff.
            //        /// </param>
            //        /// <param name="Planning">
            //        /// Indicates whether it's a planned or unplanned PowerOff operation.
            //        /// </param>
            //        /// <returns>
            //        /// <c>true</c> if the poweroff operation is initiated correctlly, <c>false</c> otherwise.
            //        /// </returns>
            //        public static bool PowerOff(string Computer = null, int Timeout = 0, string Message = null, Enums.InitiateShutdown_Force Force = Enums.InitiateShutdown_Force.Wait, Enums.ShutdownReason Reason = Enums.ShutdownReason.MajorOther, Enums.ShutdownPlanning Planning = Enums.ShutdownPlanning.Unplanned)
            //        {

            //            GetLocalShutdownPrivileges(Computer);
            //            GetRemoteShutdownPrivileges(Computer);

            //            switch (Timeout)
            //            {

            //                case  // ERROR: Case labels with binary operators are unsupported : Equality
            //0:

            //                    return !Convert.ToBoolean(NativeMethods.InitiateShutdown(Computer, Message, Timeout, Enums.InitiateShutdown_Flags.PowerOff | Enums.InitiateShutdown_Flags.GraceOverride | Force, Reason | Planning));
            //                default:

            //                    return !Convert.ToBoolean(NativeMethods.InitiateShutdown(Computer, Message, Timeout, Enums.InitiateShutdown_Flags.PowerOff | Force, Reason | Planning));
            //            }

            //        }

            //        /// <summary>
            //        /// Restarts the specified computer.
            //        /// </summary>
            //        /// <param name="Computer">
            //        /// The name of the computer to restart.
            //        /// If the value of this parameter is 'NULL', the local computer is shut down.
            //        /// This parameter can be an addres, for example: '127.0.0.1'
            //        /// </param>
            //        /// <param name="Message">
            //        /// The message to be displayed in the interactive restart dialog box.
            //        /// </param>
            //        /// <param name="Timeout">
            //        /// The number of seconds to wait before restarting the computer.
            //        /// If the value of this parameter is zero, the computer is restarted immediately.
            //        /// This value is limited to 'MAX_SHUTDOWN_TIMEOUT'.
            //        /// If the value of this parameter is greater than zero, and the 'dwShutdownFlags' parameter
            //        /// specifies the flag 'GRACE_OVERRIDE', the function fails and returns the error code 'ERROR_BAD_ARGUMENTS'.
            //        /// </param>
            //        /// <param name="Force">
            //        /// Indicates whether to force the restart.
            //        /// </param>
            //        /// <param name="Reason">
            //        /// The reason for initiating the restart.
            //        /// If this parameter is zero,
            //        /// the default is an undefined restart that is logged as "No title for this reason could be found".
            //        /// By default, it is also an 'unplanned' restart.
            //        /// </param>
            //        /// <param name="Planning">
            //        /// Indicates whether it's a planned or unplanned restart operation.
            //        /// </param>
            //        /// <returns>
            //        /// <c>true</c> if the restart operation is initiated correctlly, <c>false</c> otherwise.
            //        /// </returns>
            //        public static bool Restart(string Computer = null, int Timeout = 0, string Message = null, Enums.InitiateShutdown_Force Force = Enums.InitiateShutdown_Force.Wait, Enums.ShutdownReason Reason = Enums.ShutdownReason.MajorOther, Enums.ShutdownPlanning Planning = Enums.ShutdownPlanning.Unplanned)
            //        {

            //            GetLocalShutdownPrivileges(Computer);
            //            GetRemoteShutdownPrivileges(Computer);

            //            switch (Timeout)
            //            {

            //                case  // ERROR: Case labels with binary operators are unsupported : Equality
            //0:

            //                    return !Convert.ToBoolean(NativeMethods.InitiateShutdown(Computer, Message, Timeout, Enums.InitiateShutdown_Flags.Restart | Enums.InitiateShutdown_Flags.GraceOverride | Force, Reason | Planning));
            //                default:

            //                    return !Convert.ToBoolean(NativeMethods.InitiateShutdown(Computer, Message, Timeout, Enums.InitiateShutdown_Flags.Restart | Force, Reason | Planning));
            //            }

            //        }

            //        /// <summary>
            //        /// Restarts the specified computer,
            //        /// also restarts any applications that have been registered for restart 
            //        /// using the 'RegisterApplicationRestart' function.
            //        /// </summary>
            //        /// <param name="Computer">
            //        /// The name of the computer to restart.
            //        /// If the value of this parameter is 'NULL', the local computer is shut down.
            //        /// This parameter can be an addres, for example: '127.0.0.1'
            //        /// </param>
            //        /// <param name="Message">
            //        /// The message to be displayed in the interactive restart dialog box.
            //        /// </param>
            //        /// <param name="Timeout">
            //        /// The number of seconds to wait before restarting the computer.
            //        /// If the value of this parameter is zero, the computer is restarted immediately.
            //        /// This value is limited to 'MAX_SHUTDOWN_TIMEOUT'.
            //        /// If the value of this parameter is greater than zero, and the 'dwShutdownFlags' parameter
            //        /// specifies the flag 'GRACE_OVERRIDE', the function fails and returns the error code 'ERROR_BAD_ARGUMENTS'.
            //        /// </param>
            //        /// <param name="Force">
            //        /// Indicates whether to force the restart.
            //        /// </param>
            //        /// <param name="Reason">
            //        /// The reason for initiating the restart.
            //        /// If this parameter is zero,
            //        /// the default is an undefined restart that is logged as "No title for this reason could be found".
            //        /// By default, it is also an 'unplanned' restart.
            //        /// </param>
            //        /// <param name="Planning">
            //        /// Indicates whether it's a planned or unplanned restart operation.
            //        /// </param>
            //        /// <returns>
            //        /// <c>true</c> if the restart operation is initiated correctlly, <c>false</c> otherwise.
            //        /// </returns>
            //        public static bool RestartApps(string Computer = null, int Timeout = 0, string Message = null, Enums.InitiateShutdown_Force Force = Enums.InitiateShutdown_Force.Wait, Enums.ShutdownReason Reason = Enums.ShutdownReason.MajorOther, Enums.ShutdownPlanning Planning = Enums.ShutdownPlanning.Unplanned)
            //        {

            //            GetLocalShutdownPrivileges(Computer);
            //            GetRemoteShutdownPrivileges(Computer);

            //            switch (Timeout)
            //            {

            //                case  // ERROR: Case labels with binary operators are unsupported : Equality
            //0:

            //                    return !Convert.ToBoolean(NativeMethods.InitiateShutdown(Computer, Message, Timeout, Enums.InitiateShutdown_Flags.RestartApps || Enums.InitiateShutdown_Flags.GraceOverride | Force, Reason | Planning));
            //                default:

            //                    return !Convert.ToBoolean(NativeMethods.InitiateShutdown(Computer, Message, Timeout, Enums.InitiateShutdown_Flags.RestartApps | Force, Reason | Planning));
            //            }

            //        }

            //        /// <summary>
            //        /// Shutdowns the specified computer.
            //        /// </summary>
            //        /// <param name="Computer">
            //        /// The name of the computer to be shut down.
            //        /// If the value of this parameter is 'NULL', the local computer is shut down.
            //        /// This parameter can be an addres, for example: '127.0.0.1'
            //        /// </param>
            //        /// <param name="Message">
            //        /// The message to be displayed in the interactive shutdown dialog box.
            //        /// </param>
            //        /// <param name="Timeout">
            //        /// The number of seconds to wait before shutting down the computer.
            //        /// If the value of this parameter is zero, the computer is shut down immediately.
            //        /// This value is limited to 'MAX_SHUTDOWN_TIMEOUT'.
            //        /// If the value of this parameter is greater than zero, and the 'dwShutdownFlags' parameter
            //        /// specifies the flag 'GRACE_OVERRIDE', the function fails and returns the error code 'ERROR_BAD_ARGUMENTS'.
            //        /// </param>
            //        /// <param name="Force">
            //        /// Indicates whether to force the shutdown.
            //        /// </param>
            //        /// <param name="Reason">
            //        /// The reason for initiating the shutdown.
            //        /// If this parameter is zero,
            //        /// the default is an undefined shutdown that is logged as "No title for this reason could be found".
            //        /// By default, it is also an 'unplanned' shutdown.
            //        /// </param>
            //        /// <param name="Planning">
            //        /// Indicates whether it's a planned or unplanned shutdoen operation.
            //        /// </param>
            //        /// <returns>
            //        /// <c>true</c> if the shutdown operation is initiated correctlly, <c>false</c> otherwise.
            //        /// </returns>
            //        public static bool Shutdown(string Computer = null, int Timeout = 0, string Message = null, Enums.InitiateShutdown_Force Force = Enums.InitiateShutdown_Force.Wait, Enums.ShutdownReason Reason = Enums.ShutdownReason.MajorOther, Enums.ShutdownPlanning Planning = Enums.ShutdownPlanning.Unplanned)
            //        {

            //            GetLocalShutdownPrivileges(Computer);
            //            GetRemoteShutdownPrivileges(Computer);

            //            switch (Timeout)
            //            {

            //                case  // ERROR: Case labels with binary operators are unsupported : Equality
            //0:

            //                    return !Convert.ToBoolean(NativeMethods.InitiateShutdown(Computer, Message, Timeout, Enums.InitiateShutdown_Flags.Shutdown | Enums.InitiateShutdown_Flags.GraceOverride | Force, Reason | Planning));
            //                default:
            //                    return !Convert.ToBoolean(NativeMethods.InitiateShutdown(Computer, Message, Timeout, Enums.InitiateShutdown_Flags.Shutdown | Force, Reason | Planning));
            //            }

            //        }

            //        /// <summary>
            //        /// Use this function only for Windows 8/8.1
            //        /// Shutdowns the specified computer and prepares the system for a faster startup.
            //        /// </summary>
            //        /// <param name="Computer">
            //        /// The name of the computer to be shut down.
            //        /// If the value of this parameter is 'NULL', the local computer is shut down.
            //        /// This parameter can be an addres, for example: '127.0.0.1'
            //        /// </param>
            //        /// <param name="Message">
            //        /// The message to be displayed in the interactive shutdown dialog box.
            //        /// </param>
            //        /// <param name="Timeout">
            //        /// The number of seconds to wait before shutting down the computer.
            //        /// If the value of this parameter is zero, the computer is shut down immediately.
            //        /// This value is limited to 'MAX_SHUTDOWN_TIMEOUT'.
            //        /// If the value of this parameter is greater than zero, and the 'dwShutdownFlags' parameter
            //        /// specifies the flag 'GRACE_OVERRIDE', the function fails and returns the error code 'ERROR_BAD_ARGUMENTS'.
            //        /// </param>
            //        /// <param name="Force">
            //        /// Indicates whether to force the shutdown.
            //        /// </param>
            //        /// <param name="Reason">
            //        /// The reason for initiating the shutdown.
            //        /// If this parameter is zero,
            //        /// the default is an undefined shutdown that is logged as "No title for this reason could be found".
            //        /// By default, it is also an 'unplanned' shutdown.
            //        /// </param>
            //        /// <param name="Planning">
            //        /// Indicates whether it's a planned or unplanned shutdoen operation.
            //        /// </param>
            //        /// <returns>
            //        /// <c>true</c> if the shutdown operation is initiated correctlly, <c>false</c> otherwise.
            //        /// </returns>
            //        public static bool HybridShutdown(string Computer = null, int Timeout = 0, string Message = null, Enums.InitiateShutdown_Force Force = Enums.InitiateShutdown_Force.Wait, Enums.ShutdownReason Reason = Enums.ShutdownReason.MajorOther, Enums.ShutdownPlanning Planning = Enums.ShutdownPlanning.Unplanned)
            //        {

            //            GetLocalShutdownPrivileges(Computer);
            //            GetRemoteShutdownPrivileges(Computer);

            //            switch (Timeout)
            //            {

            //                case  // ERROR: Case labels with binary operators are unsupported : Equality

            //                    return !Convert.ToBoolean(NativeMethods.InitiateShutdown(Computer, Message, Timeout, Enums.InitiateShutdown_Flags.Shutdown | Enums.InitiateShutdown_Flags.HybridShutdown | Enums.InitiateShutdown_Flags.GraceOverride | Force, Reason | Planning));
            //                default:
            //                    return !Convert.ToBoolean(NativeMethods.InitiateShutdown(Computer, Message, Timeout, Enums.InitiateShutdown_Flags.Shutdown | Enums.InitiateShutdown_Flags.HybridShutdown | Force, Reason | Planning));
            //            }

            //        }

            #endregion

        }

        #endregion
    }
}