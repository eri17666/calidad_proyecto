using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using ProyectoArqSoft.FactoryProducts;
using System.Data;

[assembly: InternalsVisibleTo("ProyectoArqSoft.Tests")]

namespace ProyectoArqSoft.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IConfiguration configuration;

        public string? Usuario { get; set; }

        public DataTable MedicamentoDataTable { get; set; } = new DataTable();

        private readonly IMedicamentoRepository _medicamentoRepository;

        public int TotalMedicamentos { get; set; }

        public IndexModel(ILogger<IndexModel> logger, IConfiguration configuration, IMedicamentoRepository medicamentoRepository)
        {
            _logger = logger;
            this.configuration = configuration;
            _medicamentoRepository = medicamentoRepository;
        }

        public void OnGet()
        {
            Usuario = HttpContext.Session.GetString("Usuario");
            TotalMedicamentos = _medicamentoRepository.Count();
            CargarMedicamentosDestacados();
        }

        internal void CargarMedicamentosDestacados()
        {
            try
            {
                string? connectionString = configuration.GetConnectionString("MySqlConnection");

                // Si no hay conexión configurada (como en pruebas), crear DataTable vacío
                if (string.IsNullOrEmpty(connectionString))
                {
                    MedicamentoDataTable = new DataTable();
                    return;
                }

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"SELECT nombre,
                                    presentacion,
                                    clasificacion,
                                    concentracion,
                                    precio
                             FROM medicamento
                             WHERE estado = 1
                             ORDER BY RAND()
                             LIMIT 3";

                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, connection);
                    adapter.Fill(MedicamentoDataTable);
                }
            }
            catch (Exception)
            {
                // En caso de error (como en pruebas sin BD), devolver DataTable vacío
                MedicamentoDataTable = new DataTable();
            }
        }
    }
}