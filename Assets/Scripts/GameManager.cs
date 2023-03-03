using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    
    [SerializeField] private Block blockPrefab;
    [SerializeField] private GameObject placeHolder;
    
    [SerializeField] private Material red;
    [SerializeField] private Material blue;
    [SerializeField] private Material green;

    [SerializeField] private float horizontalGap;
    [SerializeField] private float verticalGap;
    
    private List<Pile> pilesList;
    private Queue<string> actionList;
    private void Start()
    {
        pilesList = new List<Pile>();
        actionList = new Queue<string>();
        ReadInput();
    }

    public void ReadInput()
    {
        bool first = true;
        var file = File.ReadAllLines(Application.dataPath + "/StreamingAssets/Test Input.txt");
        foreach (var line in file)
        {
            if (first)
            {
                first = false;
                Initialize(line);
            }
            else
            {
                var var = line.Split(',');
                actionList.Enqueue(var[0][1] + "" + var[1][1]);
            }
        }
        SetCameraPosition();
        DoAction();
    }
    
    private void Initialize(string line)
    {
        List<string> piles = line.Split(',').ToList();
        foreach (var pile in piles)
        {
            var blocks = pile.Split('=');
            CreatePile(blocks[1]);
        }
        PlaceBlocks();
    }

    private void DoAction()
    {
        if (actionList.Count == 0)
        {
            DOVirtual.DelayedCall(3f, (() => Application.Quit()));
            return;
        }
        var action = actionList.Dequeue();
        var from = pilesList[Convert.ToInt32(Char.GetNumericValue(action[0])) - 1];
        var to = pilesList[Convert.ToInt32(Char.GetNumericValue(action[1])) - 1];
        MoveBlocks(from,to);
    }

    private void MoveBlocks(Pile from, Pile to)
    {
        Vector3 target;
        if (to.blockStack.Count == 0)
            target = to.basePosition;
        else
            target = to.blockStack.Peek().transform.position;
        
        var block = from.blockStack.Pop();
        var path = new []{CalculateMiddlePoint(),target + Vector3.up * verticalGap};
        block.transform.DOPath(path, 0.15f, PathType.CatmullRom).OnComplete(() =>
        {
            to.blockStack.Push(block);
            DoAction();
        });
        Vector3 CalculateMiddlePoint()
        {
            var midPoint = (block.transform.position + target)/2 + Vector3.back * 1.5f;
            return midPoint;
        }
    }
    

    private void CreatePile(string input)
    {
        Pile pile = new Pile();
        foreach (var character in input)
        {
            if(character == 'E')
                break;
            Block block = Instantiate(blockPrefab);;
            switch (character)
            {
                case 'R':
                    block.meshRenderer.material = red;
                    break;
                case 'B':
                    block.meshRenderer.material = blue;
                    break;
                case 'G':
                    block.meshRenderer.material = green;
                    break;
            }
            pile.blockStack.Push(block);
        }
        pilesList.Add(pile);
    }

    private void PlaceBlocks()
    {
        var firstPos = transform.position + Vector3.forward * 5 + Vector3.down * 4 + Vector3.left * horizontalGap;
        foreach (var pile in pilesList)
        {
            pile.basePosition = firstPos - Vector3.up * verticalGap;
            Instantiate(placeHolder, pile.basePosition,Quaternion.identity);
            if (pile.blockStack.Count == 0)
            {
                firstPos += Vector3.down * verticalGap * pile.blockStack.Count + Vector3.right * horizontalGap;
                continue;
            }
                

            foreach (var block in pile.blockStack.Reverse())
            {
                block.transform.position = firstPos;
                firstPos+= Vector3.up * verticalGap;
            }
            firstPos += Vector3.down * verticalGap * pile.blockStack.Count + Vector3.right * horizontalGap;
        }
    }

    private void SetCameraPosition()
    {
        var midPoint = (pilesList[0].blockStack.Peek().transform.position + pilesList.Last().basePosition) / 2;
        transform.position = midPoint + Vector3.up * 4 + Vector3.back * pilesList.Count*1.5f;
    }
}
