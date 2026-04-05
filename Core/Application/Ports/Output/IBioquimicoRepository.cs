using ProyectoArqSoft.Models;

namespace ProyectoArqSoft.FactoryProducts
{
    public interface IBioquimicoRepository : IRepository<Bioquimico>
    {
        Bioquimico? GetByDocumento(string ci, string extension);
    }
}
