using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum Direction
{
    LEFT,
    RIGHT
}

public class Minecart : MonoBehaviour
{
    [Header("Movement Settings")]
    public Direction direction;
    public float speed = 3;

    [Header("Tiles")]
    public Tile forwardSlope;
    public Tile backwardSlope;
    public Tile straight;

    [Header("Sprites")]
    public Sprite cartLevel;
    public Sprite cartForward;
    public Sprite cartBackward;
    public new SpriteRenderer renderer;

    [Header("Scan Transforms")]
    public Transform left;
    public Transform right;
    public Transform upLeft;
    public Transform upRight;
    public Transform downLeft;
    public Transform downRight;
    public Transform down;

    public Transform bottomLeft;
    public Transform bottomRight;

    private void OnEnable()
    {
        start = transform.position;
        currentTile = TilemapManager.GetTile(TileLayer.RAILS, transform.position);

        //correct height
        if ((currentTile?.Equals(forwardSlope) ?? false) || (currentTile?.Equals(backwardSlope) ?? false))
        {
            transform.position += Vector3.up;
        }

        if (direction == Direction.RIGHT)
        {
            UpdateDirection(upRight, right, downRight, down, upLeft, left, downLeft, bottomRight, bottomLeft, forwardSlope, backwardSlope);
        }
        else
        {
            UpdateDirection(upLeft, left, downLeft, down, upRight, right, downRight, bottomLeft, bottomRight, backwardSlope, forwardSlope);
        }
    }

    Vector3 start;
    Vector3 end;
    float t = 0;

    Tile upFrontTile, midFrontTile, downFrontTile, downTile, backTile, currentTile, downBackTile, lastSlope, bottomFrontTile;

    private void Update()
    {
        transform.position = Vector3.Lerp(start, end, t);
        t += Time.deltaTime * speed;

        if(t >= 1)
        {
            transform.position = end;
            t = 0;
            if (direction == Direction.RIGHT)
            {
                UpdateDirection(upRight, right, downRight, down, upLeft, left, downLeft, bottomRight, bottomLeft, forwardSlope, backwardSlope);
            }
            else
            {
                UpdateDirection(upLeft, left, downLeft, down, upRight, right, downRight, bottomLeft, bottomRight, backwardSlope, forwardSlope);
            }
        }

        UpdateCartSpriteOrientation();
    }

    void UpdateCartSpriteOrientation()
    {
        Tile currentDown = TilemapManager.GetTile(TileLayer.RAILS, down.position);
        Tile current = TilemapManager.GetTile(TileLayer.RAILS, transform.position);

        if(current && current.Equals(straight))
        {
            renderer.sprite = cartLevel;
            renderer.transform.localPosition = Vector3.zero;
            return;
        }

        if(currentDown)
        {
            if(currentDown.Equals(forwardSlope))
            {
                renderer.sprite = cartForward;
                renderer.transform.localPosition = Vector3.down * 0.5f;
                return;
            }

            if (currentDown.Equals(backwardSlope))
            {
                renderer.sprite = cartBackward;
                renderer.transform.localPosition = Vector3.down * 0.5f;
                return;
            }
        }
    }

    void UpdateDirection(Transform upFront, Transform midFront, Transform downFront, Transform down, Transform upBack, Transform midBack, Transform downBack, Transform bottomFront, Transform bottomBack, Tile upRail, Tile downRail)
    {
        Vector3 startC = start;
        start = transform.position;

        upFrontTile = TilemapManager.GetTile(TileLayer.RAILS, upFront.position);
        midFrontTile = TilemapManager.GetTile(TileLayer.RAILS, midFront.position);
        downFrontTile = TilemapManager.GetTile(TileLayer.RAILS, downFront.position);
        downTile = TilemapManager.GetTile(TileLayer.RAILS, down.position);
        backTile = TilemapManager.GetTile(TileLayer.RAILS, midBack.position);
        downBackTile = TilemapManager.GetTile(TileLayer.RAILS, downBack.position);
        currentTile = TilemapManager.GetTile(TileLayer.RAILS, transform.position);
        bottomFrontTile = TilemapManager.GetTile(TileLayer.RAILS, bottomFront.position);

        //go straight
        if ((midFrontTile?.Equals(straight) ?? false) && currentTile)
        {
            end = midFront.position;
            return;
        }

        //switchback
        if ((currentTile?.Equals(straight) ?? false) && lastSlope)
        {
            if (lastSlope.Equals(upRail) && (backTile?.Equals(downRail) ?? false))
            {
                end = upBack.position;
                lastSlope = downRail;
                Direction oldDir = direction;
                direction = direction == Direction.RIGHT ? Direction.LEFT : Direction.RIGHT;

                EventBus.MinecartEvents.OnMinecartReversed(this, oldDir, direction);

                return;
            }
        }

        //dip
        if (downFrontTile?.Equals(upRail) ?? false)
        {
            end = midFront.position;
            lastSlope = upRail;
            return;
        }

        //up and over to straight
        if ((downTile?.Equals(upRail) ?? false) && (midFrontTile?.Equals(straight) ?? false))
        {
            end = midFront.position;
            lastSlope = upRail;
            return;
        }


        //continue down
        if(downTile?.Equals(downRail) ?? false)
        {
            end = downFront.position;
            lastSlope = downRail;
            return;
        }

        //up
        if ((midFrontTile?.Equals(upRail) ?? false) && !currentTile)
        {
            end = upFront.position;
            lastSlope = upRail;
            return;
        }

        //straight to down
        if (downFrontTile?.Equals(downRail) ?? false)
        {
            end = midFront.position;
            lastSlope = downRail;
            return;
        }

        //straight to up
        if (midFrontTile?.Equals(upRail) ?? false)
        {
            end = upFront.position;
            lastSlope = upRail;
            return;
        }

        end = start;
        t = 1;
        direction = direction == Direction.RIGHT ? Direction.LEFT : Direction.RIGHT;
        
    }
}