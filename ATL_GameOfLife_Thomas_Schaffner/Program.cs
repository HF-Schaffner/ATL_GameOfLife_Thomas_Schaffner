// Author Thomas Schaffner
// Erstellt am 04.01.2022
// HF-ICT FR19 
using System;
using System.Diagnostics;
using System.Threading;

namespace ATL_GameOfLife_Thomas_Schaffner
{
    class Programm
    {
        static void Main(string[] args)
        {
            // Standardwerte definieren
            int spalte = 10;
            int zeile = 10;
            int maxGen = 10;
            int momentanegeneration;
            bool parallel;

            // Stopuhr initialisieren
            Stopwatch stopWatch = new Stopwatch();

            // Spiel initialisieren
            GameOfLife spiel = new GameOfLife();

            Console.WriteLine("Hallo das ist das Game of Life spiel programmiert von Thomas Schaffner es könnnen nun die Parameter gesetzt werden um das Spiel zu konfigurieren");
            Console.WriteLine("Default Werte für das Spiel sind [10x10 Spielfeld] mit [10 Generationen] im Normalbetrieb, dies kann jedoch übersteuert werden.");

            Console.WriteLine("Spielbreite eingeben");
            string userSpalte = Console.ReadLine();         // Input einlesen
            if (userSpalte != "")                           // Abfangen ob nicht eingegeben wurde 
            {
                spalte = int.Parse(userSpalte);             // Input String in Int umwandeln 
            }
            else
            {
                Console.WriteLine("Default 10 verwenden");
            }
            Console.WriteLine("Spielhöhe eingeben");
            string userZeile = Console.ReadLine();          // Input einlesen
            if (userZeile != "")                            // Abfangen ob nicht eingegeben wurde 
            {
                zeile = int.Parse(userZeile);               // Input String in Int umwandeln 
            }
            else
            {
                Console.WriteLine("Default 10 verwenden");
            }
            Console.WriteLine("Generationen eingeben");
            string userGen = Console.ReadLine();            // Input einlesen
            if (userGen != "")                              // Abfangen ob nicht eingegeben wurde
            {
                maxGen = int.Parse(userGen);                // Input String in Int umwandeln
            }
            else
            {
                Console.WriteLine("Default 10 verwenden");
            }
            Console.WriteLine("Verarbeitungtyp P = Parallel oder N = Normal eingeben");
            string verarbeitung = Console.ReadLine();       // Input einlesen
            if (verarbeitung == "P" || verarbeitung == "p") // wenn kleines oder grosses P eingegeben wurde auf Parallel betrieb umschalten
            {
                parallel = true;
            }
            else
            {
                parallel = false;
            }

            spiel.setSpielfeld(spalte, zeile);              // Spielfeld aufbauen
            spiel.randomSpielfeld();                        // Zufälliges Muster generieren
            spiel.printSpielfeld(spiel.getSpielfeld());     // Erstes Muster in Consolde drucken
            System.Console.WriteLine();                     // Leerschlag zwecks Übersicht
            System.Console.WriteLine("------------Gen0");   // Muster beschriften

            stopWatch.Start();                              // Stopuhr starten
            if (parallel == true)
            {
                /*
                 * Version mit 5 Thread
                 * Spalten berechnen indem max spalten durch 5 gerechnet wird 
                 * danach Anfang und Endspalten für die 5 Threads definieren
                 * 1 Thread 0 - SpaltenJeThread
                 * 2 Thread SpalteJeThread+1 - SpalteJeThread*2
                 * 3 Thread SpalteJeThread+SpaltejeThread*2 - SpalteJeThread*3
                 * 4 Thread SpalteJeThread+SpaltejeThread*3 - SpalteJeThread*4
                 * 5 Thread SpalteJeThread+SpaltejeThread*4 - MaxSpalte
                 * danach Thread starten und mittels Join sichergehen das alle berechnungen abgeschlossen sind bevor das Muster gedruckt wird
                 
                int spalteJe = 0;
                int spalteThreadende = 0;
                int spalteThreadstart = 0;

                spalteJe = spalte / 5; // 11 / 5 

                ThreadStart t1 = () => sp.berechneSpielfeldParallel(0, spalteJe); // 0 , 2
                spalteThreadstart = spalteJe + 1;
                spalteThreadende = spalteJe + spalteJe;

                ThreadStart t2 = () => sp.berechneSpielfeldParallel(spalteThreadstart, spalteThreadende); // 3 , 4
                spalteThreadstart = spalteThreadende + 1;
                spalteThreadende = spalteThreadende + spalteJe;

                ThreadStart t3 = () => sp.berechneSpielfeldParallel(spalteThreadstart, spalteThreadende); // 5 , 6
                spalteThreadstart = spalteThreadende + 1;
                spalteThreadende = spalteThreadende + spalteJe;

                ThreadStart t4 = () => sp.berechneSpielfeldParallel(spalteThreadstart, spalteThreadende); // 7 , 8
                spalteThreadstart = spalteThreadende + 1;

                ThreadStart t5 = () => sp.berechneSpielfeldParallel(spalteThreadstart, spalte); // 9 , 11

                */

                // Version mit 2 Thread
                // Max Spalten Durch 2 rechnen und aufteilen
                ThreadStart t1 = () => spiel.berechneSpielfeldParallel(0, spalte / 2);
                ThreadStart t2 = () => spiel.berechneSpielfeldParallel((spalte / 2) + 1, spalte);

                for (int i = 0; i < maxGen; i++)
                {
                    momentanegeneration = i + 1;

                    Thread t11 = new Thread(t1);
                    Thread t22 = new Thread(t2);

                    //Thread t33 = new Thread(t3);  // Version mit 5 Thread
                    //Thread t44 = new Thread(t4);  // Version mit 5 Thread
                    //Thread t55 = new Thread(t5);  // Version mit 5 Thread

                    t11.Start();
                    t22.Start();
                    //t33.Start();                  // Version mit 5 Thread
                    //t44.Start();                  // Version mit 5 Thread
                    //t55.Start();                  // Version mit 5 Thread

                    try
                    {
                        t11.Join();
                        t22.Join();
                        // t33.Join();                  // Version mit 5 Thread
                        // t44.Join();                  // Version mit 5 Thread
                        // t55.Join();                  // Version mit 5 Thread

                    }
                    catch (Exception ex)
                    {
                        System.Console.WriteLine("Exception, Details of the Exception:", ex);
                    }



                    spiel.aktualisiereSpielfeld();               // Spielfeld2 zu Spielfeld1 zuweisen für die neue Generation
                    spiel.printSpielfeld(spiel.getSpielfeld());  // Spielfeld drucken
                    System.Console.WriteLine();                  // Leerschlag zwecks Übersicht
                    System.Console.WriteLine("------------Gen" + momentanegeneration.ToString()); // Muster beschriften
                }
            }
            else
            {
                // Normal Betrieb
                for (int i = 0; i < maxGen; i++)
                {
                    momentanegeneration = i + 1;
                    spiel.berechneSpielfeld();                  // Spielfeld berechen
                    spiel.aktualisiereSpielfeld();              // Spielfeld2 zu Spielfeld1 zuweisen für die neue Generation
                    spiel.printSpielfeld(spiel.getSpielfeld()); // Spielfeld drucken
                    System.Console.WriteLine();                 // Leerschlag zwecks Übersicht
                    System.Console.WriteLine("------------Gen" + momentanegeneration.ToString()); // Muster beschriften
                }
            }
            stopWatch.Stop(); // Stopuhr stoppen
            
            TimeSpan ts = stopWatch.Elapsed; // Zeit welche die Stopuhr gelaufen ist auslesen.

            // Zeitspanne umformatieren
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Console.WriteLine("Laufzeit: " + elapsedTime); // Laufzeit ausdrucken
        }
    }

    // Spielfeld erstellen
    public class GameOfLife
    {
        // Globale Variablen definieren
        public int maxZeile;
        public int maxSpalte;
        public bool[,] spielfeld1;
        public bool[,] spielfeld2;

        /*
         * Grösse speichern wird für spätere Berechnungen gebbaucht
         */
        public void setMaxValues(int maxZeile, int maxSpalte) 
        {
            this.maxZeile = maxZeile - 1;     // -1 weil Array bei 0 anfangt
            this.maxSpalte = maxSpalte - 1;   // -1 weil Array bei 0 anfangt
        }

        /*
         * Spielfeld generieren Höhe und Breite festlegen für beide Spielfelder
         */
        public void setSpielfeld(int zeilen, int spalten)
        {
            setMaxValues(zeilen, spalten);
            this.spielfeld1 = new bool[zeilen, spalten];
            this.spielfeld2 = new bool[zeilen, spalten];

        }

        /*
         * Get Methode für Spielfeld 1
         */
        public bool[,] getSpielfeld()
        {
            return spielfeld1;
        }

        /*
         * Spielfeld drucken
         * Alle Zeilen durcharbeiten 
         * In jeder Zeile alle Spalten ddurcharbeiten
         */
        public void printSpielfeld(bool[,] spielfeld)
        {
            for (int k = 0; k < spielfeld.GetLength(0); k++)
            {
                if (k > 0)
                {
                    System.Console.WriteLine();                         // Zeilenumbruch
                }
                for (int l = 0; l < spielfeld.GetLength(1); l++)
                {
                    System.Console.Write(spielfeld[k, l] ? "X" : "O");  // Lebendige Spalten als X drucken und Tote als O drucken
                }
            }
        }

        /*
         * Zufallsmuster generien 
         * Zufallgenerator verwenden
         */
        public void randomSpielfeld()
        {
            Random rand = new Random();                         // Randomizer initialisieren
            try
            {
                for (int k = 0; k < spielfeld1.GetLength(0); k++)
                {
                    for (int l = 0; l < spielfeld1.GetLength(1); l++)
                    {
                        spielfeld1[k, l] = rand.Next(8) == 0 ? true : false; // Zahl zwischen 0 und 8 Ausgeben und wenn die Zahl 0 ist Position lebend
                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Exception, Details of the Exception:", ex);
            }
        }

        /*
         * Spielfeld berechnen
         * Jede Zelle prüfen
         */
        public void berechneSpielfeld()
        {
            for (int k = 0; k < spielfeld1.GetLength(0); k++)
            {
                for (int l = 0; l < spielfeld1.GetLength(1); l++)
                {
                    checkZelle(k, l); // Prüfen ob Zelle Tod oder Lebendig ist
                }
            }
        }

        /*
         * Spielfeld berechnen
         * Jede Zelle prüfen
         * Nur in einem gewissen Bereich prüfen
         * da bei der Methode CheckZelle das ganze Spielfeld betrachet wird ist die Berechnung auch mit eingeschränktem Bereich richtig ausgewertet
         */
        public void berechneSpielfeldParallel(int spalte_start, int spalte_ende)
        {
            for (int k = spalte_start; k < spalte_ende; k++)
            {
                for (int l = 0; l < spielfeld1.GetLength(1); l++)
                {
                    checkZelle(k, l); // Prüfen ob Zelle Tod oder Lebendig ist
                }
            }

        }

        /*
         * Zustände prüfen
         * 9 verschiedene Prüfzustände ermitteln
         * jeden Fall separat weiter bearbeiten
         */
        public void checkZelle(int zeile, int spalte)
        {
            if (zeile == 0 && spalte == 0)                                          // 1: Oben Links
            {
                checkZustaende("ObenLinks", zeile, spalte);
            }
            else if (zeile == 0 && (spalte >= 1 && spalte < maxSpalte))             // 2: Oberer Rand
            {
                checkZustaende("ObenRand", zeile, spalte);
            }
            else if (zeile == 0 && spalte == maxSpalte)                             // 3: Oben Rechts
            {
                checkZustaende("ObenRechts", zeile, spalte);
            }
            else if ((zeile >= 1 && zeile < maxZeile) && spalte == 0)               // 4: Linker Rand
            {
                checkZustaende("LinkerRand", zeile, spalte);
            }
            else if (zeile == maxZeile && spalte == 0)                              // 5: Unten Links
            {
                checkZustaende("UntenLinks", zeile, spalte);
            }
            else if (zeile == maxZeile && (spalte >= 1 && spalte < maxSpalte))      // 6: Unterer Rand
            {
                checkZustaende("UntererRand", zeile, spalte);
            }
            else if (zeile == maxZeile && spalte == maxSpalte)                      // 7: Unten Rechts
            {
                checkZustaende("UntenRechts", zeile, spalte);
            }
            else if ((zeile >= 1 && zeile < maxZeile) && spalte == maxSpalte)       // 8: Rechter Rand
            {
                checkZustaende("RechterRand", zeile, spalte);
            }
            else                                                                    // 9: Zentrum
            {
                checkZustaende("Zentrum", zeile, spalte);
            }
        }

        /*
         * Zustände aller Nachbare prüfen
         * Für jeden Fall separate definitionen
         * Anzahl lebende Nachbare zählen und an setZustand weitergeben
         */
        private void checkZustaende(string position, int zeile, int spalte)
        {
            bool nachbar;
            int anzahlLebend = 0;

            switch (position)
            {
                case "ObenLinks":  // 1
                    nachbar = spielfeld1[zeile + 1, spalte];           // Unten
                    if (nachbar) { anzahlLebend++; }
                    nachbar = spielfeld1[zeile, spalte + 1];           // Rechts
                    if (nachbar) { anzahlLebend++; }
                    nachbar = spielfeld1[zeile + 1, spalte + 1];       // UntenRechts
                    if (nachbar) { anzahlLebend++; }
                    setZustand(zeile, spalte, anzahlLebend);
                    break;

                case "ObenRand":    // 2
                    nachbar = spielfeld1[zeile, spalte - 1];           // Links
                    if (nachbar) { anzahlLebend++; }
                    nachbar = spielfeld1[zeile + 1, spalte - 1];       // UntenLinks
                    if (nachbar) { anzahlLebend++; }
                    nachbar = spielfeld1[zeile + 1, spalte];           // Unten
                    if (nachbar) { anzahlLebend++; }
                    nachbar = spielfeld1[zeile + 1, spalte + 1];       // UntenRechts
                    if (nachbar) { anzahlLebend++; }
                    nachbar = spielfeld1[zeile, spalte + 1];           // Rechts
                    if (nachbar) { anzahlLebend++; }
                    setZustand(zeile, spalte, anzahlLebend);
                    break;

                case "ObenRechts":  // 3
                    nachbar = spielfeld1[zeile, spalte - 1];           // Links
                    if (nachbar) { anzahlLebend++; }
                    nachbar = spielfeld1[zeile + 1, spalte - 1];       // UntenLinks
                    if (nachbar) { anzahlLebend++; }
                    nachbar = spielfeld1[zeile + 1, spalte];           // Unten
                    if (nachbar) { anzahlLebend++; }
                    setZustand(zeile, spalte, anzahlLebend);
                    break;

                case "LinkerRand":  // 4
                    nachbar = spielfeld1[zeile - 1, spalte];           // Oben
                    if (nachbar) { anzahlLebend++; }
                    nachbar = spielfeld1[zeile - 1, spalte + 1];       // ObenRechts
                    if (nachbar) { anzahlLebend++; }
                    nachbar = spielfeld1[zeile, spalte + 1];           // Rechts
                    if (nachbar) { anzahlLebend++; }
                    nachbar = spielfeld1[zeile + 1, spalte + 1];       // UntenRechts
                    if (nachbar) { anzahlLebend++; }
                    nachbar = spielfeld1[zeile + 1, spalte];           // Unten
                    if (nachbar) { anzahlLebend++; }
                    setZustand(zeile, spalte, anzahlLebend);
                    break;

                case "UntenLinks":  // 5
                    nachbar = spielfeld1[zeile - 1, spalte];           // Oben
                    if (nachbar) { anzahlLebend++; }
                    nachbar = spielfeld1[zeile - 1, spalte + 1];       // ObenRechts
                    if (nachbar) { anzahlLebend++; }
                    nachbar = spielfeld1[zeile, spalte + 1];           // Rechts
                    if (nachbar) { anzahlLebend++; }
                    setZustand(zeile, spalte, anzahlLebend);
                    break;

                case "UntererRand": // 6
                    nachbar = spielfeld1[zeile, spalte - 1];           // Links
                    if (nachbar) { anzahlLebend++; }
                    nachbar = spielfeld1[zeile - 1, spalte - 1];       // ObenLinks
                    if (nachbar) { anzahlLebend++; }
                    nachbar = spielfeld1[zeile - 1, spalte];           // Oben
                    if (nachbar) { anzahlLebend++; }
                    nachbar = spielfeld1[zeile - 1, spalte + 1];       // ObenRechts
                    if (nachbar) { anzahlLebend++; }
                    nachbar = spielfeld1[zeile, spalte + 1];           // Rechts
                    if (nachbar) { anzahlLebend++; }
                    setZustand(zeile, spalte, anzahlLebend);
                    break;

                case "UntenRechts": // 7
                    nachbar = spielfeld1[zeile, spalte - 1];           // Links
                    if (nachbar) { anzahlLebend++; }
                    nachbar = spielfeld1[zeile - 1, spalte - 1];       // ObenLinks
                    if (nachbar) { anzahlLebend++; }
                    nachbar = spielfeld1[zeile - 1, spalte];           // Oben
                    setZustand(zeile, spalte, anzahlLebend);
                    break;

                case "RechterRand": // 8
                    nachbar = spielfeld1[zeile - 1, spalte];           // Oben
                    if (nachbar) { anzahlLebend++; }
                    nachbar = spielfeld1[zeile - 1, spalte - 1];       // ObenLinks
                    if (nachbar) { anzahlLebend++; }
                    nachbar = spielfeld1[zeile, spalte - 1];           // Links
                    if (nachbar) { anzahlLebend++; }
                    nachbar = spielfeld1[zeile + 1, spalte - 1];       // UntenLinks
                    if (nachbar) { anzahlLebend++; }
                    nachbar = spielfeld1[zeile + 1, spalte];           // Unten
                    if (nachbar) { anzahlLebend++; }
                    setZustand(zeile, spalte, anzahlLebend);
                    break;

                default:            // 9
                    nachbar = spielfeld1[zeile - 1, spalte];           // Oben
                    if (nachbar) { anzahlLebend++; }
                    nachbar = spielfeld1[zeile - 1, spalte - 1];       // ObenLinks
                    if (nachbar) { anzahlLebend++; }
                    nachbar = spielfeld1[zeile, spalte - 1];           // Links
                    if (nachbar) { anzahlLebend++; }
                    nachbar = spielfeld1[zeile + 1, spalte - 1];       // UntenLinks
                    if (nachbar) { anzahlLebend++; }
                    nachbar = spielfeld1[zeile + 1, spalte];           // Unten
                    if (nachbar) { anzahlLebend++; }
                    nachbar = spielfeld1[zeile + 1, spalte + 1];       // UntenRechts
                    if (nachbar) { anzahlLebend++; }
                    nachbar = spielfeld1[zeile, spalte + 1];           // Rechts
                    if (nachbar) { anzahlLebend++; }
                    nachbar = spielfeld1[zeile - 1, spalte + 1];       // ObenRechts
                    if (nachbar) { anzahlLebend++; }
                    setZustand(zeile, spalte, anzahlLebend);
                    break;
            }
        }

        /*
         *  Zustand setzen anhang der lebenden Nachbaren
         */ 
        private void setZustand(int zeile, int spalte, int anzahlLebend)
        {
            switch (anzahlLebend)
            {
                case 0:
                    spielfeld2[zeile, spalte] = false;                      // alle tod also sterben oder todbleiben
                    break;
                case 1:
                    spielfeld2[zeile, spalte] = false;                      // nur 1 Nachbar lebend also sterben oder todbleiben
                    break;
                case 2:
                    spielfeld2[zeile, spalte] = spielfeld1[zeile, spalte];  // 2 lebend also gleicher Zustand
                    break;
                default:
                    spielfeld2[zeile, spalte] = true;                       // mehr als 2 lebend also neu geboren werden oder lebend bleiben
                    break;
            }
        }

        /*
         * Spielfeld aktualisieren 
         * zuweisung von neu berechnetem Spielfeld zum bestehenden für die nächste Generation
         */
        public void aktualisiereSpielfeld()
        {
            spielfeld1 = spielfeld2;
        }
    }
}






