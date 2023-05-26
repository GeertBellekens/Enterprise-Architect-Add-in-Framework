using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using TSF.UmlToolingFramework.Wrappers.EA;
using UML = TSF.UmlToolingFramework.UML;
using UML_SM = TSF.UmlToolingFramework.UML.StateMachines.BehaviorStateMachines;
using UTF_EA = TSF.UmlToolingFramework.Wrappers.EA;

namespace TSF.UmlToolingFramework.Wrappers.EA.BehaviorStateMachines
{
    /// <summary>
    /// Description of Event.
    /// </summary>
    public class Event : UTF_EA.ElementWrapper, UML.CommonBehaviors.Communications.Event
    {
        public Event(UTF_EA.Model model, EADBElementWrapper wrappedElement)
        : base(model, wrappedElement)
        {
        }
    }
}
