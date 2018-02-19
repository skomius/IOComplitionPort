using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Text;
using System.Threading;



namespace IOCompletionPort
{

    public class IOCompletionPort
    {
        const uint INFINITE = 0xFFFFFFFF;

        static readonly IntPtr NoFileHandle = new IntPtr(-1);

        [Flags]
        public enum FileAccess : uint
        {
            FileCopy = 0x0001,
            FileWrite = 0x0002,
            FileRead = 0x0004,
            FileAllAccess = 0x001f,
            FileExecute = 0x0020,
            FileReadWrite = FileWrite | FileRead
        }

        [DllImport("C:/Users/Skomantas/OneDrive/visual studio/IOComplitionPort/Debug/IOPoolStub.dll", SetLastError = true)]
        static extern IntPtr CreateIoCompletionPortStub(
         IntPtr fileHandle,
         IntPtr existingCompletionPort,
         uint completionKey,
         uint numberOfConcurrentThreads);

        //Attempts to dequeue an I/O completion packet from the specified I/O 
        //completion port. If there is no completion packet queued, the function waits for a pending I/O operation associated with the completion port to complete.
        [DllImport("C:/Users/Skomantas/OneDrive/visual studio/IOComplitionPort/Debug/IOPoolStub.dll", SetLastError = true)]
        static extern int GetQueuedStub(
         IntPtr completionPort,
         out uint lpNumberOfBytes,
         out uint lpCompletionKey,
         out uint Handle,
         uint dwMilliseconds);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PostQueuedCompletionStatus(
         IntPtr completionPort,
         uint lpNumberOfBytes,
         ulong lpCompletionKey,
         out IntPtr lpOverlapped,
         uint dwMilliseconds);

        [DllImport("kernel32.dll")]
        public unsafe static extern Boolean WriteFile(IntPtr fFile, Byte[] lpBuffer, UInt32 nNumberOfBytesToWrite,
        out UInt32 lpNumberOfBytesWritten, overlap lpOverlapped /*NativeOverlapped *overlapped*/);

        [DllImport("kernel32.dll")]
        public unsafe static extern Boolean ReadFile(IntPtr fFile, Byte[] lpBuffer, UInt32 nNumberOfBytesToRead,
        out UInt32 lpNumberOfBytesRead, overlap lpOverlapped /*NativeOverlapped *overlapped*/);

        [DllImport("kernel32.dll")]
        static extern IntPtr CreateFile(string name, uint dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes,
            uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr handle);

        public IntPtr Handle { get; private set; }

        public int ConcurencyNumber { get; private set; }

        Dictionary<uint, IntPtr> fileObjects = new Dictionary<uint, IntPtr>();

        public Dictionary<uint, IntPtr> FileObjects { get { return fileObjects; } private set { } }


        public IOCompletionPort(IntPtr fileHandle, uint completionKey, uint concurencyNumber)
        {
            if (fileHandle == IntPtr.Zero)
            {
                throw new ArgumentException("File handle is null");
            }

            IntPtr handle = CreateIoCompletionPortStub(fileHandle, IntPtr.Zero, completionKey, concurencyNumber);

            if (IntPtr.Zero == handle)
            {
                throw new Win32Exception();
            }
            else
            {
                Handle = handle;
            }


            if (fileHandle != IntPtr.Zero && fileHandle != NoFileHandle)
                FileObjects.Add(completionKey, fileHandle);

        }

        public IOStatus GetPacket(uint timeout)
        {
            if(timeout == 0)
            {
                timeout = INFINITE;
            }
     
            IOStatus iOStatus = new IOStatus();

            if (0 == GetQueuedStub(Handle, out iOStatus.lpNumberOfBytes, out iOStatus.lpCompletionKey, out iOStatus.EventHandle, timeout))
                throw new Win32Exception();
            else
                return iOStatus;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    [ComVisible(true)]
    public class overlap
    {
        private IntPtr InternalLow;
        private IntPtr InternalHigh;
        public long Offset;
        public IntPtr EventHandle;
    }

    public class IOStatus
    {
        public uint lpNumberOfBytes;
        public uint lpCompletionKey;
        public uint EventHandle;
    }

    public class ThreadPoolIO
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public unsafe static extern IntPtr CreateThreadpoolIo(IntPtr fHandle, IOComplete callback,
        IntPtr pv, IntPtr ds);

        public unsafe delegate void IOComplete(IntPtr intance, IntPtr context, overlap overlapped, ulong iorezults, UIntPtr bytesTransfared, IntPtr PTP);
    }
}