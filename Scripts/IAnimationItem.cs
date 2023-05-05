using Godot;

namespace Game
{
	public partial interface IAnimationItem
	{
		public AnimationPoint Point { get; set; }

		public void SetPrefferedPosition(Vector2 prefferedPos);
	}
}
