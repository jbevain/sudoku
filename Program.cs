using System.Text;

AssertGame(@"
216___53_
35_6_2__1
8___3196_
___7__2_9
69_4_8__3
537__96__
__5_4__2_
____2_718
__1_86___
", @"
216894537
359672841
874531962
148763259
692458173
537219684
985147326
463925718
721386495");

AssertGame(@"
___2_____
______81_
_64__8__3
__2___57_
94___6___
__84_5___
_7__8____
5______62
____3___9
", @"
853219647
297364815
164758923
612893574
945176238
738425196
379682451
581947362
426531789");

AssertGame(@"
_____21_4
__8__1__3
5___6__9_
_9__8__46
6__7_____
1______8_
_372___19
_______3_
____9____
", @"
369872154
748951263
512463897
293185746
685734921
174629385
837246519
926517438
451398672");

AssertGame(@"
5_7______
93_2__168
8__439___
____4189_
_46__87__
1_8_2___5
31_9_4_7_
_7_3__9_1
_8_1_73_4
", @"
527816439
934275168
861439257
753641892
246598713
198723645
312984576
475362981
689157324");

AssertGame(@"
____15_2_
8________
_74____9_
5___9____
_9_6_2___
___7____3
_______64
__79_____
34_5_1___
", @"
963415827
815279436
274386195
538194672
791632548
426758913
159827364
687943251
342561789");

AssertGame(@"
7_652_1__
_______2_
_3__4_9__
4_______9
____12_3_
______2__
_5_96_3__
_71__8__4
_8_______
", @"
796523148
145789623
832146957
423675819
967812435
518394276
254967381
671238594
389451762");

AssertGame(@"
9___8___4
_145_____
_______61
_8_______
__1___326
___7_3___
__3_792__
__9______
__52_8_9_
", @"
927681534
614537982
538492761
382916457
791854326
456723819
863179245
249365178
175248693");

AssertGame(@"
___79____
__9_5____
__7___26_
_1__4____
_832__1__
_6_____4_
__6______
___3_8__4
_9_____87
", @"628791453
349652718
157483269
712845936
483269175
965137842
836974521
571328694
294516387");

var input = @"
___79____
__9_5____
__7___26_
_1__4____
_832__1__
_6_____4_
__6______
___3_8__4
_9_____87
";

input = input.Trim().Replace("\r", "").Replace("\n", "");

Console.WriteLine(input);

var game = ParseGame(input);

Console.WriteLine("Parsed");

var solver = new Solver(game);
var solved = solver.TrySolve();

Console.WriteLine("Solved: " + solved);

Console.WriteLine(Print(game));


static void AssertGame(string input, string expectedResult)
{
    var game = ParseGame(input);
    var solver = new Solver(game);
    if (!solver.TrySolve())
    {
        throw new Exception("Game could not be solved");
    }

    var printed = Print(game);

    if (Normalize(expectedResult) != Normalize(printed))
    {
        Console.WriteLine("Expected:");
        Console.WriteLine(expectedResult);
        Console.WriteLine("Actual:");
        Console.WriteLine(printed);

        throw new Exception("Game did not match expected result");
    }
}

static string Normalize(string input) => input.Trim().Replace("\r", "").Replace("\n", "");

static Game ParseGame(string input)
{
    input = Normalize(input);
    if (input.Length != Game.CellCount)
    {
        throw new ArgumentException("Input must be 81 characters long");
    }

    var game = new Game();
    for (int i = 0; i < input.Length; i++)
    {
        var c = input[i];
        if (c == '_')
        {
            continue;
        }
        else
        {
            game[i] = new((ushort)(c - '0'));
        }
    }

    return game;
}

static string Print(Game game)
{
    var sb = new StringBuilder();
    for (int i = 0; i < Game.CellCount; i++)
    {
        ref var cell = ref game[i];
        if (cell.HasValue)
        {
            sb.Append(cell.Value);
        }
        else
        {
            sb.Append('_');
        }

        if (i % 9 == 8)
        {
            sb.AppendLine();
        }
    }

    return sb.ToString();
}
