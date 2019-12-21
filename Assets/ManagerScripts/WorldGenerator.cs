using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using Random = System.Random;
using RoomTuple = System.Tuple<int, int>;
using RoomList = System.Collections.Generic.List<System.Tuple<int, int>>;

public class Room
{
    private readonly Room[] Neighbors;
    public RoomTuple position;
    public bool visited;
    

    public Room(RoomTuple rt, int sceneIndex, int depth)
    {
        SceneIndex = sceneIndex;
        Depth = depth;
        Neighbors = new Room[] {null, null, null, null};
        position = rt;
    }

    internal int SceneIndex { get; set; }
    internal int Depth { get; set; }

    public void Set(Room otherRoom)
    {
        //Determine what direction the room is relative to the current one
        var rX = otherRoom.position.Item1 - position.Item1;
        var rY = otherRoom.position.Item2 - position.Item2;
        var direction = (int) (Math.Atan2(rY, rX) / (Math.PI / 2));
        //Math.Atan2 correctly maps 3 of the directions to their correct enums, but (0, -1) results in a -1,
        //so we add 4 and mod by 4 to turn it into 3, while leaving other directions unaffected
        direction = (direction + 4) % 4;


        Neighbors[direction] = otherRoom;
        if (!otherRoom.HasNeighbor(World.FlipDirection((DirectorScript.Direction) direction)))
            otherRoom.Set(this);

        Assert.AreEqual(this, otherRoom.GetNeighbor(World.FlipDirection(direction)),
            "ERROR: Inconsistent neighborhood.");
    }

    public Room GetNeighbor(DirectorScript.Direction d)
    {
        return Neighbors[(int) d];
    }

    public Room GetNeighbor(int d)
    {
        return Neighbors[d];
    }

    public bool HasNeighbor(int d)
    {
        return Neighbors[d] != null;
    }

    public bool HasNeighbor(DirectorScript.Direction d)
    {
        return Neighbors[(int) d] != null;
    }
}

public static class World
{
    private static Dictionary<RoomTuple, Room> _world;
    private static Room m_DeepestRoom;
    public static int Width;
    public static int Height;
    private static Random rand;
    public static int[] normalScenes = new []{2, 3, 4, 5, 9, 10};
    public static int[] heavyScenes = new []{7, 8, 6};
    private static int rootScene = 0;
    

    /// <summary>
    ///     Creates a number of levels
    /// </summary>
    /// <param name="width">The max width of the world measured in rooms. Ideally > 10</param>
    /// <param name="height">The max height of the world measured in rooms. Ideally > 10</param>
    /// <param name="numberOfLevels">The number of levels to create.</param>
    public static void CreateLevels(int width, int height, int numberOfLevels)
    {
        rand = new Random();
        _world = new Dictionary<Tuple<int, int>, Room>();
        m_DeepestRoom = null;
        Width = width;
        Height = height;
        var RootRoom = new Room(new RoomTuple(4, 4), rootScene, 1);
        _world[new RoomTuple(4, 4)] = RootRoom;

        for (var i = 0; i < numberOfLevels; i++)
        {
            var level = CreateLevel(RootRoom, 1, new RoomList());
            if (m_DeepestRoom == null || level != null && level.Depth > m_DeepestRoom.Depth) m_DeepestRoom = level;
        }

        m_DeepestRoom.SceneIndex = heavyScenes[UnityEngine.Random.Range(0, heavyScenes.Length)];

        #if UNITY_EDITOR
            WorldTest();
        #endif
    }

    /// <summary>
    ///     A recursive method that uses a random number generator to determine in which direction to create a room.
    ///     Unless the world plane becomes full, each non-recursive call to this function should generate 1 room.
    /// </summary>
    /// <param name="x">The x position of a room that a new room may be a neighbor to.</param>
    /// <param name="y">The y position of a room that a new room may be a neighbor to.</param>
    /// <param name="depth">The current depth of the room being considered</param>
    /// <param name="visited">
    ///     A list of visted rooms. Necessary to prevent infinite recursion. The algorithm will not
    ///     reconsider entering a visted node.
    /// </param>
    /// <returns>The new room.</returns>
    public static Room CreateLevel(Room CurrentRoom, int depth, RoomList visited)
    {
        //Because changes to a visited list on one recursive call shouldn't modify lists from other calls, the visited list is copied locally.
        var newVisited = new RoomList(visited);
        newVisited.Add(CurrentRoom.position);

        //This array will assign a weight to each direction
        var direction = new float[4];
        //This array will indicate in what order each direction will be tried for creating the next room
        var directionSorted = new int[4];

        //Assign random weights to each direction. The weight determines in what order each direction will be used to try and create a room.
        //Directions that contain a room with a shallow depth are prioritized. With greater depth, this priority is decreased.
        //This encourages having long branches, but not too long.
        for (var i = 0; i < 4; i++)
        {
            direction[i] = (float) rand.NextDouble() * 0.6f;
            //Alter weight if direction contains a connected room that hasn't been visited
            if (CurrentRoom.HasNeighbor(i) && !newVisited.Contains(CurrentRoom.GetNeighbor(i).position))
                direction[i] += 0.4f - (float) Math.Log10(depth) / 2;
        }

        //Sort the weighted list so that the direction with the greatest priority is first, second high priority is second, etc
        for (var i = 0; i < 4; i++)
        {
            var indexOfMax = Array.IndexOf(direction, direction.Max());
            directionSorted[i] = indexOfMax;
            direction[indexOfMax] = 0;
        }

        //For each direction in the list, step into another room, if it exists, and recurse from there, or create a new room neighboring the current one, or if it can't be made, move on to thee next direction.
        //If a room is created, return from the function.
        for (var i = 0; i < 4; i++)
        {
            var CurrentDirection = (DirectorScript.Direction) directionSorted[i];
            //Set positions of the next room based on current direction, set by i.
            var nextPos = GetVectorFromDirection(CurrentRoom.position, CurrentDirection);

            //Step into adjacent room if it is connected and hasn't been visited
            if (CurrentRoom.HasNeighbor(CurrentDirection) &&
                !newVisited.Contains(CurrentRoom.GetNeighbor(CurrentDirection).position))
            {
                var newRoom = CreateLevel(CurrentRoom.GetNeighbor(CurrentDirection), depth + 1, newVisited);
                //If a room has been created, end function call.
                if (newRoom != null) return newRoom;
            }
            //Attempt to create room neighboring current room.
            //The parameter for RoomExists was initially nextX and nextY. Make sure that changing it to currentroom isnt a mistake
            //TODO: ^^
            else if (!RoomExists(CurrentRoom, CurrentDirection) && Enumerable.Range(0, Width).Contains(nextPos.Item1) &&
                     Enumerable.Range(0, Height).Contains(nextPos.Item2))
            {
                if (nextPos.Item1 == 4 && nextPos.Item2 == 4) Debug.Log("Exception");
                Assert.IsTrue(!_world.ContainsKey(nextPos));
                var roomScene = normalScenes[UnityEngine.Random.Range(0, normalScenes.Length - 1)];
                var NewRoom = new Room(nextPos, roomScene, depth);
                _world[nextPos] = NewRoom;
                //Connect the parameter room to the new room
                CurrentRoom.Set(NewRoom);
                return _world[nextPos];
            }
            //Otherwise, try next direction
        }

        return null;
    }


    public static Dictionary<RoomTuple, Room> GetWorld()
    {
        return _world;
    }

    /// <summary>
    ///     Returns true if a room in the direction of the given room exists
    /// </summary>
    /// <param name="r">The position of a known room</param>
    /// <param name="d">The direction from the known room being tested. </param>
    /// <returns>True if a room exists in the direction to the given room.</returns>
    private static bool RoomExists(Room r, DirectorScript.Direction d)
    {
        return _world.ContainsKey(GetVectorFromDirection(r.position, d));
    }

    public static int GetSize()
    {
        var count = 0;
        foreach (var r in _world.Values)
            if (r != null)
                count++;

        return count;
    }


    /// <summary>
    ///     Returns true if the given room is connected to a room in the given direction
    /// </summary>
    /// <param name="pos">Position of the room.</param>
    /// <param name="d">The direction of the room relative to the given room.</param>
    /// <returns></returns>
    internal static bool RoomConnected(RoomTuple pos, DirectorScript.Direction d)
    {
        var x = pos.Item1;
        var y = pos.Item2;
        if (Enumerable.Range(0, Width).Contains(x) && Enumerable.Range(0, Height).Contains(y) && _world[pos] != null)
            return _world[pos].GetNeighbor(d) != null;
        return false;
    }

    /// <summary>
    ///     Takes a Direction and returns its opposite.
    /// </summary>
    /// <param name="d">The direction to be flipped</param>
    /// <returns></returns>
    internal static DirectorScript.Direction FlipDirection(DirectorScript.Direction d)
    {
        return (DirectorScript.Direction) ((int) (d + 2) % 4);
    }

    internal static DirectorScript.Direction FlipDirection(int d)
    {
        return (DirectorScript.Direction) ((d + 2) % 4);
    }

    public static RoomTuple GetVectorFromDirection(RoomTuple pos, DirectorScript.Direction d)
    {
        var nextX = pos.Item1 + (int) Math.Cos(Math.PI / 2 * (int) d);
        var nextY = pos.Item2 + (int) Math.Sin(Math.PI / 2 * (int) d);

        return new RoomTuple(nextX, nextY);
    }

    private static void WorldTest()
    {
        foreach (var room in _world.Values)
        foreach (DirectorScript.Direction d in Enum.GetValues(typeof(DirectorScript.Direction)))
            if (room.HasNeighbor(d))
            {
                if (!Equals(room, room.GetNeighbor(d).GetNeighbor(FlipDirection(d))))
                    throw new Exception("Neighborhood inconsistency");
            }
            else
            {
                var pos = room.position;

                try
                {
                    if (_world[GetVectorFromDirection(pos, d)].HasNeighbor(FlipDirection(d)))
                        throw new Exception("Neighborhood inconsistency");
                }
                catch (KeyNotFoundException e)
                {
                }
            }
    }

    public static Room GetRoom(RoomTuple t)
    {
        return _world[t];
    }

    public static Room GetDeepestRoom()
    {
        return m_DeepestRoom;
    }
}