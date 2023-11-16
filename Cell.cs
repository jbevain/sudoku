struct Cell(ushort value)
{
    private const ushort _hasCandidates = 0b1000_0000_0000_0000;

    public readonly ushort Value => value;

    public Cell() : this(0b1000_0011_1111_1111)
    {
    }

    public readonly bool HasCandidates => (value & _hasCandidates) > 0;
    public readonly bool HasValue => (value & _hasCandidates) == 0;

    private static ushort CandidateValue(int candidate)
    {
        if (candidate < 1 || candidate > 9)
        {
            throw new ArgumentOutOfRangeException(nameof(candidate));
        }

        return (ushort)(1 << (candidate - 1));
    }

    public readonly bool HasCandidate(int candidate)
    {
        if (!HasCandidates)
        {
            return false;
        }

        return (value & CandidateValue(candidate)) > 0;
    }

    public void RemoveCandidate(int candidate)
    {
        value = (ushort)(value & ~CandidateValue(candidate));
    }
}
