using System.IO;
using Unity.Properties;

namespace Unity.Serialization.Json
{
    class JsonVisitorAdapterSystemIO : JsonVisitorAdapter,
        IVisitAdapter<DirectoryInfo>,
        IVisitAdapter<FileInfo>
    {
        public JsonVisitorAdapterSystemIO(JsonVisitor visitor) : base(visitor) { }

        public static void RegisterTypes()
        {
            TypeConversion.Register<SerializedStringView, DirectoryInfo>(view => new DirectoryInfo(view.ToString()));
            TypeConstruction.SetExplicitConstructionMethod(() => { return new DirectoryInfo("."); });

            TypeConversion.Register<SerializedStringView, FileInfo>(view => new FileInfo(view.ToString()));
            TypeConstruction.SetExplicitConstructionMethod(() => { return new FileInfo("."); });
        }

        public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref DirectoryInfo value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, DirectoryInfo>
        {
            AppendJsonString(property, value.GetRelativePath());
            return VisitStatus.Handled;
        }

        public VisitStatus Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref FileInfo value, ref ChangeTracker changeTracker)
            where TProperty : IProperty<TContainer, FileInfo>
        {
            AppendJsonString(property, value.GetRelativePath());
            return VisitStatus.Handled;
        }
    }

    static class DirectoryInfoExtensions
    {
        internal static string GetRelativePath(this DirectoryInfo directoryInfo)
        {
            var relativePath = new DirectoryInfo(".").FullName.ToForwardSlash();
            var path = directoryInfo.FullName.ToForwardSlash();
            return path.StartsWith(relativePath) ? path.Substring(relativePath.Length).TrimStart('/') : path;
        }
    }

    static class FileInfoExtensions
    {
        internal static string GetRelativePath(this FileInfo fileInfo)
        {
            var relativePath = new DirectoryInfo(".").FullName.ToForwardSlash();
            var path = fileInfo.FullName.ToForwardSlash();
            return path.StartsWith(relativePath) ? path.Substring(relativePath.Length).TrimStart('/') : path;
        }
    }
}
