using System;
using System.Collections.Generic;

namespace OkeyLibrary
{
    public partial class OkeyGame
    {
        List<OkeyPiece> allPieces;

        public OkeyGame(byte setCount, byte fakePieceCount, byte cueCount, byte regularCuePiecesCount, byte maxCuePiecesCount, byte maxPieceValue)
        {
            SetCount = setCount;
            FakePieceCount = fakePieceCount;
            CueCount = cueCount;
            RegularCuePiecesCount = regularCuePiecesCount;
            MaxCuePiecesCount = maxCuePiecesCount;
            MaxPieceValue = maxPieceValue;

            GameTable = new OkeyTable(setCount, fakePieceCount, cueCount, regularCuePiecesCount, maxCuePiecesCount, maxPieceValue);
        }

        public byte SetCount { get; set; }
        public byte FakePieceCount { get; set; }
        public byte CueCount { get; set; }
        public byte RegularCuePiecesCount { get; set; }
        public byte MaxCuePiecesCount { get; set; }
        public byte MaxPieceValue { get; set; }

        public OkeyTable GameTable { get; set; }

        /// <summary>
        /// deals all pieces onto table and cues
        /// </summary>
        public void Deal()
        {
            allPieces = new List<OkeyPiece>();
            for (int i = 0; i < GameTable.TotalPieceCount; i++) allPieces.Add(new OkeyPiece());

            SetAllPieces();

            SetOkeyPiece();

            DealPieces();

            allPieces = null;
        }

        /// <summary>
        /// Place pieces onto cues and table
        /// </summary>
        private void DealPieces()
        {
            byte i = 0;

            // deal cues
            foreach (OkeyCue cue in GameTable.Cues)
            {
                foreach (OkeyPiece piece in cue.Pieces)
                {
                    piece.IsFake = allPieces[i].IsFake;
                    piece.IsOkey = allPieces[i].IsOkey;
                    piece.IsGosterge = allPieces[i].IsGosterge;
                    piece.PieceColor = allPieces[i].PieceColor;
                    piece.PieceValue = allPieces[i].PieceValue;

                    i++;
                }
            }

            // set 1st table piece as gosterge
            GameTable.TablePieces[0].IsFake = false;
            GameTable.TablePieces[0].IsOkey = false;
            GameTable.TablePieces[0].IsGosterge = true;
            GameTable.TablePieces[0].PieceColor = GameTable.GostergeColor;
            GameTable.TablePieces[0].PieceValue = GameTable.GostergeValue;

            // set other pieces onto table
            for (byte j = 1; j < GameTable.TablePieces.Count; j++)
            {
                GameTable.TablePieces[j].IsFake = allPieces[i].IsFake;
                GameTable.TablePieces[j].IsOkey = allPieces[i].IsOkey;
                GameTable.TablePieces[j].IsGosterge = allPieces[i].IsGosterge;
                GameTable.TablePieces[j].PieceColor = allPieces[i].PieceColor;
                GameTable.TablePieces[j].PieceValue = allPieces[i].PieceValue;

                i++;
            }
        }

        /// <summary>
        /// randomize and set gostege and okey pieces
        /// </summary>
        private void SetOkeyPiece()
        {
            Random rnd = new Random();
            int gostergeIndex = rnd.Next(0, GameTable.TotalPieceCount); // creates random number between 0 and max pieces count

            while (allPieces[gostergeIndex].IsFake)
            {
                gostergeIndex = rnd.Next(0, GameTable.TotalPieceCount);
            }

            GameTable.GostergeValue = allPieces[gostergeIndex].PieceValue.Value;
            GameTable.GostergeColor = allPieces[gostergeIndex].PieceColor.Value;

            // remove gosterge
            allPieces.RemoveAt(gostergeIndex);

            //GameTable.OkeyValue = Convert.ToByte((GameTable.GostergeValue + 1) % MaxPieceValue);

            // set okey & gosterge pieces in list
            foreach (OkeyPiece piece in allPieces)
            {
                if (!piece.IsFake)
                {
                    if (piece.PieceColor.Value == GameTable.OkeyColor && piece.PieceValue.Value == GameTable.OkeyValue)
                    {
                        piece.IsOkey = true;
                    }

                    if (piece.PieceColor.Value == GameTable.GostergeColor && piece.PieceValue.Value == GameTable.GostergeValue)
                    {
                        piece.IsGosterge = true;
                    }
                }
                else
                {
                    piece.PieceColor = GameTable.OkeyColor;
                    piece.PieceValue = GameTable.OkeyValue;
                }
            }
        }

        /// <summary>
        /// create all pieces of the game included fake pieces
        /// </summary>
        private void SetAllPieces()
        {
            int piecesColorCount = Enum.GetNames(typeof(OkeyPieceColor)).Length;
            int c = 0;

            // set all pieces with colors and values
            for (byte i = 0; i < SetCount; i++)
            {
                for (byte j = 0; j < piecesColorCount; j++)
                {
                    for (byte k = 0; k < MaxPieceValue; k++)
                    {
                        allPieces[c].PieceValue = k;
                        allPieces[c].PieceColor = (OkeyPieceColor)j;
                        c++;
                    }
                }
            }

            // add fake pieces
            allPieces[c].IsFake = true;
            allPieces[c + 1].IsFake = true;

            // shuffle list
            allPieces.Shuffle();
        }

        public override string ToString()
        {
            string result = "";

            result += "Gösterge: " + GameTable.GostergeColor + " " + (GameTable.GostergeValue + 1) + Environment.NewLine;
            result += "Okey: " + GameTable.OkeyColor + " " + (GameTable.OkeyValue + 1) + Environment.NewLine + Environment.NewLine;

            result += GameTable.ToString();

            return result;
        }
    }
}