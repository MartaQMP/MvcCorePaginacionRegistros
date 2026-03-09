using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MvcCorePaginacionRegistros.Data;
using MvcCorePaginacionRegistros.Models;
using System.Data;
using System.Runtime.CompilerServices;

#region VIEWS
/*
--------------
--PAGINACION--
--------------
-----------------
--DEPARTAMENTOS--
-----------------
    CREATE VIEW V_DEPARTAMENTOS_INDIVIDUAL
    AS
        SELECT 
        CAST(ROW_NUMBER() OVER (ORDER BY DEPT_NO) AS INT)
        AS POSICION, DEPT_NO, DNOMBRE, LOC 
        FROM DEPT
    GO

---------------

    CREATE PROCEDURE SP_GRUPO_DEPARTAMENTOS (@posicion int)
    AS
	    SELECT DEPT_NO, DNOMBRE, LOC 
	    FROM V_DEPARTAMENTOS_INDIVIDUAL
	    WHERE POSICION >= @posicion AND POSICION < (@posicion +2)
    GO 

-------------
--EMPLEADOS--
-------------
    CREATE VIEW V_GRUPO_EMPLEADOS
    AS
	    SELECT CAST(ROW_NUMBER() OVER(ORDER BY APELLIDO) AS INT) AS POSICION, 
	    EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO
	    FROM EMP
    GO

---------------

    CREATE PROCEDURE SP_GRUPO_EMPLEADOS (@posicion int)
    AS
	    SELECT EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO 
	    FROM V_GRUPO_EMPLEADOS
	    WHERE POSICION >= @posicion AND POSICION < (@posicion + 3)
    GO

--------------

    CREATE PROCEDURE SP_GRUPO_EMPLEADOS_OFICIO (@posicion int, @oficio nvarchar(50))
    AS
	    SELECT EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO FROM 
		    (SELECT CAST(ROW_NUMBER() OVER(ORDER BY APELLIDO) AS INT) AS POSICION, 
		    EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO FROM EMP
		    WHERE OFICIO = @oficio) QUERY
	    WHERE (QUERY.POSICION >= @posicion AND QUERY.POSICION < (@posicion + 3))
    GO
--------------

    CREATE PROCEDURE SP_GRUPO_EMPLEADOS_OFICIO_REGISTRO (@posicion int, @oficio nvarchar(50), @registros int out)
    AS
	    SELECT @registros = COUNT(EMP_NO) FROM EMP WHERE OFICIO=@oficio;
	    SELECT EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO FROM 
		    (SELECT CAST(ROW_NUMBER() OVER(ORDER BY APELLIDO) AS INT) AS POSICION, 
		    EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO FROM EMP
		    WHERE OFICIO = @oficio) QUERY
	    WHERE (QUERY.POSICION >= @posicion AND QUERY.POSICION < (@posicion + 3))
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

        #region PRACTICA
        public async Task<List<Departamento>> GetDepartamentosAsync()
        {
            return await this.context.Departamentos.ToListAsync();
        }

        public async Task<Departamento> GetDepartamentoByIdAsync(int id)
        {
            return await this.context.Departamentos.Where(d => d.IdDepartamento == id).FirstOrDefaultAsync();
        }

        public async Task<int> GetNumeroRegistroEmpleadosAsync(int id)
        {
            return await this.context.Empleados.Where(e =>e.IdDepartamento == id).CountAsync();
        }

        public async Task<List<Empleado>> GetEmpleadosByDepartamentoAsync(int id)
        {
            return await this.context.Empleados.Where(e => e.IdDepartamento == id).ToListAsync();
        }

        #endregion

        #region CLASE
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

        public async Task<List<Departamento>> GetGrupoDepartamentosAsync(int posicion)
        {
            string sql = "SP_GRUPO_DEPARTAMENTOS @posicion";
            SqlParameter pamPosi = new SqlParameter("@posicion", posicion);
            return await this.context.Departamentos.FromSqlRaw(sql, pamPosi).ToListAsync();
        }

        public async Task<int> GetEmpleadosCountAsync()
        {
            return await this.context.Empleados.CountAsync();
        }

        public async Task<List<Empleado>> GetGrupoEmpleadosAsync(int posicion)
        {
            string sql = "SP_GRUPO_EMPLEADOS @posicion";
            SqlParameter pamPosi = new SqlParameter("@posicion", posicion);
            return await this.context.Empleados.FromSqlRaw(sql, pamPosi).ToListAsync();
        }

        public async Task<int> GetEmpleadosOficioCountAsyn(string oficio)
        {
            return await this.context.Empleados.Where(e => e.Oficio == oficio).CountAsync();
        }

        public async Task<List<Empleado>> GetGrupoEmpleadosOficioAsync(string oficio, int posicion)
        {
            string sql = "SP_GRUPO_EMPLEADOS_OFICIO @posicion, @oficio";
            SqlParameter pamPosi = new SqlParameter("@posicion", posicion);
            SqlParameter pamOfi = new SqlParameter("@oficio", oficio);
            return await this.context.Empleados.FromSqlRaw(sql, pamPosi, pamOfi).ToListAsync();
        }

        public async Task<ModelEmpleadosOficio> GetGrupoEmpleadosOficioOutAsync(string oficio, int posicion)
        {
            string sql = "SP_GRUPO_EMPLEADOS_OFICIO_REGISTRO @posicion, @oficio, @registros out";
            SqlParameter pamPosi = new SqlParameter("@posicion", posicion);
            SqlParameter pamOfi = new SqlParameter("@oficio", oficio);
            SqlParameter pamReg = new SqlParameter("@registros", SqlDbType.Int);
            pamReg.Direction = ParameterDirection.Output;
            var consulta = this.context.Empleados.FromSqlRaw(sql, pamPosi, pamOfi, pamReg);
            List<Empleado> empleados = await consulta.ToListAsync();
            int registros = (int)pamReg.Value;
            return new ModelEmpleadosOficio
            {
                Empleados = empleados,
                NumeroRegistros = registros
            };
        }
        #endregion
    }
}
