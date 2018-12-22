using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AttackAction))]
public class AttackEditor : Editor {

    bool showAIVariables = false;

    public void OnInspectorGUILegacy() {
        AttackAction myTarget = (AttackAction)target;


        GUIStyle titleStyle = new GUIStyle();
        titleStyle.fontSize = 20;

        GUILayout.Label("Base Variables", titleStyle);

        base.OnInspectorGUI();

        GUILayout.Space(8);

        showAIVariables = EditorGUILayout.Foldout(showAIVariables, "AI Variables");

        if (showAIVariables) {
            myTarget.MaxCooldown = EditorGUILayout.IntField("Cooldown", myTarget.MaxCooldown);
        }

    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
    }

}
