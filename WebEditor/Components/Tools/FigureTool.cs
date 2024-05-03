using CaptainCoder.Functional;
using CaptainCoder.TacticsEngine.Board;

namespace WebEditor.Tools;
public sealed class FigureTool : Tool
{
    public static FigureTool Shared { get; } = new();
    private Optional<Figure> _selectedFigure = new None<Figure>();
    private Optional<Position> _originalPosition = new None<Position>();
    public Optional<Positioned<Figure>> DraggedFigure { get; private set; } = new None<Positioned<Figure>>();
    private Position _offset = new(0, 0);

    public override void OnStartDragFigure(Board board, Figure figure, Optional<Position>? start = null, Position? offset = null)
    {
        _offset = new Position(0, 0);
        _selectedFigure = figure;
        _originalPosition = start ?? new None<Position>();
        _originalPosition.Invoke(board.RemoveFigure);
    }

    public override void OnMouseOver(Board board, Position position)
    {
        base.OnMouseOver(board, position);
        DraggedFigure = _selectedFigure.Map(figure => new Positioned<Figure>(figure, position + _offset));
    }

    public override void OnMouseUp(Board board, Position endPosition)
    {
        _selectedFigure
            .Filter(figure => !board.TryAddFigure(endPosition + _offset, figure))
            .FlatMap(figure => _originalPosition.Map(position => new Positioned<Figure>(figure, position)))
            .Invoke(board.Figures.Add);

        _selectedFigure = new None<Figure>();
        _originalPosition = new None<Position>();
        DraggedFigure = new None<Positioned<Figure>>();
    }
}