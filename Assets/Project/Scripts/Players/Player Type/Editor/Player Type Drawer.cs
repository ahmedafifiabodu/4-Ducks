using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerType))]
public class PlayerTypeDrawer : Editor
{
    public override void OnInspectorGUI()
    {
        PlayerType playerType = (PlayerType)target;

        bool wasPlayerCat = playerType.IsPlayerCat;
        playerType.IsPlayerCat = EditorGUILayout.Toggle("Is Player Cat", playerType.IsPlayerCat);

        if (wasPlayerCat != playerType.IsPlayerCat)
        {
            if (playerType.IsPlayerCat)
                playerType.gameObject.AddComponent<Cat>();
            else
            {
                Cat catComponent = playerType.gameObject.GetComponent<Cat>();

                if (catComponent != null)
                    DestroyImmediate(catComponent);
            }
        }

        if (!playerType.IsPlayerCat)
        {
            bool wasPlayerGhost = playerType.IsPlayerGhost;
            playerType.IsPlayerGhost = EditorGUILayout.Toggle("Is Player Ghost", playerType.IsPlayerGhost);

            if (wasPlayerGhost != playerType.IsPlayerGhost)
            {
                if (playerType.IsPlayerGhost)
                    playerType.gameObject.AddComponent<Ghost>();
                else
                {
                    Ghost ghostComponent = playerType.gameObject.GetComponent<Ghost>();
                    if (ghostComponent != null)
                        DestroyImmediate(ghostComponent);
                }
            }
        }

        if (GUI.changed)
            EditorUtility.SetDirty(playerType);
    }
}