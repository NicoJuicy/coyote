---
layout: reference
section: learn
title: OnSendEvent
permalink: /learn/ref/Microsoft.Coyote.Runtime/IActorRuntimeLog/OnSendEvent
---
# IActorRuntimeLog.OnSendEvent method

Invoked when the specified event is sent to a target actor.

```csharp
public void OnSendEvent(ActorId targetActorId, ActorId senderId, string senderStateName, Event e, 
    Guid opGroupId, bool isTargetHalted)
```

| parameter | description |
| --- | --- |
| targetActorId | The id of the target actor. |
| senderId | The id of the actor that sent the event, if any. |
| senderStateName | The state name, if the sender actor is a state machine and a state exists, else null. |
| e | The event being sent. |
| opGroupId | The id used to identify the send operation. |
| isTargetHalted | Is the target actor halted. |

## See Also

* class [ActorId](../../Microsoft.Coyote.Actors/ActorIdType)
* class [Event](../../Microsoft.Coyote/EventType)
* interface [IActorRuntimeLog](../IActorRuntimeLogType)
* namespace [Microsoft.Coyote.Runtime](../IActorRuntimeLogType)
* assembly [Microsoft.Coyote](../../MicrosoftCoyoteAssembly)

<!-- DO NOT EDIT: generated by xmldocmd for Microsoft.Coyote.dll -->