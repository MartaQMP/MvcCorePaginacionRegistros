using Microsoft.AspNetCore.Mvc;
using MvcCorePaginacionRegistros.Models;
using MvcCorePaginacionRegistros.Repositories;
using System.Threading.Tasks;

namespace MvcCorePaginacionRegistros.Controllers
{
    public class PaginacionController : Controller
    {
        private RepositoryHospital repo;

        public PaginacionController(RepositoryHospital repo)
        {
            this.repo = repo;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> RegistroVistaDepartamento(int? posicion)
        {
            if(posicion == null)
            {
                posicion = 1;
            }
            int numRegistros = await this.repo.GetNumeroRegistroVistaDepartamentosAsync();
            int siguiente = posicion.Value + 1;
            if(siguiente > numRegistros)
            {
                siguiente = numRegistros;
            }
            int anterior = posicion.Value - 1;
            if(anterior < 1)
            {
                anterior = 1;
            }
            ViewBag.Ultimo = numRegistros;
            ViewBag.Siguiente = siguiente;
            ViewBag.Anterior = anterior;
            VistaDepartamento vista = await this.repo.GetVistaDepartamentoAsync(posicion.Value);
            return View(vista);
        }

        public async Task<IActionResult> GrupoVistaDepartamentos(int? posicion)
        {
            if(posicion == null)
            {
                posicion = 1; 
            }
            /* LO SIGUIENTE SERA QUE DEBEMOS O LOS NUMEROS DE PAGINA EN LOS LINKS
             * <a href='grupodepts?posicion=1'>Pagina 1</a>
             * <a href='grupodepts?posicion=2'>Pagina 2</a>
             * <a href='grupodepts?posicion=3'>Pagina 3</a>
             * NECESITAMOS UNA VARIABLE PARA EL NUMERO DE PAGINA VOY A REALIZAR 
             * EL DIBUJO DESDE AQUI, NO DESDE RAZOR */
            int numPagina = 1;
            int numRegistros = await this.repo.GetNumeroRegistroVistaDepartamentosAsync();
            /*string html = "<div>";
            for(int i = 0; i< numRegistros; i++)
            {
                html += "<a href='GrupoVistaDepartamentos?posicion=" + i + "'>Pagina "+ numPagina+"</a>";
                numPagina += 1;
            }
            html += "</div>";
            ViewBag.Links = html;*/
            ViewBag.NumPagina = numPagina;
            ViewBag.NumRegistros = numRegistros;
            List<VistaDepartamento> departamentos = await this.repo.GetGrupoVistaDepartamentoAsync(posicion.Value);
            return View(departamentos);
        }
    }
}
