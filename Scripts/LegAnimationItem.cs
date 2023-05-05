using Godot;
using System;

namespace Game
{
	public partial class LegAnimationItem : RigidBody2D, IAnimationItem
	{
        [Export]
        public AnimationPoint Point { get => _point; set => _point = value; }

        private AnimationPoint _point;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
		{
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
            if (!_point.IsInside(this)) SetPrefferedPosition(_point.GlobalPosition + _point.GetOffset());
		}

        public void SetPrefferedPosition(Vector2 prefferedPos)
        {
            GlobalPosition = prefferedPos;
        }
    }
}