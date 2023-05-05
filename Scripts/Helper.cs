using Godot;

namespace Game
{
    public class Helper
    {
        public static void SetParent(Node newParent, Node child)
        {
            child.GetParent().RemoveChild(child);
            newParent.AddChild(child);
        }
    }
}
