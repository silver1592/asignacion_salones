using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrigenDatos.Clases
{
    public class ListaGrupos
    {
        protected List<Grupo> grupos;

        #region Constructores e inicializadores
        public ListaGrupos()
        {

        }

        public ListaGrupos(List<Grupo> grupos)
        {
            this.grupos = new List<Grupo>();
            foreach(Grupo g in grupos)
            {
                this.grupos.Add(g);
            }
        }

        public ListaGrupos(Conexion c, DataTable dtGrupos, ListaSalones salones)
        {
            grupos = new List<Grupo>();

            foreach (DataRow r in dtGrupos.Rows)
                grupos.Add(new Grupo(r, c, salones));
        }

        public ListaGrupos(ListaGrupos grp)
        {
            this.grupos = new List<Grupo>();
            foreach (Grupo g in grp.grupos)
            {
                this.grupos.Add(new Grupo(g));
            }
        }

        public void SetGrupos(List<Grupo> grupos)
        {
            this.grupos = grupos;
        }

        #endregion

        #region Consultas

        /// <summary>
        /// Checa los grupos que estan por asignar en el horario y dias marcados
        /// </summary>
        /// <param name="dias">Cadena de 6 caracteres conformada por 0 y 1 empezando del Lunes a Sabado</param>
        /// <param name="hora">Hora en la que se buscaran los grupos sin asignar</param>
        /// <param name="ciclo">Ciclo escolar a checar</param>
        /// <returns></returns>
        public ListaGrupos GruposSinAsignar(string dias, int hora)
        {
            ListaGrupos res;

            var query = from Grupo g in grupos
                        where g.empalme(hora, hora + 1, dias) && (g.Salon=="" || g.Salon==null || g.Salon == " ")
                        select g;

            res = new ListaGrupos(query.ToList<Grupo>());
            return res;
        }

        internal bool[] HorarioEnHora(int hora)
        {
            bool[] res = { false, false, false, false, false, false };

            foreach (Grupo g in grupos)
                for (int i = 0; i < 6; i++)
                    if (g.horario_ini[i] >= hora && hora + 1 >= g.horario_fin[i])
                        res[i] = true;

            return res;
        }

        /// <summary>
        /// Checa los grupos que estan por asignar en el horario y dias marcados
        /// </summary>
        /// <param name="dias">Cadena de 6 caracteres conformada por 0 y 1 empezando del Lunes a Sabado</param>
        /// <param name="hora">Hora en la que se buscaran los grupos sin asignar</param>
        /// <param name="ciclo">Ciclo escolar a checar</param>
        /// <returns></returns>
        public ListaGrupos GruposAsignados(string dias, int hora)
        {
            ListaGrupos res;

            var query = from Grupo g in grupos
                        where g.empalme(hora, hora + 1, dias) && (g.Salon != "" || g.Salon != null || g.Salon != " ")
                        select g;

            res = new ListaGrupos(query.ToList<Grupo>());
            return res;
        }

        public ListaGrupos GruposEnSalon(string salon)
        {
            ListaGrupos res;

            var query = from Grupo g in grupos
                        where g.Salon == salon
                        select g;

            res = new ListaGrupos(query.ToList<Grupo>());
            return res;
        }

        public virtual Grupo GetGrupo(int i)
        {
            return grupos[i];
        }

        public int Count()
        {
            return grupos.Count;
        }

        public ListaGrupos Grupos_Empalmados()
        {
            var query = from g in grupos
                        from g1 in grupos
                        where g.empalme(g1)
                        select g;

            return new ListaGrupos(query.Distinct().ToList());
        }

        public ListaGrupos Grupos_Empalmados(Grupo grupo)
        {
            var query = from g in grupos
                        where g.empalme(grupo)
                        select g;

            return new ListaGrupos(query.ToList());
        }

        public bool HayEmpalme(int hora_ini, int hora_fin, string dias)
        {
            var query = from g in grupos
                        where g.empalme(hora_ini, hora_fin, dias)
                        select g;

            return query.ToList().Count == 0 ? true : false;
        }

        public bool HayEmpalme(int[] hora_ini, int[] hora_fin)
        {
            var query = from g in grupos
                        where g.empalme(hora_ini, hora_fin)
                        select g;

            return query.ToList().Count == 0 ? true : false;
        }

        public void Remove(Grupo grupo)
        {
            grupos.Remove(grupo);
        }

        public void Add(Grupo grupo)
        {
            grupos.Add(grupo);
        }

        #endregion

        //Agregar consulta para obtener grupos que requieran estar en planta baja
        //Agregar consulta para obtener grupos con salon fijo
    }
}
