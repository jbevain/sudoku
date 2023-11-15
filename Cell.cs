struct Cell(ushort value)
{
    /*
    1  = 0b0000_0000_0000_0001;
    2  = 0b0000_0000_0000_0010;
    3  = 0b0000_0000_0000_0100;
    4  = 0b0000_0000_0000_1000;
    5  = 0b0000_0000_0001_0000;
    6  = 0b0000_0000_0010_0000;
    7  = 0b0000_0000_0100_0000;
    8  = 0b0000_0000_1000_0000;
    9  = 0b0000_0001_0000_0000;
    */

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
