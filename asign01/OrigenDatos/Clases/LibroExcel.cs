using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
using Excel;
using System.Data;

namespace OrigenDatos.Clases
{
    /// <summary>
    /// Abre un archivo de excel y lee o escribe en el
    /// </summary>
    public class LibroExcel
    {
        /// <summary>
        /// Nombre del archivo
        /// </summary>
        private string archivo;
        /// <summary>
        /// Direccion del archivo
        /// </summary>
        private string dir;

        #region Atributos a buscar
        /// <summary>
        /// Diccionario con los encabezados de la tabla
        /// </summary>
        public Dictionary<string, string> headers;

        /// <summary>
        /// Inicializa el diccionario
        /// </summary>
        /// <remarks>
        /// Solo se llama cuando se crea el objeto
        /// </remarks>
        private void SetHeaders()
        {
            headers = new Dictionary<string, string>();
            headers.Add("cve_mat", "CVE_MAT");
            headers.Add("cve_gpo", "CVE_GPO");

            headers.Add("cve", "CLAVEMAT");

            headers.Add("cverpe", "CVERPE");
            headers.Add("tipo", "TIPO");    //*
            headers.Add("salon", "SALON");  //*
            headers.Add("lunes", "LUNES");
            headers.Add("lunesf", "LUNESF");
            headers.Add("martes", "MARTES");
            headers.Add("martesf", "MARTESF");
            headers.Add("miercoles", "MIERCOLES");
            headers.Add("miercolesf", "MIERCOLESF");
            headers.Add("jueves", "JUEVES");
            headers.Add("juevesf", "JUEVESF");
            headers.Add("viernes", "VIERNES");
            headers.Add("viernesf", "VIERNESF");
            headers.Add("sabado", "SABADO");
            headers.Add("sabadof", "SABADOF");
            headers.Add("cupo", "CUPO");
            headers.Add("ciclo", "CICLO");  //*

            //Valores default
            headers.Add("cicloDefault", "");
            headers.Add("tipoDefault", "T");
            headers.Add("salonDefault", "");
        }
        #endregion

        #region Constructores e Inicializaciones
        /// <summary>
        /// Almacena la informacion y la guarda y checa si existe el archivo
        /// </summary>
        /// <param name="direccion">Direccion del archivo</param>
        /// <param name="ciclo">Semestre al cual se va a considerar</param>
        /// <param name="tipo">tipo por default para los grupos</param>
        public LibroExcel(string direccion, string ciclo, string tipo="")
        {
            string[] split = direccion.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
            string archivo = split[split.Length - 1];

            if (!File.Exists(direccion))
            {
                //TODO: Si no existe crealo
                Microsoft.Office.Interop.Excel.Application appExcel = new Microsoft.Office.Interop.Excel.Application();
                Microsoft.Office.Interop.Excel.Workbook workbook = appExcel.Workbooks.Add();
                workbook.SaveAs(direccion);
                workbook.Close();
                //throw new Exception("No se encontro archivo", null);
            }

            SetHeaders();
            this.archivo = archivo;
            this.dir = direccion;
            headers["cicloDefault"] = ciclo;
            headers["tipoDefault"] = tipo;
        }

        /// <summary>
        /// Lee el excel y regresa las hojas en una coleccion de tablas
        /// </summary>
        /// <returns>Hojas del excel</returns>
        private DataTableCollection GetSheets()
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
        /// Convierte un DataTable a List de tipo grupo
        /// </summary>
        /// <param name="dtGrupos">Tabla de grupos</param>
        /// <returns></returns>
        private List<Grupo> AsList(DataTable dtGrupos)
        {
            List<Grupo> res = new List<Grupo>();

            foreach (DataRow r in dtGrupos.Rows)
                res.Add(new Grupo(r, headers));

            return res;
        }
        #endregion

        #region Comandos
        /// <summary>
        /// Escribe la informacion de los grupos
        /// </summary>
        /// <param name="grupos">Nombre de la hoja que se quiere poner </param>
        /// <param name="hoja">Hoja en la que se va a escribir</param>
        public void EscribeGrupos(IList<Grupo> grupos, string hoja)
        {
            var workbook = new XLWorkbook();
            //Abre la hoja seleccionada y de no existir la crea
            IXLWorksheet worksheet;

            try
            {
                worksheet = workbook.Worksheets.Worksheet(hoja);
            }catch
            {
                List<string> keyList = new List<string>(headers.Keys);
                worksheet = workbook.Worksheets.Add(hoja);

                //Escribe los encabezados de las hojas
                for (int i = 0; i < headers.Count + 1; i++)
                {
                    if (i < headers.Count)
                        worksheet.Row(0).Cell(i).Value = headers[keyList[i]];
                    else if (i < headers.Count + 1)
                        worksheet.Row(0).Cell(i).Value = "observaciones";
                }
            }

            foreach (Grupo item in grupos)
                EscribeGrupo(item,worksheet);

            workbook.SaveAs(dir == null ? this.archivo : dir);
            workbook.Dispose();
        }

        /// <summary>
        /// Escribe un grupo en la hoja dada
        /// </summary>
        /// <param name="g">Grupo a escribir</param>
        /// <param name="worksheet">Hoja a modificar</param>
        public void EscribeGrupo(Grupo g, IXLWorksheet worksheet)
        {
            worksheet.Row(1).Cell(0).Value = g.Cve_materia;
            worksheet.Row(1).Cell(1).Value = g.num_Grupo;
        }

        /// <summary>
        /// Obtiene el nombre de las hojas del excel e inicializa la variable Sheets
        /// </summary>
        public string[] GetStringSheets()
        {
            DataTableCollection tables = GetSheets();

            string[] sheets = new string[tables.Count];
            for (int i = 0; i < tables.Count; i++)
                sheets[i] = tables[i].ToString();

            return sheets;
        }

        /// <summary>
        /// Lee el excel e inicializa los grupos con la ainfromacion
        /// </summary>
        /// <param name="hoja">Hoja de excel</param>
        public List<Grupo> GetGrupos(string hoja)
        {
            DataTable dt = GetSheets()[hoja];

            return AsList(dt);
        }
        #endregion
    }
}