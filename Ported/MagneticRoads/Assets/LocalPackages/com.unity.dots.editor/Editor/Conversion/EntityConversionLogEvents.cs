using System;
using System.Linq;
using Unity.Editor.Controls;
using Unity.Entities.Conversion;
using UnityEditor;
using UnityEngine;

namespace Unity.Entities.Editor
{
    [InitializeOnLoad]
    static class EntityConversionLogEvents
    {
        const string k_SessionStateKeyPage = "EntityConversionLogEvents.Page.{0}";

        const int k_GameObjectHeaderLogEventItemsPerPage = 1;

        static EntityConversionLogEvents()
        {
            UnityEditor.Editor.finishedDefaultHeaderGUI += EditorOnFinishedDefaultHeaderGui;
        }

        static bool IsError(LogEventData log)
            => log.Type == LogType.Exception || log.Type == LogType.Error || log.Type == LogType.Assert;

        static void EditorOnFinishedDefaultHeaderGui(UnityEditor.Editor editor)
        {
            var gameObject = editor.target as GameObject;

            if (!gameObject || gameObject == null || !GameObjectConversionEditorUtility.IsConverted(gameObject))
                return;

            var logs = EntityConversionUtility.GetConvertedComponentsInfo(gameObject, EntityConversionPreview.GetCurrentlySelectedWorld()).LogEvents;

            if (null == logs || logs.Count == 0)
                return;

            var errors = logs.Where(IsError).ToList();

            var pagination = new PaginationField
            {
                ItemsPerPage = k_GameObjectHeaderLogEventItemsPerPage,
                Count = errors.Count,
                Page = SessionState.GetInt(string.Format(k_SessionStateKeyPage, gameObject.GetInstanceID()), 0)
            };

            var startIndex = pagination.Page * pagination.ItemsPerPage;
            var endIndex = Math.Min(errors.Count, (pagination.Page + 1) * pagination.ItemsPerPage);

            for (var i = startIndex; i < endIndex; i++)
            {
                EditorGUILayout.HelpBox(errors[i].Message, MessageType.Error, true);
            }

            if (pagination.Count > pagination.ItemsPerPage)
            {
                pagination.OnGUI(GUILayoutUtility.GetRect(0, 20));
                SessionState.SetInt(string.Format(k_SessionStateKeyPage, gameObject.GetInstanceID()), pagination.Page);
            }
        }
    }
}
