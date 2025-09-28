using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Automatically defines scripting symbols based on available packages.
/// This helps ensure scripts compile correctly regardless of which packages are installed.
/// </summary>
public class PackageDefines
{
#if UNITY_EDITOR
    [InitializeOnLoadMethod]
    static void DefinePackageSymbols()
    {
        var targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
        var currentDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
        var defineList = new System.Collections.Generic.List<string>(currentDefines.Split(';'));

        // Check for Cinemachine
        if (System.Type.GetType("Cinemachine.CinemachineVirtualCamera, Cinemachine") != null)
        {
            if (!defineList.Contains("CINEMACHINE_AVAILABLE"))
            {
                defineList.Add("CINEMACHINE_AVAILABLE");
            }
        }
        else
        {
            defineList.Remove("CINEMACHINE_AVAILABLE");
        }

        // Check for TextMeshPro (try multiple assembly names)
        bool tmpAvailable = false;
        var tmpTypes = new string[]
        {
            "TMPro.TextMeshProUGUI, Unity.TextMeshPro",
            "TMPro.TextMeshProUGUI, Unity.TextMeshPro.Runtime",
            "TMPro.TextMeshProUGUI",
            "TMPro.TextMeshProUGUI, Assembly-CSharp"
        };

        foreach (var typeName in tmpTypes)
        {
            if (System.Type.GetType(typeName) != null)
            {
                tmpAvailable = true;
                break;
            }
        }

        if (tmpAvailable)
        {
            if (!defineList.Contains("TMP_PRESENT"))
            {
                defineList.Add("TMP_PRESENT");
            }
        }
        else
        {
            defineList.Remove("TMP_PRESENT");
        }

        // Check for UI package (try multiple assembly names)
        bool uiAvailable = false;
        var uiTypes = new string[]
        {
            "UnityEngine.UI.Text, UnityEngine.UI",
            "UnityEngine.UI.Text",
            "UnityEngine.UI.Text, Assembly-CSharp"
        };

        foreach (var typeName in uiTypes)
        {
            if (System.Type.GetType(typeName) != null)
            {
                uiAvailable = true;
                break;
            }
        }

        if (uiAvailable)
        {
            if (!defineList.Contains("UI_AVAILABLE"))
            {
                defineList.Add("UI_AVAILABLE");
            }
        }
        else
        {
            defineList.Remove("UI_AVAILABLE");
        }

        // Apply the updated defines
        var newDefines = string.Join(";", defineList.ToArray());
        if (newDefines != currentDefines)
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, newDefines);
            Debug.Log($"Updated scripting defines: {newDefines}");
        }
    }
#endif
}