using System.Collections.Generic;

public class Cell
{
    public bool isWater;
    //List instead of array so i dont have to set the number and can keep it dynamic
    public List<bool> isBlockLevels = new List<bool>();

    public Cell(bool isWater, bool[] isBlockLevels )
    {
        this.isWater = isWater;
        for(int i=0; i< isBlockLevels.Length; i++)
        {
            this.isBlockLevels.Add(isBlockLevels[i]);
        }
    }
}
