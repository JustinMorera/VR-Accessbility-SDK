using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System;
using System.Reflection;
using AccessibilityTags;

// Editor script that Adds AltText to ALL GameObjects
[CustomEditor(typeof(GameObject))]
public class AutoAccessibility : Editor
{    
    // Right-click option for GameObjects
    [MenuItem("Tools/Add Accessible Field(s) to entire scene")]
    private static void AddFields(MenuCommand menuCommand)
    {
    #if UNITY_EDITOR
        // Store GameObjects
        GameObject[] objects = GameObject.FindObjectsOfType<GameObject>();
        Renderer renderer;
        Collider collider;
        Rigidbody rigidbody;
        // Iterate through each object
        foreach (GameObject obj in objects)
        {   // Check if object exists
            if (obj != null)
            {
                // Store object's AccessibilityTags script, if it exists
                AccessibilityTags.AccessibilityTags script = obj.GetComponent<AccessibilityTags.AccessibilityTags>();
                // If script exists, update altText to object's name
                if (script == null)
                {
                    script = Undo.AddComponent<AccessibilityTags.AccessibilityTags>(obj);
                    Debug.Log("Alt Text successfully added to " + obj.name);
                }
                // Check if object has an active Collider and a Renderer script attached
                collider = obj.GetComponent<Collider>();
                renderer = obj.GetComponent<Renderer>();
                if (collider != null && collider.enabled == true)
                {
                    if (renderer != null)
                    {
                        string text = "This is a " + obj.name + ". ";

                        // Check if object has a description somewhere
                        Component[] components = obj.GetComponents<Component>();
                        foreach (Component component in components)
                        {
                            if (component != null)
                            {
                                Type type = component.GetType();
                                FieldInfo descriptionField = type.GetField("description", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy);

                                if (descriptionField != null)
                                {
                                    string description = (string)descriptionField.GetValue(component);
                                    Debug.Log("Description found in component: " + obj.name + " - " + type.Name);
                                    text += description + " ";
                                    break;
                                }
                            }
                        }
                        // if (obj.interactable == true)
                        // {
                        //     text += "It is interactible.";
                        // }
                        // else
                        // {
                        //     text += "It is NOT interactible.";
                        // }
                        script.AltText = text;
                        Debug.Log("Alt Text successfully updated for " + obj.name);
                    }
                    else
                    {
                        Debug.Log("Failed to add Alt Text to " + obj.name + " (selected object may not have an active Renderer)");
                    }
                }
                else
                {
                    Debug.Log("Failed to add Alt Text to " + obj.name + " (selected object may not have a Collider)");
                }

                rigidbody = obj.GetComponent<Rigidbody>();
                // Check if object has a Rigidbody script attached for interactibility
                if (rigidbody != null && rigidbody.isKinematic == false) // If isKinematic is false, object can be picked up/manipulated
                {
                    script.Interactable = true;
                }
                else
                {
                    script.Interactable = false;
                }
                // Mark selected GameObject as dirty to save changes
                EditorUtility.SetDirty(obj);
            }
        }
        // Mark scene dirty to save changes to the scene
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    #endif
    }

    [MenuItem("Tools/Remove Accessible Field(s) from entire scene")]
    private static void RemoveFields(MenuCommand menuCommand)
    {
    #if UNITY_EDITOR
        // Store GameObjects
        GameObject[] objects = GameObject.FindObjectsOfType<GameObject>();
        // Iterate through each object
        foreach (GameObject obj in objects)
        {
            if (obj != null)
            {
                // Store object's AccessibilityTags script, if it exists
                AccessibilityTags.AccessibilityTags script = obj.GetComponent<AccessibilityTags.AccessibilityTags>();
                // If script exists, update altText to object's name
                if (script != null)
                {
                    DestroyImmediate(script);
                    Debug.Log("Accessibility Fields successfully removed from " + obj.name);
                }
            }
        }
        // Mark scene dirty to save changes to the scene
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    #endif
    }
}
