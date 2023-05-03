using Godot;
using System;

namespace Game
{
	public partial class AnimationItem : Node2D
	{
		
		public void SetPrefferedPosition(Vector2 prefferedPos)
		{
			GlobalPosition = prefferedPos;
		}

		public override void _Ready()
		{
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
		}
	}
}
