using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using OrigenDatos.Estructuras;

namespace OrigenDatos.Clases
{
    public class Grupo
    {
        #region atributos
        protected string cve_materia; //[cve_materia] [varchar](4)
        protected int grupo; //[grupo] [int]
        protected string tipo; //[tipo] [varchar](1) NOT NULL
        protected int lunes_ini;//[lunes_ini] [int] NULL,
        protected int lunes_fin;//[lunes_fin] [int] NULL,
        protected int martes_ini;//[martes_ini] [int] NULL,
        protected int martes_fin;//[martes_fin] [int] NULL
        protected int miercoles_ini;//[miercoles_ini] [int] NULL,
        protected int miercoles_fin;//[miercoles_fin] [int] NULL,
        protected int jueves_ini;//[jueves_ini] [int] NULL,
        protected int jueves_fin;//[jueves_fin] [int] NULL,
        protected int viernes_ini;//[viernes_ini] [int] NULL,
        protected int viernes_fin;//[viernes_fin] [int] NULL,
        protected int sabado_ini;//[sabado_ini] [int] NULL,
        protected int sabado_fin;//[sabado_fin] [int] NULL,
        public int cupo;//[cupo] [int] NOT NULL,
        public int inscritos;//[inscritos] [int] NOT NULL,
        protected string cve_espacio;//[salon] [varchar](60) NULL,
        protected int rpe;//[rpe] [int] NULL,
        protected string ciclo;//[ciclo] [varchar](20) NOT NULL

        //Salon asignado en la base de datos antes 
        protected string salonBD;

        protected List<Requerimiento_Valor> requerimientos_Salon;
        protected bool plantaBaja;
        protected string salon_fijo;

        protected ListaSalones salones_Posibles;
        protected Grupo HoraAnterior;
        protected ListaGrupos otrosSemestres;
        #endregion

        #region gets y sets
        public string observaciones;
        public string Tipo { get { return tipo; } }
        /// <summary>
        /// Convierte el ciclo a numeros faciles de usar en un condicional para saber cual es mayor
        /// Se empieza desde el 2014-2015/I al cual se le asigna 0 y de ai en adelante aumenta en 0.5
        /// </summary>
        public bool PlantaBaja { get { return plantaBaja; } }
        public string Salon_fijo { get { return salon_fijo != null ? salon_fijo : ""; } }
        public ListaSalones Salones_posibles { get { return salones_Posibles; } }
        public string Ciclo { get { return ciclo; } }
        public string Cve_espacio { get { return cve_espacio != null ? cve_espacio.Trim() : ""; } set { cve_espacio = value; } }
        public string SalonBD { get { return salonBD; } }
        public Grupo GHoraAnterior { get { return HoraAnterior; } }
        public int hora_ini
        {
            get
            {
                int i = 0;
                foreach (int n in horario_ini)
                    if ((i > n && n != 0) || i == 0)
                        i = n;

                return i;
            }
        }         //Muestra la hora mas temprana en la que inicia el grupo
        public string Area { get { return cve_materia.Substring(0, 1); } }
        public int Cupo { get { return inscritos == 0 ? cupo : inscritos; } }
        public string Cve_materia { get { return cve_materia; } }
        public int num_Grupo { get { return grupo; } }
        public int RPE { get { return rpe; } }
        public int[] horario_ini
        {
            get
            {
                int[] res = new int[6];

                res[0] = lunes_ini;
                res[1] = martes_ini;
                res[2] = miercoles_ini;
                res[3] = jueves_ini;
                res[4] = viernes_ini;
                res[5] = sabado_ini;

                return res;
            }
        }
        public int[] horario_fin
        {
            get
            {
                int[] res = new int[6];

                res[0] = lunes_fin;
                res[1] = martes_fin;
                res[2] = miercoles_fin;
                res[3] = jueves_fin;
                res[4] = viernes_fin;
                res[5] = sabado_fin;

                return res;
            }
        }
        public int[,] horario
        {
            get
            {
                int[,] horario = new int[2, 6];

                for (int i = 0; i < 6; i++)
                    horario[0, i] = horario_ini[i];
                for (int i = 0; i < 6; i++)
                    horario[1, i] = horario_fin[i];

                return horario;

            }
        }
        public bool[] dias(int hora)
        {
            bool[] res = { false, false, false, false, false, false };

            for (int i = 0; i < 6; i++)
                if (horario_ini[i] >= hora && hora + 1 >= horario_fin[i])
                    res[i] = true;

            return res;
        }
        public string Dias
        {
            get
            {
                string res = "";

                if (horario_ini[0] != 0)
                    res += "1";
                else
                    res += "0";

                if (horario_ini[1] != 0)
                    res += "1";
                else
                    res += "0";

                if (horario_ini[2] != 0)
                    res += "1";
                else
                    res += "0";

                if (horario_ini[3] != 0)
                    res += "1";
                else
                    res += "0";

                if (horario_ini[4] != 0)
                    res += "1";
                else
                    res += "0";

                if (horario_ini[5] != 0)
                    res += "1";
                else
                    res += "0";

                return res;
            }
        }

        /// <summary>
        /// Cadena para el query de update.
        /// </summary>
        public string qUpdate(bool horario = true, bool salon = true, bool inscritos = true)
        {
            string query = "UPDATE [asignacion].[ae_horario] set ";

            if (horario || salon || inscritos)
            {
                if (salon)
                    query += "[salon] = '" + Cve_espacio + "',";
                if (inscritos)
                    query += "[inscritos] = " + this.inscritos + ",";
                if (horario)
                {
                    query += "[lunes_ini] = " + horario_ini[0] + ",";
                    query += "[lunes_fin] = " + horario_ini[0] + ",";
                    query += "[martes_ini] = " + horario_ini[0] + ",";
                    query += "[martes_fin] = " + horario_ini[0] + ",";
                    query += "[miercoles_ini] = " + horario_ini[0] + ",";
                    query += "[miercoles_fin] = " + horario_ini[0] + ",";
                    query += "[jueves_ini] = " + horario_ini[0] + ",";
                    query += "[jueves_fin] = " + horario_ini[0] + ",";
                    query += "[viernes_ini] = " + horario_ini[0] + ",";
                    query += "[viernes_fin] = " + horario_ini[0] + ",";
                    query += "[sabado_ini] = " + horario_ini[0] + ",";
                    query += "[sabado_fin] = " + horario_ini[0] + ",";
                }
            }
            else return "";

            query = query.Substring(0, query.Length - 1);

            query += " WHERE[cve_materia] = '" + cve_materia + "' and [grupo] = " + grupo.ToString() + " and [tipo] = '" + tipo + "' and [ciclo] = '" + ciclo + "';";

            return query;
        }

        public string qInsert
        {
            get
            {
                return "INSERT INTO [asignacion].[ae_horario] ([cve_materia],[grupo],[tipo],[lunes_ini],[lunes_fin],[martes_ini],[martes_fin],[miercoles_ini],[miercoles_fin],[jueves_ini],[jueves_fin],[viernes_ini],[viernes_fin],[sabado_ini],[sabado_fin],[cupo],[inscritos],[salon],[rpe],[lab_obl],[ciclo],[checador]) VALUES("
                    + "'" + cve_materia + "',"
                    + num_Grupo + ","
                    + "'" + tipo + "',"
                    + lunes_ini + ","
                    + lunes_fin + ","
                    + martes_ini + ","
                    + martes_fin + ","
                    + miercoles_ini + ","
                    + miercoles_fin + ","
                    + jueves_ini + ","
                    + jueves_fin + ","
                    + viernes_ini + ","
                    + viernes_fin + ","
                    + sabado_ini + ","
                    + sabado_fin + ","
                    + cupo + ","
                    + inscritos +","
                    + "'" + Cve_espacio + "',"
                    + rpe + ","
                    + "0,"
                    + "'" + ciclo + "',"
                    + "0)";
            }
        }

        public string cve_full { get { return (Convert.ToInt32(Cve_materia) * 100 + num_Grupo).ToString(); } }
        #endregion

        #region Constructores

        /// <summary>
        /// Constructor por copia
        /// </summary>
        /// <remarks>Es utilizado como base para ls metodos que heredan de la clase. 
        /// Ademas que permite volver a solicitar la informacion si esta no estaba cubierta</remarks>
        /// <param name="g"></param>
        /// <param name="c"></param>
        /// <param name="salones"></param>
        public Grupo(Grupo g,Conexion c=null, IList<Salon> salones =null)
        {
            cve_materia = g.cve_materia;
            grupo = g.grupo;
            tipo = g.tipo;
            lunes_ini = g.lunes_ini;
            lunes_fin = g.lunes_fin;
            martes_ini = g.martes_ini;
            martes_fin = g.martes_fin;
            miercoles_ini = g.miercoles_ini;
            miercoles_fin = g.miercoles_fin;
            jueves_ini = g.jueves_ini;
            jueves_fin = g.jueves_fin;
            viernes_ini = g.viernes_ini;
            viernes_fin = g.viernes_fin;
            sabado_ini = g.sabado_ini;
            sabado_fin = g.sabado_fin;
            cupo = g.cupo;
            inscritos = g.inscritos;
            Cve_espacio = g.Cve_espacio;
            salonBD = g.salonBD;
            rpe = g.rpe;
            ciclo = g.ciclo;

            if (c == null)
            {
                requerimientos_Salon = new List<Requerimiento_Valor>();
                Requerimiento_Valor req;
                if(g.requerimientos_Salon!=null)
                    foreach (Requerimiento_Valor ngR in g.requerimientos_Salon)
                    {
                        req = new Requerimiento_Valor();
                        req.requerimiento = ngR.requerimiento;
                        req.valor = ngR.valor;
                        requerimientos_Salon.Add(req);
                    }


                plantaBaja = g.plantaBaja;
                salon_fijo = g.salon_fijo;

                salones_Posibles = g.Salones_posibles;
                HoraAnterior = g.HoraAnterior;
                otrosSemestres = g.otrosSemestres;
            }
            else
            {
                Set_GruposHorasAnteriores(c.Grupos_HoraAnterior(RPE, hora_ini, ciclo), Conexion.DGruposBD);
                Set_NecesidadesGrupo(c.Grupo_Necesidades(Cve_materia, tipo, rpe.ToString()));
                Set_RequerimientosProfesor(c.Profesor_Necesidades(RPE.ToString()));
                Set_GruposOtrosSemestres(c.Grupos_SemestresAnteriores(cve_materia, ciclo, rpe.ToString(),num_Grupo,horario_ini), Conexion.DGruposBD);

                if (salones != null)
                    Set_SalonesPosibles(c.Salones_Posibles(cve_materia), salones);
                else
                    salones_Posibles = new ListaSalones();
            }
        }

        public Grupo(DataRow r, IDictionary<string, string> h,IDictionary<string,string> def=null, Conexion c=null, IList<Salon> salones=null)
        {
            Set_From_Row(r, h,def);

            if (c != null)
            {
                Set_GruposHorasAnteriores(c.Grupos_HoraAnterior(RPE, hora_ini, ciclo), Conexion.DGruposBD);
                Set_NecesidadesGrupo(c.Grupo_Necesidades(Cve_materia, tipo, rpe.ToString()));
                Set_RequerimientosProfesor(c.Profesor_Necesidades(RPE.ToString()));
                Set_GruposOtrosSemestres(c.Grupos_SemestresAnteriores(cve_materia, ciclo, rpe.ToString(),num_Grupo,horario_ini), Conexion.DGruposBD);

                if(salones!=null)
                    Set_SalonesPosibles(c.Salones_Posibles(cve_materia), salones);
                else
                    salones_Posibles = new ListaSalones();
            }
        }

        #region Inicializadores
        protected void Set_From_Row(DataRow r,IDictionary<string,string> h, IDictionary<string, string> def)
        {
            try
            {
                try
                {
                    ///Se usan estos si se tiene el grupo y la materia por separado
                    cve_materia = Convert.ToString(r.Field<object>(h["cve_mat"]));
                    grupo = Convert.ToInt32(Convert.ToString(r.Field<object>(h["cve_gpo"])));
                }
                catch
                {
                    try
                    {
                        ///Se usa este si el grupo y la materia estan juntos
                        cve_materia = r.Field<double>(h["cve"]).ToString().Substring(0, 4);
                        grupo = Convert.ToInt32(r.Field<double>(h["cve"]).ToString().Substring(4, 2));
                    }
                    catch
                    { throw new Exception("Los encabezados de grupo estan mal"); }
                }

                try
                {
                    rpe = Convert.ToInt32(Convert.ToString(r.Field<object>(h["cverpe"])));
                    Cve_espacio = r.Field<string>(h["salon"]);
                }
                catch
                { throw new Exception("Los encabezados de salon y rpe son incorrectos"); }

                try
                { lunes_ini = Convert.ToInt32(Convert.ToString(r.Field<object>(h["lunes"]))); }
                catch
                { lunes_ini = 0; }
                try
                { lunes_fin = Convert.ToInt32(Convert.ToString(r.Field<object>(h["lunesf"]))); }
                catch
                { lunes_fin = 0; }

                try
                { martes_ini = Convert.ToInt32(Convert.ToString(r.Field<object>(h["martes"]))); }
                catch
                { martes_ini = 0; }
                try
                { martes_fin = Convert.ToInt32(Convert.ToString(r.Field<object>(h["martesf"]))); }
                catch
                { martes_fin = 0; }

                try
                { miercoles_ini = Convert.ToInt32(Convert.ToString(r.Field<object>(h["miercoles"]))); }
                catch
                { miercoles_ini = 0; }
                try
                { miercoles_fin = Convert.ToInt32(Convert.ToString(r.Field<object>(h["miercolesf"]))); }
                catch
                { miercoles_fin = 0; }

                try
                { jueves_ini = Convert.ToInt32(Convert.ToString(r.Field<object>(h["jueves"]))); }
                catch
                { jueves_ini = 0; }
                try
                { jueves_fin = Convert.ToInt32(Convert.ToString(r.Field<object>(h["juevesf"]))); }
                catch
                { jueves_fin = 0; }

                try
                { viernes_ini = Convert.ToInt32(Convert.ToString(r.Field<object>(h["viernes"]))); }
                catch
                { viernes_ini = 0; }
                try
                { viernes_fin = Convert.ToInt32(Convert.ToString(r.Field<object>(h["viernesf"]))); }
                catch
                { viernes_fin = 0; }

                try
                { sabado_ini = Convert.ToInt32(Convert.ToString(r.Field<object>(h["sabado"]))); }
                catch
                { sabado_ini = 0; }
                try
                { sabado_fin = Convert.ToInt32(Convert.ToString(r.Field<object>(h["sabadof"]))); }
                catch
                { sabado_fin = 0; }

                try
                { inscritos = Convert.ToInt32(Convert.ToString(r.Field<object>(h["inscritos"]))); }
                catch
                { inscritos = 0; }
                try
                { tipo = r.Field<string>(h["tipo"]); }
                catch
                { tipo = def["tipo"]; }
                try
                { ciclo = Convert.ToString(r.Field<object>(h["ciclo"])); }
                catch
                { ciclo = def["ciclo"]; }

                try
                { cupo = Convert.ToInt32(Convert.ToString(r.Field<object>(h["cupo"]))); }
                catch
                { throw new Exception("Lo encabezados de cupo no son correctos"); }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "\nFormato no valido\n Cheque que los encabezados coincidan");
            }
        }

        protected void Set_NecesidadesGrupo(DataTable dt)
        {
            //DataTable ng = cn.Necesidades_Grupo(cve_materia, tipo, rpe);
            requerimientos_Salon = new List<Requerimiento_Valor>();
            Requerimiento_Valor req;

            foreach (DataRow ngR in dt.Rows)
            {
                req = new Requerimiento_Valor();
                req.requerimiento = Convert.ToInt32(ngR["idEquipo"].ToString());
                req.valor = Convert.ToInt32(ngR["peso"].ToString());
                requerimientos_Salon.Add(req);
            }
        }

        protected void Set_RequerimientosProfesor(DataTable dt)
        {
            //DataTable np = cn.Necesidades_Profesor(this.rpe);
            if (dt.Rows.Count == 1)
            {
                plantaBaja = Convert.ToBoolean(dt.Rows[0]["discapacidad"].ToString());
                salon_fijo = dt.Rows[0]["salon_unico"].ToString();
            }
        }

        protected void Set_SalonesPosibles(DataTable dt, IList<Salon> salones)
        {
            ListaSalones lSalones = new ListaSalones(salones);
            salones_Posibles = lSalones.EnTabla(dt);
        }

        /// <summary>
        /// Busca los grupos en los que dio clases el profesor anteriormente
        /// </summary>
        protected void Set_GruposHorasAnteriores(DataTable dt,IDictionary<string,string> dicctionary)
        {
            ListaGrupos GruposHorasAnteriores = new ListaGrupos();
            ListaGrupos aux;
            foreach (DataRow r in dt.Rows)
                GruposHorasAnteriores.Add(new Grupo(r, dicctionary));

            aux = GruposHorasAnteriores.IniciaEnHora(hora_ini-1);
            HoraAnterior = aux.Count() != 0 ? aux[0] : null;
        }

        protected void Set_GruposOtrosSemestres(DataTable dt,IDictionary<string,string> dictionary)
        {
            otrosSemestres = new ListaGrupos();
            foreach (DataRow r in dt.Rows)
                otrosSemestres.Add(new Grupo(r, dictionary));
        }
        #endregion
        #endregion

        #region Metodos
        public override string ToString()
        {
            return (Convert.ToInt32(cve_materia) * 100 + grupo) + "_" + Cve_espacio+"_"+ciclo;
        }

        public Grupo AsignacionSemestresAnteriores(string salon)
        {
            if (otrosSemestres == null)
                return null;

            ListaGrupos g = otrosSemestres.EnSalon(salon);

            return g.Count() != 0 ? g[0] : null;
        }

        public bool EnHora(int hora)
        {
            if (lunes_ini >= hora && lunes_fin > hora)
                return true;

            if ((martes_ini >= hora &&  martes_fin > hora))
                return true;

            if ((miercoles_ini >= hora && miercoles_fin > hora))
                return true;

            if ((jueves_ini >= hora && jueves_fin > hora))
                return true;

            if ((viernes_ini >= hora && viernes_fin > hora))
                return true;

            if ((sabado_ini >= hora && sabado_fin > hora))
                return true;

            return false;
        }
        #endregion
    }
}