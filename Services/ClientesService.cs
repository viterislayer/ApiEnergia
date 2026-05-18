using ApiEnergia.DTOs;
using ApiEnergia.Interfaces;
using ApiEnergia.Models;

namespace ApiEnergia.Services
{
    public class ClientesService : IClientesService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ClientesService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CrearClienteConContadorResponse> CrearClienteConContadorAsync(CrearClienteConContadorRequest request)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            var cliente = (await _unitOfWork.Clientes.FindAsync(c => c.Dpi == request.Dpi)).FirstOrDefault();
            if (cliente is null)
            {
                cliente = new ClienteLuz
                {
                    Dpi = request.Dpi,
                    Nombre = request.Nombre,
                    Apellido = request.Apellido,
                    Correo = request.Correo
                };

                await _unitOfWork.Clientes.AddAsync(cliente);
            }

            var numeroContador = GenerarNumeroContador();
            var contador = new ContadorEnergia
            {
                NumeroContador = numeroContador,
                DireccionInmueble = request.DireccionInmueble,
                FechaInstalacion = DateTime.UtcNow,
                Estado = "ACTIVO",
                Cliente = cliente
            };

            await _unitOfWork.Contadores.AddAsync(contador);

            await _unitOfWork.SaveChangesAsync();

            return new CrearClienteConContadorResponse(numeroContador);
        }

        private static string GenerarNumeroContador()
        {
            return Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
        }
    }
}
