using OkeyLibrary;
using System;
using System.Configuration;

namespace OkeyApplication
{
    class Program
    {
        private static readonly byte setCount = byte.Parse(ConfigurationManager.AppSettings["MaxSetCount"].ToString());
        private static readonly byte fakePieceCount = byte.Parse(ConfigurationManager.AppSettings["MaxFakePieceCount"].ToString());
        private static readonly byte cueCount = byte.Parse(ConfigurationManager.AppSettings["MaxCueCount"].ToString());
        private static readonly byte regularCuePiecesCount = byte.Parse(ConfigurationManager.AppSettings["RegularCuePiecesCount"].ToString());
        private static readonly byte maxCuePiecesCount = byte.Parse(ConfigurationManager.AppSettings["MaxCuePiecesCount"].ToString());
        private static readonly byte maxPieceValue = byte.Parse(ConfigurationManager.AppSettings["MaxPieceValue"].ToString());

        static void Main(string[] args)
        {
            OkeyGame game = new OkeyGame(setCount, fakePieceCount, cueCount, regularCuePiecesCount, maxCuePiecesCount, maxPieceValue);

            game.Deal();

            Console.Write(game.ToString());

            OkeyCue bestCue = game.FindBestCue();
            Console.WriteLine("En İyi Istaka: " + (game.GameTable.Cues.FindIndex(a => a == bestCue) + 1).ToString() + ". Istaka");
            Console.Write(bestCue.ToString());

            Console.Read();
        }
    }
}
