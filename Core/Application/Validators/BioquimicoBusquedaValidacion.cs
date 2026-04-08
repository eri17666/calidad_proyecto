using System.Text.RegularExpressions;

namespace ProyectoArqSoft.Validaciones
{
    public class BioquimicoBusquedaValidacion : IValidacion<string>
    {
        private const int MaxLongitud = 20;
        private const string PatronValido = @"^([a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+|\d+|\d+-\d[A-Z])$";

        public Validacion Validar(string filtro)
        {
            if (string.IsNullOrWhiteSpace(filtro))
                return Validacion.Ok();

            filtro = filtro.Trim();

            var validacionLongitud = ValidarLongitud(filtro);
            if (validacionLongitud != null)
                return validacionLongitud;

            var validacionFormato = ValidarFormatoBusqueda(filtro);
            if (validacionFormato != null)
                return validacionFormato;

            return Validacion.Ok();
        }

        public Validacion? ValidarLongitud(string filtro)
        {
            if (filtro.Length > MaxLongitud)
                return Validacion.Fail($"El criterio excede los {MaxLongitud} caracteres");

            return null;
        }

        public Validacion? ValidarFormatoBusqueda(string filtro)
        {
            if (!Regex.IsMatch(filtro, PatronValido))
                return Validacion.Fail("Criterio de búsqueda inválido. Use letras, números o rango (ej: 1-5A)");

            return null;
        }
    }
}