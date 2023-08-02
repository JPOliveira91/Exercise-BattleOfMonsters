namespace Lib.ProblemSolving;

public static class Challenge1
{
    public static Challenge1Result FractionsCalculator(int[] numbers)
    {
        decimal countPos = 0;
        decimal countZer = 0;
        decimal countNeg = 0;

        foreach(var number in numbers)
        {
            if (number > 0)
            {
                countPos++;
            }
            else if (number < 0)
            {
                countNeg++;
            }
            else {
                countZer++;
            }
        }

        decimal total = countPos + countNeg + countZer;

        var result = new Challenge1Result();

        result.Positives = Math.Round(countPos/total, 6);
        result.Zeros = Math.Round(countZer / total, 6);
        result.Negatives = Math.Round(countNeg / total, 6);

        return result;
    }
}

public class Challenge1Result
{
    public decimal Positives { get; set; }
    public decimal Negatives { get; set; }
    public decimal Zeros { get; set; }
}