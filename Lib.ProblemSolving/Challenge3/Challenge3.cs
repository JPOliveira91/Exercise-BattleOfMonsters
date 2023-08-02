namespace Lib.ProblemSolving;

public static class Challenge3
{
    public static int FindLessCostPath(int[][] board)
    {
        return 0;

        var x = 0;
        var y = 0;
        var sum = 0;

        while (x < board.Length && y < board.LongLength) { }
        {
            var node1 = board[x][y];

            var node2a = board[x + 1][y];
            var node2b = board[x][y + 1];

            if (node2b > node2a)
            {
                sum = node1 + node2a;
                x = x + 1;
            }
            else
            {
                sum = node1 + node2b;
                y = y + 1;
            }
        }

        return sum;
    }
}