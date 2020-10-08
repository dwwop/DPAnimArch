//Data structure for single animation

public struct Anim 
{
    public string Code { set; get; }
    public string AnimationName { set; get; }
    public Anim (string animation_name, string code)
    {
        Code = code;
        AnimationName = animation_name;
    }
    public Anim(string animation_name)
    {
        AnimationName = animation_name;
        Code = null;
    }

}
