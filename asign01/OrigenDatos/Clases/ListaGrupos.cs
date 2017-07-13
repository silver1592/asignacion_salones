using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrigenDatos.Clases
{
    public class ListaGrupos : IList<Grupo>
    {
        protected List<Grupo> grupos;

        #region IList
        int ICollection<Grupo>.Count
        {
            get
            {
                return ((IList<Grupo>)grupos).Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return ((IList<Grupo>)grupos).IsReadOnly;
            }
        }

        public Grupo this[int index]
        {
            get
            {
                return ((IList<Grupo>)grupos)[index];
            }

            set
            {
                ((IList<Grupo>)grupos)[index] = value;
            }
        }

        public int IndexOf(Grupo item)
        {
            return ((IList<Grupo>)grupos).IndexOf(item);
        }

        public void Insert(int index, Grupo item)
        {
            ((IList<Grupo>)grupos).Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            ((IList<Grupo>)grupos).RemoveAt(index);
        }

        public void Clear()
        {
            ((IList<Grupo>)grupos).Clear();
        }

        public bool Contains(Grupo item)
        {
            return ((IList<Grupo>)grupos).Contains(item);
        }

        public void CopyTo(Grupo[] array, int arrayIndex)
        {
            ((IList<Grupo>)grupos).CopyTo(array, arrayIndex);
        }

        bool ICollection<Grupo>.Remove(Grupo item)
        {
            return ((IList<Grupo>)grupos).Remove(item);
        }

        public IEnumerator<Grupo> GetEnumerator()
        {
            return ((IList<Grupo>)grupos).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<Grupo>)grupos).GetEnumerator();
        }

        public void Add(Grupo grupo)
        {
            grupos.Add(grupo);
        }
        #endregion

        #region Constructores e inicializadores
        public ListaGrupos()
        {
            grupos = new List<Grupo>();
        }

        public ListaGrupos(IList<Grupo> grupos)
        {
            this.grupos = new List<Grupo>();
            foreach(Grupo g in grupos)
            {
                this.grupos.Add(new Grupo(g));
            }
        }

        public ListaGrupos(Conexion c, DataTable dtGrupos, IList<Salon> salones)
        {
            grupos = new List<Grupo>();

            foreach (DataRow r in dtGrupos.Rows)
                grupos.Add(new Grupo(r, c.DGrupos,null,c,salones));
        }

        public void SetGrupos(List<Grupo> grupos)
        {
            this.grupos = grupos;
        }

        #endregion

        #region Basicos
        /// <summary>
        /// Checa si los grupos estan asignados a cierta hora
        /// </summary>
        /// <param name="hora">Hora a checar</param>
        /// <returns>Arreglo de boleanos que sera false cuando este disponible ese horario</returns>
        public bool[] Dias(int hora)
        {
            bool[] res = { false, false, false, false, false, false };

            foreach (Grupo g in grupos)
                for (int i = 0; i < 6; i++)
                    if (g.horario_ini[i] >= hora && hora + 1 >= g.horario_fin[i])
                        res[i] = true;

            return res;
        }

        public override string ToString()
        {
            string cad = "";
            foreach(Grupo g in this)
                cad+=g.ToString()+"\n";
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
                        where g.EnHora(hora, hora + 1, dias) && (g.Salon=="" || g.Salon==null || g.Salon == " ")
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
                        where g.EnHora(hora, hora + 1, dias) && (g.Salon != "" || g.Salon != null || g.Salon != " ")
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
                        where g.EnHora(hora_ini, hora_fin, dias) && g.Salon == salon
                        select g;

            return new ListaGrupos(query.ToList());
        }

        public ListaGrupos EnHora(int hora_ini, int hora_fin)
        {
            var query = from Grupo g in this.grupos
                        where g.EnHora(hora_ini, hora_fin)
                        select g;

            return new ListaGrupos(query.ToList());
        }

        public ListaGrupos EnHora(int[] hora_ini, int[] hora_fin)
        {
            var query = from g in grupos
                        where g.EnHora(hora_ini, hora_fin)
                        select g;

            return new ListaGrupos(query.ToList());
        }

        public ListaGrupos EnHora(int hora_ini, int hora_fin, string dias)
        {
            var query = from g in grupos
                        where g.EnHora(hora_ini, hora_fin, dias)
                        select g;

            return new ListaGrupos(query.ToList());
        }

        public ListaGrupos NoEn(IList<Grupo> grupos)
        {
            var query = from Grupo g in this.grupos
                        where !grupos.Contains(g)
                        select g;

            return new ListaGrupos(query.ToList());
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
        public ListaSalones Ocupados(ListaSalones salones, int ini, int fin, string dias = "111111")
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

        #region _Algoritmo
        protected IList<Profesor> profesores;
        protected IList<Materia> materias;

        public IList<Profesor> Profesores { get { return profesores; } }
        public IList<Materia> Materias { get { return materias; } }

        #region Constuctores
        public ListaGrupos(IList<Grupo> grupos, IList<Profesor> profesores = null, IList<Materia> materia = null)
        {
            this.profesores = profesores;
            this.materias = materia;
            this.grupos = new List<Grupo>();
            foreach (Grupo g in grupos)
                this.grupos.Add(new Grupo(g));
        }

        public void Actualiza(ListaGrupos _grupos)
        {
            if (this == _grupos)
                return;

            Grupo temp;
            int index = 0;

            foreach (Grupo g in _grupos)
            {
                temp = Busca(g.Cve_materia, g.num_Grupo);
                index = IndexOf(temp);
                grupos[index] = g;
            }
        }

        public ListaGrupos(IList<Grupo> grupos, List<Materia> materias, List<Profesor> profesores, Conexion c = null, ListaSalones salones = null) : base()
        {
            this.materias = materias;
            this.profesores = profesores;

            this.grupos = new List<Grupo>();

            foreach (Grupo g in grupos)
                this.grupos.Add(new Grupo(g, c, salones));
        }
        #endregion

        #region consultas
        /// <summary>
        /// Obtiene los Grupos que requieran un salon en especifico
        /// </summary>
        /// <returns></returns>
        public ListaGrupos EnSalonesFijos()
        {
            var query = from g in grupos
                        where g.Salon_fijo == g.Salon
                        select (Grupo)g;

            return new ListaGrupos(query.ToList());
        }

        /// <summary>
        /// Obtiene los grupos con mejor puntiacion con cierto salon
        /// </summary>
        /// <param name="s"></param>
        /// <param name="limite"></param>
        /// <returns></returns>
        public ListaGrupos MejorPuntuacion(Salon s, int limite = 1)
        {
            var query = from g in grupos
                        where g.SalonValido(s) > 0
                        orderby g.SalonValido(s)
                        select (Grupo)g;

            return new ListaGrupos(query.Take(limite).ToList());
        }

        /// <summary>
        /// Obtiene los grupos que requieren planta baja
        /// </summary>
        /// <returns></returns>
        public ListaGrupos QuierenPlantabaja()
        {
            var query = from g in grupos
                        where g.PlantaBaja
                        select (Grupo)g;

            return new ListaGrupos(query.ToList());
        }

        /// <summary>
        /// Obtiene los grupos que han estado en el salon en otros semestres
        /// </summary>
        /// <param name="salon"></param>
        /// <returns></returns>
        public ListaGrupos AsignacionOtrosSemestres(string salon)
        {
            var query = from g in grupos
                        where g.AsignacionSemestresAnteriores(salon) != null
                        select (Grupo)g;

            return new ListaGrupos(query.ToList());
        }

        /// <summary>
        /// Obtiene los grupos que inicial a cierta hora
        /// </summary>
        /// <param name="hola"></param>
        /// <returns></returns>
        public ListaGrupos InicialALas(int hora)
        {
            var query = from g in this
                        where g.hora_ini == hora
                        select g;

            return new ListaGrupos(query.ToList());
        }

        /// <summary>
        /// Obtiene el grupo que coincida
        /// </summary>
        /// <param name="cve_materia"></param>
        /// <param name="num_Grupo"></param>
        /// <returns></returns>
        public Grupo Busca(string cve_materia, int num_Grupo)
        {
            var query = from g in grupos
                        where g.Cve_materia == cve_materia && g.num_Grupo == num_Grupo
                        select g;

            if (query.Count() != 0)
                return (Grupo)query.ToList()[0];

            return null;
        }

        /// <summary>
        /// Obtiene el grupo que tenga mas puntos para el salon
        /// </summary>
        /// <param name="salon"></param>
        /// <returns></returns>
        public Grupo MejorPara(Salon salon)
        {
            ListaGrupos aux = this;

            if (salon.plantaBaja)
                aux = aux.QuierenPlantabaja();

            //Salon de otros semestres
            if (aux.Count() > 1)
                aux = aux.AsignacionOtrosSemestres(salon.Cve_espacio);

            //Mejor puntuacion de equipamiento
            if (aux.Count() > 1)
                aux = aux.MejorPuntuacion(salon);

            return aux.Count() != 0 ? (Grupo)aux[0] : null;
        }
        #endregion

        #region Operaciones
        public void Update(Grupo g)
        {
            Grupo gAux = Busca(g.Cve_materia, g.num_Grupo);

            if (gAux != null)
            {
                grupos.Remove(gAux);
                grupos.Add(g);
            }
            else
            {
                throw new Exception("Error al actualizar un grupo. El grupo solicitado no se encuentra (A01-H-LG-UPDATE)");
            }
        }

        public void Update(ListaGrupos g)
        {
            foreach (Grupo grupo in grupos)
                Update(grupo);
        }
        #endregion

        public bool EnSalones(ListaSalones salones, Grupo grupo)
        {
            foreach (Grupo g in grupos)
            {
                var query = from Salon sal in salones
                            where sal.Cve_espacio == g.Salon && grupo.SalonValido(sal) > 0 && sal.Disponible_para_grupo(grupo)
                            select sal;

                if (query.Count() != 0)
                    return true;
            }

            return false;
        }
        #endregion
    }
}
