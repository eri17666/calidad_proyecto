namespace ProyectoArqSoft.Validaciones
{
    public abstract class ResultadoOperacionBase
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public string Error { get; }

        protected ResultadoOperacionBase(bool isSuccess, string error = "")
        {
            IsSuccess = isSuccess;
            Error = error;
        }
    }
}
