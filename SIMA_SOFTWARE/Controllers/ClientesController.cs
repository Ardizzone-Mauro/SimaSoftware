using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIMA_SOFTWARE.Data;
using SIMA_SOFTWARE.Models;
using SIMA_SOFTWARE.Models.ViewModels;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIMA_SOFTWARE.Controllers
{
    public class ClientesController : Controller
    {
        private readonly SimaDbContext _context;

        public ClientesController(SimaDbContext context)
        {
            _context = context;
        }

        // GET: Clientes
        public async Task<IActionResult> Index(bool? activos)
        {
            var query = _context.Clientes
                .Include(c => c.Cuenta)
                .Include(c => c.Direccion)
                .Include(c => c.TipoCliente)
                .Include(c => c.User)
                .AsQueryable();

            if (activos == true)
                query = query.Where(c => c.Activo);
            else if (activos == false)
                query = query.Where(c => !c.Activo);

            return View(await query.ToListAsync());
        }

        //Activar un cliente
        public async Task<IActionResult> Activar(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null)
                return NotFound();

            cliente.Activo = true;

            _context.Update(cliente);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Clientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes
                .Include(c => c.Cuenta)
                .Include(c => c.Direccion)
                .Include(c => c.TipoCliente)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.IdCliente == id);
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // GET: Clientes/Create
       

        public IActionResult Create()
        {
            var vm = new ClienteViewModel
            {
                TiposCliente = _context.TipoClientes
                    .Select(t => new SelectListItem
                    {
                        Value = t.IdTipoCliente.ToString(),
                        Text = t.Categoria
                    }).ToList()
            };

            return View(vm);
        }

        // POST: Clientes/Create
        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClienteViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var direccion = new Direccion
                {
                    Calle = vm.Calle,
                    NroPuerta = vm.NroPuerta,
                    Barrio = vm.Barrio
                };

                _context.Direcciones.Add(direccion);
                await _context.SaveChangesAsync();

                var cuenta = new Cuenta
                {
                    Saldo = vm.Saldo,
                    CondicionesPago = vm.CondicionesPago
                };

                _context.Cuentas.Add(cuenta);
                await _context.SaveChangesAsync();

                var cliente = new Cliente
                {
                    Nombre = vm.Nombre,
                    Email = vm.Email,
                    Telefono = vm.Telefono,
                    Activo = vm.Activo,
                    DireccionId = direccion.IdDireccion,
                    CuentaId = cuenta.IdCuenta,
                    TipoClienteId = vm.TipoClienteId
                };

                _context.Clientes.Add(cliente);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            vm.TiposCliente = _context.TipoClientes
                .Select(t => new SelectListItem
                {
                    Value = t.IdTipoCliente.ToString(),
                    Text = t.Categoria
                }).ToList();

            return View(vm);
        }



        // GET: Clientes/Edit/5
      

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var cliente = await _context.Clientes
                .Include(c => c.Direccion)
                .Include(c => c.Cuenta)
                .FirstOrDefaultAsync(c => c.IdCliente == id);

            if (cliente == null)
                return NotFound();

            var vm = new ClienteViewModel
            {
                IdCliente = cliente.IdCliente,
                Nombre = cliente.Nombre,
                Email = cliente.Email,
                Telefono = cliente.Telefono,
                Activo = cliente.Activo,

                // Dirección
                Calle = cliente.Direccion.Calle,
                NroPuerta = cliente.Direccion.NroPuerta,
                Barrio = cliente.Direccion.Barrio,

                // Cuenta
                Saldo = cliente.Cuenta.Saldo,
                CondicionesPago = cliente.Cuenta.CondicionesPago,

                // Tipo
                TipoClienteId = cliente.TipoClienteId,

                TiposCliente = _context.TipoClientes
                    .Select(t => new SelectListItem
                    {
                        Value = t.IdTipoCliente.ToString(),
                        Text = t.Categoria
                    }).ToList()
            };

            return View(vm);
        }

        // POST: Clientes/Edit/5
    

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ClienteViewModel vm)
        {
            if (id != vm.IdCliente)
                return NotFound();

            if (ModelState.IsValid)
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    var cliente = await _context.Clientes
                        .Include(c => c.Direccion)
                        .Include(c => c.Cuenta)
                        .FirstOrDefaultAsync(c => c.IdCliente == id);

                    if (cliente == null)
                        return NotFound();

                    // CLIENTE
                    cliente.Nombre = vm.Nombre;
                    cliente.Email = vm.Email;
                    cliente.Telefono = vm.Telefono;
                    cliente.Activo = vm.Activo;
                    cliente.TipoClienteId = vm.TipoClienteId;

                    // DIRECCION
                    cliente.Direccion.Calle = vm.Calle;
                    cliente.Direccion.NroPuerta = vm.NroPuerta;
                    cliente.Direccion.Barrio = vm.Barrio;

                    // CUENTA
                    cliente.Cuenta.Saldo = vm.Saldo;
                    cliente.Cuenta.CondicionesPago = vm.CondicionesPago;

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }

            // Recargar dropdown si falla
            vm.TiposCliente = _context.TipoClientes
                .Select(t => new SelectListItem
                {
                    Value = t.IdTipoCliente.ToString(),
                    Text = t.Categoria
                }).ToList();

            return View(vm);
        }

        // GET: Clientes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes
                .Include(c => c.Cuenta)
                .Include(c => c.Direccion)
                .Include(c => c.TipoCliente)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.IdCliente == id);
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // POST: Clientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente != null)
            {
                cliente.Activo = false; // BORRADO LÓGICO
                _context.Clientes.Update(cliente);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
