using UnityEngine;

[CreateAssetMenu(fileName = "PlayerRole", menuName = "Scriptable Objects/PlayerRole")]
public class PlayerRole : ScriptableObject
{
    public string self_name;
    public GameObject prefab;
    public Sprite icon;
}
