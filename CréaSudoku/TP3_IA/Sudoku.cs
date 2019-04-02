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
using System.Reflection;

namespace TP3_IA
{
    class Sudoku
    {
        public Cellule[,] grille = new Cellule[9, 9];

        #region Constructeurs
        /// <summary>
        /// Dans ce constructeur, on utilise la matrice d'entiers"en dur" créée dans le main en paramètre.
        /// Pour chacune des 81 cases de la matrice, on crée une nouvelle cellule bénéficiant des attributs de valeur de la case et de sa liste de chiffre impossibles.
        /// </summary>
        /// <param name="grilletab"></param Grille d'entiers en dur dans le main>
        public Sudoku(int[,] grilletab)
        {
            for (int ligne = 0; ligne < grille.GetLength(0); ligne++)
            {
                for (int colonne = 0; colonne < grille.GetLength(1); colonne++)
                {
                    this.grille[ligne, colonne] = new Cellule(grilletab[ligne, colonne]);
                }
            }
        }

        public StreamReader OpenFile(string File)
        {
            StreamReader flux = null;
            try
            {
                flux = new StreamReader(File);

            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }

            return flux;

        }
        /// <summary>
        /// Ici, même principe de construction que dans le premier constructeur.
        /// Pour chaque entier issu d'une ligne du fichier, on construit une cellule.
        /// </summary>
        /// <param name="grillefichier"></param Grille au format csv>
        public Sudoku(string grillefichier)
        {
            StreamReader gfich = new StreamReader(grillefichier);
            string line;
            int i = 0;
            char[] separateur = { ';' };
            try
            {
                while ((line = gfich.ReadLine()) != null && i < 9)
                {
                    string[] s = line.Split(separateur);

                    for (int colonne = 0; colonne < grille.GetLength(1); colonne++)
                    {
                        this.grille[i, colonne] = new Cellule(int.Parse(s[colonne]));
                    }
                    i++;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (grillefichier != null) { gfich.Close(); }
            }
        }
        #endregion

        /// <summary>
        /// Le but de cette méthode est de compléter les tableaux de valeurs impossibles de chaque cellule.
        /// Il n'est pas question ici de résolution de la grille car on n'affecte pas de valeur à une cellule vide.
        /// Si la case étudiée est non nulle on passe à "faux" sa valeur dans le tableau des Impossible pour chaque case de sa ligne, colonne ou région de 3x3.
        /// On test si la case étudiée est différente de 0;
        ///     Si oui, on parcourt les cases de sa ligne et de sa colonne;
        ///         Si la case est vide ET qu'elle n'est pas déjà à "faux" pour le chiffre en question; //ceci permet de ne pas répeter une même consigne 3 ou 4 fois lorsque la combinaison ligne/colonne donne la même case
        ///             On lui renseigne "faux" pour le chiffre en question;
        /// Même raisonnement pour la région 3x3.
        /// </summary>
        /// <param name="ligne"></param Permet de parcourir les lignes>
        /// <param name="colonne"></param Permet de parcourir les colonnes>
        /// <param name="ligcol"></param Permet de parcourir les lignes de la colonne ou les colonnes de la ligne de la case étudiée>
        /// <param name="lig"></param Fixe avec "col" le point de départ de la région dans laquelle on se trouve>
        public void ini2(int ligne, int colonne)
        {
            if (grille[ligne, colonne].Val != 0)
            {
                for (int ligcol = 0; ligcol < 9; ligcol++) //traitement des cellules de la ligne ou de la colonne
                {
                    if (grille[ligcol, colonne].Val == 0 && grille[ligcol, colonne].Impossibles[grille[ligne, colonne].Val - 1] != false)
                    {
                        grille[ligcol, colonne].deleteValue(grille[ligne, colonne].Val - 1);
                    }
                    if (grille[ligne, ligcol].Val == 0 && grille[ligne, ligcol].Impossibles[grille[ligne, colonne].Val - 1] != false)
                    {
                        grille[ligne, ligcol].deleteValue(grille[ligne, colonne].Val - 1);
                    }
                }
                int lig = ligne - (ligne % 3);
                int col = colonne - (colonne % 3);
                for (int i = lig; i < lig + 3; i++) //traitement par région de 3x3
                {
                    for (int j = col; j < col + 3; j++)
                    {
                        if (grille[i, j].Val == 0 && grille[i, j].Impossibles[grille[ligne, colonne].Val - 1] != false)
                        {
                            grille[i, j].deleteValue(grille[ligne, colonne].Val - 1);
                        }
                    }
                }
            }

        }

        /// <summary>
        /// Dans cette méthode, on parcourt toutes les cases de la grille à la recherche des cases vides et on les compte.
        /// Si on compte plus de 0 case vide, c'est que la grille n'est pas résolue.
        /// On retourne donc l'information "faux" à la question "résolue ?".
        /// </summary>
        /// <returns></returns Resolu à vrai si aucune case vide>
        public bool resolu()
        {
            bool resolu = false;
            int comptcasesvides = 0;
            for (int ligne = 0; ligne < 9; ligne++)
            {
                for (int colonne = 0; colonne < 9; colonne++)
                {
                    if (grille[ligne, colonne].Val == 0)
                    {
                        comptcasesvides++;
                    }
                }
            }
            if (comptcasesvides == 0)
            {
                resolu = true;
            }
            return resolu;
        }

        /// <summary>
        /// On parcourt les cases de la grille à la recherche de cases vide qui ne peuvent plus contenir qu'un seul chiffre.
        ///     Dans une case, pour chaque chiffre on regarde si on peut encore le mettre et on compte combien de chiffre on peut encore mettre. On note le chiffre possible
        ///         Si on dénombre un seul chiffre possible, la case peut être remplie par celui-ci
        /// On réinitialise le compteur de nombre possible ainsi que la case dans laquelle se trouve le dernier nombre possible.
        /// Lorsqu'on trouve une telle case :
        /// *On lui affecte sa dernière valeur possible
        /// *On met à jour la grille : à présent qu'une case comporte une valeur, les autres cases de la ligne/colonne/région ne peuvent plus la comporter
        /// *On passe toutes les valeurs de la cellule à "faux" (même si il n'en restait plus qu'une)
        /// </summary>
        /// <param name="compttrue"></param Compte le nombre de chiffre encore possible pour la case>
        /// <param name="casetrue"></param Note le chiffre possible>


        #region Méthodes d'affichage

        /// <summary>
        /// Pour afficher la grille, on utilise une chaine de caractères faite d'entiers, d'espaces et de barres à intervale régulier pour l'apparence.
        /// La varriable "aff" pour "afficher" regroupe cet assemblage. Il s'agit d'un string, on peut donc concatener tour à tour les éléments.
        /// Cette méthode retourne une variable string que l'on peut afficher à l'aide de Console.WriteLine.
        /// </summary>
        /// <returns></returns>
        public string toString()
        {
            string aff = null;
            if (grille != null)
            {
                aff += "\n";
                for (int ligne = 0; ligne < 9; ligne++)
                {
                    for (int colonne = 0; colonne < 9; colonne++)
                    {
                        if (grille[ligne, colonne].Val != 0)
                        {
                            aff = aff + grille[ligne, colonne].toString() + " ";
                        }
                        else
                        {
                            aff += "  ";
                        }
                        if (colonne == 2 || colonne == 5)
                        {
                            aff += " | ";
                        }
                    }
                    aff += "\n";
                    if (ligne == 2 || ligne == 5)
                    {
                        aff += "_______________________";
                        aff += "\n";
                        aff += "\n";
                    }
                }
            }
            return aff;
        }

        public void WriteFile(string grilleresolue)
        {
            string[] lines = new string[9];
            for (int ligne = 0; ligne < 9; ligne++)
            {
                string line = "";
                for (int colonne = 0; colonne < 9; colonne++)
                {
                    line += Convert.ToString(grille[ligne, colonne].Val) + " ";
                    if (colonne == 2 || colonne == 5)
                    {
                        line += " | ";
                    }
                }
                lines[ligne] = line;
            }
            File.WriteAllLines(grilleresolue, lines);
        }
        #endregion

        public void Vider(string niveau)
        {
            int comptvide;
            byte[] al = new byte[81];
            Random random = new Random();
            random.NextBytes(al);
            
            if (niveau == "facile") { comptvide = 41; }
            else { comptvide = 55; }

                for (int ligne = 0; ligne < grille.GetLength(0); ligne++)
                {
                    for (int colonne = 0; colonne < grille.GetLength(1) && comptvide != 0; colonne++)
                    {
                        //Console.WriteLine(al[9 * ligne + colonne]);
                        if (grille[ligne, colonne].Val != 0 && al[9*ligne+colonne]%2==0)
                        {

                            grille[ligne, colonne].Val = 0;
                           comptvide--;

                        }

                    }
                }
            }

        }

    }




