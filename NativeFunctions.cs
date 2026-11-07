using System.Runtime.InteropServices;

namespace Rascunho_Injetor_Dll
{
    // Define a assinatura comum utilizada pelos métodos de carregamento de DLL.
    // Permite armazenar diferentes funções em um mesmo delegate.
    public delegate IntPtr DllLoadMethod(IntPtr hProcess, string pathDll, out uint errorCode);

    // Disponibiliza os métodos de carregamento de DLL implementados nas bibliotecas nativas.
    internal static class LoadDll
    {
        // Identificadores utilizados para selecionar o método de carregamento.
        public const string LOAD_LIBRARY_A = "LoadLibraryA";
        public const string LOAD_LIBRARY_W = "LoadLibraryW";
        public const string LDR_LOAD_DLL = "LdrLoadDll";

        //Função que ultiliza API LdrLoadDll para a injeção 
        [DllImport("DllLdrLoadDll.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr LdrLoadDll(IntPtr hProcess, string pathDll, out uint errorCode);

        //Função que ultiliza API LoadLibraryA para a injeção 
        [DllImport("DllLoadA.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr LoadDllA(IntPtr hProcess, string pathDll, out uint errorCode);

        //Função que ultiliza API LoadLibraryW para a injeção 
        [DllImport("DllLoadW.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr LoadDllW(IntPtr hProcess, string pathDll, out uint errorCode);

        // Associa o nome de cada método à sua respectiva função nativa.
        // O delegate permite selecionar dinamicamente qual implementação será executada.
        public static Dictionary<string, DllLoadMethod> LoadMethods = new Dictionary<string, DllLoadMethod>()
        {
            {LOAD_LIBRARY_A, LoadDllA },
            {LOAD_LIBRARY_W, LoadDllW},
            {LDR_LOAD_DLL, LdrLoadDll }
        };
    }

    internal static class SeDebugPrivilege
    {
        //Função Capaz de Habilitar ou Desabilitar privilegios de Debug neste injetor
        [DllImport("SeDebugPrivilegeDll.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern bool EnableSeDebugPrivilege(bool fEnable);
        public static bool SetEnabled(bool fEnable) => EnableSeDebugPrivilege(fEnable);
    }

    // Disponibiliza funcionalidades paraà restauração dos bytes originais
    // de uma API no processo alvo.
    internal static class UnHooking
    {
        static UnHooking()
        {
            // Define a quantidade de bytes de acordo com a arquitetura do injetor.
            NUM_BYTES = Environment.Is64BitProcess ? new UIntPtr(7) : new UIntPtr(6);
            // Array utilizado para armazenar os bytes originais recuperados.
            OldBytes = new byte[NUM_BYTES];
        }

        public static UIntPtr NUM_BYTES; // Quantidade de bytes utilizados no procedimento de UnHook.
        public static byte[] OldBytes; // Armazena os bytes originais da API antes da restauração.
        public static uint OldProtect = 0; // Armazena a proteção de memória anterior da região modificada.


        //Função responsavel por restaurar possiveis bytes modificados das APIs de injeçoes propositalmente
        //Hookados para proteção obs: CharSet.Unicode é essencial nesta API
        [DllImport("UnHookDLL.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnHook(IntPtr hProcess, int pid, string moduleName, string APIname, UIntPtr numBytes, out uint oldProtect, [Out] byte[] oldBytes);

        // Relaciona cada API ao módulo do Windows onde ela está localizada.
        public static Dictionary<string, string> APIModule = new Dictionary<string, string>()
        {
            {LoadDll.LOAD_LIBRARY_A, "kernel32.dll" },
            {LoadDll.LOAD_LIBRARY_W, "kernel32.dll" },
            {LoadDll.LDR_LOAD_DLL, "ntdll.dll" }
        };

        // Define as constantes de proteção de memória utilizadas pelo Windows.
        // O atributo Flags permite representar combinações de valores.
        [Flags]
        public enum MemoryProtection : uint
        {
            PAGE_NOACCESS = 0x01,
            PAGE_READONLY = 0x02,
            PAGE_READWRITE = 0x04,
            PAGE_WRITECOPY = 0x08,

            PAGE_EXECUTE = 0x10,
            PAGE_EXECUTE_READ = 0x20,
            PAGE_EXECUTE_READWRITE = 0x40,
            PAGE_EXECUTE_WRITECOPY = 0x80,

            PAGE_GUARD = 0x100,
            PAGE_NOCACHE = 0x200,
            PAGE_WRITECOMBINE = 0x400
        }

    }
}
