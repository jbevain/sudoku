struct Cell(ushort value)
{
    /*
    private const ushort _1  = 0b0000_0000_0000_0001;
    private const ushort _2  = 0b0000_0000_0000_0010;
    private const ushort _3  = 0b0000_0000_0000_0100;
    private const ushort _4  = 0b0000_0000_0000_1000;
    private const ushort _5  = 0b0000_0000_0001_0000;
    private const ushort _6  = 0b0000_0000_0010_0000;
    private const ushort _7  = 0b0000_0000_0100_0000;
    private const ushort _8  = 0b0000_0000_1000_0000;
    private const ushort _9  = 0b0000_0001_0000_0000;
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
