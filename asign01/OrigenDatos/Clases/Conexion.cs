using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace OrigenDatos.Clases
{
    public class Conexion
    {
        public static string datosConexionPrueba
        {
            get
            {
                string dir = @"148.224.58.4\FINGENIERIA;";
                string usuario = "asignacion";
                string pass = "Asigna#2016Ing";
                //Datos de la coneccion
                string datosConexion = "Data Source=" + dir
                                        + "Initial Catalog=asignacion;"
                                        + "Integrated Security =false;"
                                        + "Uid = " + usuario + ";"
                                        + "Pwd= " + pass + ";";

                return datosConexion;
            }
        }
        public static string datosConexionCasa
        {
            get
            {
                string dir = @"SILVER1592-PC\SQLEXPRESS;";
                string usuario = "sa";
                string pass = "1111";
                //Datos de la coneccion
                string datosConexion = "Data Source=" + dir
                                        + "Initial Catalog=asignacion;"
                                        + "Integrated Security =false;"
                                        + "Uid = " + usuario + ";"
                                        + "Pwd= " + pass + ";";

                return datosConexion;
            }
        }
        private string datosConexion;
        private LibroExcel Excel;
        public LibroExcel GetExcel { get { return Excel; } }
        public bool excel { get { return Excel != null; } }
        public string PageExcel
        {
            set
            {
                Excel.setHojaHorarios(value);
            }
        }

        public Conexion(string Datos, string excelDireccion=null, string archivoEntrada=null, string hoja = "SIAMDIF", string ciclo= "2016-2017/II", string tipo ="")
        {
            datosConexion = Datos;

            if(excelDireccion != null)
            {
                Excel = new LibroExcel(excelDireccion,archivoEntrada, ciclo, tipo);
                Excel.setHojaHorarios(hoja);
            }
        }

        public bool Autenticacion()
        {
            SqlConnection con = new SqlConnection();
            con.ConnectionString = datosConexion;

            try
            {
                con.Open();
                con.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Obtiene el listado de equipo (catalogo de equipo)
        /// </summary>
        /// <returns></returns>
        public DataTable Equipo()
        {
            string textoCmd = "SELECT * FROM [asignacion].[asignacion].[ae_cat_equipo]";

            DataTable datos = Querry(textoCmd);

            return datos;
        }

        /// <summary>
        /// Obtiene la informacion de las excepciones en la base de datos
        /// </summary>
        /// <param name="cve_espacio"></param>
        /// <returns></returns>
        public DataTable Exepciones(string cve_espacio)
        {
            string textoCmd = "SELECT * FROM [asignacion].[ae_excepciones] where cve_espacio='" + cve_espacio + "';";

            DataTable datos = Querry(textoCmd);

            return datos;
        }

        /// <summary>
        /// Obtiene el valor que tiene un edificio con respecto a un edificio
        /// </summary>
        /// <param name="cve_edificio">Clave del edificio a buscar</param>
        /// <returns></returns>
        public DataTable Edificio_Area(string cve_edificio)
        {
            ///ver lo de funciones almacenadas.
            string textoCmd = "SELECT * FROM[asignacion].[ae_area_edificio]  where idEdificio = '" + cve_edificio + "'";

            DataTable datos = Querry(textoCmd);

            return datos;
        }

        /// <summary>
        /// Obtiene de la base de datos las necesidades marcadas al profesor
        /// </summary>
        /// <param name="rpe">Profesor a buscar</param>
        /// <returns></returns>
        public DataTable Necesidades_prof(string rpe)
        {
            ///ver lo de funciones almacenadas.
            string textoCmd = "SELECT [rpe], [discapacidad], [salon_unico] FROM [asignacion].[asignacion].[ae_necesidad_profesor]"
                              + "where rpe=" + rpe + ";";

            DataTable datos = Querry(textoCmd);

            return datos;
        }

        /// <summary>
        /// Obtiene el registro del horario de otros semestres si es que el salon ya habia sido asignado a ese profesor para esa materia
        /// </summary>
        /// <param name="cve_materia">Clave de la materia</param>
        /// <param name="rpe">Clave unica del profesor</param>
        /// <param name="tipo">Tipo de materia (T/L)</param>
        /// <param name="salon">Salon asignado</param>
        /// <returns></returns>
        public DataTable asignacionesSemestresAnteriores(string cve_materia, int rpe, string tipo, string salon, int horario, string ciclo)
        {
            ///ver lo de funciones almacenadas.
            string textoCmd = "SELECT * FROM [asignacion].[ae_salones_otros_semestres] ('" + cve_materia + "'," + rpe + ", '" + tipo + "', '" + salon + "'," + horario + ",'" + ciclo + "')";

            DataTable datos = Querry(textoCmd);

            return datos;
        }

        /// <summary>
        /// obtiene de la base de datos el registro de un salon
        /// </summary>
        /// <param name="cve_espacio">Clave del salon</param>
        /// <returns></returns>
        public DataTable Salon(string cve_espacio)
        {
            ///ver lo de funciones almacenadas.
            string textoCmd = "SELECT * "
                              + "FROM [asignacion].[ae_cat_espacio] "
                              + "where cve_espacio='" + cve_espacio + "'; ";

            DataTable datos = Querry(textoCmd);

            return datos;
        }

        /// <summary>
        /// Muestra los salones de un edificio
        /// </summary>
        /// <param name="Edificio">id del edificio</param>
        /// <returns></returns>
        public DataTable Salones(string Edificio)
        {
            ///ver lo de funciones almacenadas.
            string textoCmd = "SELECT * "
                              + "FROM [asignacion].[ae_cat_espacio] "
                              + "where cve_edificio='" + Edificio + "'; ";

            DataTable datos = Querry(textoCmd);

            return datos;
        }

        /// <summary>
        /// Muestra los salones registrados en la base de datos
        /// </summary>
        /// <param name="Edificio">id del edificio</param>
        /// <returns></returns>
        public DataTable Salones()
        {
            ///ver lo de funciones almacenadas.
            string textoCmd = "SELECT * "
                              + "FROM [asignacion].[ae_cat_espacio] "
                              + "where not(cve_edificio='F') and not(cve_edificio='P') and not(cve_edificio='ZP')";


            DataTable datos = Querry(textoCmd);

            return datos;
        }

        /// <summary>
        /// Muestra los salones registrados en la base de datos
        /// </summary>
        /// <param name="Edificio">id del edificio</param>
        /// <returns></returns>
        public DataTable Salones_Asignacion()
        {
            ///ver lo de funciones almacenadas.
            string textoCmd = "SELECT ae_cat_espacio.* "
                              + "FROM [asignacion].[ae_cat_espacio] left join asignacion.ae_excepciones on ae_cat_espacio.cve_espacio=ae_excepciones.cve_espacio"
                              + " where not(cve_edificio='F') and not(cve_edificio='P') and not(cve_edificio='ZP') and (no_asignable is null or no_asignable=0)";


            DataTable datos = Querry(textoCmd);

            return datos;
        }

        public DataTable Salon_equipo(string cve_espacio)
        {
            ///ver lo de funciones almacenadas.
            string textoCmd = "SELECT * "
                              + "FROM [asignacion].[ae_equipamiento]"
                              + "where cve_espacio='" + cve_espacio + "'; ";

            DataTable datos = Querry(textoCmd);

            return datos;
        }

        /// <summary>
        /// Obtiene las necesidades del profesor
        /// </summary>
        /// <param name="rpe">Clave unica del profesor</param>
        /// <returns></returns>
        public DataTable Necesidades_Profesor(int rpe)
        {
            ///ver lo de funciones almacenadas.
            string textoCmd = "SELECT * "
                              + "FROM [asignacion].[ae_necesidad_profesor]"
                              + "where rpe = " + rpe.ToString() + "";

            DataTable datos = Querry(textoCmd);

            return datos;
        }

        /// <summary>
        /// Obtiene las necesidades de un grupo
        /// </summary>
        /// <param name="cve_materia">Clave de la materia</param>
        /// <param name="grupo">grupo de la materia</param>
        /// <param name="tipo">tipo de claser (T/L)</param>
        /// <returns></returns>
        public DataTable Necesidades_Grupo(string cve_materia, string tipo, string rpe)
        {
            ///ver lo de funciones almacenadas.
            string textoCmd ="";

            if(cve_materia!="" && rpe!="")
                textoCmd = "SELECT idEquipo,Equipo, peso "
                              + "FROM  [asignacion].[ae_necesidades_curso] inner join asignacion.ae_cat_equipo on cve_equipo=idEquipo"
                              + " where cve_materia = '" + cve_materia + "' and tipo = '" + tipo + "' and rpe = " + rpe + "; ";
            else if (cve_materia!="" && rpe=="")
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
        /// Obtiene los semestres que se encuentran en la base de datos y los ordena decendentemente
        /// </summary>
        /// <returns></returns>
        public DataTable Semestres()
        {
            string textoCmd = "SELECT distinct ciclo FROM ae_horario order by ciclo DESC";

            DataTable datos = Querry(textoCmd);

            return datos;
        }

        /// <summary>
        /// Obtiene cuales son las necesidades de los profesores con respecto al equipo que necesitan en su salon de clases.
        /// </summary>
        /// <param name="materia"></param>
        /// <param name="salon"></param>
        /// <param name="profesor"></param>
        /// <param name="nombre_profesor"></param>
        /// <param name="semestre"></param>
        /// <returns></returns>
        public DataTable Nece_Equipo(string materia, string salon, string profesor, string nombre_profesor, string semestre)
        {
            string textoCmd = "SELECT * FROM [asignacion].[vista_nece_Horarios] ('"
                                + materia + "','"
                                + salon + "','"
                                + profesor + "','"
                                + nombre_profesor + "','"
                                + semestre + "')";

            DataTable datos = Querry(textoCmd);

            return datos;
        }

        /// <summary>
        /// Ejecuta una consulta en SQL utilizando el comando enviado como parametro
        /// Usado principalmente para consultas DLL (Data Definition Language)
        /// </summary>
        /// <param name="cad"></param>
        public void Comando(string textoCmd)
        {
            SqlCommand cmd;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = datosConexion;
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
        /// Busca el registro de un profesor
        /// </summary>
        /// <param name="rpe"></param>
        /// <returns></returns>
        public DataTable profesor(string rpe)
        {
            string textoCmd = "SELECT [rpe],[titulo]+' '+[nombre] as 'Nombre' FROM [vae_cat_profesor] where rpe=" + rpe;
            DataTable datos = Querry(textoCmd);

            return datos;
        }

        /// <summary>
        /// Obtiene la informacion de todos los profesores
        /// </summary>
        /// <param name="rpe"></param>
        /// <returns></returns>
        public DataTable Profesores()
        {
            //string textoCmd = "SELECT [rpe],[titulo]+' '+[nombre] as 'Nombre' FROM [vae_cat_profesor]";
            string textoCmd = "SELECT [rpe],[nombre] as 'Nombre' FROM [vae_cat_profesor]";
            DataTable datos = Querry(textoCmd);

            return datos;
        }

        /// <summary>
        /// Obtiene de la base de datos las materias que tengan una clave parecida a cve_materia o al nombre
        /// </summary>
        /// <param name="cve_materia">clave similar</param>
        /// <param name="nombre">nombre de la materia</param>
        /// <returns></returns>
        public DataTable Materias(string cve_materia, string nombre)
        {
            string textoCmd = "SELECT cve_area as Area, cve_materia as Materia, materia as Nombre  FROM [vae_cat_materia] where (cve_materia like '%" + cve_materia + "%') and (materia like '%" + nombre + "%')";
            DataTable datos = Querry(textoCmd);

            return datos;
        }

        /// <summary>
        /// Obtiene de la base de datos las materias que se encuentren en la base de datos
        /// </summary>
        /// <param name="cve_materia">clave similar</param>
        /// <param name="nombre">nombre de la materia</param>
        /// <returns></returns>
        public DataTable Materias()
        {
            string textoCmd = "SELECT cve_area as Area, cve_materia as Materia, materia as Nombre  FROM [vae_cat_materia]";
            DataTable datos = Querry(textoCmd);

            return datos;
        }


        /// <summary>
        /// -----Interfas------
        /// Busca en la base de datos los salones que no estan asignados a esa materia para mostarlos en el combo box
        /// </summary>
        /// <param name="cve">clave de la materia a buscar</param>
        public DataTable Salones_SinAsignar_Posibles(string cve)
        {
            string textoCmd = "select distinct s.cve_espacio from ae_cat_espacio as s left join (select * from ae_PosiblesSalones where cve_mat=" + cve + ") as a on s.cve_espacio=a.cve_espacio where a.cve_espacio is null";

            DataTable datos = Querry(textoCmd);

            return datos;
        }

        /// <summary>
        /// -------Interfaz------
        /// Consulta para obtener los salones que ya estan asignados en las materias
        /// </summary>
        /// <param name="cve"></param>
        /// <returns></returns>
        public DataTable Salones_Asignados_posibles(string cve)
        {
            string textoCmd = "select distinct s.cve_espacio from ae_cat_espacio as s left join (select * from ae_PosiblesSalones where cve_mat=" + cve + ") as a on s.cve_espacio=a.cve_espacio where a.cve_mat=" + cve;

            DataTable datos = Querry(textoCmd);

            return datos;
        }

        /// <summary>
        /// Hace una consulta a la base de datos para obtener los grupos que se impartieron una hora antes por el profesor
        /// </summary>
        /// <param name="rpe">Clave unica del profesor</param>
        /// <param name="hora">Hora de la cual se quiere obtener la informacion</param>
        /// <returns>DataTable con los datos de los grupos impartidos</returns>
        public DataTable GruposAnteriores(int rpe, int hora,string ciclo)
        {
            string textoCmd = "select * "
                               +"from ae_Grupos_a_las("+(hora-1)+") "
                               + "where rpe = "+rpe+" and ciclo = '"+ciclo+"';";

            DataTable datos = Querry(textoCmd);

            return datos;
        }

        /// <summary>
        /// Consulta para obtener los salones asignados en las materias
        /// </summary>
        /// <param name="cve_materia"></param>
        /// <returns></returns>
        public DataTable salonesPosibles(string cve_materia)
        {
            string textoCmd = "SELECT [cve_mat],[cve_espacio] FROM[asignacion].[asignacion].[ae_PosiblesSalones] where cve_mat = " + cve_materia;

            DataTable datos = Querry(textoCmd);

            return datos;
        }

        public DataTable Querry(string textoCmd)
        {
            SqlCommand cmd;
            SqlConnection con = new SqlConnection();
            con.ConnectionString = datosConexion;
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


        ///En esta region intente agrupar todas las consultas que requieren de la tabla grupos y por lo tanto
        ///son las que pudiese tener otro origen de datos (excel)
        #region grupo

        /// <summary>
        /// Obtiene el grupo especificado por los parametros
        /// </summary>
        /// <param name="cve_materia"></param>
        /// <param name="grupo"></param>
        /// <param name="tipo"></param>
        /// <param name="ciclo"></param>
        /// <returns></returns>
        public DataTable Grupo(string cve_materia, int grupo, string tipo, string ciclo)
        {
            DataTable datos = null;

            if (!excel)
            {
                ///ver lo de funciones almacenadas.
                string textoCmd = "SELECT * "
                                  + "FROM [asignacion].[ae_horario] "
                                  + "where cve_materia = '" + cve_materia + "' and grupo = " + grupo.ToString() + " and tipo = '" + tipo + "' and ciclo = '" + ciclo + "'; ";

                datos = Querry(textoCmd);
            }
            else
                datos = Excel.GetGrupo(Excel.RawGrupos, cve_materia, grupo.ToString());

            return datos;

        }

        /// <summary>
        /// Muestra los salones disponibles en los dias saolicitados y en la hora que se solicita (unicamente una hora despues de la  marcada)
        /// </summary>
        /// <param name="dias">'111111' -> LMmJVS'</param>
        /// <param name="hora">Hora en la que se va a buscar los salones. ejem: hora=7 -> hora_ini=7, hora_fin=8</param>
        /// <param name="ciclo">ciclo escolar en que se va a buscar</param>
        /// <returns></returns>
        public DataTable SalonesDisponibles(string dias, int hora, string ciclo)
        {
            DataTable datos = null;
            if (!excel)
            {
                ///ver lo de funciones almacenadas.
                string textoCmd = "SELECT * FROM [asignacion].[ae_salones_disponibles] ("
                                  + "'" + dias + "',"
                                  + hora.ToString()
                                  + "," + (hora + 1).ToString()
                                  + ",'" + ciclo + "');";

                datos = Querry(textoCmd);
            }
            else
            {
                //Obtiene los salones asignados
                DataTable gAsignados = Excel.GetGruposAsignados(hora);
                DataTable salones = Querry("select S.* from asignacion.ae_cat_espacio as S left join asignacion.ae_excepciones as E on S.cve_espacio = E.cve_espacio where E.cve_espacio is null and not cve_edificio = 'ZP'; ");

                var sAsignados = from DataRow grup in gAsignados.Rows
                                       join DataRow sal in salones.Rows
                                       on grup[Excel.salon] equals sal["cve_espacio"]
                                       select sal;

                //obtiene complemeto de salones asignados
                var sDisponibles = from DataRow s in salones.Rows
                                         where !sAsignados.ToList<DataRow>().Contains(s)
                                         select s;

                //Convierte la lista a DataTable
                foreach (DataRow r in sDisponibles.ToList<DataRow>())
                {
                    if (datos.Columns.Count == 0)
                        datos = salones.Clone();

                    datos.ImportRow(r);
                }

            }

            return datos;
        }

        /// <summary>
        /// Hace una consulta a la base de datos solisitando los grupos que tienen el salon empalmado
        /// </summary>
        /// <param name="hora">Hora a checar (se evalua hora por hora)</param>
        /// <returns>Informacion de los grupos empalmados</returns>
        public DataTable grupos_Empalme(int hora, string ciclo)
        {
            DataTable datos;
            if (!excel)
            {
                ///ver lo de funciones almacenadas.
                string textoCmd = "SELECT distinct * FROM [ae_Grupos_Empalmados] ('111111'," + hora.ToString() + "," + (hora + 1).ToString() + ",'" + ciclo + "')";

                datos = Querry(textoCmd);
            }
            else
            {
                ///Obtiene los grupos asignados
                datos = Excel.GetGruposAsignados(hora);

                ///obtiene los grupos que tienen empalmes
                ///Esta parte no es tan necesaria, cuando llegue al apartado de empalmes este checara los grupos y separara los que tienen empalmes
            }

            return datos;
        }

        /// <summary>
        /// Nota(2016-12-25):Excel agregado
        /// </summary>
        /// <param name="dias"></param>
        /// <param name="hora"></param>
        /// <param name="ciclo"></param>
        /// <returns></returns>
        public DataTable Grupos_Sin_Asignar(string dias, int hora, string ciclo)
        {
            ///ver lo de funciones almacenadas.

            DataTable datos;

            if (!excel)
            {
                string textoCmd = "SELECT * FROM [asignacion].[ae_grupos_sin_asignar] ("
                              + "'" + dias + "',"
                              + hora.ToString()
                              + "," + (hora + 1).ToString()
                              + ",'" + ciclo + "')"
                              + " order by cupo ASC;";

                datos = Querry(textoCmd);
            }
            else
                datos = Excel.RGrupos_IniHora(hora,"");

            return datos;
        }

        /// <summary>
        /// Vista para los horarios de la ventana principal y necesidades del profesor
        /// </summary>
        /// <param name="materia"></param>
        /// <param name="salon"></param>
        /// <param name="profesor"></param>
        /// <param name="semestre"></param>
        /// <param name="hora"></param>
        /// <returns></returns>
        public DataTable VHorarios(string materia, string salon, string profesor, string semestre, string hora)
        {
            string textoCmd = "SELECT [cve_materia] ,[grupo], [materia],[tipo],[salon],[maestro],[lunes],[martes],[miercoles],[jueves],[viernes],[sabado],[cupo],[inscritos] FROM [asignacion].[vista_mHorarios] ('"
                                + materia + "','"
                                + salon + "','"
                                + profesor + "','"
                                + semestre + "','"
                                + hora + "')";

            DataTable datos = Querry(textoCmd);

            return datos;
        }

        public DataTable HorarioDelSalon(string salon, int hora, string ciclo)
        {
            DataTable datos = new DataTable();

            if (!excel)
            {
                string textoCmd = "SELECT salon, lunes_ini,martes_ini,miercoles_ini,jueves_ini,viernes_ini,sabado_ini, cve_materia, grupo, rpe"
                                + " FROM[asignacion].[ae_gruposAsignados] ("
                                + "'1111111'"
                                + "," + hora + ""
                                + "," + (hora + 1) + ""
                                + ",'" + ciclo + "')"
                                + "where salon = '" + salon + "'";

                datos = Querry(textoCmd);
            }
            else
            {
                //obten los salones que tenga asigado el salon a esa hora

                //formando data table
            }
            

            return datos;
        }
               
        public void UpdateGrupo(Grupo g, string observaciones="")
        {
            if (excel)
            {
                Excel.Update(g, observaciones);
            }
            else
                Comando(g.modificacion);

        }

        public void Update(string nombre = null)
        {
            if (excel)
                Excel.Escribe_Horario_Excel(nombre != null ? nombre : "asignacion_" + DateTime.Today.ToString("yyyy_MM_dd"),
                                            Excel.Direccion+"\\asignacion_2016-2017-II.xlsx");
        }
        #endregion
    }
}
