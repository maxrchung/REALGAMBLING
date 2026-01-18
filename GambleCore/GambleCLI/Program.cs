using GambleCore;

var wheel = GamblingWheel.TestWheel;
var board = new GamblingBoard();
board.AddWheel(wheel);
board.AddWheel(wheel);
board.AddWheel(wheel);

Console.WriteLine(board.BuildWheelStateString());
Console.WriteLine();
for (var i = 0; i < 3; ++i)
{
    for (var j = 0; j < 5 - i; ++j)
    {
        board.SpinExcept(i);
        Console.WriteLine(board.BuildWheelStateString());
        Console.WriteLine();
    }
}