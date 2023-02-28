using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pile
{
    public Vector3 basePosition;
    public Stack<Block> blockStack;

    public Pile()
    {
        blockStack = new Stack<Block>();
    }
}
