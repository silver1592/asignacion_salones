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
        protected int cupo;//[cupo] [int] NOT NULL,
        protected int inscritos;//[inscritos] [int] NOT NULL,
        protected string salon;//[salon] [varchar](60) NULL,
        protected int rpe;//[rpe] [int] NULL,
        protected string ciclo;//[ciclo] [varchar](20) NOT NULL

        //Salon asignado en la base de datos antes 
        protected string salonBD;

        protected List<Requerimiento_Valor> requerimientos_Salon;
        protected bool plantaBaja;
        protected string salon_fijo;

        protected ListaSalones salones_Posibles;
        protected ListaSalones salonesAnteriores;
        #endregion

        #region gets y sets
        /// <summary>
        /// Convierte el ciclo a numeros faciles de usar en un condicional para saber cual es mayor
        /// Se empieza desde el 2014-2015/I al cual se le asigna 0 y de ai en adelante aumenta en 0.5
        /// </summary>
        public List<Requerimiento_Valor> Requerimientos { get { return requerimientos_Salon; } }
        public bool PlantaBaja { get { return plantaBaja; } }
        public string Salon_fijo { get { return salon_fijo; } }
        public ListaSalones Salones_posibles { get { return salones_Posibles; } }
        public float Ciclo
        {
            get
            {
                string aux;
                int yIni;
                int yFin;
                float semPar = .5f;

                if (ciclo.Contains("/II"))
                {
                    aux = ciclo.Replace("/II", "");
                    semPar = 2;
                }
                else
                    aux = ciclo.Replace("/I", "");

                yIni = Convert.ToInt32(aux.Split('-')[0]);
                yFin = Convert.ToInt32(aux.Split('-')[1]);

                if (yIni >= 2014)
                    return (yIni - 2014 + semPar);
                else
                    return -1;
            }
        }
        public string Salon { get { return salon; } set { salon = value; } }
        public string SalonBD { get { return salonBD; } }
        public string observaciones;

        public int hora_ini
        {
            get
            {
                if (lunes_ini != 0)
                    return lunes_ini;
                else if (martes_ini != 0)
                    return martes_ini;
                else if (miercoles_ini != 0)
                    return miercoles_ini;
                else if (jueves_ini != 0)
                    return jueves_ini;
                else if (viernes_ini != 0)
                    return viernes_ini;
                else
                    return sabado_ini;
            }
        }        
        public string Area { get { return cve_materia.Substring(0, 1); } }
        public int Cupo { get { return cupo; } }

        public bool AsignacionSemestresAnteriores(string salon)
        {
            Salon s = salonesAnteriores.busca(salon);

            return s!=null ? true : false;
        }

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
        public bool[] dias(int hora)
        {
            bool[] res = { false,false,false,false,false,false};

            for(int i=0;i<6;i++)
                if (horario_ini[i] >= hora && hora + 1 >= horario_fin[i])
                    res[i] = true;

            return res;
        }
        public int catDias
        {
            get
            {
                int i = 0;

                foreach (int b in horario_ini)
                    if (b!=0)
                        i++;

                return i;
            }
        }
        public string Dias
        {
            get
            {
                string res = "";

                if (horario_ini[0] != 0)
                    res += "L";
                else
                    res += "-";

                if (horario_ini[1] != 0)
                    res += "M";
                else
                    res += "-";

                if (horario_ini[2] != 0)
                    res += "m";
                else
                    res += "-";

                if (horario_ini[3] != 0)
                    res += "J";
                else
                    res += "-";

                if (horario_ini[4] != 0)
                    res += "V";
                else
                    res += "-";

                if (horario_ini[5] != 0)
                    res += "S";
                else
                    res += "-";

                return res;
            }
        }
        public ListaGrupos GruposAnteriores;

        /// <summary>
        /// Cadena para el query de update.
        /// </summary>
        public string modificacion
        {
            get
            {
                return "UPDATE [asignacion].[ae_horario] "
                              + "SET[salon] = '" + salon + "'"
                              + " WHERE[cve_materia] = '" + cve_materia + "' and [grupo] = " + grupo.ToString() + " and [tipo] = '" + tipo + "' and [ciclo] = '" + ciclo + "';";
            }
        }
        #endregion

        #region Constructores
        #region Inicializadores
        public void Set_From_Row(DataRow grupo)
        {
            this.cve_materia = grupo["cve_materia"].ToString();
            this.grupo = Convert.ToInt32(grupo["grupo"].ToString());
            this.tipo = grupo["tipo"].ToString();
            lunes_ini = Convert.ToInt32(grupo["lunes_ini"].ToString());
            lunes_fin = Convert.ToInt32(grupo["lunes_fin"].ToString());
            martes_ini = Convert.ToInt32(grupo["martes_ini"].ToString());
            martes_fin = Convert.ToInt32(grupo["martes_fin"].ToString());
            miercoles_ini = Convert.ToInt32(grupo["miercoles_ini"].ToString());
            miercoles_fin = Convert.ToInt32(grupo["miercoles_fin"].ToString());
            jueves_ini = Convert.ToInt32(grupo["jueves_ini"].ToString());
            jueves_fin = Convert.ToInt32(grupo["jueves_fin"].ToString());
            viernes_ini = Convert.ToInt32(grupo["viernes_ini"].ToString());
            viernes_fin = Convert.ToInt32(grupo["viernes_fin"].ToString());
            sabado_ini = Convert.ToInt32(grupo["sabado_ini"].ToString());
            sabado_fin = Convert.ToInt32(grupo["sabado_fin"].ToString());
            cupo = Convert.ToInt32(grupo["cupo"].ToString());
            inscritos = Convert.ToInt32(grupo["inscritos"].ToString());
            salon = grupo["salon"].ToString();
            salonBD = salon;
            rpe = Convert.ToInt32(grupo["rpe"].ToString());
            this.ciclo = grupo["ciclo"].ToString();
        }

        public void Set_NecesidadesGrupo(DataTable dt)
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

        public void Set_RequerimientosProfesor(DataTable dt)
        {
            //DataTable np = cn.Necesidades_Profesor(this.rpe);
            if (dt.Rows.Count == 1)
            {
                plantaBaja = Convert.ToBoolean(dt.Rows[0]["discapacidad"].ToString());
                salon_fijo = dt.Rows[0]["salon_unico"].ToString();
            }
        }

        public void Set_SalonesPosibles(DataTable dt, ListaSalones salones)
        {
            //DataTable sPosibles = cn.salonesPosibles(cve_materia);
            salones_Posibles = new ListaSalones(dt);
        }

        /// <summary>
        /// Busca los grupos en los que dio clases el profesor anteriormente
        /// </summary>
        protected void Set_GruposAnteriores(DataTable dt)
        {
            ListaGrupos Grupos = new ListaGrupos();
            //DataTable dGrupos = Consultas.GruposAnteriores(rpe, hora, ciclo);
            /*
            foreach (DataRow r in dt.Rows)
                Grupos.Add(new Grupo(r["cve_materia"].ToString(), Convert.ToInt32(r["grupo"].ToString()), r["tipo"].ToString(), r["ciclo"].ToString()));
                */
            GruposAnteriores = Grupos;
        }
        #endregion

        //Construcctor base para los heredados
        public Grupo() { }

        /// <summary>
        /// Constructor usado cuando se crea a partir de un excel
        /// </summary>
        /// <param name=""></param>
        public Grupo(string cve_materia,string grupo, string rpe, string tipo, string salon, string li, string lf,string mi, string mf, string Mii,string Mif, string ji, string jf, string vi, string vf, string si, string sf, string cupo, string ciclo)
        {
            //cn = new Conexion(Conexion.datosConexionPrueba);

            this.cve_materia = cve_materia;
            this.grupo = Convert.ToInt32(grupo);
            this.tipo = tipo;
            lunes_ini = Convert.ToInt32(li);
            lunes_fin = Convert.ToInt32(lf);
            martes_ini = Convert.ToInt32(mi);
            martes_fin = Convert.ToInt32(mf);
            miercoles_ini = Convert.ToInt32(Mii);
            miercoles_fin = Convert.ToInt32(Mif);
            jueves_ini = Convert.ToInt32(ji);
            jueves_fin = Convert.ToInt32(jf);
            viernes_ini = Convert.ToInt32(vi);
            viernes_fin = Convert.ToInt32(vf);
            sabado_ini = Convert.ToInt32(si);
            sabado_fin = Convert.ToInt32(sf);

            this.cupo = Convert.ToInt32(cupo);
            inscritos = 0;
            this.salon = salon!=null ? salon : "";
            salonBD = salon;
            this.rpe = Convert.ToInt32(rpe);
            this.ciclo = ciclo;

            requerimientos_Salon = new List<Requerimiento_Valor>();
            salones_Posibles = new ListaSalones();

            GruposAnteriores = new ListaGrupos();
        }

        public Grupo(DataRow grupo, DataTable necesidadesGrupo=null, DataTable necesidadesProfesor=null, DataTable salonesPosibles=null, ListaSalones salones=null)
        {
            Set_NecesidadesGrupo(necesidadesGrupo);
            Set_RequerimientosProfesor(necesidadesProfesor);
            Set_SalonesPosibles(salonesPosibles, salones);

            GruposAnteriores = new ListaGrupos();
        }

        public Grupo(Grupo g)
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
            salon = g.salon;
            salonBD = g.salonBD;
            rpe = g.rpe;
            ciclo = g.ciclo;

            requerimientos_Salon = new List<Requerimiento_Valor>();
            Requerimiento_Valor req;
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

            GruposAnteriores = g.GruposAnteriores;
        }

        public Grupo (DataRow r, Conexion c, ListaSalones salones)
        {
            Set_From_Row(r);

            Set_GruposAnteriores(c.GruposAnteriores(RPE, hora_ini, ciclo));
            Set_NecesidadesGrupo(c.Necesidades_Grupo(Cve_materia, tipo, rpe.ToString()));
            Set_RequerimientosProfesor(c.Necesidades_prof(RPE.ToString()));
            Set_SalonesPosibles(c.salonesPosibles(cve_materia), salones);
        }

        public Grupo(DataRow r, IDictionary<string, string> h)
        {
            try
            {
                try
                {
                    ///Se usan estos si se tiene el grupo y la materia por separado
                    cve_materia = Convert.ToString(r.Field<double>(h["cve_mat"]));
                    grupo = Convert.ToInt32(Convert.ToString(r.Field<double>(h["cve_gpo"])));
                }
                catch
                {
                    ///Se usa este si el grupo y la materia estan juntos
                    cve_materia = Convert.ToString(r.Field<double>(h["cve"])).Substring(0, 4);
                    grupo = Convert.ToInt32(Convert.ToString(r.Field<double>(h["cve"])).Substring(4, 2));
                }

                rpe = Convert.ToInt32(Convert.ToString(r.Field<double>(h["cverpe"])));
                tipo = h["tipoDefault"] == "" ? r.Field<string>(h["tipo"]) : h["tipoDefault"];
                salon = r.Field<string>(h["salon"]);
                lunes_ini = Convert.ToInt32(Convert.ToString(r.Field<double>(h["lunes"])));
                lunes_fin = Convert.ToInt32(Convert.ToString(r.Field<double>(h["lunesf"])));
                martes_ini = Convert.ToInt32(Convert.ToString(r.Field<double>(h["martes"])));
                martes_fin = Convert.ToInt32(Convert.ToString(r.Field<double>(h["martesf"])));
                miercoles_ini = Convert.ToInt32(Convert.ToString(r.Field<double>(h["miercoles"])));
                miercoles_fin = Convert.ToInt32(Convert.ToString(r.Field<double>(h["miercolesf"])));
                jueves_ini = Convert.ToInt32(Convert.ToString(r.Field<double>(h["jueves"])));
                jueves_fin = Convert.ToInt32(Convert.ToString(r.Field<double>(h["juevesf"])));
                viernes_ini = Convert.ToInt32(Convert.ToString(r.Field<double>(h["viernes"])));
                viernes_fin = Convert.ToInt32(Convert.ToString(r.Field<double>(h["viernesf"])));
                sabado_ini = Convert.ToInt32(Convert.ToString(r.Field<double>(h["sabado"])));
                sabado_fin = Convert.ToInt32(Convert.ToString(r.Field<double>(h["sabadof"])));
                cupo = Convert.ToInt32(Convert.ToString(r.Field<double>(h["cupo"])));
                ciclo = h["cicloDefault"];
            }
            catch
            {
                throw new Exception("Formato no valido\n Cheque que los encabezados coinsidan");
            }
        }
        #endregion

        /// <summary>
        /// Regresa cual es el valor que tiene un salon para el area del grupo.
        /// 02/06/2016--Decidi definir el -1 como un valor completamente incorrecto y que fuerse al sistema a ignorar esta asignacion.
        /// </summary>
        /// <param name="salon"></param>
        /// <returns></returns>
        public float SalonValido(Salon salon)
        {
            float peso = -1;

            //Checa si esta en la lista de posibles salones o si esta en uno de los salones anteriores
            if (salones_Posibles.busca(salon.Cve_espacio)!=null)
                peso = 10;
            ///Checa si ya habia sido asignado en ese salon un horario anterior
            else if (en_Anteriores(salon))
                peso = 10;
            //Y si no esta....
            //Checa si corresponden las areas
            //Si hay cupo para el salon
            //Si tienen que ser en un salon de la planta baja o no
            else if (salon.Area.Contains(Area)
                        && salon.Cupo >= cupo
                        && !(plantaBaja && !salon.plantaBaja))
                peso = salon.PrioridadArea(Area);

            return peso;
        }

        /// <summary>
        /// Checa si el salon esta dentro de los salones anteriores del grupo
        /// </summary>
        /// <param name="salon"></param>
        /// <returns></returns>
        public bool en_Anteriores(Salon salon)
        {
            /*
            var query = from g in GruposAnteriores
                        where g.salon == salon.Cve_espacio
                        select g;

            if (query.Count() != 0)
                return true;
            else*/
                return false;
                
        }

        public bool EnDias(string dias)
        {
            for (int i = 0; i < 6; i++)
                if (Dias[i] != dias[i] && dias[i] == '1')
                    return false;

            return true;
        }

        /// <summary>
        /// Checa si hay empalme con el grupo que se pasa por parametro
        /// </summary>
        /// <param name="grupo">Grupo a checar si hay empalme</param>
        /// <returns>Regresa true si hay un empalme entre los grupos.</returns>
        public bool empalme(Grupo grupo)
        {
            if (this == grupo)
                return false;

            if ((lunes_ini >= grupo.lunes_ini && lunes_ini < grupo.lunes_fin) ||
                (lunes_fin <= grupo.lunes_fin && lunes_fin > grupo.lunes_ini))
                return true;

            if ((martes_ini >= grupo.martes_ini && martes_ini < grupo.martes_fin) ||
                (martes_fin <= grupo.martes_fin && martes_fin > grupo.martes_ini))
                return true;

            if ((miercoles_ini >= grupo.miercoles_ini && miercoles_ini < grupo.miercoles_fin) ||
                (miercoles_fin <= grupo.miercoles_fin && miercoles_fin > grupo.miercoles_ini))
                return true;

            if ((jueves_ini >= grupo.jueves_ini && jueves_ini < grupo.jueves_fin) ||
                (jueves_fin <= grupo.jueves_fin && jueves_fin > grupo.jueves_ini))
                return true;

            if ((viernes_ini >= grupo.viernes_ini && viernes_ini < grupo.viernes_fin) ||
                (viernes_fin <= grupo.viernes_fin && viernes_fin > grupo.viernes_ini))
                return true;

            if ((sabado_ini >= grupo.sabado_ini && sabado_ini < grupo.sabado_fin) ||
                (sabado_fin <= grupo.sabado_fin && sabado_fin > grupo.sabado_ini))
                return true;

            return false;
        }

        public bool empalme(int ini, int fin, string dias)
        {
            if (dias[0] == '1')
                if ((lunes_ini >= ini && lunes_ini < fin) ||
                    (lunes_fin <= fin && lunes_fin > ini))
                    return true;

            if (dias[1] == '1')
                if ((martes_ini >= ini && martes_ini < fin) ||
                (martes_fin <= fin && martes_fin > ini))
                    return true;

            if (dias[2] == '1')
                if ((miercoles_ini >= ini && miercoles_ini < fin) ||
                (miercoles_fin <= fin && miercoles_fin > ini))
                    return true;

            if (dias[3] == '1')
                if ((jueves_ini >= ini && jueves_ini < fin) ||
                (jueves_fin <= fin && jueves_fin > ini))
                    return true;

            if (dias[4] == '1')
                if ((viernes_ini >= ini && viernes_ini < fin) ||
                (viernes_fin <= fin && viernes_fin > ini))
                    return true;

            if (dias[5] == '1')
                if ((sabado_ini >= ini && sabado_ini < fin) ||
                (sabado_fin <= fin && sabado_fin > ini))
                    return true;

            return false;
        }

        public bool empalme(int[] ini, int[] fin)
        {
            if ((lunes_ini >= ini[0] && lunes_ini < fin[0]) ||
                (lunes_fin <= fin[0] && lunes_fin > ini[0]))
                return true;

            if ((martes_ini >= ini[1] && martes_ini < fin[1]) ||
            (martes_fin <= fin[1] && martes_fin > ini[1]))
                return true;

            if ((miercoles_ini >= ini[2] && miercoles_ini < fin[2]) ||
            (miercoles_fin <= fin[2] && miercoles_fin > ini[2]))
                return true;

            if ((jueves_ini >= ini[3] && jueves_ini < fin[3]) ||
            (jueves_fin <= fin[3] && jueves_fin > ini[3]))
                return true;

            if ((viernes_ini >= ini[4] && viernes_ini < fin[4]) ||
            (viernes_fin <= fin[4] && viernes_fin > ini[4]))
                return true;

            if ((sabado_ini >= ini[5] && sabado_ini < fin[5]) ||
            (sabado_fin <= fin[5] && sabado_fin > ini[5]))
                return true;

            return false;
        }

        public bool EligeSalon(ListaSalones salones, Salon _Salon)
        {
            //Checa si hay salones que esten especialmente relacionados con el
            ListaSalones preferentes = salones.Preferenciales(this);
            ListaSalones posibes = salones.Validos(this);

            if (preferentes.busca(_Salon.Cve_espacio) != null)
            {
                salon = _Salon.Cve_espacio;
                _Salon.agregaGrupo(this);

                return true;
            }
            else
            {

                if (posibes.busca(_Salon.Cve_espacio) != null)
                {
                    salon = _Salon.Cve_espacio;
                    return true;
                }

                return false;
            }
        }

        public override string ToString()
        {
            return (Convert.ToInt32(cve_materia) * 100 + grupo) + "\t" + salon;
        }

        public static float cicloToFloat(string ciclo)
        {
            string aux;
            int yIni;
            int yFin;
            float semPar = .5f;

            if (ciclo.Contains("/II"))
            {
                aux = ciclo.Replace("/II", "");
                semPar = 2;
            }
            else
                aux = ciclo.Replace("/I", "");

            yIni = Convert.ToInt32(aux.Split('-')[0]);
            yFin = Convert.ToInt32(aux.Split('-')[1]);

            if (yIni >= 2014)
                return (yIni - 2014 + semPar);
            else
                return -1;
        }
    }
}