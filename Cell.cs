struct Cell(short value)
{
    private const short _hasCandidates = 0b0001_0000_0000_0000;

    public readonly short Value => value;

    public Cell() : this(0b0001_0011_1111_1111)
    {
    }

    public readonly bool HasCandidates => (value & _hasCandidates) > 0;
    public readonly bool HasValue => (value & _hasCandidates) == 0;

    private static short CandidateValue(int candidate)
    {
        if (candidate < 1 || candidate > 9)
        {
            throw new ArgumentOutOfRangeException(nameof(candidate));
        }

        return (short)(1 << (candidate - 1));
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
        value = (short)(value & ~CandidateValue(candidate));
    }
}
