using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Xml.Serialization;

namespace TP3_IA
{
    public class Cellule
    {
        int valCellule;
        bool[] impossibles = new bool[9];

        #region properties
        public int Val
        {
            get { return valCellule; }
            set { valCellule = value; }
        }

        public bool[] Impossibles
        {
            get { return impossibles; }
            set { impossibles = value; }
        }
        #endregion

        public Cellule(int valeurCel)
        {
            valCellule = valeurCel;
            initialise();
        }

        public string toString()
        {
            return Convert.ToString(Val);
        }

        public void deleteValue(int value)
        {
            impossibles[value] = !impossibles[value];
        }

        public void initialise()
        {
            if (this.Val != 0)
            {
                for (int i = 0; i < 9; i++)
                {
                    impossibles[i] = false;
                }
            }
            else
            {
                for (int i = 0; i < 9; i++)
                {
                    impossibles[i] = true;
                }
            }
        }

        public bool valequals(Cellule C)
        {
            bool cellsequals = false;
            if (this.Val == C.Val)
            {
                cellsequals = true;
            }
            return cellsequals;
        }

        public bool impequals(Cellule C)
        {
            bool cellsequals = false;
            if (this.Impossibles == C.Impossibles)
            {
                cellsequals = true;
            }
            return cellsequals;
        }

        public int nbtrue()
        {
            int nbtrue = 0;
            for (int chiffre = 0; chiffre < 9; chiffre++)
            {
                if (this.Impossibles[chiffre] == true)
                {
                    nbtrue++;
                }
            }
            return nbtrue;
        }

    }
}
