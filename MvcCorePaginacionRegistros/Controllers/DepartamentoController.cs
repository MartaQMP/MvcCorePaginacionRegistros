using Microsoft.AspNetCore.Mvc;
using MvcCorePaginacionRegistros.Models;
using MvcCorePaginacionRegistros.Repositories;
using System.Threading.Tasks;

namespace MvcCorePaginacionRegistros.Controllers
{
    public class DepartamentoController : Controller
    {
        private RepositoryHospital repo;

        public DepartamentoController(RepositoryHospital repo)
        {
            this.repo = repo;
        }

        public async Task<IActionResult> Details(int? posicion, int id)
        {
            Departamento dept = await this.repo.GetDepartamentoByIdAsync(id);
            if (posicion == null)
            {
                posicion = 1;
            }
            int numRegistros = await this.repo.GetNumeroRegistroEmpleadosAsync(id);
            int siguiente = posicion.Value + 1;
            if (siguiente > numRegistros)
            {
                siguiente = numRegistros;
            }
            int anterior = posicion.Value - 1;
            if (anterior < 1)
            {
                anterior = 1;
            }
            ViewBag.Posicion = posicion;
            ViewBag.Ultimo= numRegistros;
            ViewBag.Siguiente = siguiente;
            ViewBag.Anterior = anterior;
            ViewBag.Departamento = dept;
            List<Empleado> empleados = await this.repo.GetEmpleadosByDepartamentoAsync(id);
            if (empleados.Count == 0)
            {
                return View();
            }
            Empleado empleado = empleados[posicion.Value - 1];
            return View(empleado);
        }

        public async Task<IActionResult> DetailsEF(int? posicion, int id)
        {
            Departamento dept = await this.repo.GetDepartamentoByIdAsync(id);
            if (posicion == null)
            {
                posicion = 1;
            }
            int numRegistros = await this.repo.GetNumeroRegistroEmpleadosAsync(id);
            int siguiente = posicion.Value + 1;
            if (siguiente > numRegistros)
            {
                siguiente = numRegistros;
            }
            int anterior = posicion.Value - 1;
            if (anterior < 1)
            {
                anterior = 1;
            }
            ViewBag.Posicion = posicion;
            ViewBag.Ultimo= numRegistros;
            ViewBag.Siguiente = siguiente;
            ViewBag.Anterior = anterior;
            ViewBag.Departamento = dept;
            List<Empleado> empleados = await this.repo.GetEmpleadosByDepartamentoEFAsync(id, posicion.Value - 1);
            if (empleados.Count == 0)
            {
                return View();
            }
            return View(empleados.FirstOrDefault());
        }



    }
}
