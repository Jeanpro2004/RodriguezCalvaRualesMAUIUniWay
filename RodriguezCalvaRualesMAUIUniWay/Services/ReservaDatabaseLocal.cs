using RodriguezCalvaRualesMAUIUniWay.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RodriguezCalvaRualesMAUIUniWay.Services
{
    public class ReservaDatabaseLocal
    {
        private readonly SQLiteAsyncConnection _db;
        private bool _isInitialized = false;

        public ReservaDatabaseLocal(string dbPath)
        {
            _db = new SQLiteAsyncConnection(dbPath);
        }

        private async Task InitializeAsync()
        {
            if (!_isInitialized)
            {
                await _db.CreateTableAsync<ReservaLocal>();
                _isInitialized = true;
            }
        }

        public async Task<List<ReservaLocal>> GetReservasAsync()
        {
            try
            {
                await InitializeAsync();
                return await _db.Table<ReservaLocal>()
                    .OrderByDescending(r => r.FechaReserva)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                // Log error
                throw new Exception($"Error al obtener reservas: {ex.Message}", ex);
            }
        }

        public async Task<ReservaLocal> GetReservaByIdAsync(int id)
        {
            try
            {
                await InitializeAsync();
                return await _db.Table<ReservaLocal>()
                    .Where(r => r.Id == id)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener reserva por ID: {ex.Message}", ex);
            }
        }

        public async Task<List<ReservaLocal>> GetReservasByEstadoAsync(string estado)
        {
            try
            {
                await InitializeAsync();
                return await _db.Table<ReservaLocal>()
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
                await InitializeAsync();

                // Validaciones básicas
                if (string.IsNullOrWhiteSpace(reserva.Estado))
                    throw new ArgumentException("El estado es requerido");

                if (string.IsNullOrWhiteSpace(reserva.MetodoPago))
                    throw new ArgumentException("El método de pago es requerido");

                if (reserva.Id == 0)
                {
                    reserva.FechaCreacion = DateTime.Now;
                    return await _db.InsertAsync(reserva);
                }
                else
                {
                    reserva.FechaActualizacion = DateTime.Now;
                    return await _db.UpdateAsync(reserva);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al guardar reserva: {ex.Message}", ex);
            }
        }

        public async Task<int> UpdateReservaAsync(ReservaLocal reserva)
        {
            try
            {
                await InitializeAsync();
                reserva.FechaActualizacion = DateTime.Now;
                return await _db.UpdateAsync(reserva);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al actualizar reserva: {ex.Message}", ex);
            }
        }

        public async Task<int> DeleteReservaAsync(ReservaLocal reserva)
        {
            try
            {
                await InitializeAsync();
                return await _db.DeleteAsync(reserva);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al eliminar reserva: {ex.Message}", ex);
            }
        }

        public async Task<int> DeleteReservaByIdAsync(int id)
        {
            try
            {
                await InitializeAsync();
                return await _db.DeleteAsync<ReservaLocal>(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al eliminar reserva por ID: {ex.Message}", ex);
            }
        }

        public async Task<bool> ExisteReservaAsync(int reservaRemotaId)
        {
            try
            {
                await InitializeAsync();
                var count = await _db.Table<ReservaLocal>()
                    .Where(r => r.ReservaRemotaId == reservaRemotaId)
                    .CountAsync();
                return count > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al verificar existencia de reserva: {ex.Message}", ex);
            }
        }

        public async Task<List<ReservaLocal>> GetReservasByFechaAsync(DateTime fecha)
        {
            try
            {
                await InitializeAsync();
                var fechaInicio = fecha.Date;
                var fechaFin = fechaInicio.AddDays(1);

                return await _db.Table<ReservaLocal>()
                    .Where(r => r.FechaViaje >= fechaInicio && r.FechaViaje < fechaFin)
                    .OrderBy(r => r.FechaViaje)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener reservas por fecha: {ex.Message}", ex);
            }
        }
    }
}