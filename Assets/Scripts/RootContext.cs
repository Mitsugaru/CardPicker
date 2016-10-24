using UnityEngine;
using System.Collections;
using strange.extensions.context.impl;
using strange.extensions.context.api;

public class RootContext : MVCSContext, IRootContext
{

    public RootContext(MonoBehaviour view) : base(view)
    {

    }

    public RootContext(MonoBehaviour view, ContextStartupFlags flags) : base(view, flags)
    {

    }

    protected override void mapBindings()
    {
        base.mapBindings();

        GameObject managers = GameObject.Find("Managers");

        injectionBinder.Bind<IRootContext>().ToValue(this).ToSingleton().CrossContext();

        EventManager eventManager = managers.GetComponent<EventManager>();
        injectionBinder.Bind<IEventManager>().ToValue(eventManager).ToSingleton().CrossContext();

        //The following are dependent on the Event Manager
        CardManager cardManager = managers.GetComponent<CardManager>();
        injectionBinder.Bind<ICardManager>().ToValue(cardManager).ToSingleton().CrossContext();

        InputManager inputManager = managers.GetComponent<InputManager>();
        injectionBinder.Bind<IInputManager>().ToValue(inputManager).ToSingleton().CrossContext();
    }

    public void Inject(Object o)
    {
        injectionBinder.injector.Inject(o);
    }
}
