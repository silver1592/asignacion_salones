﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace OrigenDatos.Clases
{
    public class Conexion
    {
        #region estaticos
        public static string datosConexionPrueba
        {
            get
            {
                string dir = @"148.224.93.146\FINGENIERIA,2433;";
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
        #endregion

        protected string datosConexion;
        protected LibroExcel Excel;

        #region Diccionarios
        public Dictionary<string, string> DGruposExcel { get { return Excel.headers; } }

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
        }

        public Dictionary<string,string> DGrupos { get { return Excel != null ? Excel.headers : DGruposBD; } }
        #endregion

        public Conexion()
        {
            datosConexion = Conexion.datosConexionPrueba;
        }
        public Conexion(string Datos, string excelDireccion=null, string archivoEntrada=null, string hoja = null, string ciclo= "2016-2017/II", string tipo ="")
        {
            datosConexion = Datos;

            if(excelDireccion != null)
            {
                Excel = new LibroExcel(excelDireccion,archivoEntrada, ciclo, tipo);

                if(hoja!=null)
                    Excel.setHojaHorarios(hoja);
            }
        }

        #region Consultas
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
        /// Hace una consulta a la base de datos para obtener los grupos que se impartieron una hora antes por el profesor
        /// </summary>
        /// <param name="rpe">Clave unica del profesor</param>
        /// <param name="hora">Hora de la cual se quiere obtener la informacion</param>
        /// <returns>DataTable con los datos de los grupos impartidos</returns>
        public DataTable GruposAnteriores(int rpe, int hora, string ciclo)
        {
            string textoCmd = "select * "
                               + "from ae_Grupos_a_las(" + (hora - 1) + ") "
                               + "where rpe = " + rpe + " and ciclo = '" + ciclo + "';";

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

        public DataTable SemestresAnteriores(string cve_materia,string ciclo,string rpe)
        {
            string query = "select * from ae_horario where not(ciclo = '" + ciclo + "') and rpe = '"+rpe+"' and cve_materia = '"+cve_materia+"'";
            DataTable dt = Querry(query);

            return dt;
        }
        #endregion

        #region Base
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
            catch
            {
                return false;
            }
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
        public void UpdateGrupoExcel(Grupo g, string observaciones="", bool bd=false)
        {
            if (Excel!=null && !bd)
            {
                Excel.Update(g, observaciones);
            }
            else
                Comando(g.qUpdate);
        }
        #endregion
    }
}
