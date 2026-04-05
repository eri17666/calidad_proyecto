using ProyectoArqSoft.Models;

namespace ProyectoArqSoft.FactoryProducts
{
    public interface IMedicamentoRepository : IRepository<Medicamento>
    {
        int Count();
    }
}
