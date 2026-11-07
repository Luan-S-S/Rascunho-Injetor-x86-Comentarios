using System.Runtime.InteropServices;

namespace Rascunho_Injetor_Dll
{

    static internal class Kernel32 // Disponibiliza constantes e declarações de funções nativas da kernel32.dll.
    {
        // Direitos de acesso utilizados na abertura de processos.
        public const uint PROCESS_CREATE_THREAD = 0x0002,
                          PROCESS_QUERY_INFORMATION = 0x0400,
                          PROCESS_VM_OPERATION = 0x0008,
                          PROCESS_VM_WRITE = 0x0020,
                          PROCESS_VM_READ = 0x0010,
                          PROCESS_QUERY_LIMITED_INFORMATION = 0x1000;

        // Identificadores de arquitetura utilizados pelo formato PE.
        // Correspondem aos valores do campo Machine do cabeçalho PE.
        public const uint IMAGE_FILE_MACHINE_UNKNOWN = 0x000;
        public const uint IMAGE_FILE_MACHINE_I386 = 0x014C;
        public const uint IMAGE_FILE_MACHINE_AMD64 = 0x8664;
        public const uint IMAGE_FILE_MACHINE_ARM64 = 0xAA64;
        public const uint IMAGE_FILE_MACHINE_ARM = 0x01C4;

        // Abre um processo existente e retorna um handle para ele.
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        // Fecha um handle aberto pelo processo atual.
        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr Handle);

        // Identifica a arquitetura de um processo e a arquitetura nativa do sistema.
        [DllImport("kernel32.dll")]
        public static extern bool IsWow64Process2(IntPtr hProcess, out ushort pProcessMachine, out ushort pNativeMachine);
    }
}
