﻿using System;
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
        public Dictionary<string, string> dHeaders;

        public Dictionary<string,string> dDefault;

        /// <summary>
        /// Inicializa el diccionario
        /// </summary>
        /// <remarks>
        /// Solo se llama cuando se crea el objeto
        /// </remarks>
        private void SetHeaders()
        {
            dHeaders = new Dictionary<string, string>();
            dDefault = new Dictionary<string, string>();

            dHeaders.Add("cve_mat", "CVE_MAT");
            dHeaders.Add("cve_gpo", "CVE_GPO");
            dHeaders.Add("cve", "CLAVEMAT");
            dHeaders.Add("cverpe", "CVERPE");
            dHeaders.Add("tipo", "TIPO");    //*
            dHeaders.Add("salon", "SALON");  //*
            dHeaders.Add("lunes", "LUNES");
            dHeaders.Add("lunesf", "LUNESF");
            dHeaders.Add("martes", "MARTES");
            dHeaders.Add("martesf", "MARTESF");
            dHeaders.Add("miercoles", "MIERCOLES");
            dHeaders.Add("miercolesf", "MIERCOLESF");
            dHeaders.Add("jueves", "JUEVES");
            dHeaders.Add("juevesf", "JUEVESF");
            dHeaders.Add("viernes", "VIERNES");
            dHeaders.Add("viernesf", "VIERNESF");
            dHeaders.Add("sabado", "SABADO");
            dHeaders.Add("sabadof", "SABADOF");
            dHeaders.Add("cupo", "CUPO");
            dHeaders.Add("ciclo", "CICLO");  //*

            //Valores default
            dDefault.Add("ciclo", "");
            dDefault.Add("tipo", "T");
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
                Microsoft.Office.Interop.Excel.Application appExcel = new Microsoft.Office.Interop.Excel.Application();
                Microsoft.Office.Interop.Excel.Workbook workbook = appExcel.Workbooks.Add();
                workbook.SaveAs(direccion);
                workbook.Close();
            }

            SetHeaders();
            this.archivo = archivo;
            this.dir = direccion;
            dHeaders["cicloDefault"] = ciclo;
            dHeaders["tipoDefault"] = tipo;
        }

        /// <summary>
        /// Lee el excel y regresa las hojas en una coleccion de tablas
        /// </summary>
        /// <returns>Hojas del excel</returns>
        private DataTableCollection GetSheets()
        {
            //Es necesaria para que se inicialize ContCoumn
            FileStream stream = File.Open(dir, FileMode.Open, FileAccess.Read);

            IExcelDataReader excelReader;
            if (dir.Contains(".xlsx"))
                // De lo contrario descomentaremos esta (2007 format; *.xlsx)
                excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            else if (dir.Contains(".xls"))
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
                res.Add(new Grupo(r, dHeaders,dDefault));

            return res;
        }
        #endregion

        #region Comandos
        /// <summary>
        /// Escribe la informacion de los grupos
        /// </summary>
        /// <param name="grupos">Nombre de la hoja que se quiere poner </param>
        /// <param name="hoja">Hoja en la que se va a escribir</param>
        public void EscribeGrupos(IList<Grupo> grupos, string hoja, IDictionary<string, string> materia = null, IDictionary<int, string> profesor = null)
        {
            var workbook = new XLWorkbook(dir);
            //Abre la hoja seleccionada y de no existir la crea
            IXLWorksheet worksheet;

            try
            {
                worksheet = workbook.Worksheets.Worksheet(hoja);
            }catch
            {
                List<string> keyList = new List<string>(dHeaders.Keys);
                worksheet = workbook.Worksheets.Add(hoja);

                //Escribe los encabezados de las hojas
                for (int i = 1; i <= dHeaders.Count + 1; i++)
                {
                    if (i < dHeaders.Count)
                        worksheet.Cell(1,i).Value = dHeaders[keyList[i-1]];
                    else if (i < dHeaders.Count + 1)
                        worksheet.Cell(1,i).Value = "observaciones";
                }
            }

            foreach (Grupo item in grupos)
                EscribeGrupo(item,worksheet,materia,profesor);

            workbook.SaveAs(dir);
            workbook.Dispose();
        }

        /// <summary>
        /// Escribe un grupo en la hoja dada
        /// </summary>
        /// <param name="g">Grupo a escribir</param>
        /// <param name="worksheet">Hoja a modificar</param>
        public void EscribeGrupo(Grupo g, IXLWorksheet worksheet,IDictionary<string,string> materia, IDictionary<int,string> profesor)
        {
            //Inserta una fila al principio desplazando lo demas hacia abajo
            //De esa manera siempre escribes en la primera fila
            worksheet.Row(1).InsertRowsBelow(1);

            worksheet.Cell(2,1).Value = g.Cve_materia;
            worksheet.Cell(2,2).Value = g.num_Grupo;
            worksheet.Cell(2,3).Value = materia != null ? materia[g.Cve_materia] : (Convert.ToInt32(g.Cve_materia) * 100 + g.num_Grupo).ToString();
            worksheet.Cell(2,4).Value = profesor !=null ? profesor[g.RPE] : g.RPE.ToString();
            worksheet.Cell(2,5).Value = g.Tipo;
            worksheet.Cell(2,6).Value = g.Salon;
            worksheet.Cell(2,7).Value = g.horario_ini[0];
            worksheet.Cell(2,8).Value = g.horario_fin[0];
            worksheet.Cell(2,9).Value = g.horario_ini[1];
            worksheet.Cell(2,10).Value = g.horario_fin[1];
            worksheet.Cell(2,11).Value = g.horario_ini[2];
            worksheet.Cell(2,12).Value = g.horario_fin[2];
            worksheet.Cell(2,13).Value = g.horario_ini[3];
            worksheet.Cell(2,14).Value = g.horario_fin[3];
            worksheet.Cell(2,15).Value = g.horario_ini[4];
            worksheet.Cell(2,16).Value = g.horario_fin[4];
            worksheet.Cell(2,17).Value = g.horario_ini[5];
            worksheet.Cell(2,18).Value = g.horario_fin[5];
            worksheet.Cell(2,19).Value = g.Cupo;
            worksheet.Cell(2,20).Value = g.Ciclo;
            worksheet.Cell(2,21).Value = g.observaciones;
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

            return dt != null ? AsList(dt) : new List<Grupo>();
        }
        #endregion
    }
}