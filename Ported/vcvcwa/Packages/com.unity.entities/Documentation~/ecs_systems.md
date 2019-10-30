---
uid: ecs-systems
---
# Systems

A **System**, the *S* in ECS,  provides the logic that transforms the component data from its current state to its next state — for example, a system might update the positions of all moving entities by their velocity times the time interval since the previous update.

Unity ECS provides a number of different kinds of systems. The main systems that you can implement to transform your entity data are the [ComponentSystem](xref:ecs-component-system) and the [JobComponentSystem](xref:ecs-job-component-system). Both these system types facilitate selecting and iterating over a set of entities based on their associated components.

Systems provide event-style callback functions, such as OnCreate() and OnUpdate() that you can implement to run code at the correct time in a system's life cycle. These functions are invoked on the main thread. In a Job Component System, you typically schedule Jobs in the OnUpdate() function. The Jobs themselves run on worker threads. In general, Job Component Systems provide the best performance since they take advantage of multiple CPU cores. Performance can be improved even more when your Jobs are compiled by the Burst compiler.

Unity ECS automatically discovers system classes in your project and instantiates them at runtime. Systems are organized within a [World](xref:Unity.Entities.World) by group. You can control which group a system is added to and the order of that system within the group using [system attributes](system_update_order.md#attributes). By default, all systems are added to the Simulation System Group of the default world in a deterministic, but unspecified, order. You can disable automatic creation using a system attribute.

A system's update loop is driven by its parent [Component System Group](xref:ecs-system-update-order). A Component System Group is, itself, a specialized kind of system that is responsible for updating its child systems. 

You can view the runtime system configuration using the Entity Debugger window (menu: **Window** > **Analysis** > **Entity Debugger**). 

<a name="events"></a>
## System event functions

You can implement a set of system lifecycle event functions when you implement a system. Unity ECS invokes these functions in the following order:

* [OnCreate()](xref:ComponentSystemBase.OnCreate*) -- called when the system is created.
* [OnStartRunning()](xref:ComponentSystemBase.OnStartRunning*) -- before the first OnUpdate and whenever the system resumes running.
* `OnUpdate()` -- every frame as long as the system has work to do (see ShouldRunSystem()) and the system is Enabled. Note that the OnUpdate function is defined in the subclasses of ComponentSystemBase; each type of system class can define its own update behavior.
* [OnStopRunning()](xref:ComponentSystemBase.OnStopRunning*) -- whenever the system stops updating because it finds no entities matching its queries. Also called before OnDestroy.
* [OnDestroy()](xref:ComponentSystemBase.OnDestroy*) -- when the system is destroyed.

All of these functions are executed on the main thread. Note that you can schedule Jobs from the OnUpdate(JobHandle) function of a JobComponentSystem to perform work on background threads.

<a name="types"></a>
## System types

Unity ECS provides several types of systems. In general, the systems you write to implement your game behaviour and data transformation will extend either [ComponentSystem](xref:ecs-component-system) or [JobComponentSystem](xref:ecs-job-component-system). The other system classes have specialized purposes; you typically use existing instances of the Entity Command Buffer System and Component System Group classes.

* [Component Systems](xref:ecs-component-system) -- Implement a [ComponentSystem](xref:Unity.Entities.ComponentSystem) subclass for systems that perform their work on the main thread or that use Jobs not specifically optimized for ECS.
* [Job Component Systems](xref:ecs-job-component-system) -- Implement a [JobComponentSystem](xref:Unity.Entities.JobComponentSystem)  subclass for systems that perform their work using [IJobForEach](xref:Unity.Entities.IJobForEach`1) or [IJobChunk](xref:Unity.Entities.IJobChunk).
* [Entity Command Buffer Systems](xref:ecs-entity-command-buffer) -- provides [EntityCommandBuffer](xref:Unity.Entities.EntityCommandBuffer) instances for other systems. Each of the default system groups maintains an Entity Command Buffer System at the beginning and end of its list of child systems.
* [Component System Groups](xref:ecs-system-update-order) -- provides nested organization and update order for other systems. Unity ECS creates several Component System Groups by default.
