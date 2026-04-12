using ProyectoArqSoft.Models;
using System.Data;

namespace ProyectoArqSoft.FactoryProducts
{
    public interface IMedicamentoRepository : IRepository<Medicamento>
    {
        int Count();
        DataTable GetDestacados();
    }
}
