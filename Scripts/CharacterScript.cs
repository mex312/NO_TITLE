using Godot;
using System;

public partial class CharacterScript : CharacterBody2D
{
	public float TWO_ROOT = Mathf.Sqrt(2);

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	public float meter = ProjectSettings.GetSetting("game/metrics/meter_length").AsSingle();
    public float softJumpTime = ProjectSettings.GetSetting("game/mechanics/delayed_jump_time").AsSingle();

    private float timeSinceLastOnFloor = 0;

    private int layer = 0;

	private void CalculateVelocity(float delta)
	{

        timeSinceLastOnFloor = IsOnFloor() ? 0 : Mathf.Clamp(timeSinceLastOnFloor + delta, 0, 1);

        float Speed /* meters per second */ = (float)GetMeta("Speed");
        float JumpHeight /* meters */ = (float)GetMeta("JumpHeight");
        float JumpSpeed /* meters per second */ = -Mathf.Sqrt(JumpHeight * gravity);

        Vector2 velocity /* meters per second */ = Velocity / meter;

        // Player's WASD/ULDR input
        Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");

        // Applying horizontal motion
        if (direction != Vector2.Zero)
        {
            velocity.X = direction.X * Speed;
        }
        else
        {
            velocity.X = Mathf.MoveToward(velocity.X, 0, Speed);
        }

        // Applying gravity
        velocity.Y += gravity * delta;

        // Handle Jump.
        if (Input.IsActionJustPressed("ui_accept") && timeSinceLastOnFloor <= softJumpTime)
        {
            velocity.Y = JumpSpeed;
            timeSinceLastOnFloor = 1;
        }

        Velocity /* pixels per second */ = velocity * meter;
    }

    private void CalculateCameraPos()
    {
        Camera2D camera = GetNode<Camera2D>(GetMeta("AttachedCamera").AsNodePath());

        Vector2 mousePos = GetViewport().GetMousePosition() / camera.GetViewportRect().Size * 2.0f - new Vector2(1.0f, 1.0f);

        camera.Position = mousePos * meter * 3;
    }


    public override void _PhysicsProcess(double delta)
	{
        CalculateVelocity((float)delta);

        CalculateCameraPos();

        if (Input.IsActionJustPressed("game_layer_up")) layer--;
        if (Input.IsActionJustPressed("game_layer_down")) layer++;

        layer = Math.Clamp(layer, 0, 2);

        CollisionMask = 1u << layer;

		MoveAndSlide();
	}
}