using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Enhanced level generator that creates a sequence of random rooms.
/// Supports different room types and random generation patterns.
/// </summary>
public class RoomGenerator : MonoBehaviour
{
    [Header("Room Configuration")]
    [Tooltip("List of starting room prefabs.")]
    public GameObject[] startRooms;
    [Tooltip("List of normal room prefabs.")]
    public GameObject[] normalRooms;
    [Tooltip("List of boss/end room prefabs.")]
    public GameObject[] endRooms;

    [Header("Generation Settings")]
    [Tooltip("Number of rooms to generate in the level.")]
    public int roomCount = 5;
    [Tooltip("Horizontal offset between rooms.")]
    public float roomWidth = 20f;
    [Tooltip("Random seed for level generation. 0 = random.")]
    public int levelSeed = 0;

    private List<GameObject> generatedRooms = new List<GameObject>();

    void Start()
    {
        GenerateLevel();
    }

    public void GenerateLevel()
    {
        // Clear existing rooms
        ClearLevel();

        // Set random seed if specified
        if (levelSeed != 0)
        {
            Random.InitState(levelSeed);
        }

        float offset = 0f;

        for (int i = 0; i < roomCount; i++)
        {
            GameObject roomPrefab = GetRoomPrefab(i);
            if (roomPrefab != null)
            {
                Vector3 pos = new Vector3(offset, 0f, 0f);
                GameObject room = Instantiate(roomPrefab, pos, Quaternion.identity, transform);
                generatedRooms.Add(room);
                offset += roomWidth;
            }
        }
    }

    GameObject GetRoomPrefab(int roomIndex)
    {
        if (roomIndex == 0 && startRooms.Length > 0)
        {
            // First room - use start room
            return startRooms[Random.Range(0, startRooms.Length)];
        }
        else if (roomIndex == roomCount - 1 && endRooms.Length > 0)
        {
            // Last room - use end room
            return endRooms[Random.Range(0, endRooms.Length)];
        }
        else if (normalRooms.Length > 0)
        {
            // Middle rooms - use normal rooms
            return normalRooms[Random.Range(0, normalRooms.Length)];
        }

        return null;
    }

    public void ClearLevel()
    {
        foreach (GameObject room in generatedRooms)
        {
            if (room != null)
            {
                DestroyImmediate(room);
            }
        }
        generatedRooms.Clear();
    }

    void OnValidate()
    {
        // Ensure room count is at least 1
        roomCount = Mathf.Max(1, roomCount);
    }
}