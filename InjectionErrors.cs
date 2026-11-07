namespace Rascunho_Injetor_Dll
{
    // Centraliza os códigos, descrições e mensagens de sucesso relacionados
    // às operações realizadas durante o processo de injeção.
    internal class InjectionErrors
    {
        // Define os códigos de erro retornados pelas implementações nativas.
        // Cada valor identifica uma falha específica durante a injeção.
        public enum InjectionErrorCode : uint
        {
            ERROR_INVALID_HANDLE = 0x6, //Erro handle do processo invalida
            ERROR_INVALID_PARAMETER = 0x57, //Path da dll null ou invalida
            ERROR_REMOTE_PROCESS_WRITE_PATHDLL_FAILED = 0xA, //Erro ao escrever pathDll  no processo
            ERROR_REMOTE_PROCESS_WRITE_UNICODE_STRING_FAILED = 0xB, //Erro ao escrever UNICODE_STRING no processo
            ERROR_REMOTE_PROCESS_WRITE_LDRSTRUCT_FAILED = 0xC, //Erro ao escrever estrutura LDR no processo
            ERROR_REMOTE_PROCESS_WRITE_REMOTE_LDRLOADDLL_FAILED = 0xD, //Erro ao escrever Função remote_LDR no processo
            ERROR_FUNCTION_ADDRESS_NOT_FOUND = 0xE, // Erro ao encontrar endereço da função LDR nativa no processo
            ERROR_REMOTE_THREAD_CREATE_FAILED = 0xF, // Erro ao criar Thread remota de injeção
            ERROR_MEMORY_RELEASE_FAILED = 0x10, // Erro ao liberar Memorias usadas
            ERROR_GET_EXIT_CODE_FAILED = 0x11, // Erro ao obter exitCode da injeção
            ERROR_LOADLIBRARY_FAILED = 0x12, // A API de injeção falhou 
        }
        public struct InjectionErrorInfo // Armazena as informações relacionadas a um erro específico.
        {
            public InjectionErrorCode errorCode; // Código que identifica o erro.
            public string Description; // Descrição apresentada quando a operação correspondente falha.
            public string SuccessMessage; // Mensagem apresentada quando a operação correspondente é concluída.
        }

        static InjectionErrors() // Inicializa as definições e listas de erros compartilhadas pela classe.
        {
            InitializeErrorDefinitions();
            InitializeErrorLists();
        }

        // Seleciona a lista de erros correspondente ao método de carregamento utilizado.
        public InjectionErrors(string typeLoadLibrary)
        {
            // O método LdrLoadDll possui etapas adicionais, como o tratamento
            // de UNICODE_STRING e da estrutura utilizada pela implementação .
            this.ErrorList = typeLoadLibrary == LoadDll.LDR_LOAD_DLL ? LdrLoadDllErrors : LoadLibraryErrors;
        }

        // Lista de erros utilizada pela implementação selecionada.
        public List<InjectionErrorInfo> ErrorList { get; private set; }

        // Listas compartilhadas entre todas as instâncias da classe.
        private static List<InjectionErrorInfo> LoadLibraryErrors, LdrLoadDllErrors;

        // Definições individuais das mensagens e códigos de erro.
        private static InjectionErrorInfo InvalidHandleError, InvalidDllPathError, WriteDllPathError, WriteUnicodeStringError, WriteLdrStructureError, WriteRemoteLdrLoadDllError,
            FunctionAddressError, RemoteThreadCreationError, MemoryReleaseError, ExitCodeRetrievalError, DllLoadApiError, ModuleRetrievalError;

        // Inicializa os objetos que representam cada possível etapa ou erro da injeção.
        private static void InitializeErrorDefinitions()
        {
            InvalidHandleError = new InjectionErrorInfo
            {
                errorCode = InjectionErrorCode.ERROR_INVALID_HANDLE,
                Description = "Falha ao obter handle do processo",
            };

            InvalidDllPathError = new InjectionErrorInfo
            {
                errorCode = InjectionErrorCode.ERROR_INVALID_PARAMETER,
                Description = "Dll Selecionada Invalida"
            };

            WriteDllPathError = new InjectionErrorInfo
            {
                errorCode = InjectionErrorCode.ERROR_REMOTE_PROCESS_WRITE_PATHDLL_FAILED,
                Description = "Falha ao escrever o caminho da Dll no processo",
                SuccessMessage = "Caminho da DLL escrito no processo!"
            };

            WriteUnicodeStringError = new InjectionErrorInfo
            {
                errorCode = InjectionErrorCode.ERROR_REMOTE_PROCESS_WRITE_UNICODE_STRING_FAILED,
                Description = "Falha ao escrever UNICODE_STRING no processo",
                SuccessMessage = "UNICODE_STRING escrita no processo!"
            };

            WriteLdrStructureError = new InjectionErrorInfo
            {
                errorCode = InjectionErrorCode.ERROR_REMOTE_PROCESS_WRITE_LDRSTRUCT_FAILED,
                Description = "Falha ao escrever estrutura LDR no processo",
                SuccessMessage = "Estrutura LDR escrita no processo!"
            };

            WriteRemoteLdrLoadDllError = new InjectionErrorInfo
            {
                errorCode = InjectionErrorCode.ERROR_REMOTE_PROCESS_WRITE_REMOTE_LDRLOADDLL_FAILED,
                Description = "Falha ao escrever Função 'remote_LDR' no processo",
                SuccessMessage = "Função 'remote_LDR' escrita no processo!"
            };

            FunctionAddressError = new InjectionErrorInfo
            {
                errorCode = InjectionErrorCode.ERROR_FUNCTION_ADDRESS_NOT_FOUND,
                Description = "Falha ao localizar endereço da função de injeção nativa do processo",
                SuccessMessage = "Função de injeção nativa encontrada com sucesso!"
            };

            RemoteThreadCreationError = new InjectionErrorInfo
            {
                errorCode = InjectionErrorCode.ERROR_REMOTE_THREAD_CREATE_FAILED,
                Description = "Falha ao criar Thread remota de injeção",
                SuccessMessage = "Thread remota de injeção criada com sucesso!"
            };

            MemoryReleaseError = new InjectionErrorInfo
            {
                errorCode = InjectionErrorCode.ERROR_MEMORY_RELEASE_FAILED,
                Description = "Falha ao liberar Endereços usados para injeção",
                SuccessMessage = "Endereços liberados com sucesso!"
            };

            ExitCodeRetrievalError = new InjectionErrorInfo
            {
                errorCode = InjectionErrorCode.ERROR_GET_EXIT_CODE_FAILED,
                Description = "Falha ao obter 'exitCode' da injeção",
                SuccessMessage = "'exitCode' obtido com sucesso!"
            };

            DllLoadApiError = new InjectionErrorInfo
            {
                errorCode = InjectionErrorCode.ERROR_LOADLIBRARY_FAILED,
                Description = "A API de injeção falhou",
                SuccessMessage = "A API de injeção executou com sucesso!"
            };

        }

        // Monta as listas de erros de acordo com as etapas executadas por cada método.
        private static void InitializeErrorLists()
        {
            // Lista utilizada pelos métodos LoadLibraryA e LoadLibraryW.
            LoadLibraryErrors = new List<InjectionErrorInfo>() {/*handle,pathDll,*/WriteDllPathError, FunctionAddressError, RemoteThreadCreationError, DllLoadApiError, ExitCodeRetrievalError, MemoryReleaseError };

            // Lista utilizada pelo método LdrLoadDll.
            LdrLoadDllErrors = new List<InjectionErrorInfo>(){/*handle,pathDll,*/WriteDllPathError, WriteUnicodeStringError, WriteLdrStructureError, FunctionAddressError, WriteRemoteLdrLoadDllError,
            RemoteThreadCreationError, DllLoadApiError, ExitCodeRetrievalError, MemoryReleaseError};
        }
    }
}
