using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public static class WordStorage
{
    public static string[][] standard = new string[][] 
    {
        //Easy
        new string[]{ "Apple", "Banana", "Orange", "Car", "Robot", "Shirt", "Pants", "Underwear", "Shoes", "Socks", "Spider", "Mountain", "Ice", "Cactus", "Tree", "Bush", "Key", "Kirby", "Book", "Basketball" },
        //Medium
        new string[]{ "Computer", "Human", "Xbox", "Playstation", "Rocket", "Lake", "Beach", "Ice cream", "Super Mario", "McDonald's", "Burger King", "Wendy's", "Grass", "Keyboard", "Mouse", "Monitor" },
        //Hard
        new string[]{"Maze", "Code", "Fist", "Password", "Magic", "E-mail", "Pictionary", "Dictionary", "Transformers", "Voltage", "Current", "Power Line", "Shell" }
    };
}