using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrigenDatos.Clases
{
    //TODO: Implements IList<Grupo> y tambien para salones
    public class ListaGrupos
    {
        protected List<Grupo> grupos;

        /// <summary>
        /// ToDo: Eliminar este metodo
        /// </summary>
        public List<Grupo> Grupos { get { return grupos; } }

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

        #region Basicos
        public virtual Grupo Get(int i)
        {
            return grupos[i];
        }

        public int Count()
        {
            return grupos.Count;
        }

        /// <summary>
        /// Checa si los grupos estan asignados a cierta hora
        /// </summary>
        /// <param name="hora">Hora a checar</param>
        /// <returns>Arreglo de boleanos que sera false cuando este disponible ese horario</returns>
        public bool[] EnHora_Bool(int hora)
        {
            bool[] res = { false, false, false, false, false, false };

            foreach (Grupo g in grupos)
                for (int i = 0; i < 6; i++)
                    if (g.horario_ini[i] >= hora && hora + 1 >= g.horario_fin[i])
                        res[i] = true;

            return res;
        }

        public void Remove(Grupo grupo)
        {
            grupos.Remove(grupo);
        }

        public void Add(Grupo grupo)
        {
            grupos.Add(grupo);
        }

        public override string ToString()
        {
            string cad = "";
            for (int i = 0; i < Count(); i++)
                cad+=Get(i).ToString()+"\n";
            return base.ToString();
        }
        #endregion

        #region Consultas Grupos

        /// <summary>
        /// Checa los grupos que estan por asignar en el horario y dias marcados
        /// </summary>
        /// <param name="dias">Cadena de 6 caracteres conformada por 0 y 1 empezando del Lunes a Sabado</param>
        /// <param name="hora">Hora en la que se buscaran los grupos sin asignar</param>
        /// <param name="ciclo">Ciclo escolar a checar</param>
        /// <returns></returns>
        public ListaGrupos SinAsignar(string dias, int hora)
        {
            ListaGrupos res;

            var query = from Grupo g in grupos
                        where g.empalme(hora, hora + 1, dias) && (g.Salon=="" || g.Salon==null || g.Salon == " ")
                        select g;

            res = new ListaGrupos(query.ToList<Grupo>());
            return res;
        }

        /// <summary>
        /// Checa los grupos que estan por asignar en el horario y dias marcados
        /// </summary>
        /// <param name="dias">Cadena de 6 caracteres conformada por 0 y 1 empezando del Lunes a Sabado</param>
        /// <param name="hora">Hora en la que se buscaran los grupos sin asignar</param>
        /// <param name="ciclo">Ciclo escolar a checar</param>
        /// <returns></returns>
        public ListaGrupos Asignados(string dias, int hora)
        {
            ListaGrupos res;

            var query = from Grupo g in grupos
                        where g.empalme(hora, hora + 1, dias) && (g.Salon != "" || g.Salon != null || g.Salon != " ")
                        select g;

            res = new ListaGrupos(query.ToList<Grupo>());
            return res;
        }

        public ListaGrupos EnSalon(string salon)
        {
            ListaGrupos res;

            var query = from Grupo g in grupos
                        where g.Salon == salon
                        select g;

            res = new ListaGrupos(query.ToList<Grupo>());
            return res;
        }

        public ListaGrupos Empalmados()
        {
            var query = from g in grupos
                        from g1 in grupos
                        where g.empalme(g1)
                        select g;

            return new ListaGrupos(query.Distinct().ToList());
        }

        public ListaGrupos Empalmados(Grupo grupo)
        {
            var query = from g in grupos
                        where g.empalme(grupo)
                        select g;

            return new ListaGrupos(query.ToList());
        }

        public ListaGrupos EnHora(int hora_ini, int hora_fin, string salon, string dias)
        {
            var query = from Grupo g in this.grupos
                        where g.empalme(hora_ini, hora_fin, dias) && g.Salon == salon
                        select g;

            return new ListaGrupos(query.ToList());
        }

        public ListaGrupos NoEn(List<Grupo> grupos)
        {
            var query = from Grupo g in this.grupos
                        where !grupos.Contains(g)
                        select g;

            return new ListaGrupos(query.ToList());
        }

        public List<ListaGrupos> PorHorario()
        {
            List<ListaGrupos> res = new List<ListaGrupos>();

            var query = from Grupo g in grupos
                        group g by g.Salon into horarioSalon
                        select horarioSalon;

            foreach (var lg in query)
                res.Add( new ListaGrupos(lg.ToList<Grupo>()));

            return res;
        }

        public List<ListaGrupos> Empalmes()
        {
            List<ListaGrupos> res = new List<ListaGrupos>();
            ListaGrupos aux;

            foreach (ListaGrupos lg in PorHorario())
            {
                aux = lg.Empalmados();

                if (aux.Count()!=0)
                    res.Add(aux);
            }

            return res;
        }

        public ListaGrupos ConProfesor(string rpe)
        {
            var query = from Grupo g in grupos
                        where g.RPE == Convert.ToInt32(rpe)
                        select g;

            return new ListaGrupos(query.ToList<Grupo>());
        }

        public ListaGrupos RequeirePlantaBaja()
        {
            var query = from Grupo g in grupos
                        where g.PlantaBaja
                        select g;

            return new ListaGrupos(query.ToList());
        }

        public ListaGrupos SalonFijo()
        {
            var query = from Grupo g in grupos
                        where g.Salon_fijo != ""
                        select g;

            return new ListaGrupos(query.ToList());
        }
        #endregion

        #region Consultas Salones
        /// <summary>
        /// busca salon a salon los que esten ocupados entre las horas designadas y los dias
        /// </summary>
        /// <param name="salones">Grupo de salones validos para checar</param>
        /// <param name="ini">hora inicial para el rango de horas</param>
        /// <param name="fin">hora final para el rango de horas</param>
        /// <param name="dias">dias que se van a buscar. L-M-Mi-J-V-S Marcar con un 1 los dias que quieres obtener</param>
        /// <returns></returns>
        public ListaSalones Disponibles(ListaSalones salones, int ini, int fin, string dias = "111111")
        {
            List<Salon> res = new List<Salon>();
            ListaGrupos auxG;
            Salon s;

            for(int i = 0; i<salones.Count;i++)
            {
                s = salones.Get(i);
                auxG = EnHora(ini, fin, s.Cve_espacio, dias);
                if (auxG.Count() == 0)
                    res.Add(s);
            }

            return new ListaSalones(res);
        }
        #endregion

        #region Chequeos
        public bool HayEmpalme(int[] hora_ini, int[] hora_fin)
        {
            var query = from g in grupos
                        where g.empalme(hora_ini, hora_fin)
                        select g;

            return query.ToList().Count == 0 ? true : false;
        }

        public bool HayEmpalme(int hora_ini, int hora_fin, string dias)
        {
            var query = from g in grupos
                        where g.empalme(hora_ini, hora_fin, dias)
                        select g;

            return query.ToList().Count == 0 ? true : false;
        }
        #endregion
    }
}
