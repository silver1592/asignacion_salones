using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
using Excel;
using System.Data;

namespace OrigenDatos.Clases
{
    public class LibroExcel
    {
        private string archivo = @"";
        private string dir;
        private int ContColumn;
        private List<Grupo> grupos;
        private DataTable rawGrupos;
        private string[,] posicionesHorarios;

        public List<Grupo> Grupos { get { return grupos; } }
        public DataTable RawGrupos { get { return rawGrupos; } }
        public string[,] PosicionesHorarios{ get { return posicionesHorarios; } }
        public string Direccion { get { return dir; } }

        #region Atributos a buscar
        public string cve_mat = "CVE_MAT";
        public string cve_gpo = "CVE_GPO";

        public string cve = "CLAVEMAT";

        public string cverpe = "rpe";
        public string tipo = "TIPO";
        public string salon = "salon";
        public string lunes = "lunes_ini";
        public string lunesf = "lunes_fin";
        public string martes = "martes_ini";
        public string martesf = "martes_fin";
        public string miercoles = "miercoles_ini";
        public string miercolesf = "miercoles_fin";
        public string jueves = "jueves_ini";
        public string juevesf = "jueves_fin";
        public string viernes = "viernes_ini";
        public string viernesf = "viernes_fin";
        public string sabado = "sabado_ini";
        public string sabadof = "sabado_fin";
        public string cupo = "cupo";

        private string cicloDefault= "";
        public string CicloDefault { set { cicloDefault = value; } }
        private string tipoDefault = "";
        public string TipoDefault { set { tipoDefault = value; } }

        #endregion

        #region Constructores e Inicializaciones
        public LibroExcel(string direccion, string archivo, string ciclo, string tipo="")
        {
            if (File.Exists(direccion + archivo))
            {
                this.archivo += direccion + archivo;
                this.dir = direccion;
                this.cicloDefault = ciclo;
                this.tipoDefault = tipo;
            }
            else
                throw new Exception("No se encontro archivo", null);
        }

        /// <summary>
        /// Lee el excel y toma la informacion de una hoja original y la almacena en la variable leeGrupos
        /// </summary>
        /// <param name="hoja">Hoja de excel</param>
        public void setHojaHorarios(string hoja)
        {
            DataTable dt = GetHojaHorarios(hoja);

            setPosicionesHorarios(dt);

            rawGrupos = dt;
            grupos = AsList(dt);
        }

        /// <summary>
        /// Revisa el excel y obtiene el nombre de las columnas
        /// Nota: Esta es considerando que las columnas adicionales de Observaciones y de salon anterior estan al finla
        /// </summary>
        public void setPosicionesHorarios(DataTable dt)
        {
            string[,] pos;

            if (dt.Columns.Contains("salon anterior"))
                ContColumn = dt.Columns.Count - 3;
            else
                ContColumn = dt.Columns.Count;
            pos = new string[ContColumn, 2];

            for (int i = 0; i < ContColumn; i++)
            {
                pos[i, 0] = ((char)('A' + i)).ToString();
                pos[i, 1] = dt.Columns[i].ToString();
            }

            posicionesHorarios = pos;
        }
        #endregion

        #region Consultas
        #region DataTable
        private DataTable AsDataTable(List<DataRow> lr)
        {
            DataTable dt = new DataTable();

            foreach (DataRow r in lr)
            {
                if (dt.Columns.Count == 0)
                    dt = rawGrupos.Clone();

                dt.ImportRow(r);
            }

            return dt;
        }

        public DataTable GetHojaHorarios(string hoja)
        {
            string filePath = archivo;
            //Es necesaria para que se inicialize ContCoumn

            FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);

            IExcelDataReader excelReader;
            if (filePath.Contains(".xlsx"))
                // De lo contrario descomentaremos esta (2007 format; *.xlsx)
                excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            else if (filePath.Contains(".xls"))
                // Si es un archivo de excel anterior a 2009, debemos usar esta línea
                excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
            else
                throw new Exception("No es un archivo valido");

            // Cada grupo de elementos en el DataSet tomará el nombre de la primera celda en cada columna
            excelReader.IsFirstRowAsColumnNames = true;

            DataSet result = excelReader.AsDataSet();
            excelReader.Close();

            return result.Tables[hoja];
        }

        public DataTable RGrupos_IniHora(int hora_ini, string salon)
        {
            var query = from Grupo g in this.grupos
                        from DataRow r in rawGrupos.Rows
                        where g.Cve_materia == r[cve_mat].ToString()
                              && g.num_Grupo == Convert.ToInt32(r[cve_gpo])
                              && g.hora_ini==hora_ini
                              && g.Salon == salon
                        select r;

            return AsDataTable(query.ToList<DataRow>());
        }

        public DataTable GetGruposAsignados(int hora)
        {
            
            var query = from Grupo g in this.grupos
                        from DataRow r in rawGrupos.Rows
                        where g.num_Grupo == Convert.ToInt32(r[cve_gpo])
                              && g.Cve_materia == r[cve_mat].ToString()
                              && g.empalme(hora, hora + 1, "1111111")
                              && salon != ""
                        select r;

            return AsDataTable(query.ToList<DataRow>());
        }

        public DataTable GetGrupo(DataTable datos,string materia, string grupo)
        {
            var query = from DataRow r in datos.Rows
                        where r[cve_mat].ToString() == materia && r[cve_gpo].ToString() == grupo
                        select r;

            return AsDataTable(query.ToList<DataRow>());
        }

        public DataTable GetGrupos(DataTable datos, string materia, string grupo, string tipo, string ciclo)
        {
            var query = from DataRow r in datos.Rows
                        where r[cve_mat].ToString() == materia && r[cve_gpo].ToString() == grupo
                        select r;

            return AsDataTable(query.ToList<DataRow>());
        }

        public string[] getHojas()
        {
            string filePath = archivo;
            //Es necesaria para que se inicialize ContCoumn

            FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);

            IExcelDataReader excelReader;
            if (filePath.Contains(".xlsx"))
                // De lo contrario descomentaremos esta (2007 format; *.xlsx)
                excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            else if (filePath.Contains(".xls"))
                // Si es un archivo de excel anterior a 2009, debemos usar esta línea
                excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
            else
                throw new Exception("No es un archivo valido");

            // Cada grupo de elementos en el DataSet tomará el nombre de la primera celda en cada columna
            excelReader.IsFirstRowAsColumnNames = true;

            DataSet result = excelReader.AsDataSet();

            excelReader.Close();

            return GetNombres(result);
        }

        private string[] GetNombres(DataSet ds)
        {
            string[] res = new string[ds.Tables.Count];

            for(int i =0; i<ds.Tables.Count;i++)
                res[i] = ds.Tables[i].TableName;

            return res;
        }
        #endregion

        #region List<Grupo>
        public List<Grupo> Grupos_EnHora(int hora_ini,int hora_fin,string salon, string dias)
        {
            var query = from Grupo g in this.grupos
                        where g.empalme(hora_ini, hora_fin, dias) && g.Salon == salon
                        select g;

            return query.ToList<Grupo>();
        }

        public List<Grupo> Grupos_NoEn(List<Grupo> grupos)
        {
            var query = from Grupo g in this.grupos
                        where !grupos.Contains(g)
                        select g;

            return query.ToList<Grupo>();
        }

        public List<Grupo> AsList(DataTable dtGrupos)
        {
            Grupo aux;
            List<Grupo> res = new List<Grupo>();
            string[] datos = new string[19];

            foreach(DataRow r in dtGrupos.Rows)
            {
                ///Se usan estos si se tiene el grupo y la materia por separado
                ///datos[0] = Convert.ToString(r.Field<double>(cve_mat));
                ///datos[1] = Convert.ToString(r.Field<double>(cve_gpo));

                ///Se usa este si el grupo y la materia estan juntos
                datos[0] = Convert.ToString(r.Field<double>(cve)).Substring(0,4);
                datos[1] = Convert.ToString(r.Field<double>(cve)).Substring(4, 2);

                datos[2] = Convert.ToString(r.Field<double>(cverpe));
                datos[3] = tipoDefault == "" ? r.Field<string>(tipo) : tipoDefault;
                datos[4] = r.Field<string>(salon);
                datos[5] = Convert.ToString(r.Field<double>(lunes));
                datos[6] = Convert.ToString(r.Field<double>(lunesf));
                datos[7] = Convert.ToString(r.Field<double>(martes));
                datos[8] = Convert.ToString(r.Field<double>(martesf));
                datos[9] = Convert.ToString(r.Field<double>(miercoles));
                datos[10] = Convert.ToString(r.Field<double>(miercolesf));
                datos[11] = Convert.ToString(r.Field<double>(jueves));
                datos[12] = Convert.ToString(r.Field<double>(juevesf));
                datos[13] = Convert.ToString(r.Field<double>(viernes));
                datos[14] = Convert.ToString(r.Field<double>(viernesf));
                datos[15] = Convert.ToString(r.Field<double>(sabado));
                datos[16] = Convert.ToString(r.Field<double>(sabadof));
                datos[17] = Convert.ToString(r.Field<double>(cupo));
                datos[18] = cicloDefault;

                aux = new Grupo(datos[0],
                                  datos[1],
                                  datos[2],
                                  datos[3],
                                  datos[4],
                                  datos[5],
                                  datos[6],
                                  datos[7],
                                  datos[8],
                                  datos[9],
                                  datos[10],
                                  datos[11],
                                  datos[12],
                                  datos[13],
                                  datos[14],
                                  datos[15],
                                  datos[16],
                                  datos[17],
                                  datos[18]);

                res.Add(aux);
            }

            return res;
        }

        public List<List<Grupo>> GetEmpalmes()
        {
            List<List<Grupo>> res = new List<List<Grupo>>();

            foreach (List<Grupo> lg in HorarioGrupos())
            {
                var query = from Grupo g1 in lg
                            from Grupo g2 in lg
                            where g2 != g1 && g1.empalme(g2)
                            select g1;



                if(query.Count()>0)
                    res.Add(query.Distinct<Grupo>().ToList<Grupo>());
            }

            return res;
        }

        public List<List<Grupo>> HorarioGrupos()
        {
            List<List<Grupo>> res = new List<List<Grupo>>();

            var query = from Grupo g in grupos
                        group g by g.Salon into horarioSalon
                        select horarioSalon;

            foreach(var lg in query)
                res.Add(lg.ToList<Grupo>());

            return res;
        }

        public List<Grupo> HorarioSalon(string salon)
        {
            var query = from Grupo g in grupos
                        where g.Salon == salon
                        select g;

            return query.ToList<Grupo>();
        }

        public List<Grupo> HorarioProfesor(string rpe)
        {
            var query = from Grupo g in grupos
                        where g.RPE==Convert.ToInt32(rpe)
                        select g;

            return query.ToList<Grupo>();
        }

        #endregion

        #region List<Salon>
        /// <summary>
        /// busca salon a salon los que esten ocupados entre las horas designadas y los dias
        /// </summary>
        /// <param name="salones">Grupo de salones validos para checar</param>
        /// <param name="ini">hora inicial para el rango de horas</param>
        /// <param name="fin">hora final para el rango de horas</param>
        /// <param name="dias">dias que se van a buscar. L-M-Mi-J-V-S Marcar con un 1 los dias que quieres obtener</param>
        /// <returns></returns>
        public List<Salon> salonesDisponibles(List<Salon> salones, int ini, int fin, string dias="111111")
        {
            List<Salon> res = new List<Salon>();
            List<Grupo> auxG;

            foreach(Salon s in salones)
            {
                auxG = Grupos_EnHora(ini, fin, s.Cve_espacio,dias);
                if (auxG.Count == 0)
                    res.Add(s);
            }

            return res;
        }
        #endregion

        #region index
        public int GetGrupoRawIndex(string materia, int grupo)
        {
            int i = 0;
            foreach (DataRow r in rawGrupos.Rows)
            {
                if (r[cve_mat].ToString() == materia && r[cve_gpo].ToString() == grupo.ToString())
                    return i;
                i++;
            }

            return -1;
        }

        public int GetGrupoIndex(string materia, int grupo)
        {
            int i = 0;

            foreach (Grupo g in grupos)
            {
                if (g.Cve_materia == materia && g.num_Grupo == grupo)
                    return i;
                i++;
            }

            return -1;
        }
        #endregion
        #endregion

        #region Comandos
        /// <summary>
        /// Escribe la informaciion que tenga en la variable LeeGrupos
        /// </summary>
        /// <param name="hoja">Nombre de la hoja que se quiere poner </param>
        public void Escribe_Horario_Excel(string hoja, string dir)
        {
            var workbook = new XLWorkbook();
            //Abre la hoja seleccionada y de no existir la crea
            IXLWorksheet worksheet;

            try
            {
                worksheet = workbook.Worksheets.Worksheet(hoja);
            }catch
            {
                worksheet = workbook.Worksheets.Add(hoja);
            }

            //Escribe los encabezados de las hojas
            for (int i = 0; i < ContColumn + 3; i++)
            {
                if (i < ContColumn)
                    worksheet.Cell(posicionesHorarios[i, 0] + "1").Value = posicionesHorarios[i, 1];
                else if (i<ContColumn+1)
                    worksheet.Cell(((char)(posicionesHorarios[ContColumn - 1, 0].ToCharArray()[0] + 1))+"1").Value = "salon anterior";
                else if (i<ContColumn+2)
                    worksheet.Cell(((char)(posicionesHorarios[ContColumn - 1, 0].ToCharArray()[0] + 2)) + "1").Value = "puntos de asignacion";
                else if (i < ContColumn + 3)
                    worksheet.Cell(((char)(posicionesHorarios[ContColumn - 1, 0].ToCharArray()[0] + 3)) + "1").Value = "observaciones";
            }

            //Escribe la informacion de los grupos
            int counter = 2;
            int rawIndex;

            foreach (Grupo item in grupos)
            {
                for (int i = 0; i < ContColumn + 3; i++)
                {
                    rawIndex = GetGrupoRawIndex(item.Cve_materia, item.num_Grupo);

                    if (i < ContColumn)
                        if (posicionesHorarios[i, 1] == salon)
                            worksheet.Cell(posicionesHorarios[i, 0] + counter).Value = item.Salon;
                        else
                            worksheet.Cell(posicionesHorarios[i, 0] + counter).Value = rawGrupos.Rows[rawIndex][posicionesHorarios[i, 1]];
                    else if (i < ContColumn + 1)
                        worksheet.Cell(((char)(posicionesHorarios[ContColumn - 1, 0].ToCharArray()[0] + 1)) + counter.ToString()).Value = item.SalonBD;
                    else if (i < ContColumn + 2)
                        worksheet.Cell(((char)(posicionesHorarios[ContColumn - 1, 0].ToCharArray()[0] + 2)) + counter.ToString()).Value = "";
                    else if (i < ContColumn + 3 && item.observaciones != "")
                        worksheet.Cell(((char)(posicionesHorarios[ContColumn - 1, 0].ToCharArray()[0] + 3)) + counter.ToString()).Value = item.observaciones;
                }
                counter++;
            }

            workbook.SaveAs(dir == null ? this.archivo : dir);
            workbook.Dispose();
        }

        public void Update(Grupo g, string observaciones = null)
        {
            ///Busca el grupo en el dataTable
            int rawIndex = GetGrupoRawIndex(g.Cve_materia, g.num_Grupo);
            int grupoIndex = GetGrupoIndex(g.Cve_materia, g.num_Grupo);

            if (grupoIndex == -1 || rawIndex == -1)
                throw new Exception("No se pudo guardar el grupo, error al intentar guardar en la columna");

            if (observaciones != null && observaciones != "")
            {
                grupos[grupoIndex].observaciones += "-" + observaciones;
            }

            ///Modifica el grupo en el dataRow
            rawGrupos.Rows[rawIndex][salon] = g.Salon;
            grupos[grupoIndex].Salon = g.Salon;
        }
        #endregion
    }
}