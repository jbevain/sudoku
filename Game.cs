readonly struct Game
{
    private static readonly byte[] _rows = [
        0,1,2,3,4,5,6,7,8,
        9,10,11,12,13,14,15,16,17,
        18,19,20,21,22,23,24,25,26,
        27,28,29,30,31,32,33,34,35,
        36,37,38,39,40,41,42,43,44,
        45,46,47,48,49,50,51,52,53,
        54,55,56,57,58,59,60,61,62,
        63,64,65,66,67,68,69,70,71,
        72,73,74,75,76,77,78,79,80,
    ];

    private static readonly byte[] _columns = [
        0,9,18,27,36,45,54,63,72,
        1,10,19,28,37,46,55,64,73,
        2,11,20,29,38,47,56,65,74,
        3,12,21,30,39,48,57,66,75,
        4,13,22,31,40,49,58,67,76,
        5,14,23,32,41,50,59,68,77,
        6,15,24,33,42,51,60,69,78,
        7,16,25,34,43,52,61,70,79,
        8,17,26,35,44,53,62,71,80,
    ];

    private static readonly byte[] _boxes = [
        0,1,2,9,10,11,18,19,20,
        3,4,5,12,13,14,21,22,23,
        6,7,8,15,16,17,24,25,26,
        27,28,29,36,37,38,45,46,47,
        30,31,32,39,40,41,48,49,50,
        33,34,35,42,43,44,51,52,53,
        54,55,56,63,64,65,72,73,74,
        57,58,59,66,67,68,75,76,77,
        60,61,62,69,70,71,78,79,80,
    ];

    public static ReadOnlySpan<byte> Row(int rowIndex)
    {
        if (rowIndex < 0 || rowIndex > 8)
        {
            throw new ArgumentOutOfRangeException(nameof(rowIndex));
        }

        return _rows.AsSpan(rowIndex * 9, 9);
    }

    public static ReadOnlySpan<byte> Column(int columnIndex)
    {
        if (columnIndex < 0 || columnIndex > 8)
        {
            throw new ArgumentOutOfRangeException(nameof(columnIndex));
        }

        return _columns.AsSpan(columnIndex * 9, 9);
    }

    public static ReadOnlySpan<byte> Box(int boxIndex)
    {
        if (boxIndex < 0 || boxIndex > 8)
        {
            throw new ArgumentOutOfRangeException(nameof(boxIndex));
        }

        return _boxes.AsSpan(boxIndex * 9, 9);
    }

    public static int ColumnOf(int index)
    {
        if (index < 0 || index > 80)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        return index % 9;
    }

    public static int RowOf(int index)
    {
        if (index < 0 || index > 80)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        return index / 9;
    }

    public static int BoxOf(int index)
    {
        if (index < 0 || index > 80)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        return (index / 9 / 3) * 3 + (index % 9 / 3);
    }

    public const int CellCount = 9 * 9;

    private readonly Cell[] _cells;

    public ref Cell this[int index] {
        get { return ref _cells[index]; }
    }

    public Game()
    {
        _cells = new Cell[CellCount];
        for (int i = 0; i < _cells.Length; i++)
        {
            _cells[i] = new();
        }
    }
}
