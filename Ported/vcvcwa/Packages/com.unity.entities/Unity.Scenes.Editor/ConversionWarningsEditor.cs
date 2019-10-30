using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Entities;
using Unity.Entities.Conversion;
using Unity.Profiling;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Unity.Scenes.Editor
{
    [InitializeOnLoad]
    class ConversionWarningsGui
    {
        static readonly ProfilerMarker k_MsConversionWarningsMarker = new ProfilerMarker("ConversionWarningsGui.ConversionWarnings");
        static readonly List<ConvertToEntity> s_ConvertToEntityBuffer = new List<ConvertToEntity>(8);

        static ConversionWarningsGui()
        {
            UnityEditor.Editor.finishedDefaultHeaderGUI += Callback;
        }

        static void Callback(UnityEditor.Editor editor)
        {
            using (k_MsConversionWarningsMarker.Auto())
            {
                var warning = GetWarning(editor.target as GameObject);
                if (warning != null)
                    EditorGUILayout.HelpBox(warning, MessageType.Error, true);
            }
        }
        
        static string GetWarning(GameObject gameObject)
        {
            // only care about scene objects
            if (gameObject == null)
                return null;

            // assets have no scene context
            if (gameObject.IsPrefab())
                return null;

            // editing in a preview scene (like a prefab in isolation mode) also has no context
            if (EditorSceneManager.IsPreviewSceneObject(gameObject))
                return null;

            var isSubScene = EditorEntityScenes.IsEntitySubScene(gameObject.scene);
            gameObject.GetComponentsInParent(true, s_ConvertToEntityBuffer);
            var convertToEntity = s_ConvertToEntityBuffer.Count > 0;
            s_ConvertToEntityBuffer.Clear();

            var willBeConverted = convertToEntity | isSubScene;

            if (!willBeConverted)
            {
                Type convertType = null;
                foreach (var behaviour in gameObject.GetComponents<MonoBehaviour>())
                {
                    if (behaviour != null && behaviour.GetType().GetCustomAttribute<RequiresEntityConversionAttribute>(true) != null)
                    {
                        convertType = behaviour.GetType();
                        break;
                    }
                }

                if (convertType != null)
                    return
                        $"The {convertType.Name} component on '{gameObject.name}' is meant for entity conversion, " +
                        $"but it is not part of a {nameof(SubScene)} or {nameof(ConvertToEntity)} component.\n" +
                        $"Please move the {nameof(GameObject)} to a {nameof(SubScene)} or add the {nameof(ConvertToEntity)} component.";
            }

            if (isSubScene && convertToEntity)
                return
                    $"'{gameObject.name}' will be converted due to being in a {nameof(SubScene)}. {nameof(ConvertToEntity)} " +
                    $"will have no effect. Please remove the {nameof(ConvertToEntity)} component.";
            
            if (isSubScene && gameObject.GetComponent<GameObjectEntity>() != null)
                return
                    $"'{gameObject.name}' will be converted due to being in a {nameof(SubScene)}. {nameof(GameObjectEntity)} " +
                    $"will have no effect the {nameof(GameObject)} will not be loaded.\nPlease remove the {nameof(GameObjectEntity)} component";

            if (convertToEntity && gameObject.GetComponent<GameObjectEntity>() != null)
                return
                    $"'{gameObject.name}' will be converted due to being in a {nameof(ConvertToEntity)} hierarchy. " +
                    $"{nameof(GameObjectEntity)} will have no effect.\nPlease remove the {nameof(GameObjectEntity)} component.";

            return null;
        }
    }
}
