using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

using CaptainCoder.Linq;

using Optional;
using Optional.Collections;
using Optional.Linq;

namespace CaptainCoder.TacticsEngine.Board;

public sealed class Board : IEquatable<Board>
{
    public HashSet<Position> Tiles { get; set; } = [];
    public PositionMap<Figure> Figures { get; set; } = [];
    public bool Equals(Board? other)
    {
        return other is not null &&
        Tiles.SetEquals(other.Tiles) &&
        Figures.Equals(other.Figures);
    }
}

public static class BoardExtensions
{
    public static void CreateEmptyTile(this Board board, int x, int y) => board.CreateEmptyTile(new Position(x, y));

    public static void CreateEmptyTile(this Board board, Position position) => board.Tiles.Add(position);

    public static void CreateEmptyTiles(this Board board, IEnumerable<Position> positions) =>
        positions.ForEach(board.CreateEmptyTile);

    public static bool HasTile(this Board board, int x, int y) => board.Tiles.Contains(new Position(x, y));

    public static Option<Tile> GetTile(this Board board, Position position) =>
        board.NoneWhen(board => !board.Tiles.Contains(position))
             .Select(board => board.Figures.FirstOrNone(f => f.BoundingBox().Contains(position)))
             .Select(positionedFigure => new Tile() { Figure = positionedFigure.Select(f => f.Element) });

    public static Option<Tile> GetTile(this Board board, int x, int y) => board.GetTile(new Position(x, y));

    public static bool CanAddFigure(this Board board, Position position, Figure toAdd)
    {
        BoundingBox bbox = new(position, toAdd.Width, toAdd.Height);
        return board.HasTiles(bbox) && board.Figures.CanAdd(position, toAdd);
    }
    public static bool CanAddFigure(this Board board, int x, int y, Figure toAdd) =>
        board.CanAddFigure(new Position(x, y), toAdd);

    public static Option<Positioned<Figure>> TryAddFigure(this Board board, Position position, Figure toAdd) =>
        new BoundingBox(position, toAdd.Width, toAdd.Height)
            .SomeWhen(board.HasTiles)
            .SelectMany(_ => board.Figures.TryAdd(position, toAdd));

    public static Option<Positioned<Figure>> TryAddFigure(this Board board, int x, int y, Figure toAdd) =>
        board.TryAddFigure(new Position(x, y), toAdd);

    public static bool HasTiles(this Board board, BoundingBox box) => box.Positions().All(board.Tiles.Contains);

    public static Option<Tile> RemoveTile(this Board board, int x, int y) => board.RemoveTile(new Position(x, y));

    public static Option<Tile> RemoveTile(this Board board, Position position)
    {
        Option<Tile> tile = board.GetTile(position);
        board.Tiles.Remove(position);
        board.RemoveFigure(position);
        return tile;
    }

    public static Option<Positioned<Figure>> RemoveFigure(this Board board, Position position) =>
        board.Figures.Remove(position);

    private static JsonSerializerOptions Options { get; } = new()
    {
        Converters = { FigureMapConverter.Shared }
    };

    public static string ToJson(this Board board) => JsonSerializer.Serialize(board, Options);

    public static bool TryFromJson(string json, [NotNullWhen(true)] out Board? board)
    {
        board = JsonSerializer.Deserialize<Board>(json, Options);
        return board is not null;
    }
}