using System;
using System.Collections.Generic;

namespace OkeyLibrary
{
    /// <summary>
    /// Store pieces
    /// </summary>
    public class OkeyCue
    {
        public OkeyCue(byte pieceCount)
        {
            Pieces = new List<OkeyPiece>();

            for (byte i = 0; i < pieceCount; i++)
            {
                Pieces.Add(new OkeyPiece());
            }
        }

        /// <summary>
        /// Pieces property
        /// </summary>
        public List<OkeyPiece> Pieces { get; set; }

        /// <summary>
        /// Returns cue has okey or not
        /// </summary>
        public bool HasOkey
        {
            get
            {
                foreach (OkeyPiece piece in Pieces)
                {
                    if (piece.IsOkey)
                        return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Returns cue has okey or not
        /// </summary>
        public byte OkeyCount
        {
            get
            {
                byte okeyCount = 0;

                foreach (OkeyPiece piece in Pieces)
                {
                    if (piece.IsOkey)
                        okeyCount++;
                }

                return okeyCount;
            }
        }

        public override string ToString()
        {
            string result = "";

            for (int i = 0; i < Pieces.Count; i++)
            {
                result += (i + 1).ToString("D2") + ". Taş: " + Pieces[i].PieceColor.ToString() + " " + (Pieces[i].PieceValue.Value + 1).ToString();

                if (Pieces[i].IsFake)
                    result += " (Sahte Okey)";

                if (Pieces[i].IsGosterge)
                    result += " (Gösterge)";

                if (Pieces[i].IsOkey)
                    result += " (Okey)";

                result += Environment.NewLine;
            }

            return result;
        }
    }
}
