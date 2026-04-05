namespace ProyectoArqSoft.Validaciones
{
    public class Validacion : ResultadoOperacionBase
    {
        public Validacion(bool isSuccess, string error = "")
            : base(isSuccess, error)
        {
        }

        public static Validacion Ok()
            => new Validacion(true);

        public static Validacion Fail(string error)
            => new Validacion(false, error);
    }
}
