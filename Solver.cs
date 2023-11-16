readonly struct Solver(Game game)
{
    public bool TrySolve()
    {
        // int iteration = 0;

        while (!IsComplete())
        {
            // Console.WriteLine("Iteration: " + iteration++);
            // Print();

            RemoveCandidates();
            TryFindPointingPairs();
            TryFindNakedPairs();
            TryFindHiddenPairs();

            if (TryFindNakedSingles())
            {
                continue;
            }

            if (TryFindHiddenSingles())
            {
               continue;
            }

            return false;
        }

        return IsValid();
    }

    private void RemoveCandidates()
    {
        for (int i = 0; i < Game.CellCount; i++)
        {
            ref var cell = ref game[i];
            if (cell.HasValue)
            {
                continue;
            }

            foreach (ref var colCell in Cells(Game.Column(Game.ColumnOf(i))))
            {
                if (colCell.HasValue)
                {
                    cell.RemoveCandidate(colCell.Value);
                }
            }

            foreach (ref var rowCell in Cells(Game.Row(Game.RowOf(i))))
            {
                if (rowCell.HasValue)
                {
                    cell.RemoveCandidate(rowCell.Value);
                }
            }

            foreach (ref var boxCell in Cells(Game.Box(Game.BoxOf(i))))
            {
                if (boxCell.HasValue)
                {
                    cell.RemoveCandidate(boxCell.Value);
                }
            }
        }
    }

    private bool TryFindNakedSingles()
    {
        for (int i = 0; i < Game.CellCount; i++)
        {
            if (TryGetNakedSingle(in game[i], out var candidate))
            {
                game[i] = new(candidate);
                return true;
            }
        }

        return false;
    }

    private static bool TryGetNakedSingle(in Cell cell, out ushort candidate)
    {
        candidate = 0;
        if (!cell.HasCandidates)
        {
            return false;
        }

        int candidates = 0;
        for (ushort i = 1; i < 10; i++)
        {
            if (cell.HasCandidate(i))
            {
                candidates++;
                candidate = i;
            }
        }

        return candidates == 1;
    }

    private bool TryFindHiddenSingles()
    {
        for (int i = 0; i < 9; i++)
        {
            if (TryFindHiddenSingle(Game.Row(i), out var index, out var value))
            {
                game[index] = new(value);

                RemoveCandidateInContainer(Game.Column(Game.ColumnOf(index)), value);
                RemoveCandidateInContainer(Game.Box(Game.BoxOf(index)), value);
                return true;
            }

            if (TryFindHiddenSingle(Game.Column(i), out index, out value))
            {
                game[index] = new(value);

                RemoveCandidateInContainer(Game.Row(Game.RowOf(index)), value);
                RemoveCandidateInContainer(Game.Box(Game.BoxOf(index)), value);
                return true;
            }

            if (TryFindHiddenSingle(Game.Box(i), out index, out value))
            {
                game[index] = new(value);

                RemoveCandidateInContainer(Game.Row(Game.RowOf(index)), value);
                RemoveCandidateInContainer(Game.Column(Game.ColumnOf(index)), value);
                return true;
            }
        }

        return false;
    }

    private void RemoveCandidateInContainer(ReadOnlySpan<byte> indexes, ushort value)
    {
        foreach (ref var cell in Cells(indexes))
        {
            if (cell.HasValue)
            {
                continue;
            }

            if (cell.HasCandidate(value))
            {
                cell.RemoveCandidate(value);
            }
        }
    }

    private bool TryFindHiddenSingle(ReadOnlySpan<byte> indexes, out int index, out ushort value)
    {
        Span<byte> candidates = stackalloc byte[9];

        for (int i = 0; i < indexes.Length; i++)
        {
            for (int j = 1; j < 10; j++)
            {
                if (game[indexes[i]].HasCandidate(j))
                {
                    candidates[j - 1]++;
                }
            }
        }

        for (int i = 0; i < indexes.Length; i++)
        {
            for (int j = 1; j < 10; j++)
            {
                if (game[indexes[i]].HasCandidate(j) && candidates[j - 1] == 1)
                {
                    index = indexes[i];
                    value = (ushort)j;
                    return true;
                }
            }
        }

        index = 0;
        value = 0;
        return false;
    }

    private void TryFindPointingPairs()
    {
        for (int i = 0; i < 9; i++)
        {
            if (TryFindPointingPair(Game.Column(i), out int index1, out int index2, out ushort value))
            {
                RemoveCandidateInContainer(Game.Box(Game.BoxOf(index1)), index1, index2, value);
                RemoveCandidateInContainer(Game.Column(Game.ColumnOf(index1)), index1, index2, value);
            }

            if (TryFindPointingPair(Game.Row(i), out index1, out index2, out value))
            {
                RemoveCandidateInContainer(Game.Box(Game.BoxOf(index1)), index1, index2, value);
                RemoveCandidateInContainer(Game.Row(Game.RowOf(index1)), index1, index2, value);
            }
        }
    }

    private bool TryFindPointingPair(ReadOnlySpan<byte> indexes, out int index1, out int index2, out ushort value)
    {
        Span<byte> candidates = stackalloc byte[9];

        for (int i = 0; i < indexes.Length; i++)
        {
            for (int j = 1; j < 10; j++)
            {
                if (game[indexes[i]].HasCandidate(j))
                {
                    candidates[j - 1]++;
                }
            }
        }

        index1 = -1;
        index2 = -1;
        value = 0;

        for (int i = 0; i < indexes.Length; i++)
        {
            for (ushort j = 1; j < 10; j++)
            {
                ref var cell1 = ref game[indexes[i]];
                if (cell1.HasCandidate(j) && candidates[j - 1] == 2)
                {
                    index1 = indexes[i];
                    value = j;
                    
                    int count = 1;
                    for (int k = i + 1; k < indexes.Length; k++)
                    {
                        ref var cell2 = ref game[indexes[k]];
                        if (cell2.HasCandidate(j))
                        {
                            index2 = indexes[k];
                            count++;
                        }
                    }

                    if (count == 2 && Game.BoxOf(index1) == Game.BoxOf(index2))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private void RemoveCandidateInContainer(ReadOnlySpan<byte> indexes, int index1, int index2, ushort value)
    {
        for (int i = 0; i < indexes.Length; i++)
        {
            if (indexes[i] == index1 || indexes[i] == index2)
            {
                continue;
            }

            ref var cell = ref game[indexes[i]];
            if (!cell.HasValue && cell.HasCandidate(value))
            {
                cell.RemoveCandidate(value);
            }
        }
    }

    private void TryFindNakedPairs()
    {
        for (int i = 0; i < 9; i++)
        {
            if (TryFindNakedPairs(Game.Column(i), out int index1, out int index2, out ushort value1, out ushort value2))
            {
                RemoveCandidateInContainer(Game.Column(Game.ColumnOf(index1)), index1, index2, value1);
                RemoveCandidateInContainer(Game.Column(Game.ColumnOf(index1)), index1, index2, value2);

                if (Game.BoxOf(index1) == Game.BoxOf(index2))
                {
                    RemoveCandidateInContainer(Game.Box(Game.BoxOf(index1)), index1, index2, value1);
                    RemoveCandidateInContainer(Game.Box(Game.BoxOf(index1)), index1, index2, value2);
                }
            }

            if (TryFindNakedPairs(Game.Row(i), out index1, out index2, out value1, out value2))
            {
                RemoveCandidateInContainer(Game.Row(Game.RowOf(index1)), index1, index2, value1);
                RemoveCandidateInContainer(Game.Row(Game.RowOf(index1)), index1, index2, value2);

                if (Game.BoxOf(index1) == Game.BoxOf(index2))
                {
                    RemoveCandidateInContainer(Game.Box(Game.BoxOf(index1)), index1, index2, value1);
                    RemoveCandidateInContainer(Game.Box(Game.BoxOf(index1)), index1, index2, value2);
                }
            }

            if (TryFindNakedPairs(Game.Box(i), out index1, out index2, out value1, out value2))
            {
                RemoveCandidateInContainer(Game.Box(Game.BoxOf(index1)), index1, index2, value1);
                RemoveCandidateInContainer(Game.Box(Game.BoxOf(index1)), index1, index2, value2);
            }
        }
    }

    private bool TryFindNakedPairs(ReadOnlySpan<byte> indexes, out int index1, out int index2, out ushort value1, out ushort value2)
    {
        Span<byte> candidates = stackalloc byte[9];

        for (int i = 0; i < indexes.Length; i++)
        {
            for (int j = 1; j < 10; j++)
            {
                if (game[indexes[i]].HasCandidate(j))
                {
                    candidates[i]++;
                }
            }
        }

        index1 = -1;
        index2 = -1;
        value1 = 0;
        value2 = 0;

        for (int i = 0; i < indexes.Length; i++)
        {
            if (candidates[i] != 2)
            {
                continue;
            }

            int count = 1;
            ref var cell1 = ref game[indexes[i]];

            for (int j = i + 1; j < indexes.Length; j++)
            {
                if (candidates[j] != 2)
                {
                    continue;
                }

                ref var cell2 = ref game[indexes[j]];

                if (game[indexes[i]].Value != game[indexes[j]].Value)
                {
                    continue;
                }

                index1 = indexes[i];
                index2 = indexes[j];

                for (int k = 1; k < 10; k++)
                {
                    if (value1 == 0 && cell1.HasCandidate(k))
                    {
                        value1 = (ushort)k;
                    }

                    if (value1 != 0 && cell2.HasCandidate(k))
                    {
                        value2 = (ushort)k;
                    }
                }

                count++;
            }

            if (count == 2)
            {
                return true;
            }
        }

        return false;
    }

    private void TryFindHiddenPairs()
    {
        for (int i = 0; i < 9; i++)
        {
            if (TryFindHiddenPair(Game.Row(i), out int index1, out int index2, out ushort candidate1, out ushort candidate2))
            {
                RemoveCandidatesInCell(index1, candidate1, candidate2);
                RemoveCandidatesInCell(index2, candidate1, candidate2);
            }

            if (TryFindHiddenPair(Game.Column(i), out index1, out index2, out  candidate1, out candidate2))
            {
                RemoveCandidatesInCell(index1, candidate1, candidate2);
                RemoveCandidatesInCell(index2, candidate1, candidate2);
            }
        }
    }

    private bool TryFindHiddenPair(ReadOnlySpan<byte> indexes, out int index1, out int index2, out ushort candidate1, out ushort candidate2)
    {
        Span<byte> candidates = stackalloc byte[9];

        for (int i = 0; i < indexes.Length; i++)
        {
            for (int j = 1; j < 10; j++)
            {
                if (game[indexes[i]].HasCandidate(j))
                {
                    candidates[j - 1]++;
                }
            }
        }

        index1 = -1;
        index2 = -1;
        candidate1 = 0;
        candidate2 = 0;

        for (int i = 0; i < indexes.Length; i++)
        {
            int count = 0;
            for (int j = 1; j < 10; j++)
            {
                if (game[indexes[i]].HasCandidate(j) && candidates[j - 1] == 2)
                {
                    count++;
                    if (count == 1)
                    {
                        candidate1 = (ushort)j;
                    }
                    else if (count == 2)
                    {
                        candidate2 = (ushort)j;
                    }
                }
            }

            if (count == 2)
            {
                index1 = indexes[i];
                for (int j = i + 1; j < indexes.Length; j++)
                {
                    if (game[indexes[j]].HasCandidate(candidate1) && game[indexes[j]].HasCandidate(candidate2))
                    {
                        index2 = indexes[j];
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private void RemoveCandidatesInCell(int index, ushort candidate1, ushort candidate2)
    {
        ref var cell = ref game[index];
        for (ushort i = 1; i < 10; i++)
        {
            if (cell.HasCandidate(i) && i != candidate1 && i != candidate2)
            {
                cell.RemoveCandidate(i);
            }
        }
    }

    private CellEnumerator Cells(ReadOnlySpan<byte> indexes) => new(game, indexes);

    private ref struct CellEnumerator
    {
        private readonly Game _game;
        private readonly ReadOnlySpan<byte> _indexes;
        private int _index;

        public CellEnumerator(Game game, ReadOnlySpan<byte> indexes)
        {
            _game = game;
            _indexes = indexes;
            _index = -1;
        }

        public readonly CellEnumerator GetEnumerator() => this;

        public bool MoveNext()
        {
            return ++_index < _indexes.Length;
        }

        public readonly ref Cell Current => ref _game[_indexes[_index]];
    }

    private bool IsComplete()
    {
        for (int i = 0; i < Game.CellCount; i++)
        {
            if (!game[i].HasValue)
            {
                return false;
            }
        }

        return true;
    }

    private bool IsValid()
    {
        for (int i = 0; i < 9; i++)
        {
            if (!IsValid(Game.Row(i)))
            {
                return false;
            }

            if (!IsValid(Game.Column(i)))
            {
                return false;
            }

            if (!IsValid(Game.Box(i)))
            {
                return false;
            }
        }

        return true;
    }

    private bool IsValid(ReadOnlySpan<byte> indexes)
    {
        Span<byte> values = stackalloc byte[9];
        for (int i = 0; i < 9; i++)
        {
            ref var cell = ref game[indexes[i]];
            if (cell.HasValue)
            {
                values[i]++;
            }
            else
            {
                return false;
            }
        }

        for (int i = 0; i < 9; i++)
        {
            if (values[i] != 1)
            {
                return false;
            }
        }

        return true;
    }

    public void Print()
    {
        for (int i = 0; i < Game.CellCount; i++)
        {
            if (i % 9 == 0)
            {
                Console.Write("[");
            }

            ref var cell = ref game[i];

            Console.Write("(");
            for (int j = 1; j < 10; j++)
            {
                if (cell.HasCandidate(j))
                {
                    Console.Write(j);
                }
                else
                {
                    Console.Write(" ");
                }
            }
            Console.Write(")");

            if (cell.HasValue)
            {
                Console.Write(" " + cell.Value + " ");
            }
            else
            {
                Console.Write(" _ ");
            }

            if (i % 9 == 8)
            {
                Console.WriteLine("]");
            }
        }
    }
}
