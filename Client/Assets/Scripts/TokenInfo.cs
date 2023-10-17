using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TokenInfo : MonoBehaviour
{
    public PlayManager playManager;
    public TokenMove tokenMove;
    public bool isMine = false;
    private Vector2 originPos;

    public SUSPECT suspect;
    public ROOM currentState = ROOM.MOVING;
    [HideInInspector]
    public int stepCount = 0;



    public void UpdateLocation(float x, float y)
    {
        tokenMove.MoveToTarget(x, y);
    }

    public Vector2 GetMovingDistance()
    {
        return new Vector2(originPos.x - tokenMove.currentPoint.x, originPos.y - tokenMove.currentPoint.y);
    }

    public void UpdateState(ROOM _stateInfo = ROOM.MOVING)
    {
        currentState = _stateInfo;

        if (!isMine)
            return;

        if(currentState == ROOM.FINALCLUE)
        {
            playManager.isFinal = true;
        }
        if (_stateInfo != ROOM.MOVING && stepCount > 0)
        {
            stepCount = 0;
            playManager.UpdateStepCount(stepCount);
        }
        else
        {
            if (stepCount > 0)
                stepCount--;
        }
    }

    public void MoveToDice(int _count)
    {
        originPos = tokenMove.currentPoint;
        stepCount = _count;
    }
}
