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
    class Program
    {
        

        public static void Affichage(int[,] grille)
        {
            if (grille != null)
            {
                Console.WriteLine();
                for (int ligne = 0; ligne < 9; ligne++)
                {
                    for (int colonne = 0; colonne < 9; colonne++)
                    {
                        Console.Write(grille[ligne, colonne] + " ");
                        if (colonne == 2 || colonne == 5)
                        {
                            Console.Write(" | ");
                        }
                    }
                    Console.WriteLine();
                    if (ligne == 2 || ligne == 5)
                    {
                        Console.WriteLine("_______________________");
                        Console.WriteLine();
                    }
                }
            }
        }


        static void Main(string[] args)
        {
            int[,] grille1 = { { 2, 5, 4, 7, 9, 6, 3, 1, 8 },
                              { 6,1,9,3,8,2,5,7,4 },
                              {3,7,8,4,1,5,6,2,9},
                              { 5,9,3,6,2,1,4,8,7 },
                              { 4,2,7,9,3,8,1,6,5 },
                              { 8,6,1,5,7,4,2,9,3 },
                              { 1,8,5,2,4,9,7,3,6 },
                              { 9,3,6,1,5,7,8,4,2 },
                              {7,4,2,8,6,3,9,5,1} };

            int[,] grille2 = { { 9,  3,  4,  1,  7,  2,  6,  8,  5 },
                              {7,  6,  5,  8,  9,  3,  2,  4,  1 },
                              {8,  1,  2,  6,  4,  5,  3,  9,  7 },
                              {4,  2,  9,  5,  8,  1,  7,  6,  3  },
                              {6,  5,  8,  7,  3,  9,  1,  2,  4  },
                              {1 , 7 , 3 , 4 , 2 , 6  ,8 , 5,  9},
                              {2 , 9 , 7 , 3 , 5,  8 , 4 , 1 , 6 },
                              {5  ,4 , 6,  2,  1 , 7 , 9 , 3 , 8 },
                              {3 , 8 , 1 , 9 , 6 , 4 , 5 , 7 , 2} };

            ConsoleKeyInfo cki = Console.ReadKey();
            bool swap = false;
            int[,] grille;
            while (cki.Key != ConsoleKey.Escape)
            {
                Console.Clear();
                Console.WriteLine("********************************************************" + "\n"
                                 + "   Bienvenu dans la page de création de grille sudoku" + "\n"
                                 + "********************************************************" + "\n");

                if (swap == false) grille = grille1;
                else grille = grille2;
                Sudoku s = new TP3_IA.Sudoku(grille);
                swap = !swap;
                for (int ligne = 0; ligne < 9; ligne++)
                {
                    for (int colonne = 0; colonne < 9; colonne++)
                    {
                        s.ini2(ligne, colonne);
                    }
                }

                Console.WriteLine("Appuyez sur entrée");
                Console.ReadKey();
                Console.WriteLine("Voici une nouvelle grille de sudoku :");

                s.Vider("difficile");
                Console.Write(s.toString());
                Console.ReadKey();
                Console.WriteLine("Souhaitez-vous exporter la grille au format .txt ?");
                Console.WriteLine("Si oui, tapez le nom du fichier et .txt (ex : grilleresolue.txt) ; si non tapez non");
                string grilleresolue = Console.ReadLine();
                if (grilleresolue != "non")
                {
                    s.WriteFile(grilleresolue);
                }
                Console.WriteLine("Merci !");
                Console.WriteLine("Appuyez sur echap pour sortir ou sur entrée pour rejouer");
                cki = Console.ReadKey();

                
            }

        }
    }
}
