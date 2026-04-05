using ProyectoArqSoft.Models;
using System.Data;

namespace ProyectoArqSoft.FactoryProducts
{
    public interface IBioquimicoRepository : IRepository<Bioquimico>
    {
        DataTable GetByDocumento(string ci, string extension);
    }
}
