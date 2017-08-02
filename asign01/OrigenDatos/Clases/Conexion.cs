using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace OrigenDatos.Clases
{
    /// <summary>
    /// Clase encargada de la conexion con la base de datos
    /// </summary>
    /// <remarks>
    /// Esta clase esta echa administrar todas las conexiones a 
    /// las bases de datos, ya sean en distiontos manejadores o 
    /// los excel o lo que fuere.
    /// Aqui van todas las consultas SQL.
    /// Tambien si se quieren manejar bases de datos distribuidas
    /// tiene que hacerse desde aqui.
    /// </remarks>
    public class Conexion
    {
        #region estaticos
        public static string datosConexion
        {
            get
            {
                string dir = @"(localdb)\asignacion;";
                string database = "asignacion";
                string datosConexion = "Data Source=" + dir
                                        + "Initial Catalog=" + database + ";"
                                        + "Integrated Security = false;";

                //string dir = @"148.224.93.146\FINGENIERIA,2433;"; //servidor de la escuela
                //string usuario = "asignacion";
                //string pass = "Asigna#2016Ing";
                //Datos de la coneccion
                /*
                string datosConexion = "Data Source=" + dir
                                        + "Initial Catalog="+database+";"
                                        + "Integrated Security =false;"
                                        + "Uid = " + usuario + ";"
                                        + "Pwd= " + pass + ";";
                */
                return datosConexion;
            }
        } // Contiene la informacion para la conexion con la base de datos sql

        public static Dictionary<string, string> DGruposBD
        {
            get
            {
                Dictionary<string, string> headers = new Dictionary<string, string>();
                headers.Add("cve_mat", "cve_materia");
                headers.Add("cve_gpo", "grupo");

                headers.Add("cve", "cve");

                headers.Add("cverpe", "rpe");
                headers.Add("tipo", "tipo");        //*
                headers.Add("salon", "salon");      //*
                headers.Add("lunes", "lunes_ini");
                headers.Add("lunesf", "lunes_fin");
                headers.Add("martes", "martes_ini");
                headers.Add("martesf", "martes_fin");
                headers.Add("miercoles", "miercoles_ini");
                headers.Add("miercolesf", "miercoles_fin");
                headers.Add("jueves", "jueves_ini");
                headers.Add("juevesf", "jueves_fin");
                headers.Add("viernes", "viernes_ini");
                headers.Add("viernesf", "viernes_fin");
                headers.Add("sabado", "sabado_ini");
                headers.Add("sabadof", "sabado_fin");
                headers.Add("cupo", "cupo");
                headers.Add("ciclo", "ciclo");      //*

                //Valores default
                headers.Add("cicloDefault", "");
                headers.Add("tipoDefault", "T");
                headers.Add("salonDefault", "");

                return headers;
            }
        } // Diccionario para leer la informacion de la base de datos

        public Grupo Grupo(string cve_full, string semestre)
        {
            string query = "select * from asignacion.ae_horario where cve_materia*100+grupo =" + cve_full + " and ciclo='" + semestre+"'";

            DataTable dt = Querry(query);

            if (dt.Rows.Count == 1)
                return new Grupo(dt.Rows[0], DGruposBD, null, this, null);

            return null;
        }
        #endregion

        #region Atributos, Get y Set
        protected string DatosConexion; // Informacion de la conexion del objeto
        protected LibroExcel Excel; //Excel Utilizado
        protected string hoja; // Hoja de excel a utilizar
        public string Sheet { set { hoja = value; } } // Asigna la hoja a utilizar
        public string[] Sheets
        {
            get
            {
                if (Excel != null)
                    return Excel.GetStringSheets();
                else
                    return new string[0];
            }
        } // Nombre de las Hojas disponibles

        #region Diccionarios
        public Dictionary<string, string> DGruposExcel { get { return Excel.dHeaders; } } // Diccionario para leer la informacion del excel

        public Dictionary<string,string> DGrupos { get { return Excel != null ? Excel.dHeaders : DGruposBD; } } // Selecciona un Diccionario para utilizarlo con la conexion actual
        #endregion
        #endregion

        #region Constructores
        public Conexion()
        {
            DatosConexion = Conexion.datosConexion;
        }// Conexion por default

        public Conexion(string Datos)
        {
            DatosConexion = Datos;
        } //Conexion por cadena

        /// <summary>
        /// Constructor completo
        /// </summary>
        /// <param name="Datos">Datos de conexion</param>
        /// <param name="excelDireccion">Direccion del archivo de excel</param>
        /// <param name="ciclo">Valor por default si el excel no trae ciclo</param>
        /// <param name="tipo">Valor por default si el excel no trae tipo</param>
        public Conexion(string Datos, string excelDireccion, string ciclo="2016-2017/I", string tipo="T")
        {
            DatosConexion = Datos;

            Excel = new LibroExcel(excelDireccion, ciclo, tipo);
        }
        #endregion

        #region Base
        /// <summary>
        /// Checa si la conexion es correcta
        /// </summary>
        /// <returns></returns>
        public bool Autenticacion()
        {
            SqlConnection con = new SqlConnection();
            con.ConnectionString = DatosConexion;

            try
            {
                con.Open();
                con.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Ejecuta una consulta en SQL utilizando el comando enviado como parametro
        /// Usado principalmente para consultas DLL (Data Definition Language)
        /// </summary>
        /// <param name="textoCmd"></param>
        public void Comando(string textoCmd)
        {
            SqlCommand cmd;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = DatosConexion;
            cmd = new SqlCommand(textoCmd, con);

            try
            {
                con.Open();

                cmd.ExecuteNonQuery();

                con.Close();
            }
            catch (Exception ex)
            {
                //Log.Add("Error al Ejecutar la consulta: \r\n\n"+textoCmd+"\n\t" + ex.Message + "\r\n\n");
                //MessageBox.Show("Error al Ejecutar la consulta: \r\n\n" + textoCmd + "\n\t" + ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// Manda un comando a la base de datos y regresa el DataTable como resultado
        /// </summary>
        /// <param name="textoCmd"> Query a ejecutar</param>
        /// <returns></returns>
        public DataTable Querry(string textoCmd)
        {
            SqlCommand cmd;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = DatosConexion;
            cmd = new SqlCommand(textoCmd, con);
            DataTable datos = null;

            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                datos = new DataTable();
                adapter.Fill(datos);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return datos;
        }
        #endregion

        #region Salon y Salones
        /// <summary>
        /// Muestra los salones registrados en la base de datos
        /// </summary>
        /// <returns></returns>
        public DataTable Salones()
        {
            string textoCmd = "SELECT * "
                              + "FROM [asignacion].[ae_cat_espacio] "
                              + "where not(cve_edificio='F') and not(cve_edificio='P') and not(cve_edificio='ZP')";


            DataTable datos = Querry(textoCmd);

            return datos;
        }

        /// <summary>
        /// Obtiene la informacion de los salones que tienen un trato especial
        /// </summary>
        /// <param name="cve_espacio"></param>
        /// <returns></returns>
        public DataTable Salones_Exepciones(string cve_espacio)
        {
            string textoCmd = "SELECT * FROM [asignacion].[ae_excepciones] where cve_espacio='" + cve_espacio + "';";

            DataTable datos = Querry(textoCmd);

            return datos;
        }

        /// <summary>
        /// Obtiene el valor que tiene un edificio con respecto a una area
        /// </summary>
        /// <param name="cve_edificio">Clave del edificio a buscar</param>
        /// <returns></returns>
        public DataTable Salones_Edificio_Area(string cve_edificio)
        {
            string textoCmd = "SELECT * FROM[asignacion].[ae_area_edificio]  where idEdificio = '" + cve_edificio + "'";

            DataTable datos = Querry(textoCmd);

            return datos;
        }

        /// <summary>
        /// Obtiene de la base de datos el equipo instalado en el salon
        /// </summary>
        /// <param name="cve_espacio">Clave del salon</param>
        /// <returns></returns>
        public DataTable Salones_Salon_equipo(string cve_espacio)
        {
            string textoCmd = "SELECT * "
                              + "FROM [asignacion].[ae_equipamiento]"
                              + "where cve_espacio='" + cve_espacio + "'; ";

            DataTable datos = Querry(textoCmd);

            return datos;
        }

        /// <summary>
        /// Consulta para obtener los salones asignados en las materias
        /// </summary>
        /// <param name="cve_materia"></param>
        /// <returns></returns>
        public DataTable Salones_Posibles(string cve_materia)
        {
            string textoCmd = "SELECT [cve_mat],[cve_espacio] FROM[asignacion].[asignacion].[ae_PosiblesSalones] where cve_mat = " + cve_materia;

            DataTable datos = Querry(textoCmd);

            return datos;
        }
        #endregion

        #region Grupo y Grupos
        public ListaGrupos Grupos(string semestre, string salon)
        {
            ListaGrupos res = null;
            IList<Grupo> grupos;
            List<Materia> materias = Materias();
            List<Profesor> profesores = Profesores();

            DataTable dt = Querry("SELECT DISTINCT * FROM asignacion.ae_horario where ciclo = '" + semestre + "' and salon='" + salon + "'");

            grupos = Grupos_AsList(dt);
            res = new ListaGrupos(grupos, materias, profesores, this);

            return res;
        }

        /// <summary>
        /// Obtiene los grupos de la base de datos o del excel
        /// </summary>
        /// <param name="semestre">Semestre del cual se obtendran los grupos, si es excel los inicializara a ese semestre</param>
        /// <param name="ini"></param>
        /// <param name="fin"></param>
        /// <param name="bExcel"></param>
        /// <returns></returns>
        public ListaGrupos Grupos(string semestre, int ini = 7, int fin = 22, bool bExcel = true)
        {
            ListaGrupos res = null;
            IList<Grupo> grupos;
            List<Materia> materias = Materias();
            List<Profesor> profesores = Profesores();

            if (Excel == null || !bExcel)
            {
                DataTable dt = Querry("SELECT DISTINCT * FROM  [asignacion].[Grupos_a_las] (" + ini + "," + fin + ") where ciclo = '" + semestre + "'");

                grupos = Grupos_AsList(dt);
                res = new ListaGrupos(grupos, materias, profesores, this);
            }
            else
            {
                res = new ListaGrupos(Excel.GetGrupos(hoja, semestre, "T"), materias, profesores, this);
            }

            return res;
        }

        public ListaGrupos Grupos_Light(string semestre, int ini = 7, int fin = 22, bool bExcel = true)
        {
            ListaGrupos res = null;
            IList<Grupo> grupos;

            if (Excel == null || !bExcel)
            {
                DataTable dt = Querry("SELECT * FROM  [asignacion].[Grupos_a_las] (" + ini + "," + fin + ") where ciclo = '" + semestre + "'");

                grupos = Grupos_AsList(dt);
                res = new ListaGrupos(grupos);
            }
            else
            {
                res = new ListaGrupos(Excel.GetGrupos(hoja, semestre));
            }

            return res;
        }

        public ListaGrupos Grupos_EmpiezanA(string semestre, int ini, bool bExcel)
        {
            ListaGrupos res = null;
            IList<Grupo> grupos;
            List<Materia> materias = Materias();
            List<Profesor> profesores = Profesores();

            if (Excel == null || !bExcel)
            {
                DataTable dt = Querry("SELECT DISTINCT *  FROM ae_Grupos_ini (" + ini + ") where ciclo = '" + semestre + "'");

                grupos = Grupos_AsList(dt);
                res = new ListaGrupos(grupos, materias, profesores, this);
            }
            else
            {
                res = new ListaGrupos(Excel.GetGrupos(hoja, semestre), materias, profesores, this);
            }

            return res;
        }

        private IList<Grupo> Grupos_AsList(DataTable dt)
        {
            List<Grupo> g = new List<Grupo>();
            foreach (DataRow r in dt.Rows)
                g.Add(new Grupo(r, DGruposBD,null,this));

            return g;
        }

        /// <summary>
        /// Obtiene las necesidades de un grupo
        /// </summary>
        /// <param name="cve_materia">Clave de la materia</param>
        /// <param name="rpe">clave del profesor</param>
        /// <param name="tipo">tipo de claser (T/L)</param>
        /// <returns></returns>
        public DataTable Grupo_Necesidades(string cve_materia, string tipo, string rpe)
        {
            string textoCmd = "";

            if (cve_materia != "" && rpe != "")
                textoCmd = "SELECT idEquipo,Equipo, peso "
                              + "FROM  [asignacion].[ae_necesidades_curso] inner join asignacion.ae_cat_equipo on cve_equipo=idEquipo"
                              + " where cve_materia = '" + cve_materia + "' and tipo = '" + tipo + "' and rpe = " + rpe + "; ";
            else if (cve_materia != "" && rpe == "")
                textoCmd = "SELECT idEquipo,Equipo, peso "
                              + "FROM  [asignacion].[ae_necesidades_curso] inner join asignacion.ae_cat_equipo on cve_equipo=idEquipo"
                              + " where cve_materia = '" + cve_materia + "' and tipo = '" + tipo + "'; ";
            else if (cve_materia == "" && rpe != "")
                textoCmd = "SELECT idEquipo,Equipo, peso "
                              + "FROM  [asignacion].[ae_necesidades_curso] inner join asignacion.ae_cat_equipo on cve_equipo=idEquipo"
                              + " where tipo = '" + tipo + "' and rpe = " + rpe + "; ";

            DataTable datos = Querry(textoCmd);

            return datos;
        }

        /// <summary>
        /// Obtiene los grupos que se impartieron una hora antes por el profesor
        /// </summary>
        /// <param name="rpe">Clave unica del profesor</param>
        /// <param name="hora">Hora de la cual se quiere obtener la informacion</param>
        /// <param name="ciclo">Semestre</param>
        /// <returns>DataTable con los datos de los grupos impartidos</returns>
        public DataTable Grupos_HoraAnterior(int rpe, int hora, string ciclo)
        {
            string textoCmd = "select * "
                               + "from ae_Grupos_ini(" + (hora - 1) + ") "
                               + "where rpe = " + rpe + " and ciclo = '" + ciclo + "';";

            DataTable datos = Querry(textoCmd);

            return datos;
        }

        /// <summary>
        /// Obtiene Los grupos de semestres anteriores
        /// </summary>
        /// <param name="cve_materia">Clave de la materia</param>
        /// <param name="ciclo"></param>
        /// <param name="rpe"></param>
        /// <returns></returns>
        public DataTable Grupos_SemestresAnteriores(string cve_materia, string ciclo, string rpe)
        {
            string query = "select * from asignacion.ae_horario where not(ciclo = '" + ciclo + "') and rpe = '" + rpe + "' and cve_materia = '" + cve_materia + "'";
            DataTable dt = Querry(query);

            return dt;
        }

        public bool Grupo_Existe(Grupo g)
        {
            string query = "select * from asignacion.ae_horario where cve_materia=" + g.Cve_materia + " and grupo=" + g.num_Grupo + " and ciclo='" + g.Ciclo + "'";

            DataTable dt = Querry(query);

            return dt.Rows.Count != 0;
        }

        /// <summary>
        /// Actualiza la informacion en la base de datos
        /// </summary>
        /// <param name="grupos">Lista de grupos a escribir</param>
        /// <param name="hojaExcel">Hoja en la que se va a escribir(No importa si existe)</param>
        public void Grupos_Carga(ListaGrupos grupos, string hojaExcel = "resultado")
        {
            foreach (Grupo g in grupos)
                if (Grupo_Existe(g))
                    Comando(g.qUpdate);
                else
                    Comando(g.qInsert);

            if (Excel != null && hojaExcel != null)
                Excel.EscribeGrupos(grupos, hojaExcel);
        }
        #endregion

        #region Profesor y Profesores
        /// <summary>
        /// Obtiene las necesidades marcadas al profesor
        /// </summary>
        /// <param name="rpe">Profesor a buscar</param>
        /// <returns></returns>
        public DataTable Profesor_Necesidades(string rpe)
        {
            string textoCmd = "SELECT [rpe], [discapacidad], [salon_unico] FROM [asignacion].[asignacion].[ae_necesidad_profesor]"
                              + "where rpe=" + rpe + ";";

            DataTable datos = Querry(textoCmd);

            return datos;
        }

        public List<Profesor> Profesores()
        {
            List<Profesor> profesores = new List<Profesor>();
            try
            {
                DataTable dt = Querry("SELECT * FROM [asignacion].[vae_cat_profesor]");

                foreach (DataRow r in dt.Rows)
                    profesores.Add(new Profesor(r));

                return profesores;
            }
            catch (Exception ex) //para que no truene en las pruebas
            {
                return null;
            }
        }

        public Dictionary<int, string> Profesores_AsDicctionary()
        {
            Dictionary<int, string> profesores = new Dictionary<int, string>();
            DataTable dt = Querry("SELECT * FROM [asignacion].[vae_cat_profesor]");

            foreach (DataRow r in dt.Rows)
                profesores.Add(Convert.ToInt32(r["rpe"].ToString()), r["nombre"].ToString());

            return profesores;
        }
        #endregion

        #region Materias
        public List<Materia> Materias()
        {
            List<Materia> materias = new List<Materia>();
            try
            {
                DataTable dt = Querry("SELECT * FROM [asignacion].[vae_cat_materia]");

                foreach (DataRow r in dt.Rows)
                    materias.Add(new Materia(r));

                return materias;
            }
            catch(Exception ex) //para que no truene en las pruebas
            {
                return null;
            }
        }

        public Dictionary<string, string> Materias_AsDictionary()
        {
            Dictionary<string, string> materias = new Dictionary<string, string>();
            DataTable dt = Querry("SELECT * FROM [asignacion].[vae_cat_materia]");

            foreach (DataRow r in dt.Rows)
                materias.Add(r["cve_materia"].ToString(), r["materia"].ToString());

            return materias;
        }
        #endregion

        #region Semestre
        public string[] Semestres()
        {
            string query = "SELECT distinct ciclo FROM[asignacion].[ae_horario] order by ciclo desc";
            List<string> res = new List<string>();

            DataTable dt = Querry(query);

            foreach (DataRow r in dt.Rows)
                res.Add(r[0].ToString());

            return res.ToArray();
        }

        public bool Semestre_Valido(string semestre)
        {
            DataTable dt = Querry("select count(*) from asignacion.ae_horario where ciclo = '" + semestre + "'");
            if (Convert.ToInt32(dt.Rows[0][0].ToString()) == 0)
                return false;

            return true;

        }
        #endregion

        #region Equipo
        public Dictionary<int, string> Equipos()
        {
            string query = "select * from asignacion.ae_cat_equipo";
            Dictionary<int, string> res = new Dictionary<int, string>();

            DataTable dt = Querry(query);

            foreach (DataRow r in dt.Rows)
                res.Add(Convert.ToInt32(r["cve_equipo"].ToString()), r["equipo"].ToString());

            return res;
        }
        #endregion
    }
}