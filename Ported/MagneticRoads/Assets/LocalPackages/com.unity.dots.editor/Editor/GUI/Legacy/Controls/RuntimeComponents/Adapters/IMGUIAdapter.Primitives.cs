using Unity.Properties;
using Unity.Properties.Adapters;

namespace Unity.Editor.Legacy
{
    sealed partial class RuntimeComponentsDrawer : IVisitPrimitives, IVisit<string>
    {
        public VisitStatus Visit<TContainer>(Property<TContainer, sbyte> property, ref TContainer container, ref sbyte value)
        {
            PropertyField(property, value);
            return VisitStatus.Stop;
        }

        public VisitStatus Visit<TContainer>(Property<TContainer, short> property, ref TContainer container, ref short value)
        {
            PropertyField(property, value);
            return VisitStatus.Stop;
        }

        public VisitStatus Visit<TContainer>(Property<TContainer, int> property, ref TContainer container, ref int value)
        {
            PropertyField(property, value);
            return VisitStatus.Stop;
        }

        public VisitStatus Visit<TContainer>(Property<TContainer, long> property, ref TContainer container, ref long value)
        {
            PropertyField(property, value);
            return VisitStatus.Stop;
        }

        public VisitStatus Visit<TContainer>(Property<TContainer, byte> property, ref TContainer container, ref byte value)
        {
            PropertyField(property, value);
            return VisitStatus.Stop;
        }

        public VisitStatus Visit<TContainer>(Property<TContainer, ushort> property, ref TContainer container, ref ushort value)
        {
            PropertyField(property, value);
            return VisitStatus.Stop;
        }

        public VisitStatus Visit<TContainer>(Property<TContainer, uint> property, ref TContainer container, ref uint value)
        {
            PropertyField(property, value);
            return VisitStatus.Stop;
        }

        public VisitStatus Visit<TContainer>(Property<TContainer, ulong> property, ref TContainer container, ref ulong value)
        {
            PropertyField(property, value);
            return VisitStatus.Stop;
        }

        public VisitStatus Visit<TContainer>(Property<TContainer, float> property, ref TContainer container, ref float value)
        {
            PropertyField(property, value);
            return VisitStatus.Stop;
        }

        public VisitStatus Visit<TContainer>(Property<TContainer, double> property, ref TContainer container, ref double value)
        {
            PropertyField(property, value);
            return VisitStatus.Stop;
        }

        public VisitStatus Visit<TContainer>(Property<TContainer, bool> property, ref TContainer container, ref bool value)
        {
            PropertyField(property, value);
            return VisitStatus.Stop;
        }

        public VisitStatus Visit<TContainer>(Property<TContainer, char> property, ref TContainer container, ref char value)
        {
            PropertyField(property, value);
            return VisitStatus.Stop;
        }

        public VisitStatus Visit<TContainer>(Property<TContainer, string> property, ref TContainer container, ref string value)
        {
            PropertyField(property, value);
            return VisitStatus.Stop;
        }
    }
}
