using System.IO;
using UnityEngine;

public static class PlayerSaveSystem
{
    private static string FilePath =>
        Path.Combine(Application.persistentDataPath, "PlayerData.json");

    public static void SavePlayerData(PlayerData playerData)
    {
        string json = JsonUtility.ToJson(playerData, true);
        File.WriteAllText(FilePath, json);

        Debug.Log("PlayerData Saved!");
    }
}