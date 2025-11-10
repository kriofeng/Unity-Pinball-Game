using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class TagSetup
{
    static TagSetup()
    {
        // 在编辑器加载时自动设置标签
        SetupTags();
    }
    
    [MenuItem("Tools/Setup Tags")]
    static void SetupTags()
    {
        // 添加所需的标签
        AddTag("Ball");
        AddTag("Target");
        
        Debug.Log("标签设置完成！");
    }
    
    static void AddTag(string tag)
    {
        // 检查标签是否已存在
        if (!TagExists(tag))
        {
            // 获取标签管理器
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty tagsProp = tagManager.FindProperty("tags");
            
            // 添加新标签
            tagsProp.arraySize++;
            SerializedProperty newTagProp = tagsProp.GetArrayElementAtIndex(tagsProp.arraySize - 1);
            newTagProp.stringValue = tag;
            tagManager.ApplyModifiedProperties();
            
            Debug.Log($"标签 '{tag}' 已添加");
        }
        else
        {
            Debug.Log($"标签 '{tag}' 已存在");
        }
    }
    
    static bool TagExists(string tag)
    {
        // 检查标签是否存在
        try
        {
            GameObject.FindGameObjectWithTag(tag);
            return true;
        }
        catch
        {
            return false;
        }
    }
}




