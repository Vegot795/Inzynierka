using UnityEngine;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "EnvironmentData", menuName = "MapComponents/EnvironmentType")]
public class EnvironmentData : ScriptableObject
{
    public EnvironmentType type;
    public Sprite WallLeft;
    public Sprite WallRight;
    public Sprite WallTop;
    public Sprite WallBottom;
    public Sprite WallTopRight;
    public Sprite WallBottomRight;
    public Sprite WallTopLeft;
    public Sprite WallBottomLeft;
    public Sprite Floor;
    public Sprite CorridorLeftTop;
    public Sprite CorridorLeftBottom;
    public Sprite CorridorRightTop;
    public Sprite CorridorRightBottom;
}

