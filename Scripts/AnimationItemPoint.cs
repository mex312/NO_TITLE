using Godot;
using Godot.Collections;
using System;
using System.Linq;

namespace Game
{
	public partial class AnimationItemPoint : Node2D
	{
		[ExportGroup("Tolerance")]
		[Export]
		public Vector2 toleranceBorder = Vector2.Zero;
		[Export]
		public Vector2 toleranceOffset = Vector2.Zero;
		[ExportGroup("")]

		[Export]
		private Array<NodePath> pinnedItemsPaths;
		public Array<AnimationItem> pinnedItems;

		public override void _Ready()
		{
			pinnedItems = new Array<AnimationItem>(System.Array.ConvertAll(pinnedItemsPaths.ToArray(), item => GetNode<AnimationItem>(item)));
		}

		public override void _Process(double delta)
		{
			foreach (var item in pinnedItems)
			{
				if (item != null)
				{
					Vector2 deltaPos = (toleranceOffset + GlobalPosition - item.GlobalPosition).Rotated(GlobalRotation).Abs();

					if (deltaPos.X > toleranceBorder.X || deltaPos.Y > toleranceBorder.Y) item.SetPrefferedPosition(GlobalPosition);
				}
			}
		}
	}
}
