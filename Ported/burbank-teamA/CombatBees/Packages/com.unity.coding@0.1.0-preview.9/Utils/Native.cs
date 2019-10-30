using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

// ReSharper disable MemberCanBePrivate.Local
// ReSharper disable InconsistentNaming
#pragma warning disable 169

namespace Unity.Coding.Utils
{
    // $ do not use #if's for platforms. keep the binaries universal so we can build and post just
    //   one set of them to the package for CLI usage.

    public static class NativeUnix
    {
        struct timespec
        {
            ulong tv_sec;
            ulong tv_nsec;
        }

        static class Mac
        {
            [StructLayout(LayoutKind.Sequential, Pack = 0)]
            public struct STAT64
            {
                public uint st_dev;
                public ushort st_mode;
                public ushort st_nlink;
                public ulong st_ino;
                public uint st_uid;
                public uint st_gid;
                public uint st_rdev;
                public timespec st_atimespec;
                public timespec st_mtimespec;
                public timespec st_ctimespec;
                public timespec st_birthtimespec;
                public ulong st_size;
                public ulong st_blocks;
                public uint st_blksize;
                public uint st_flags;
                public uint st_gen;
                public uint st_lspare;
                public ulong st_qspare1;
                public ulong st_qspare2;
            }

            [DllImport("libc", EntryPoint = "stat$INODE64", SetLastError = true)]
            public static extern int stat(string file, ref STAT64 buf);
        }

        static class Linux
        {
            [StructLayout(LayoutKind.Sequential, Pack = 0)]
            public struct STAT64
            {
                public ulong st_dev;
                public ulong st_ino;
                public ulong st_nlink;
                public uint st_mode;
                public uint st_uid;
                public uint st_gid;
                public ulong st_rdev;
                public long st_size;
                public long st_blksize;
                public long st_blocks;
                public timespec st_atimespec;
                public timespec st_mtimespec;
                public timespec st_ctimespec;
            }

            // This refers to the version of the native stat structure used
            const int k_StatVersion = 1;

            // The native stat function is an inlined and statically-linked wrapper calling __xstat with the correct version
            public static int stat(string file, ref STAT64 buf)
                => __xstat(k_StatVersion, file, ref buf);

            [DllImport("libc", SetLastError = true)]
            static extern int __xstat(int statVersion, string file, ref STAT64 buf);
        }

        [Flags]
        public enum UnixFilePermissions
        {
            // user permissions
            S_IRUSR = 0x100,
            S_IWUSR = 0x80,
            S_IXUSR = 0x40,

            // group permission
            S_IRGRP = 0x20,
            S_IWGRP = 0x10,
            S_IXGRP = 0x08,

            // other permissions
            S_IROTH = 0x04,
            S_IWOTH = 0x02,
            S_IXOTH = 0x01,

            None = 0,
            All = 0x1FF
        }

        public static int SetFileMode([NotNull] string pathname, UnixFilePermissions mode)
            => chmod(pathname, mode);

        [DllImport("libc", SetLastError = true)]
        static extern int chmod(string pathname, UnixFilePermissions mode);

        public static int GetFileMode([NotNull] string filePath, out UnixFilePermissions permission)
        {
            permission = UnixFilePermissions.None;
            int ret;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                var stat = new Mac.STAT64();
                ret = Mac.stat(filePath, ref stat);
                if (ret == 0)
                    permission = (UnixFilePermissions)(stat.st_mode & (uint)UnixFilePermissions.All);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var stat = new Linux.STAT64();
                ret = Linux.stat(filePath, ref stat);
                if (ret == 0)
                    permission = (UnixFilePermissions)(stat.st_mode & (uint)UnixFilePermissions.All);
            }
            else
                throw new NotSupportedException("Not supported on this platform");

            return ret;
        }
    }
}
