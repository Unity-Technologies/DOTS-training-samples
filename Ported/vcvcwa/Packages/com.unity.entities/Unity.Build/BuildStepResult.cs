using System;
using Unity.Properties;
using UnityEditor;

namespace Unity.Build
{
    /// <summary>
    /// Holds the result of the execution of a <see cref="IBuildStep"/>.
    /// </summary>
    public class BuildStepResult
    {
        /// <summary>
        /// Determine if the execution of the <see cref="IBuildStep"/> succeeded.
        /// </summary>
        public bool Succeeded { get; internal set; }

        /// <summary>
        /// Determine if the execution of the <see cref="IBuildStep"/> failed.
        /// </summary>
        public bool Failed { get => !Succeeded; }

        /// <summary>
        /// The message resulting from the execution of this <see cref="IBuildStep"/>.
        /// </summary>
        public string Message { get; internal set; }

        /// <summary>
        /// Duration of the execution of this <see cref="IBuildStep"/>.
        /// </summary>
        public TimeSpan Duration { get; internal set; }

        /// <summary>
        /// The <see cref="IBuildStep"/> that was executed.
        /// </summary>
        public IBuildStep BuildStep { get; internal set; }

        /// <summary>
        /// Implicit conversion to <see cref="bool"/>.
        /// </summary>
        /// <param name="result">Instance of <see cref="BuildStepResult"/>.</param>
        public static implicit operator bool(BuildStepResult result) => result.Succeeded;

        static BuildStepResult()
        {
            PropertyBagResolver.Register(new BuildStepResultPropertyBag());
        }

        internal BuildStepResult() { }

        internal static BuildStepResult Success(IBuildStep step) => new BuildStepResult
        {
            BuildStep = step,
            Succeeded = true
        };

        internal static BuildStepResult Failure(IBuildStep step, string message) => new BuildStepResult
        {
            BuildStep = step,
            Succeeded = false,
            Message = message
        };

        internal static BuildStepResult Exception(IBuildStep step, Exception exception) => new BuildStepResult
        {
            BuildStep = step,
            Succeeded = false,
            Message = exception.Message
        };

        class BuildStepResultPropertyBag : PropertyBag<BuildStepResult>
        {
            static readonly Property<BuildStepResult, bool> s_Succeeded = new Property<BuildStepResult, bool>(
                nameof(Succeeded),
                (ref BuildStepResult result) => result.Succeeded,
                (ref BuildStepResult result, bool value) => result.Succeeded = value);

            static readonly Property<BuildStepResult, string> s_Message = new Property<BuildStepResult, string>(
                nameof(Message),
                (ref BuildStepResult result) => result.Message,
                (ref BuildStepResult result, string value) => result.Message = value);

            static readonly Property<BuildStepResult, TimeSpan> s_Duration = new Property<BuildStepResult, TimeSpan>(
                nameof(Duration),
                (ref BuildStepResult result) => result.Duration,
                (ref BuildStepResult result, TimeSpan value) => result.Duration = value);

            static readonly Property<BuildStepResult, string> s_BuildStep = new Property<BuildStepResult, string>(
                nameof(BuildStep),
                (ref BuildStepResult result) =>
                {
                    if (result.BuildStep is BuildPipeline buildPipeline)
                    {
                        return GlobalObjectId.GetGlobalObjectIdSlow(buildPipeline).ToString();
                    }
                    else
                    {
                        var type = result.BuildStep.GetType();
                        return $"{type}, {type.Assembly.GetName().Name}";
                    }
                },
                (ref BuildStepResult result, string value) =>
                {
                    if (GlobalObjectId.TryParse(value, out var id))
                    {
                        if (GlobalObjectId.GlobalObjectIdentifierToObjectSlow(id) is BuildPipeline buildPipeline)
                        {
                            result.BuildStep = buildPipeline;
                        }
                    }
                    else
                    {
                        var type = Type.GetType(value);
                        if (TypeConstruction.TryConstruct<IBuildStep>(type, out var buildStep))
                        {
                            result.BuildStep = buildStep;
                        }
                    }
                });

            static readonly Property<BuildStepResult, string> s_Description = new Property<BuildStepResult, string>(
                nameof(IBuildStep.Description),
                (ref BuildStepResult result) => result.BuildStep.Description,
                (ref BuildStepResult result, string value) => { /* discard value */ });

            public override void Accept<TVisitor>(ref BuildStepResult container, ref TVisitor visitor, ref ChangeTracker changeTracker)
            {
                visitor.VisitProperty<Property<BuildStepResult, bool>, BuildStepResult, bool>(s_Succeeded, ref container, ref changeTracker);
                visitor.VisitProperty<Property<BuildStepResult, string>, BuildStepResult, string>(s_Message, ref container, ref changeTracker);
                visitor.VisitProperty<Property<BuildStepResult, TimeSpan>, BuildStepResult, TimeSpan>(s_Duration, ref container, ref changeTracker);
                visitor.VisitProperty<Property<BuildStepResult, string>, BuildStepResult, string>(s_BuildStep, ref container, ref changeTracker);
                visitor.VisitProperty<Property<BuildStepResult, string>, BuildStepResult, string>(s_Description, ref container, ref changeTracker);
            }

            public override bool FindProperty<TAction>(string name, ref BuildStepResult container, ref ChangeTracker changeTracker, ref TAction action)
            {
                if (name == nameof(Succeeded))
                {
                    action.VisitProperty<Property<BuildStepResult, bool>, bool>(s_Succeeded, ref container, ref changeTracker);
                    return true;
                }
                else if (name == nameof(Message))
                {
                    action.VisitProperty<Property<BuildStepResult, string>, string>(s_Message, ref container, ref changeTracker);
                    return true;
                }
                else if (name == nameof(Duration))
                {
                    action.VisitProperty<Property<BuildStepResult, TimeSpan>, TimeSpan>(s_Duration, ref container, ref changeTracker);
                    return true;
                }
                else if (name == nameof(BuildStep))
                {
                    action.VisitProperty<Property<BuildStepResult, string>, string>(s_BuildStep, ref container, ref changeTracker);
                    return true;
                }
                else if (name == nameof(IBuildStep.Description))
                {
                    action.VisitProperty<Property<BuildStepResult, string>, string>(s_Description, ref container, ref changeTracker);
                    return true;
                }
                return false;
            }
        }
    }
}
