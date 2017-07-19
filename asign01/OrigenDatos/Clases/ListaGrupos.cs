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
        protected IList<Profesor> profesores;
        protected IList<Materia> materias;

        public IList<Profesor> Profesores { get { return profesores; } }
        public IList<Materia> Materias { get { return materias; } }

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

        public ListaGrupos(Conexion c, DataTable dtGrupos, IList<Salon> salones)
        {
            grupos = new List<Grupo>();

            foreach (DataRow r in dtGrupos.Rows)
                grupos.Add(new Grupo(r, c.DGrupos,null,c,salones));
        }

        public ListaGrupos(IList<Grupo> grupos, IList<Profesor> profesores = null, IList<Materia> materia = null)
        {
            this.profesores = profesores;
            this.materias = materia;
            SetGrupos(grupos);
        }

        public ListaGrupos(IList<Grupo> grupos, IList<Materia> materias, IList<Profesor> profesores, Conexion c = null, ListaSalones salones = null) : base()
        {
            this.materias = materias;
            this.profesores = profesores;
            SetGrupos(grupos,c,salones);
        }

        public void SetGrupos(IList<Grupo> grupos, Conexion c=null, IList<Salon> salones=null)
        {
            this.grupos = new List<Grupo>();

            foreach (Grupo g in grupos)
                if(c!=null && salones!=null)
                    this.grupos.Add(new Grupo(g, c, salones));
                else
                    this.grupos.Add(new Grupo(g));
        }

        #endregion

        #region Basicos

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

        public ListaGrupos SinAsignar()
        {
            var query = from g in grupos
                        where g.Cve_espacio == "" || g.Cve_espacio == null
                        select g;

            return new ListaGrupos(query.ToList());
        }

        public ListaGrupos Asignados()
        {
            var query = from g in grupos
                        where g.Cve_espacio != "" || g.Cve_espacio != null
                        select g;

            return new ListaGrupos(query.ToList());
        }

        public ListaGrupos EnSalon(string salon)
        {
            var query = from g in grupos
                        where g.Cve_espacio == salon
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

        public ListaGrupos NoEn(IList<Salon> salones)
        {
            var query = from Grupo g in this.grupos
                        from Salon s in salones
                        where s.Cve_espacio == g.Cve_espacio
                        select g;

            return this.NoEn(query.Distinct().ToList());
        }

        public ListaGrupos ConProfesor(string rpe)
        {
            var query = from Grupo g in grupos
                        where g.RPE == Convert.ToInt32(rpe)
                        select g;

            return new ListaGrupos(query.ToList<Grupo>());
        }

        public ListaGrupos DeMateria(string cve)
        {
            var query = from g in this
                        where g.Cve_materia == cve
                        select g;

            return new ListaGrupos(query.ToList(), profesores, materias);
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

        /// <summary>
        /// Obtiene los Grupos que requieran un salon en especifico
        /// </summary>
        /// <returns></returns>
        public ListaGrupos EnSalonesFijos()
        {
            var query = from g in grupos
                        where g.Salon_fijo == g.Cve_espacio
                        select (Grupo)g;

            return new ListaGrupos(query.ToList());
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

        public List<ListaGrupos> EnSalones(ListaSalones salones)
        {
                var query = from sal in salones
                            from g in this
                            where sal.Cve_espacio == g.Cve_espacio
                            group g by g.Cve_espacio into gs
                            select new ListaGrupos(gs.ToList());

            return query.ToList();
        }

        public ListaGrupos NoRepetidos()
        {
            var query = from g in this
                        select g;

            return new ListaGrupos(query.GroupBy(p => new { p.Cve_materia, p.num_Grupo, p.Ciclo }).Select(g => g.First()).ToList(), profesores, materias);
        }

        /// <summary>
        /// Busca la materia del grupo que se encuentra en el indice [index]
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Materia buscaMateria(int index)
        {
            return buscaMateria(this[index].Cve_materia);
        }

        public Materia buscaMateria(string cve_materia)
        {
            var query = from m in materias
                        where m.CVE == cve_materia
                        select m;

            if (query.Count() > 0)
                return new Materia(query.ToList()[0]);
            else
                return new Materia("-----", cve_materia, 0);
            //throw new Exception("No se encontro la materia. CVE="+cve_materia);
        }

        public Profesor buscaProfesor(string RPE)
        {
            int rpe = Convert.ToInt32(RPE);
            var query = from p in profesores
                        where p.RPE == rpe
                        select p;

            if (query.Count() > 0)
                return new Profesor(query.ToList()[0]);
            else
                return new Profesor(rpe);
            //throw new Exception("No se encontro el RPE");
        }

        /// <summary>
        /// Busca al profesor del grupo que se encuentra en el indice [index]
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Profesor buscaProfesor(int index)
        {
            Grupo g = (Grupo)this[index];

            return buscaProfesor(g.RPE.ToString());
        }

        public List<ListaGrupos> Agrupados_Salon()
        {
            var query = from g in grupos
                        group g by g.Cve_espacio into gs
                        select new ListaGrupos(gs.ToList());

            return query.ToList();
        }

        public ListaGrupos IniciaEnHora(int hora)
        {
            var query = from g in grupos
                        where g.hora_ini == hora
                        select g;

            return new ListaGrupos(query.ToList());
        }
    }
}
