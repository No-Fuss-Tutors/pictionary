using System;
using System.Collections.Generic;
using System.Linq;


//Utility functions for random generation
public static class GenerationUtility
{
    

    ///Random Number Generator that allows for the exclusion of certain numbers
    public static int GenerateRandomInteger(int lowerBound, int upperBound, HashSet<int> exclude)
    {
        System.Random random = new System.Random();
        //distance between the lower bound and upper bound
        int fullRange = lowerBound - upperBound;
        IEnumerable<int> numberRange = Enumerable.Range(lowerBound, upperBound-lowerBound).Where(toExclude => !exclude.Contains(toExclude));
        int generatedIndex = random.Next(0, numberRange.Count());
        int generatedNumber = numberRange.ElementAt(generatedIndex);
        return generatedNumber;
    }

    //converts an integer to it's corresponding placement (i.e 1st, 2nd, 3rd, 4th)
    public static string IntToPlace(int num)
    {
        string suffix = "th";
        switch(num)
        {
            case 1:
                suffix = "st";
                break;
            case 2:
                suffix = "nd";
                break;
            case 3:
                suffix = "rd";
                break;
        }
        return num.ToString() + suffix;
    }

    public static float AbsMin(float a, float b)
    {
        if(MathF.Abs(b) < MathF.Abs(a))
        {
            return b;
        }
        else
        {
            return a;
        }
    }

    

}