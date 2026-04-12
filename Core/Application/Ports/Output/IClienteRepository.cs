using ProyectoArqSoft.Models;

namespace ProyectoArqSoft.FactoryProducts
{
    public interface IClienteRepository : IRepository<Cliente>
    {
        Cliente? ObtenerPorNit(string nit);
        int Count();
    }
}
