using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using ProyectoArqSoft.FactoryProducts;
using ProyectoArqSoft.Models;
using System.Data;

namespace ProyectoArqSoft.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");

        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IMedicamentoRepository>();
            services.RemoveAll<IClienteRepository>();
            services.RemoveAll<IBioquimicoRepository>();
            services.RemoveAll<IRepository<Medicamento>>();
            services.RemoveAll<IRepository<Cliente>>();
            services.RemoveAll<IRepository<Bioquimico>>();

            services.AddDataProtection()
                .UseEphemeralDataProtectionProvider();

            services.AddScoped<IMedicamentoRepository, FakeMedicamentoRepository>();
            services.AddScoped<IClienteRepository, FakeClienteRepository>();
            services.AddScoped<IBioquimicoRepository, FakeBioquimicoRepository>();
            services.AddScoped<IRepository<Medicamento>>(provider => provider.GetRequiredService<IMedicamentoRepository>());
            services.AddScoped<IRepository<Cliente>>(provider => provider.GetRequiredService<IClienteRepository>());
            services.AddScoped<IRepository<Bioquimico>>(provider => provider.GetRequiredService<IBioquimicoRepository>());
        });
    }

    private sealed class FakeMedicamentoRepository : IMedicamentoRepository
    {
        public int Count() => 2;

        public DataTable GetDestacados()
        {
            DataTable table = CreateMedicamentoTable();
            table.Rows.Add("Paracetamol", "Tableta", "Analgesico", "500mg", 12.5m);
            return table;
        }

        public int Insert(Medicamento t) => 1;
        public int Update(Medicamento t) => 1;
        public int Delete(Medicamento t) => 1;
        public DataTable GetAll() => CreateMedicamentoTable();
        public DataTable GetAll(string filtro) => CreateMedicamentoTable();
        public Medicamento? GetById(int id) => new() { Id = id, Nombre = "Paracetamol" };

        private static DataTable CreateMedicamentoTable()
        {
            DataTable table = new();
            table.Columns.Add("nombre", typeof(string));
            table.Columns.Add("presentacion", typeof(string));
            table.Columns.Add("clasificacion", typeof(string));
            table.Columns.Add("concentracion", typeof(string));
            table.Columns.Add("precio", typeof(decimal));
            return table;
        }
    }

    private sealed class FakeClienteRepository : IClienteRepository
    {
        public int Count() => 3;
        public Cliente? ObtenerPorNit(string nit) => null;
        public int Insert(Cliente t) => 1;
        public int Update(Cliente t) => 1;
        public int Delete(Cliente t) => 1;
        public DataTable GetAll() => new();
        public DataTable GetAll(string filtro) => new();
        public Cliente? GetById(int id) => new() { IdCliente = id, Nit = "123456", RazonSocial = "Cliente Demo" };
    }

    private sealed class FakeBioquimicoRepository : IBioquimicoRepository
    {
        public int Count() => 1;
        public Bioquimico? GetByDocumento(string ci, string extension) => null;
        public int Insert(Bioquimico t) => 1;
        public int Update(Bioquimico t) => 1;
        public int Delete(Bioquimico t) => 1;
        public DataTable GetAll() => new();
        public DataTable GetAll(string filtro) => new();
        public Bioquimico? GetById(int id) => new() { IdBioquimico = id, Nombres = "Bioquimico Demo" };
    }
}
