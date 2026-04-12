using ProyectoArqSoft.FactoryProducts;
using ProyectoArqSoft.Model.DTOS;

namespace ProyectoArqSoft.Services
{
    public class EstadisticasService
    {
        private readonly IMedicamentoRepository _medicamentoRepo;
        private readonly IClienteRepository _clienteRepo;
        private readonly IBioquimicoRepository _bioquimicoRepo;

        public EstadisticasService(
            IMedicamentoRepository medicamentoRepo,
            IClienteRepository clienteRepo,
            IBioquimicoRepository bioquimicoRepo)
        {
            _medicamentoRepo = medicamentoRepo;
            _clienteRepo = clienteRepo;
            _bioquimicoRepo = bioquimicoRepo;
        }

        public EstadisticasDTO ObtenerEstadisticas()
        {
            return new EstadisticasDTO
            {
                TotalMedicamentos = _medicamentoRepo.Count(),
                TotalClientes = _clienteRepo.Count(),
                TotalBioquimicos = _bioquimicoRepo.Count(),
            };
        }
    }
}
