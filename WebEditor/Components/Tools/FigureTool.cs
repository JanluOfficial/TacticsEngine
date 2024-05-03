using CaptainCoder.TacticsEngine.Board;

namespace WebEditor.Tools;
public sealed class FigureTool : Tool
{
    public static FigureTool Shared { get; } = new();
    private Optional<Figure> _selectedFigure = new None<Figure>();
    private Optional<Position> _originalPosition = new None<Position>();
    public Positioned<Figure>? DraggedFigure { get; private set; }
    private Position _offset = new(0, 0);

    public override void OnStartDragFigure(Board board, Figure figure, Optional<Position>? start = null, Position? offset = null)
    {
        _offset = new Position(0, 0);
        _selectedFigure = figure;
        _originalPosition = start ?? new None<Position>();
        _originalPosition.Select(board.RemoveFigure);
    }

    public override void OnMouseOver(Board board, Position position)
    {
        base.OnMouseOver(board, position);
        _selectedFigure.Apply(figure => DraggedFigure = new(figure, position + _offset));
    }

    public override void OnMouseUp(Board board, Position endPosition)
    {
        _selectedFigure
            .Where(figure => !board.TryAddFigure(endPosition + _offset, figure))
            .SelectMany(figure => _originalPosition.Select(position => new Positioned<Figure>(figure, position)))
            .Apply(board.Figures.Add);

        _selectedFigure = new None<Figure>();
        _originalPosition = new None<Position>();
        DraggedFigure = null;
    }
}