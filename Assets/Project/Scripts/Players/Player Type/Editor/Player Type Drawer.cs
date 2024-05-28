using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerType))]
public class PlayerTypeDrawer : Editor
{
    public override void OnInspectorGUI()
    {
        PlayerType playerType = (PlayerType)target;

        bool wasPlayerGhost = playerType.IsPlayerGhost;
        bool wasPlayerCat = playerType.IsPlayerCat;

        if (!wasPlayerGhost)
            playerType.IsPlayerCat = EditorGUILayout.Toggle("Is Player Cat", playerType.IsPlayerCat);

        if (!wasPlayerCat)
            playerType.IsPlayerGhost = EditorGUILayout.Toggle("Is Player Ghost", playerType.IsPlayerGhost);

        if (wasPlayerCat != playerType.IsPlayerCat)
        {
            if (playerType.IsPlayerCat)
            {
                playerType.gameObject.AddComponent<Cat>();

                if (playerType.gameObject.TryGetComponent<Ghost>(out var ghostComponent))
                    DestroyImmediate(ghostComponent);
            }
            else
            {
                if (playerType.gameObject.TryGetComponent<Cat>(out var catComponent))
                    DestroyImmediate(catComponent);
            }
        }

        if (wasPlayerGhost != playerType.IsPlayerGhost)
        {
            if (playerType.IsPlayerGhost)
            {
                playerType.gameObject.AddComponent<Ghost>();

                if (playerType.gameObject.TryGetComponent<Cat>(out var catComponent))
                    DestroyImmediate(catComponent);
            }
            else
            {
                if (playerType.gameObject.TryGetComponent<Ghost>(out var ghostComponent))
                    DestroyImmediate(ghostComponent);
            }
        }

        if (GUI.changed)
            EditorUtility.SetDirty(playerType);
    }
}