using Unity.Entities;

namespace Components
{
    public struct Anchor : IComponentData
    {
        public byte isAnchor;
    }

    // bar-visuals - no component yet, as we think the hybrid renderer components for the entity will contain a render transform

    // no PlayerInputs for now - will update TornadoParameters from inputs directly

    // not adding Position - will use built-in Translation component
}