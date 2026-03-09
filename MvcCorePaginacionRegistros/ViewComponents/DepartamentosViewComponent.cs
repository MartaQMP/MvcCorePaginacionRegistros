using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MvcCorePaginacionRegistros.Models;
using MvcCorePaginacionRegistros.Repositories;

namespace MvcCorePaginacionRegistros.ViewComponents
{
    public class DepartamentosViewComponent: ViewComponent
    {
        private RepositoryHospital repo;

        public DepartamentosViewComponent(RepositoryHospital repo)
        {
            this.repo = repo;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<Departamento> departamentos= await this.repo.GetDepartamentosAsync();
            return View(departamentos);
        }
    }
}
