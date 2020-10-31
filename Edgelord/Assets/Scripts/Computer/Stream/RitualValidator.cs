using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RitualValidator : MonoBehaviour
{
    public static RitualValidator Instance; //singleton

    private Dictionary<int[], int> Recipes = new Dictionary<int[], int>(new IntArrayEqualityComparer())
    {
        {new int[]{10, 0, 0, 1, 0, 2, 3}, 0}, //ressurection
        {new int[]{4, 1, 1, 1, 2, 2, 2}, 2}, //arch
        {new int[]{9, 3, 3, 3, 0, 0, 0}, 3}, //blessing
        {new int[]{10, 1, 1, 1, 0, 0, 0}, 4}, //bloom
        {new int[]{6, 1, 2, 3, 0, 0, 0}, 5}, //blossom
        {new int[]{5, 1, 2, 3, 0, 0, 0}, 6}, //cog
        {new int[]{11, 1, 2, 3, 1, 2, 3}, 7}, //Oracle
        {new int[]{7, 1, 2, 3, 0, 0, 0}, 8}, //pike
        {new int[]{10, 1, 2, 2, 0, 0, 0}, 9}, //reborn
        {new int[]{-4, 10, 10, 10, 0, 0, 0}, 10}, //symphony
        {new int[]{-3, 0, 0, 0, 0, 0, 0}, 11}, //blessed
        {new int[]{}, 1}
    };

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    // Returns value corresponding to the given key
    public int AttemptRitual(int[] key)
    {
        if(Recipes.ContainsKey(key))
        {
            //return value if it exists
            return Recipes[key];
        } else
        {
            //otherwise return -1 to show error
            return -1;
        }
    }
}
