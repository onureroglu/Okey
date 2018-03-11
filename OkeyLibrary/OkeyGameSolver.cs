using System;
using System.Collections.Generic;
using System.Linq;

namespace OkeyLibrary
{
    public class SeriesPiece
    {
        public SeriesPiece()
        {
            Pieces = new List<OkeyPiece>();
        }

        public SeriesPiece(OkeyPieceColor color)
        {
            Pieces = new List<OkeyPiece>();
            Color = color;
        }

        public OkeyPieceColor Color { get; set; }
        public List<OkeyPiece> Pieces { get; set; }

        public int PiecesCount
        {
            get
            {
                return Pieces.Count;
            }
        }
    }

    public class ValuesSeries
    {
        public ValuesSeries(byte value, List<OkeyPieceColor> colors)
        {
            Value = value;
            Colors = colors;
        }

        public byte Value { get; set; }
        public List<OkeyPieceColor> Colors { get; set; }
    }

    public class ColorSeries
    {
        public ColorSeries()
        {
            Series = new List<SeriesPiece>();

            int piecesColorCount = Enum.GetNames(typeof(OkeyPieceColor)).Length;

            for (int i = 0; i < piecesColorCount; i++) Series.Add(new SeriesPiece((OkeyPieceColor)i));
        }

        public List<SeriesPiece> Series { get; set; }

        public int PiecesCount
        {
            get
            {
                int totalPieces = 0;

                foreach (SeriesPiece p in Series)
                {
                    totalPieces += p.PiecesCount;
                }

                return totalPieces;
            }
        }
    }

    public partial class OkeyGame
    {
        /// <summary>
        /// Find best cue to finish game
        /// </summary>
        /// <returns></returns>
        public OkeyCue FindBestCue()
        {
            byte minPieceRequired = RegularCuePiecesCount;
            int minPieceRequiredCueIndex = -1;

            for (int i = 0; i < GameTable.Cues.Count; i++)
            {
                // find required piece count to finish for cue
                byte requiredPieceCount = RequiredPieceCount(GameTable.Cues[i]);

                if (requiredPieceCount <= minPieceRequired)
                {
                    minPieceRequired = requiredPieceCount;
                    minPieceRequiredCueIndex = i;
                }
            }

            return GameTable.Cues[minPieceRequiredCueIndex];
        }

        /// <summary>
        /// Calculates required piece count to finish for selected cue.
        /// </summary>
        /// <param name="cue"></param>
        /// <returns></returns>
        private byte RequiredPieceCount(OkeyCue cue)
        {
            byte minPieceRequired = RegularCuePiecesCount;

            // check for twins
            byte requiredPieceCount = RequiredPieceCountTwin(cue);

            if (requiredPieceCount < minPieceRequired)
            {
                minPieceRequired = requiredPieceCount;
            }

            // check for same color series
            List<OkeyPiece> notInPerPieces = new List<OkeyPiece>(); // store pieces that not suitable for series
            List<SeriesPiece> colorSeries = new List<SeriesPiece>(); // all available color series in cue
            List<ValuesSeries> valueSeries = new List<ValuesSeries>(); // all available value series in cue

            requiredPieceCount = RequiredPieceCountColorSeries(cue, colorSeries, notInPerPieces);

            requiredPieceCount -= SameValueSeriesPiecesCount(valueSeries, notInPerPieces);

            if (cue.HasOkey)
                requiredPieceCount--;

            if (requiredPieceCount < minPieceRequired)
            {
                minPieceRequired = requiredPieceCount;
            }

            return minPieceRequired;
        }

        private byte SameValueSeriesPiecesCount(List<ValuesSeries> valueSeries, List<OkeyPiece> pieces)
        {
            List<OkeyPieceColor> colors;

            for (int i = 0; i < pieces.Count; i++)
            {
                int foundValue = -1;
                colors = new List<OkeyPieceColor>();

                for (int j = i + 1; j < pieces.Count; j++)
                {
                    if (pieces[i].PieceColor != pieces[j].PieceColor && pieces[i].PieceValue == pieces[j].PieceValue)
                    {
                        foundValue = pieces[i].PieceValue.Value;

                        if (!colors.Contains(pieces[i].PieceColor.Value))
                            colors.Add(pieces[i].PieceColor.Value);

                        if (!colors.Contains(pieces[j].PieceColor.Value))
                            colors.Add(pieces[j].PieceColor.Value);
                    }
                }

                if (colors.Count >= 3)
                {
                    valueSeries.Add(new ValuesSeries(Convert.ToByte(foundValue), colors));
                }
            }

            int oldPiecesCount = pieces.Count;

            clearSameValueSeriesPieces(valueSeries, pieces);

            int newPiecesCount = pieces.Count;

            return Convert.ToByte(oldPiecesCount - newPiecesCount);
        }

        private void clearSameValueSeriesPieces(List<ValuesSeries> valueSeries, List<OkeyPiece> pieces)
        {
            if (valueSeries.Count > 0)
            {
                foreach (ValuesSeries v in valueSeries)
                {
                    foreach (OkeyPieceColor color in v.Colors)
                    {
                        for (int i = pieces.Count - 1; i >= 0; i--)
                        {
                            if (pieces[i].PieceValue != null && pieces[i].PieceValue.Value == v.Value && pieces[i].PieceColor == color)
                            {
                                pieces.RemoveAt(i);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Calculates required piece count to finish for selected cue as series.
        /// </summary>
        /// <param name="cue"></param>
        /// <returns></returns>
        private byte RequiredPieceCountColorSeries(OkeyCue cue, List<SeriesPiece> series, List<OkeyPiece> notInPerPieces)
        {
            // create series list for each color
            ColorSeries colorSeries = new ColorSeries();

            // select and fill pieces into series
            foreach (OkeyPiece piece in cue.Pieces)
            {
                foreach (SeriesPiece s in colorSeries.Series)
                {
                    if (piece.PieceColor == s.Color)
                    {
                        s.Pieces.Add(piece);
                    }
                }
            }

            int perCountInSeries = CalculatePerCountInSeries(colorSeries, series, notInPerPieces);

            return Convert.ToByte(cue.Pieces.Count - (perCountInSeries * 3));
        }

        private int CalculatePerCountInSeries(ColorSeries colorSeries, List<SeriesPiece> series, List<OkeyPiece> notInPerPieces)
        {
            int totalPerCount = 0;

            foreach (SeriesPiece s in colorSeries.Series)
            {
                // sort pieces in series
                if (s.Pieces.Count >= 3)
                {
                    s.Pieces = s.Pieces.OrderBy(o => o.PieceValue).ToList();
                    totalPerCount += CalculatePerCountInSerie(s.Pieces, series, notInPerPieces);
                }
                else
                {
                    foreach (OkeyPiece p in s.Pieces)
                    {
                        notInPerPieces.Add(p);
                    }
                }
            }

            return totalPerCount;
        }

        private int CalculatePerCountInSerie(List<OkeyPiece> pieces, List<SeriesPiece> series, List<OkeyPiece> notInPerPieces)
        {
            int totalPerCount = 0;

            for (int i = 0; i < pieces.Count;)
            {
                if (i + 2 < pieces.Count)
                {
                    // search for per
                    if (pieces[i].PieceValue + 1 == pieces[i + 1].PieceValue && pieces[i + 1].PieceValue + 1 == pieces[i + 2].PieceValue)
                    {
                        SeriesPiece newSeries = new SeriesPiece(pieces[i].PieceColor.Value);
                        newSeries.Pieces.Add(pieces[i]);
                        newSeries.Pieces.Add(pieces[i + 1]);
                        newSeries.Pieces.Add(pieces[i + 2]);

                        totalPerCount++;
                        i += 3;
                    }
                    else
                    {
                        notInPerPieces.Add(pieces[i]);
                        i++;
                    }
                }
                else
                {
                    notInPerPieces.Add(pieces[i]);
                    i++;
                }
            }

            return totalPerCount;
        }

        /// <summary>
        /// Calculates required piece count to finish for selected cue as twins.
        /// </summary>
        /// <param name="cue"></param>
        /// <returns></returns>
        private byte RequiredPieceCountTwin(OkeyCue cue)
        {
            byte minPieceRequired = RegularCuePiecesCount;

            byte twinsCount = 0;

            for (int i = 0; i < cue.Pieces.Count; i++)
            {
                for (int j = i + 1; j < cue.Pieces.Count; j++)
                {
                    if (!cue.Pieces[i].IsOkey && !cue.Pieces[j].IsOkey && cue.Pieces[i].PieceColor == cue.Pieces[j].PieceColor && cue.Pieces[i].PieceValue == cue.Pieces[j].PieceValue)
                    {
                        twinsCount++;
                    }
                }
            }

            // if cue has okey, use it
            if (cue.HasOkey)
            {
                twinsCount += cue.OkeyCount;
            }

            return Convert.ToByte(minPieceRequired - Convert.ToByte(twinsCount * 2));
        }
    }
}