using System;
using System.Collections.Generic;

namespace OkeyLibrary
{
    public class OkeyTable
    {
        public OkeyTable(byte setCount, byte fakePieceCount, byte cueCount, byte regularCuePiecesCount, byte maxCuePiecesCount, byte maxPieceValue)
        {
            Cues = new List<OkeyCue>();
            TablePieces = new List<OkeyPiece>();

            // one player has maximum number of pieces, other has regular amount of pieces.
            Cues.Add(new OkeyCue(maxCuePiecesCount));

            for (byte i = 0; i < cueCount - 1; i++)
            {
                Cues.Add(new OkeyCue(regularCuePiecesCount));
            }

            int piecesColorCount = Enum.GetNames(typeof(OkeyPieceColor)).Length;
            int maxPieceCount = (setCount * maxPieceValue * piecesColorCount) + fakePieceCount;
            int cuePiecesCount = ((cueCount - 1) * regularCuePiecesCount) + maxCuePiecesCount;
            int tablePiecesCount = maxPieceCount - cuePiecesCount;

            for (int i = 0; i < tablePiecesCount; i++)
            {
                TablePieces.Add(new OkeyPiece());
            }
        }

        public List<OkeyCue> Cues { get; set; }
        public List<OkeyPiece> TablePieces { get; set; }

        public byte GostergeValue { get; set; }
        public OkeyPieceColor GostergeColor { get; set; }

        public byte OkeyValue
        {
            get
            {
                if (GostergeValue == 12)
                    return 0;
                else
                    return Convert.ToByte(GostergeValue + 1);
            }
        }

        public OkeyPieceColor OkeyColor
        {
            get
            {
                return GostergeColor;
            }
        }

        public int TotalCuePieceCount
        {
            get
            {
                int cuePiecesCount = 0;

                foreach (OkeyCue cue in Cues)
                {
                    cuePiecesCount += cue.Pieces.Count;
                }

                return cuePiecesCount;
            }
        }

        public int TotalPieceCount
        {
            get
            {
                return TablePieces.Count + TotalCuePieceCount;
            }
        }

        public override string ToString()
        {
            string result = "";

            for (int i = 0; i < Cues.Count; i++)
            {
                result += (i + 1).ToString("D2") + ". Istaka ---------" + Environment.NewLine + Cues[i].ToString() + Environment.NewLine;
            }

            result += "Yer Taşları ---------" + Environment.NewLine;

            for (int i = 0; i < TablePieces.Count; i++)
            {
                result += (i + 1).ToString("D2") + ". Taş: " + TablePieces[i].PieceColor.ToString() + " " + (TablePieces[i].PieceValue.Value + 1).ToString();

                if (TablePieces[i].IsFake)
                    result += " (Sahte Okey)";

                if (TablePieces[i].IsGosterge)
                    result += " (Gösterge)";

                if (TablePieces[i].IsOkey)
                    result += " (Okey)";

                result += Environment.NewLine;
            }

            result += Environment.NewLine;

            return result;
        }
    }
}
