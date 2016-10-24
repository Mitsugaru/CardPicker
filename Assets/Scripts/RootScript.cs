using strange.extensions.context.impl;

public class RootScript : ContextView
{

    // Use this for initialization
    void Awake()
    {
        context = new RootContext(this);
    }

}
