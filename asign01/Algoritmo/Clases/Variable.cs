using OrigenDatos.Clases;
using OrigenDatos.Estructuras;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Algoritmo02.Clases
{
    public class Variable : Grupo
    {
        private Salon salon;
        private int Hora;

        private float fCiclo
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
                    return ((yIni - 2014) * 10 + semPar);
                else
                    return -1;
            }
        }

        public Salon Salon
        {
            get { return salon; }
            set
            {
                salon = value;

                if (salon != null)
                {
                    Cve_espacio = salon.Cve_espacio;
                }
                else
                    Cve_espacio = "";
            }
        }

        /// <summary>
        /// Retorna cuantos son los puntos que tiene esta asignacion para el algoritmo
        /// </summary>
        public float puntos
        {
            get
            {
                if (salon != null)
                {
                    return calcula_PuntosSalon(salon);
                }
                else
                    return 0;
            }
        }

        public string salonAnioPasado()
        {
            Variable sel = null;
            Variable v;
            foreach (Grupo g in otrosSemestres)
            {
                v = new Variable(g, 0);
                if (v.fCiclo % 2 == this.fCiclo % 2)
                    if (sel == null || sel.fCiclo < v.fCiclo)
                        sel = v;
            }

            return sel != null ? sel.ciclo : "";
        }

        /// <summary>
        /// Retorna los puntos que se tiene con el equipo instalado en el salon y los que necesita el grupo
        /// </summary>
        public float puntosEquipo
        {
            get
            {
                float pT = this.valorTotalEquipo;
                float pE = this.valorEquipo(salon);

                if (pT == 0)
                    return 1;

                return pE / pT;
            }
        }

        #region Constuctores
        /// <summary>
        /// Constructor de una variable (cromosoma) del individuo
        /// --No es necesario asignarle un salon
        /// </summary>
        /// <param name="salon">Salon que va a representar</param>
        public Variable(Grupo grupo, int hora):base(grupo)
        {
            Hora = hora;
            salon = null;
        }

        public Variable(Variable v):base(v)
        {
            Salon = new Salon(v.salon);
            Hora = v.Hora;
        }
        #endregion

        /// <summary>
        /// checa si un grupo es compatible con el salon
        /// </summary>
        /// <param name="salon">salon a checar compatibilidad</param>
        /// <returns>Si el grupo es valido para asignarse en este salon</returns>
        public bool valido(Salon salon)
        {
            bool res = false;

            if (this.SalonValido(salon) > 0)
                res = true;

            return res;
        }

        /// <summary>
        /// Calcula los puntos que tiene el grupo con el salon solicitado
        /// </summary>
        /// <param name="s">Salon con que se quiere checar</param>
        /// <returns>Valor que se obtubo de los puntos. \nSi es -1 quiere decir que no es un salon valido</returns>
        public float calcula_PuntosSalon(Salon s)
        {
            //CHECA SI EL PROFESOR TIENE UN SALON FIJO Y CHECA SI ES EL SALON AL QUE SE LE ASIGNO, Y DE SER ASI REGRESA EL VALOR INMEDIATAMENTE, SI NO 
            if (this.Salon_fijo != null && this.Salon_fijo != "")
            if (this.Salon_fijo == s.Cve_espacio)
                return 10;
            else
                return -1;

            //INICIALIZA EL VALOR DEL GRUPO AL VALOR QUE LE DA EL AREA.
            float fRes = this.SalonValido(s);

            //CHECA SI EL PROFESOR REQUERE ESTAR EN LA PLANTA BAJA Y SI EL SALON TAMBIEN ESTA EN LA PLANTA BAJA
            //La validacion comentada es para cuando se haya instalado un elevador en el edificio T
            if (this.PlantaBaja && (!s.plantaBaja /*|| salon.Edificio == "T"*/))
                fRes = -1;

            if (fRes > -1)
                fRes *= puntosEquipo;

            return fRes;
        }

        public float valorEquipo(Salon salon)
        {
            int res = 0;

            foreach (Requerimiento_Valor req in requerimientos_Salon)
                if (salon.Equipo.Contains(req.requerimiento))
                    res += req.valor;

            return res;
        }

        public bool horario(int hora)
        {
            if (lunes_ini != 0 && lunes_ini <= hora && lunes_fin > hora)
                return true;
            if (martes_ini != 0 && martes_ini <= hora && martes_fin > hora)
                return true;
            if (miercoles_ini != 0 && miercoles_ini <= hora && miercoles_fin > hora)
                return true;
            if (jueves_ini != 0 && jueves_ini <= hora && jueves_fin > hora)
                return true;
            if (viernes_ini != 0 && viernes_ini <= hora && viernes_fin > hora)
                return true;
            if (sabado_ini != 0 && sabado_ini <= hora && sabado_fin > hora)
                return true;

            return false;
        }

        public int valorTotalEquipo
        {
            get
            {
                int res = 0;

                foreach (Requerimiento_Valor req in requerimientos_Salon)
                    res = req.valor;

                return res;
            }
        }

        /// <summary>
        /// Checa si tiene un grupo con salon fijo, y de ser asi lo asigna, sino regresa false
        /// </summary>
        /// <returns></returns>
        public bool salonPreferencial()
        {
            if (salon_fijo != null && salon_fijo != "")
            {
                cve_espacio = salon_fijo;

                return true;
            }

            return false;
        }
    }
}