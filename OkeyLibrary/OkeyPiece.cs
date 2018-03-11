namespace OkeyLibrary
{
    public enum OkeyPieceColor
    {
        Sarı,
        Mavi,
        Siyah,
        Kırmızı
    }

    public class OkeyPiece : Piece
    {
        public bool IsFake { get; set; }
        public bool IsOkey { get; set; }
        public bool IsGosterge { get; set; }
        public OkeyPieceColor? PieceColor { get; set; }
    }
}
