using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;

namespace OrigenDatos.Clases
{
    /// <summary>
    /// Contiene la informacion de un salon
    /// </summary>
    public class Salon
    {
        #region atributos <protected>
        protected string cve_espacio; //cve_espacio varchar(60) 
        protected string cve_edificio;//cve_edificio    varchar(20) 
        protected string cve_salon;//cve_salon   varchar(20) 
        protected string cve_sub_salon;//cve_sub_salon   varchar(20) 
        protected string cve_tipo_espacio;//cve_tipo_espacio    int 
        protected int cupo_max;//cupo_max int 
        protected string cve_ubicacion;//cve_ubicacion varchar(30) 

        protected List<int> equipo_instalado;
        protected List<string> area;
        protected int[] val;

        protected bool asignable;
        protected bool empalmes;
        protected ListaGrupos gruposAsignados;

        #endregion

        #region atributos <public>
        public int hora;
        public bool Asignable { get { return asignable; } }
        public bool empalme { get { return empalmes; } }
        public List<int> Equipo { get { return equipo_instalado; } }
        public bool plantaBaja
        {
            get
            {
                string salon = cve_espacio.Split('-')[1];

                try
                {
                    if (Convert.ToInt32(salon) < 10)
                        return true;
                }
                catch { }

                return false;
            }
        }
        public string Cve_espacio { get { return cve_espacio; } }
        public string Edificio { get { return cve_edificio; } }
        public List<string> Area { get { return area; } }
        public int Cupo { get { return cupo_max; } }

        /// <summary>
        /// Marca con True cuando ya ahy una materia que lo ocupa
        /// Este solo es en base al horario marcado por la hora
        /// </summary>
        public bool[] horario
        {
            get
            {
                return gruposAsignados.Dias(hora);
            }
        }
        public string dias
        {
            get
            {
                string res="";

                if (horario[0] == false)
                    res += "L";
                else
                    res += "-";

                if (horario[1] == false)
                    res += "M";
                else
                    res += "-";

                if (horario[2] == false)
                    res += "m";
                else
                    res += "-";

                if (horario[3] == false)
                    res += "J";
                else
                    res += "-";

                if (horario[4] == false)
                    res += "V";
                else
                    res += "-";

                if (horario[5] == false)
                    res += "S";
                else
                    res += "-";

                return res;
            }
        }
        public float puntos
        {
            get
            {
                float p = 0;

                for(int i = 0;i<gruposAsignados.Count();i++)
                    p += gruposAsignados[i].SalonValido(this);

                return p;
            }
        }
        #endregion

        #region Constructores
        /// <summary>
        /// Constructor por copia 
        /// (Solo usarlo para asignarlo a las variables del algoritmo)
        /// </summary>
        /// <param name="s">Salon a copiar</param>
        public Salon(Salon s)
        {
            #region detalles del salon

            cve_espacio = s.cve_espacio;
            cve_edificio = s.cve_edificio;
            cve_salon = s.cve_salon;
            cve_sub_salon = s.cve_sub_salon;
            cve_tipo_espacio = s.cve_tipo_espacio;
            cupo_max = s.cupo_max;
            cve_ubicacion = s.cve_ubicacion;

            equipo_instalado = new List<int>();
            foreach (int t in s.equipo_instalado)
                equipo_instalado.Add(t);
            #endregion

            #region Edificio X Area
            val = s.val;
            area = s.area;
            #endregion

            #region exepciones
            
            asignable = s.asignable;
            empalmes = s.empalme;
            #endregion

            #region Horarios
            hora = s.hora;
            gruposAsignados = new ListaGrupos(s.gruposAsignados);
            #endregion
        }

        public Salon(DataRow salon, int hora, DataTable excep=null, DataTable Equipo=null, DataTable AreaEdif=null)
        {
            ///Tabla Horario
            this.hora = hora;

            SetValues(salon);

            if (Equipo != null)
                SetEquipo(Equipo);

            if (AreaEdif != null)
                SetAreaEdificio(AreaEdif);

            if (excep != null)
                SetExcepciones(excep);
        }

        public Salon(DataRow datos, int hora, Conexion c)
        {

            ///Tabla Horario
            this.hora = hora;
            SetValues(datos);

            SetEquipo(c.Salones_Salon_equipo(cve_espacio));
            SetAreaEdificio(c.Salones_Edificio_Area(cve_edificio));
            SetExcepciones(c.Salones_Exepciones(cve_espacio));
        }

        #region Inicializadores
        protected void SetValues(DataRow salon)
        {
            if (salon != null)
            {
                //DataTable salon = Consultas.Salon(cve_espacio);
                this.cve_espacio = salon["cve_espacio"].ToString();
                cve_edificio = salon["cve_edificio"].ToString();
                cve_salon = salon["cve_salon"].ToString();
                cve_sub_salon = salon["cve_sub_salon"].ToString();
                cve_tipo_espacio = salon["cve_tipo_espacio"].ToString();
                cupo_max = Convert.ToInt32(salon["cupo_max"].ToString());
                cve_ubicacion = salon["cve_ubicacion"].ToString();

                gruposAsignados = new ListaGrupos();
            }
            else
                throw new Exception("Datos del salon no validos");
        }

        protected void SetEquipo(DataTable Equipo)
        {
            //DataTable Equipo = Consultas.Salon_equipo(cve_espacio);

            equipo_instalado = new List<int>();
            foreach (DataRow equipo in Equipo.Rows)
                equipo_instalado.Add(Convert.ToInt32(equipo["cve_equipo"].ToString()));
        }

        protected void SetAreaEdificio(DataTable AreaEdif)
        {
            //DataTable AreaEdif = Consultas.Edificio_Area(cve_edificio);
            int i = 0;
            int e = 0;
            if (AreaEdif.Rows.Count > 0)
                val = new int[AreaEdif.Rows.Count];
            area = new List<string>();
            foreach (DataRow rowA in AreaEdif.Rows)
            {
                if (!area.Contains(AreaEdif.Rows[e]["idArea"].ToString()))
                {
                    area.Add(AreaEdif.Rows[e]["idArea"].ToString());
                    val[i] = Convert.ToInt32(AreaEdif.Rows[e]["importancia"].ToString());
                    i++;
                }
                e++;
            }
        }

        protected void SetExcepciones(DataTable excep)
        {
            //DataTable excep = Consultas.Exepciones(cve_espacio);
            if (excep.Rows.Count == 1)
            {
                asignable = !Convert.ToBoolean(excep.Rows[0]["no_asignable"]);
                empalmes = Convert.ToBoolean(excep.Rows[0]["empalme"]);
            }
            else
            {
                asignable = true;
                empalmes = false;
            }
        }

        #endregion
        #endregion

        public bool ContieneEquipo(int idEquipo)
        {
            foreach (int i in Equipo)
                if (i == idEquipo)
                    return true;

            return false;
        }

        /// <summary>
        /// Obtiene el valor que tiene el salon para el area
        /// </summary>
        /// <param name="area">Numero del area a checar</param>
        /// <returns></returns>
        public int PrioridadArea(string area)
        {
            for (int i = 0; i < this.area.Count; i++)
            {
                if (this.area[i] == area)
                    return val[i];
            }

            return -1;

        }

        public override string ToString()
        {
            return cve_espacio;
        }

        #region _Algoritmo

        /// <summary>
        /// Checa si hay horario y si cabe para el grupo que se le pasa por parametro.
        /// </summary>
        /// <param name="grupo"></param>
        /// <returns></returns>
        public bool Disponible_para_grupo(Grupo grupo)
        {
            for (int i = 0; i < 6; i++)
                if (horario[i] && grupo.dias(hora)[i] || grupo.Cupo > Cupo)
                    return false;

            return true;
        }

        /// <summary>
        /// Puntos que otorga al Grupo
        /// -corregir-
        /// que la salida sea una estructura de multiples puntos
        /// </summary>
        /// <param name="grupo"></param>
        /// <returns></returns>
        public float puntosCon(Grupo grupo)
        {
            float p = 0;

            if (gruposAsignados.Empalmados(grupo).Count() == 0)
                p += grupo.SalonValido(this);
            else
                p = -1;

            return p;
        }

        /// <summary>
        /// Busca los grupos dentro de una lista que esten asignados en el salon y los almacena 
        /// para poder generar un horario de un salon
        /// </summary>
        /// <param name="hora">Solo se genera el horario por hora</param>
        /// <param name="ciclo">Ciclo escolar a generar el horario</param>
        public void ObtenHorario(List<Grupo> grupos)
        {
            var query = from Grupo g in grupos
                        where g.Salon == Cve_espacio
                        select g;

            foreach (Grupo g in query.ToList<Grupo>())
                gruposAsignados.Add(g);
        }
        #endregion
    }
}
