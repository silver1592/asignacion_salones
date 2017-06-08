using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;

namespace OrigenDatos.Clases
{
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
                return gruposAsignados.EnHora_Bool(hora);
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
                    p += gruposAsignados.Get(i).SalonValido(this);

                return p;
            }
        }
        #endregion

        #region Constructores
        /// <summary>
        /// Constructor por copia </br>
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

                if (Equipo != null)
                    SetEquipo(Equipo);
                
                if(AreaEdif != null)
                    SetAreaEdificio(AreaEdif);

                if(excep!=null)
                    SetExcepciones(excep);

                gruposAsignados = new ListaGrupos();
            }
            else
                throw new Exception("Datos del salon no validos");
        }

        #region Inicializadores
        public void SetEquipo(DataTable Equipo)
        {
            //DataTable Equipo = Consultas.Salon_equipo(cve_espacio);

            equipo_instalado = new List<int>();
            foreach (DataRow equipo in Equipo.Rows)
                equipo_instalado.Add(Convert.ToInt32(equipo["cve_equipo"].ToString()));
        }

        public void SetAreaEdificio(DataTable AreaEdif)
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

        public void SetExcepciones(DataTable excep)
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

        public bool Disponible(int hora_ini, int hora_fin, string dias)
        {

            return !gruposAsignados.HayEmpalme(hora_ini, hora_fin, dias);
        }

        public bool Disponible(int[] hora_ini, int[] hora_fin)
        {
            return !gruposAsignados.HayEmpalme(hora_ini, hora_fin);
        }

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

        /// <summary>
        /// Agrega un grupo al salon para seguir su horario y elimina todos los grupos con los que queda empalmado
        /// </summary>
        /// <param name="grupo">Grupo a agregar</param>
        public void agregaGrupo(Grupo grupo)
        {
            var grupos = gruposAsignados.Empalmados(grupo);

            for(int i = 0; i<grupos.Count();i++)
                remueveGrupo(grupos.Get(i));

            gruposAsignados.Add(grupo);
        }

        /// <summary>
        /// Elimina un grupo de la lista de grupos
        /// (Nota: tiene que ser el mismo objeto)
        /// </summary>
        /// <param name="grupo">Grupo a eliminar</param>
        public void remueveGrupo(Grupo grupo)
        {
            gruposAsignados.Remove(grupo);
            grupo.Salon = "";
        }

        /// <summary>
        /// Checa y obtiene una lista con los grupos con los que tiene conflicto de empalmes el grupo pasado por parametro
        /// </summary>
        /// <param name="grupo">Grupo a checar si hay empalme</param>
        /// <returns></returns>
        public ListaGrupos EmpalmesCon(Grupo grupo)
        {
            return gruposAsignados.Empalmados(grupo);
        }

        public override string ToString()
        {
            return cve_espacio;
        }

        #region Static
        public static Salon ToSalon(DataRow datos,int hora,Conexion c)
        {
            Salon aux;

            aux = new Salon(datos, hora);
            aux.SetEquipo(c.Salon_equipo(aux.cve_espacio));
            aux.SetAreaEdificio(c.Edificio_Area(aux.cve_edificio));
            aux.SetExcepciones(c.Exepciones(aux.cve_espacio));

            return aux;
        }
        #endregion
    }
}
