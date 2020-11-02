using System.Collections.Generic;
using Unity.Properties.UI;
using UnityEngine;

namespace Unity.Entities.Editor
{
    /// <summary>
    /// Wrapper type to allow to inspect a <see cref="DynamicBuffer{TElement}"/>.
    /// </summary>
    /// <typeparam name="TList">Type that maps to <see cref="DynamicBufferContainer{TElement}"/>.</typeparam>
    /// <typeparam name="TElement">The <see cref="IBufferElementData"/> type.</typeparam>
    struct InspectedBuffer<TList, TElement>
        where TList : IList<TElement>
    {
        [InspectorName("Buffer"), InspectorOptions(HideResetToDefault = true), Pagination]
        public TList Value;
    }
}
