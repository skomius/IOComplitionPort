using System;
using System.Threading;
using System.Runtime.InteropServices;
using System.ComponentModel;
using IOCompletionPort;

namespace Testing
{
    unsafe class Program
    {
        static readonly IntPtr NoFileHandle = new IntPtr(-1);

        unsafe static void Main(string[] args)
        {
            IntPtr fileHandle = CreateFile("failas.txt", (uint)FileAccess.FileReadWrite, 0, IntPtr.Zero, 1, 0x40000000, IntPtr.Zero);

            if (fileHandle == IntPtr.Zero)
            {
                throw new Win32Exception();
            }

            Console.WriteLine(fileHandle.ToInt32());

            Console.ReadKey();

            overlap lapped = new overlap();
            ManualResetEvent evente = new ManualResetEvent(false);
            lapped.EventHandle = evente.SafeWaitHandle.DangerousGetHandle();
            //Console.WriteLine(lapped.EventHandle.ToInt32() + '\n');

            IOCompletionPort.IOCompletionPort comPort = new IOCompletionPort.IOCompletionPort(fileHandle, 3, 4);


            ThreadStart action = () => {
                Console.WriteLine(comPort.Handle.ToInt32());
                IOStatus iOStatus = comPort.GetPacket(0); 
                Console.WriteLine( iOStatus.lpCompletionKey + " " + iOStatus.lpNumberOfBytes + " " + iOStatus.EventHandle + '\n');
            };

            new Thread(action).Start();

            byte[] buffer = new byte[] { 34, 2, 100, 200, 45 };
            uint numberWritten = 0;
            IOCompletionPort.IOCompletionPort.WriteFile(fileHandle, buffer, 5, out numberWritten, lapped);
            Console.WriteLine(numberWritten + '\n');

            Console.ReadKey();
        }

        unsafe static void IOcomp(IntPtr intance, IntPtr context, overlap overlapped, ulong iorezults, UIntPtr bytesTransfared, IntPtr PTP)
        {

        }

        [StructLayout(LayoutKind.Sequential)]
        class SystemTime
        {
            public ushort Year;
            public ushort Month;
            public ushort DayOfWeek;
            public ushort Day;
            public ushort Hour;
            public ushort Minute;
            public ushort Second;
            public ushort Milliseconds;
        }

        [DllImport("kernel32.dll")]
        static extern void GetSystemTime(SystemTime t);

        [DllImport("C:/Users/Skomantas/OneDrive/visual studio/IOComplitionPort/Release/IOPoolStub.dll", SetLastError = true)]
        static extern void Destroy(long skaicius);

        [DllImport("C:/Users/Skomantas/OneDrive/visual studio/IOComplitionPort/Debug/IOPoolStub.dll", SetLastError = true)]
        static extern IntPtr createIoCompletionPort(IntPtr fileHandle, IntPtr existing, uint completionKey, uint concurency);

        [DllImport("kernel32.dll")]
        static extern IntPtr CreateFile(string name, uint dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes,
            uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr handle);

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
    }
}



