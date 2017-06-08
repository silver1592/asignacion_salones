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
        private string[] sheets;

        public ListaGrupos Grupos { get { return new ListaGrupos(grupos); } }
        public DataTable RawGrupos { get { return rawGrupos; } }
        public string[,] PosicionesHorarios{ get { return posicionesHorarios; } }
        public string Direccion { get { return dir; } }
        public string[] Sheets { get { return sheets; } }

        #region Atributos a buscar
        public Dictionary<string, string> headers;
        private void SetHeaders()
        {
            headers = new Dictionary<string, string>();
            headers.Add("cve_mat","CVE_MAT");
            headers.Add("cve_gpo","CVE_GPO");

            headers.Add("cve", "CLAVEMAT");

            headers.Add("cverpe","CVERPE");
            headers.Add("tipo","TIPO");
            headers.Add("salon","SALON");
            headers.Add("lunes","LUNES");
            headers.Add("lunesf","LUNESF");
            headers.Add("martes ", "MARTES");
            headers.Add("martesf ", "MARTESF");
            headers.Add("miercoles ", "MIERCOLES");
            headers.Add("miercolesf ", "MIERCOLESF");
            headers.Add("jueves ", "JUEVES");
            headers.Add("juevesf ", "JUEVESF");
            headers.Add("viernes ", "VIERNES");
            headers.Add("viernesf ", "VIERNESF");
            headers.Add("sabado ", "SABADO");
            headers.Add("sabadof ", "SABADOF");
            headers.Add("cupo ", "CUPO");

            //Valores default
            headers.Add("cicloDefault ", "");
            headers.Add("tipoDefault ", "");
        }

        #endregion

        #region Constructores e Inicializaciones
        public LibroExcel(string direccion, string archivo, string ciclo, string tipo="")
        {
            if (File.Exists(direccion + archivo))
            {
                SetHeaders();
                this.archivo += direccion + archivo;
                this.dir = direccion;
                headers["cicloDefault"] = ciclo;
                headers["tipoDefault"] = tipo;
            }
            else
                throw new Exception("No se encontro archivo", null);
        }

        /// <summary>
        /// Lee el excel y regresa las hojas en una coleccion de tablas
        /// </summary>
        /// <returns></returns>
        public DataTableCollection GetSheets()
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

            return result.Tables;
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

        /// <summary>
        /// Obtiene el nombre de las hojas del excel y lo asigna a la variable Sheets
        /// </summary>
        private void SetHojas()
        {
            DataTableCollection tables = GetSheets();

            sheets = new string[tables.Count];
            for (int i = 0; i < tables.Count; i++)
                sheets[i] = tables[i].ToString();
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
            DataTableCollection tables = GetSheets();

            return tables[hoja];
        }

        public DataTable GetGrupo(DataTable datos,string materia, string grupo)
        {
            var query = from DataRow r in datos.Rows
                        where r[headers["cve_mat"]].ToString() == materia && r[headers["cve_gpo"]].ToString() == grupo
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

        public List<Grupo> AsList(DataTable dtGrupos)
        {
            List<Grupo> res = new List<Grupo>();

            foreach (DataRow r in dtGrupos.Rows)
                res.Add(new Grupo(r, headers));

            return res;
        }

        #endregion

        #region index
        public int GetGrupoRawIndex(string materia, int grupo)
        {
            int i = 0;
            foreach (DataRow r in rawGrupos.Rows)
            {
                if (r[headers["cve_mat"]].ToString() == materia && r[headers["cve_gpo"]].ToString() == grupo.ToString())
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
                        if (posicionesHorarios[i, 1] == headers["salon"])
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
            rawGrupos.Rows[rawIndex][headers["salon"]] = g.Salon;
            grupos[grupoIndex].Salon = g.Salon;
        }
        #endregion
    }
}