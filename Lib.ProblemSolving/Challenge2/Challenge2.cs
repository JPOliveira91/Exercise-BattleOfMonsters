namespace Lib.ProblemSolving;

public static class Challenge2
{
    public static int DiceFacesCalculator(int dice1, int dice2, int dice3)
    {
        var resultList = new List<int>();

        resultList.Add(dice1);
        resultList.Add(dice2);
        resultList.Add(dice3);

        if (resultList.Min() < 1 || resultList.Max() > 6) throw new Exception("Dice out of number range");

        if (resultList.Distinct().Count() == 1)
        {
            return dice1 * 3;
        }
        else if (resultList.Distinct().Count() == 2)
        {
            if (resultList[1] == resultList[2])
            {
                return resultList[1] * 2;
            }
            else
            {
                return resultList[0] * 2;
            }
        }

        // else if (resultList.Distinct().Count() == 3)
        return resultList.Max();
    }
}