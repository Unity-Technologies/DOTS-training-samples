using System.Collections.Generic;
using Unity.Scenes.Editor;
using UnityEngine;

namespace Unity.Entities.Editor
{
    enum GameObjectConversionResultStatus
    {
        /// <summary>
        /// This <see cref="GameObject"/> will not be converted.
        /// </summary>
        NotConverted,

        /// <summary>
        /// This <see cref="GameObject"/> will be converted as part of a sub-scene.
        /// </summary>
        ConvertedBySubScene,
        /// <summary>
        /// This <see cref="GameObject"/> will be converted as a result of having the <see cref="ConvertToEntity"/> directly on it.
        /// </summary>
        ConvertedByConvertToEntity,

        /// <summary>
        /// This <see cref="GameObject"/> will be converted as a result of having <see cref="ConvertToEntity"/> on ancestors using <see cref="ConvertToEntity.Mode.ConvertAndDestroy"/> mode.
        /// </summary>
        ConvertedByAncestor,

        /// <summary>
        /// This <see cref="GameObject"/> will not be converted as a result of the <see cref="StopConvertToEntity"/> existing in it's hierarchy.
        /// </summary>
        NotConvertedByStopConvertToEntityComponent,

        /// <summary>
        /// This <see cref="GameObject"/> will not be converted as a result of the <see cref="ConvertToEntity"/> using <see cref="ConvertToEntity.Mode.ConvertAndInjectGameObject"/> mode.
        /// </summary>
        /// <remarks>
        /// NOTE: This mode will be DEPRECATED soon.
        /// </remarks>
        NotConvertedByConvertAndInjectMode
    }

    static class GameObjectConversionEditorUtility
    {
        static readonly List<ConvertToEntity> s_ConvertToEntity = new List<ConvertToEntity>(8);
        static readonly List<StopConvertToEntity> s_StopConvertToEntity = new List<StopConvertToEntity>(8);

        /// <summary>
        /// Returns an enum detailing if the given <see cref="GameObject"/> will be converted and how.
        /// </summary>
        /// <param name="gameObject">The <see cref="GameObject"/> to be converted.</param>
        /// <returns>A <see cref="GameObjectConversionResultStatus"/> code detailing how the <see cref="GameObject"/> will be converted.</returns>
        public static GameObjectConversionResultStatus GetGameObjectConversionResultStatus(GameObject gameObject)
        {
            if (null == gameObject || !gameObject)
            {
                return GameObjectConversionResultStatus.NotConverted;
            }

            if (gameObject.scene.isSubScene)
            {

                // Any gameObject in a sub-scene will ignore special conversion components and always result in a converted entity.
                return GameObjectConversionResultStatus.ConvertedBySubScene;
            }

            s_StopConvertToEntity.Clear();
            gameObject.GetComponentsInParent(true, s_StopConvertToEntity);
            if (s_StopConvertToEntity.Count > 0)
            {
                // This gameObject or an ancestor has the StopConvertToEntity component.
                // This means all of its children (including itself) are not converted.
                return GameObjectConversionResultStatus.NotConvertedByStopConvertToEntityComponent;
            }

            s_ConvertToEntity.Clear();
            gameObject.GetComponentsInParent(true, s_ConvertToEntity);
            foreach (var convertToEntity in s_ConvertToEntity)
            {
                if (convertToEntity.gameObject != gameObject && convertToEntity.ConversionMode == ConvertToEntity.Mode.ConvertAndInjectGameObject)
                {
                    // An ancestor is being converted and injected.
                    // This means all of its children are not converted.
                    return GameObjectConversionResultStatus.NotConvertedByConvertAndInjectMode;
                }
            }

            foreach (var convertToEntity in s_ConvertToEntity)
            {
                if (convertToEntity.gameObject != gameObject && convertToEntity.ConversionMode == ConvertToEntity.Mode.ConvertAndDestroy)
                {
                    // An ancestor is being converted and destroyed.
                    // This means all of its children are converted.
                    return GameObjectConversionResultStatus.ConvertedByAncestor;
                }
            }

            var convertToEntityOnCurrentGO = gameObject.GetComponent<ConvertToEntity>();
            return (convertToEntityOnCurrentGO && convertToEntityOnCurrentGO.enabled)
                ? GameObjectConversionResultStatus.ConvertedByConvertToEntity
                : GameObjectConversionResultStatus.NotConverted;
        }

        public static bool IsConverted(this GameObjectConversionResultStatus status) =>
            status == GameObjectConversionResultStatus.ConvertedBySubScene ||
            status == GameObjectConversionResultStatus.ConvertedByAncestor ||
            status == GameObjectConversionResultStatus.ConvertedByConvertToEntity;

        public static bool IsConverted(GameObject gameObject) =>
            GetGameObjectConversionResultStatus(gameObject).IsConverted();
    }
}
