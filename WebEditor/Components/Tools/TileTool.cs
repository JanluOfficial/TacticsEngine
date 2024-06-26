using CaptainCoder.TacticsEngine.Board;

namespace WebEditor.Tools;

public sealed class TileTool : Tool
{
    public static TileTool Shared { get; } = new();
    public override void OnClickTile(Board board, Position position)
    {
        if (board.HasTile(position.X, position.Y)) { return; }
        board.CreateEmptyTile(position.X, position.Y);
    }
}