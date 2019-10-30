using System;
using NUnit.Framework;
using UnityEditor.Build.Pipeline.Interfaces;
using UnityEditor.Build.Pipeline.Injector;

namespace UnityEditor.Build.Pipeline.Tests
{
    [TestFixture]
    class ContextInjectionTests
    {
        interface IInjectionContext : IContextObject
        {
            int State { get; set; }
        }

        class InjectionClass : IInjectionContext
        {
            public int State { get; set; }
        }

        struct InjectionStruct : IInjectionContext
        {
            public int State { get; set; }
        }

        struct TaskStruct : IBuildTask
        {
            public int Version { get { return 1; } }
            public int NewState { get; private set; }
            
#pragma warning disable 649
            [InjectContext]
            internal IInjectionContext InjectedObject;
#pragma warning restore 649

            public TaskStruct(int newState)
                : this()
            {
                NewState = newState;
            }

            public ReturnCode Run()
            {
                InjectedObject.State = NewState;
                return ReturnCode.Success;
            }
        }

        class TaskClass : IBuildTask
        {
            public int Version { get { return 1; } }
            public int NewState { get; private set; }
            
#pragma warning disable 649
            [InjectContext]
            internal IInjectionContext InjectedObject;
#pragma warning restore 649

            public TaskClass(int newState)
            {
                NewState = newState;
            }

            public ReturnCode Run()
            {
                InjectedObject.State = NewState;
                return ReturnCode.Success;
            }
        }

        class TaskContext : IBuildTask
        {
            public int Version { get { return 1; } }
            
#pragma warning disable 649
            [InjectContext]
            internal IBuildContext InjectedContext;
#pragma warning restore 649

            public ReturnCode Run()
            {
                return ReturnCode.Success;
            }
        }


        [Test]
        public void CanInjectAndExtractWithStructs()
        {
            IInjectionContext injection = new InjectionStruct();
            injection.State = 1;

            IBuildContext context = new BuildContext();
            context.SetContextObject(injection);

            TaskStruct task = new TaskStruct(2);
            Assert.IsNull(task.InjectedObject);

            // Still need to box / unbox the struct task
            IBuildTask boxed = task;
            ContextInjector.Inject(context, boxed);
            task = (TaskStruct)boxed;

            Assert.IsNotNull(task.InjectedObject);
            Assert.AreEqual(1, task.InjectedObject.State);

            ReturnCode result = task.Run();
            Assert.AreEqual(ReturnCode.Success, result);
            
            ContextInjector.Extract(context, task);

            IInjectionContext modifiedInjection = context.GetContextObject<IInjectionContext>();
            Assert.AreEqual(task.NewState, modifiedInjection.State);
        }


        [Test]
        public void CanInjectAndExtractWithClasses()
        {
            IInjectionContext injection = new InjectionClass();
            injection.State = 1;

            IBuildContext context = new BuildContext();
            context.SetContextObject(injection);

            TaskClass task = new TaskClass(2);
            Assert.IsNull(task.InjectedObject);

            ContextInjector.Inject(context, task);

            Assert.IsNotNull(task.InjectedObject);
            Assert.AreEqual(1, task.InjectedObject.State);

            ReturnCode result = task.Run();
            Assert.AreEqual(ReturnCode.Success, result);
            
            ContextInjector.Extract(context, task);

            IInjectionContext modifiedInjection = context.GetContextObject<IInjectionContext>();
            Assert.AreEqual(task.NewState, modifiedInjection.State);
        }

        [Test]
        public void CanInjectIBuildContextAsInOnly()
        {
            IBuildContext context = new BuildContext();

            TaskContext task = new TaskContext();
            Assert.IsNull(task.InjectedContext);

            ContextInjector.Inject(context, task);

            Assert.IsNotNull(task.InjectedContext);
            Assert.AreEqual(context, task.InjectedContext);

            Assert.Throws<InvalidOperationException>(() =>
            {
                ContextInjector.Extract(context, task);
            });
        }
    }
}
