using UnityEngine;

public class GameTile : MonoBehaviour
{
    static Quaternion northRotation = Quaternion.Euler( 0f,    0f,     0f),
                    eastRotation = Quaternion.Euler(    0f,    90f,    0f),
                    southRotation = Quaternion.Euler(   0f,    180f,   0f),
                    westRotation = Quaternion.Euler(    0f,    270f,   0f);


    [SerializeField] Transform arrow = default;
    public bool IsAlternative { get; set; }

    GameTile north, east, south, west; //각 tile은 주변 타일 ref 가지고 있음
    GameTile nextOnPath; //갈 수 있는 다음 tile ref
    int distance; //목적지까지 남은 거리

    GameTileContent content;

    public GameTileContent Content
    {
        get => content;
        set
        {
            Debug.Assert(value != null, "Null assigned to content!");
            if (content != null)
            {
                content.Recycle();
            }
            content = value;
            content.transform.localPosition = transform.localPosition;
        }
    }

    public bool HasPath => distance != int.MaxValue; //property getter

    public GameTile NextTileOnPath => nextOnPath; //next on path를 반환하는 propery, enemy에게 정보를 주기 위해.

    //각 방향으로의 growpath public methods
    public GameTile GrowPathNorth() => GrowPathTo(north,Direction.South);
    public GameTile GrowPathEast() => GrowPathTo(east, Direction.West);
    public GameTile GrowPathSouth() => GrowPathTo(south, Direction.North);
    public GameTile GrowPathWest() => GrowPathTo(west, Direction.East);

    public Vector3 ExitPoint { get; private set; } //부드러운 움직임을 만들기 위해. enemy에 할당하는 것이 아니라 타일별로 할당
    public Direction PathDirection { get; private set; }


    public static void MakeEastWestNeighbor(GameTile east, GameTile west) //특정 tile이 주체가 되는 method가 아니므로 static으로 만드는 것이 명확(ex, vector3.dot / distance)
    {
        Debug.Assert(west.east == null && east.west == null, "Redefine neighbors!"); //한번 정하면 바뀔 수 없도록 
        west.east = east;
        east.west = west;
    }
    public static void MakeNorthSouthNeighbors(GameTile north, GameTile south)
    {
        Debug.Assert(south.north == null && north.south == null, "Redefined neighbors!");
        south.north = north;
        north.south = south;
    }

    public void ClearPath()
    {
        distance = int.MaxValue;
        nextOnPath = null;
    }

    //목적지 타일 setting method
    public void BecomeDestination()
    {
        distance = 0;
        ExitPoint = transform.localPosition; //destination의 경우 center가 exit point
        nextOnPath = null;
    }

    //현재 타일로 도달 가능한 근처 tile에 정보 삽입
    GameTile GrowPathTo(GameTile neighbor, Direction direction)
    {
        Debug.Assert(HasPath, "No Path!");
        if(neighbor == null || neighbor.HasPath) //null이거나, 이미 path가 설정된 경우 중단
        {
            return null;
        }
        neighbor.distance = distance + 1;
        neighbor.nextOnPath = this;
        neighbor.ExitPoint = neighbor.transform.localPosition + direction.GetHalfVector();
        neighbor.PathDirection = direction;
        return neighbor.Content.Type != GameTileContentType.Wall ? neighbor : null; //wall이면 null 반환
    }

    public void ShowPath()
    {
        if(distance == 0)
        {
            arrow.gameObject.SetActive(false); //목적지 tile은 deactivate
            return;
        }
        arrow.gameObject.SetActive(true);
        arrow.localRotation = nextOnPath == north ? northRotation : //nextonpath가 어느 방향에 있는지에 따라 rotation
                            nextOnPath == east ? eastRotation :
                            nextOnPath == south ? southRotation :
                            westRotation;
    }

    public void HidePath()
    {
        arrow.gameObject.SetActive(false);
    }
}
