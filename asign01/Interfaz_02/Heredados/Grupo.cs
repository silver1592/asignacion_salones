using System.Data;

namespace Interfaz_02.Heredados
{
    public class Grupo : Algoritmo01.Heredados.Grupo
    {
        #region Constructores
        /// <summary>
        /// Constructor usado cuando se crea a partir de un excel
        /// </summary>
        /// <param name=""></param>
        public Grupo(string cve_materia, string grupo, string rpe, string tipo, string salon, string li, string lf, string mi, string mf, string Mii, string Mif, string ji, string jf, string vi, string vf, string si, string sf, string cupo, string ciclo) : base(cve_materia, grupo, rpe, tipo, salon, li, lf, mi, mf, Mii, Mif, ji, jf, vi, vf, si, sf, cupo, ciclo) { }
        public Grupo(DataRow grupo, DataTable necesidadesGrupo = null, DataTable necesidadesProfesor = null, DataTable salonesPosibles = null, ListaSalones salones = null) : base(grupo, necesidadesGrupo, necesidadesProfesor, salonesPosibles, salones) { }

        public Grupo(Grupo g) : base(g) { }

        public Grupo(DataRow r, Conexion c, ListaSalones salones) : base(r, c, salones) { }
        #endregion
    }
}
