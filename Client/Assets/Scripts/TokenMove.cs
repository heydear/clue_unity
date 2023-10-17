using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenMove : MonoBehaviour
{
    public PlayManager playManager;
    public TokenInfo tokenInfo;
    public Vector2 currentPoint = Vector2.zero;

    private readonly float OneStepValue = 3.7f;

    private float horizontalMove = 0f;
    private float verticalMove = 0f;

    

    private void Update()
    {
        if (!tokenInfo.isMine)
            return;
        if (tokenInfo.stepCount <= 0/* && tokenInfo.currentState == ROOM.MOVING*/)
            return;

        horizontalMove = 0f;
        verticalMove = 0f;

        if (Input.GetKeyDown(KeyCode.W) && !Input.GetMouseButton(1))
        {
            verticalMove = OneStepValue;

            currentPoint.y++;
            if (tokenInfo.currentState == ROOM.MOVING)
            {
                tokenInfo.stepCount--;
                playManager.UpdateStepCount(tokenInfo.stepCount);
            }
        }
        else if (Input.GetKeyDown(KeyCode.S) && !Input.GetMouseButton(1))
        {
            verticalMove = -OneStepValue;

            currentPoint.y--;
            if (tokenInfo.currentState == ROOM.MOVING)
            {
                tokenInfo.stepCount--;
                playManager.UpdateStepCount(tokenInfo.stepCount);
            }
        }
        else if (Input.GetKeyDown(KeyCode.D) && !Input.GetMouseButton(1))
        {
            horizontalMove = OneStepValue;

            currentPoint.x++;
            if (tokenInfo.currentState == ROOM.MOVING)
            {
                tokenInfo.stepCount--;
                playManager.UpdateStepCount(tokenInfo.stepCount);
            }
        }
        else if (Input.GetKeyDown(KeyCode.A) && !Input.GetMouseButton(1))
        {
            horizontalMove = -OneStepValue;

            currentPoint.x--;
            if (tokenInfo.currentState == ROOM.MOVING)
            {
                tokenInfo.stepCount--;
                playManager.UpdateStepCount(tokenInfo.stepCount);
            }
        }

        if ((transform.position.z + verticalMove) < -40f || (transform.position.z + verticalMove) > 42f)
            verticalMove = 0f;
        if ((transform.position.x + horizontalMove) < -41f || (transform.position.x + horizontalMove) > 41f)
            horizontalMove = 0f;

        transform.Translate(new Vector3(horizontalMove, 0f, verticalMove));
    }

    public void MoveToTarget(float x, float y)
    {
        float targetX = x - currentPoint.x;
        float targetY = y - currentPoint.y;

        currentPoint.x = x;
        currentPoint.y = y;

        transform.Translate(new Vector3(OneStepValue * targetX, 0f, OneStepValue * targetY));
    }
}
