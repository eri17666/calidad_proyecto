using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProyectoArqSoft.FactoryProducts;
using ProyectoArqSoft.Repositories;
using ProyectoArqSoft.Services;
using System.Data;

[assembly: InternalsVisibleTo("ProyectoArqSoft.Tests")]

namespace ProyectoArqSoft.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IMedicamentoRepository _medicamentoRepository;
        private readonly IClienteRepository _clienteRepo;
        private readonly IBioquimicoRepository _bioquimicoRepo;

        public string? Usuario { get; set; }

        public DataTable MedicamentoDataTable { get; set; } = new();

        public int TotalMedicamentos { get; set; }
        public int TotalClientes { get; set; }
        public int TotalBioquimicos { get; set; }

        public IndexModel(
            ILogger<IndexModel> logger,
            IMedicamentoRepository medicamentoRepository,
            IClienteRepository clienteRepo,
            IBioquimicoRepository bioquimicoRepo)
        {
            _logger = logger;
            _medicamentoRepository = medicamentoRepository;
            _clienteRepo = clienteRepo;
            _bioquimicoRepo = bioquimicoRepo;
        }

        public void OnGet()
        {
            Usuario = HttpContext.Session.GetString("Usuario");

            TotalMedicamentos = _medicamentoRepository.Count();
            TotalClientes = _clienteRepo.Count();
            TotalBioquimicos = _bioquimicoRepo.Count();

            MedicamentoDataTable = _medicamentoRepository.GetDestacados();
        }
    }
}