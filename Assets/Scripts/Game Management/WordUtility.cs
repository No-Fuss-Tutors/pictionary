using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;
using UnityEngine;

public static class WordUtility
{
    
    //Generates one easy, one medium and one hard word
    public static string[] GenerateWords()
    {
        string easy = WordStorage.standard[0][UnityEngine.Random.Range(0, WordStorage.standard[0].Length)];
        string medium = WordStorage.standard[1][UnityEngine.Random.Range(0, WordStorage.standard[1].Length)];
        string hard = WordStorage.standard[2][UnityEngine.Random.Range(0, WordStorage.standard[2].Length)];
        return new string[] {easy.ToLower(), medium.ToLower(), hard.ToLower()};
    }

    public static string HiddenWord(string word, float timeLeft, float totalTime)
    {
        List<int> displayIndexes = new List<int>();
        if(timeLeft/totalTime <= 0.5f){
            displayIndexes.Add(0);
        }

        if(timeLeft/totalTime <= 0.3f){
            displayIndexes.Add(2);
        }

        if(timeLeft/totalTime <= 0.2f){
            displayIndexes.Add(5);
        }

        if(timeLeft/totalTime <= 0.15f){
            displayIndexes.Add(1);
        }

        if(timeLeft/totalTime <= 0.05f){
            displayIndexes.Add(4);
        }
        string hiddenString = "";
        for(int letterIndex = 0; letterIndex < word.Length; letterIndex++)
        {
            if(displayIndexes.Contains(letterIndex) || !Char.IsLetter(word[letterIndex]))
            {
                hiddenString += word[letterIndex];
            }
            else
            {
                hiddenString += "_";
            }
        }
        return hiddenString;
    }
    
    
}
