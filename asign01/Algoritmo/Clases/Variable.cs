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

        public float fCiclo
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

            return sel != null ? sel.Cve_espacio : "";
        }

        /// <summary>
        /// Retorna los puntos que se tiene con el equipo instalado en el salon y los que necesita el grupo
        /// </summary>
        public float puntosEquipo
        {
            get
            {
                if (salon == null) return 0;
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
            if (v.salon != null)
                Salon = new Salon(v.salon);
            else
                Salon = null; 
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
                Cve_espacio = salon_fijo;

                return true;
            }

            return false;
        }

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
            if (salones_Posibles.busca(salon.Cve_espacio) != null)
                peso = 10;
            //Checa si ya habia sido asignado en ese salon un horario anterior
            else if (GHoraAnterior != null && GHoraAnterior.Cve_espacio == salon.Cve_espacio)
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
        /// Checa si hay empalme con el grupo que se pasa por parametro
        /// </summary>
        /// <param name="grupo">Grupo a checar si hay empalme</param>
        /// <returns>Regresa true si hay un empalme entre los grupos.</returns>
        public bool empalme(Grupo grupo)
        {
            if (this == grupo)
                return false;

            Variable v = new Variable(grupo, 0);

            if ((lunes_ini >= v.lunes_ini && lunes_ini < v.lunes_fin) ||
                (lunes_fin <= v.lunes_fin && lunes_fin > v.lunes_ini))
                return true;

            if ((martes_ini >= v.martes_ini && martes_ini < v.martes_fin) ||
                (martes_fin <= v.martes_fin && martes_fin > v.martes_ini))
                return true;

            if ((miercoles_ini >= v.miercoles_ini && miercoles_ini < v.miercoles_fin) ||
                (miercoles_fin <= v.miercoles_fin && miercoles_fin > v.miercoles_ini))
                return true;

            if ((jueves_ini >= v.jueves_ini && jueves_ini < v.jueves_fin) ||
                (jueves_fin <= v.jueves_fin && jueves_fin > v.jueves_ini))
                return true;

            if ((viernes_ini >= v.viernes_ini && viernes_ini < v.viernes_fin) ||
                (viernes_fin <= v.viernes_fin && viernes_fin > v.viernes_ini))
                return true;

            if ((sabado_ini >= v.sabado_ini && sabado_ini < v.sabado_fin) ||
                (sabado_fin <= v.sabado_fin && sabado_fin > v.sabado_ini))
                return true;

            return false;
        }

        public bool EnHora(int ini, int fin, string dias = "111111")
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

        public bool EnHora(int[] ini, int[] fin)
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

        public bool EnDias(string dias="111111")
        {
            for (int i = 0; i < 6; i++)
                if ((dias[i] == '1' && this.Dias[i] == '1') || dias[i] == '0')
                    return true;

            return false;
        }

        public bool EnDia(int dia)
        {
            if (this.Dias[dia] == '1')
                return true;

            return false;
        }
    }
}