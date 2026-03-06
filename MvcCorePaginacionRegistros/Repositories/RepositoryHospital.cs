using Microsoft.EntityFrameworkCore;
using MvcCorePaginacionRegistros.Data;
using MvcCorePaginacionRegistros.Models;

#region VIEWS
/*
--------------
--PAGINACION--
--------------
    CREATE VIEW V_DEPARTAMENTOS_INDIVIDUAL
        AS
            SELECT 
            CAST(ROW_NUMBER() OVER (ORDER BY DEPT_NO) AS INT)
            AS POSICION, DEPT_NO, DNOMBRE, LOC 
            FROM DEPT
        GO
*/
#endregion


namespace MvcCorePaginacionRegistros.Repositories
{
   
    public class RepositoryHospital
    {
        private HospitalContext context;

        public RepositoryHospital(HospitalContext context)
        {
            this.context = context;
        }

        public async Task<int> GetNumeroRegistroVistaDepartamentosAsync()
        {
            return await this.context.VistaDepartamentos.CountAsync();
        }

        public async Task<VistaDepartamento> GetVistaDepartamentoAsync(int posicion)
        {
            return await this.context.VistaDepartamentos.Where(v => v.Posicion == posicion).FirstOrDefaultAsync();
        }

        public async Task<List<VistaDepartamento>> GetGrupoVistaDepartamentoAsync(int posicion)
        {
            return await this.context.VistaDepartamentos.Where(v => v.Posicion >= posicion && v.Posicion < posicion + 2).ToListAsync();
        }
    }
}
