// Services/ReservaDatabaseService.cs
using SQLite;
using RodriguezCalvaRualesMAUIUniWay.Models;
using RodriguezCalvaRualesMAUIUniWay.API;

namespace RodriguezCalvaRualesMAUIUniWay.Services
{
    public class ReservaDatabaseService
    {
        private readonly SQLiteAsyncConnection _database;

        public ReservaDatabaseService(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<ReservaLocal>().Wait();
        }

        public async Task<List<ReservaLocal>> GetReservasAsync()
        {
            try
            {
                return await _database.Table<ReservaLocal>()
                    .OrderByDescending(r => r.FechaReserva)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener reservas: {ex.Message}", ex);
            }
        }

        public async Task<ReservaLocal> GetReservaByIdAsync(int id)
        {
            try
            {
                return await _database.Table<ReservaLocal>()
                    .Where(r => r.Id == id)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener reserva: {ex.Message}", ex);
            }
        }

        public async Task<List<ReservaLocal>> GetReservasByEstadoAsync(Estado estado)
        {
            try
            {
                return await _database.Table<ReservaLocal>()
                    .Where(r => r.Estado == estado)
                    .OrderByDescending(r => r.FechaReserva)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener reservas por estado: {ex.Message}", ex);
            }
        }

        public async Task<int> SaveReservaAsync(ReservaLocal reserva)
        {
            try
            {
                if (reserva.Id != 0)
                {
                    return await _database.UpdateAsync(reserva);
                }
                else
                {
                    return await _database.InsertAsync(reserva);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al guardar reserva: {ex.Message}", ex);
            }
        }

        public async Task<int> DeleteReservaAsync(ReservaLocal reserva)
        {
            try
            {
                return await _database.DeleteAsync(reserva);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al eliminar reserva: {ex.Message}", ex);
            }
        }

        public async Task<int> UpdateEstadoReservaAsync(int id, Estado nuevoEstado)
        {
            try
            {
                var reserva = await GetReservaByIdAsync(id);
                if (reserva != null)
                {
                    reserva.Estado = nuevoEstado;
                    return await _database.UpdateAsync(reserva);
                }
                return 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al actualizar estado: {ex.Message}", ex);
            }
        }
    }
}