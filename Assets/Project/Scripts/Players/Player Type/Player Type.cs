using UnityEngine;

public class PlayerType : MonoBehaviour
{
    [SerializeField] private bool isPlayerCat;
    [SerializeField] private bool isPlayerGhost;

    public bool IsPlayerCat { get => isPlayerCat; set => isPlayerCat = value; }

    public bool IsPlayerGhost { get => isPlayerGhost; set => isPlayerGhost = value; }
}