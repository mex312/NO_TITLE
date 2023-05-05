using Godot;

namespace Game
{
	public partial class AnimationPoint : Node2D
	{
		[Export]
		protected Vector2 tolerance;

		[Export]
		protected Vector2 offset;

		public Vector2 GetOffset() { return offset; }

		public bool IsInside(Node2D node)
        {
            Vector2 deltaPos = (GlobalPosition - node.GlobalPosition).Rotated(GlobalRotation).Abs();
            return !(deltaPos.X > tolerance.X || deltaPos.Y > tolerance.Y);
        }
	}
}
