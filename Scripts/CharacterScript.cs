using Godot;
using System;
using System.Linq;

public partial class CharacterScript : CharacterBody2D
{
	private float TWO_ROOT = Mathf.Sqrt(2);

	private float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	private float meter = ProjectSettings.GetSetting("game/metrics/meter_length").AsSingle();
    private float softJumpTime = ProjectSettings.GetSetting("game/mechanics/delayed_jump_time").AsSingle();

    private float parallaxScale = ProjectSettings.GetSetting("game/mechanics/parallax_scale").AsSingle();
    private float modulationScale = ProjectSettings.GetSetting("game/mechanics/modulation_scale").AsSingle();

    private float timeSinceLastOnFloor = 0;

    private int layer = 1;

    // This player's camera
    private Camera2D attachedCamera;

    // Terrain layers nodes
    private Node2D[] layerNodes;

    public override void _Ready()
    {
        attachedCamera = GetNode<Camera2D>(GetMeta("AttachedCamera").AsNodePath());

        Node2D layersOrigin = GetNode<Node2D>(GetMeta("LayersOrigin").AsNodePath());

        layerNodes = Array.ConvertAll(layersOrigin.GetChildren().ToArray(), item => (Node2D)item);

        //SetParent(layerNodes[layer]);
    }

    private void SetParent(Node2D newParent)
    {
        GetParent().RemoveChild(this);
        newParent.AddChild(this);
    }

    private void CalculateLayerPosition()
    {
        int deltaLayer = 0;

        deltaLayer += Input.IsActionJustPressed("game_layer_up") ? -1 : 0;
        deltaLayer += Input.IsActionJustPressed("game_layer_down") ? 1 : 0;

        deltaLayer = layer + deltaLayer > 2 || layer + deltaLayer < 0 ? 0 : deltaLayer;

        if (deltaLayer != 0) {
            CollisionMask = 1u << (layer + deltaLayer);
            SetParent(layerNodes[layer + deltaLayer]);

            layer = TestMove(GlobalTransform, Vector2.Zero) ? layer : layer + deltaLayer;

            CollisionMask = 1u << layer;
            SetParent(layerNodes[layer]);
        }
    }

    private void CalculateVelocity(float delta)
	{

        timeSinceLastOnFloor = IsOnFloor() ? 0 : Mathf.Clamp(timeSinceLastOnFloor + delta, 0, 1);

        float Speed /* meters per second */ = (float)GetMeta("Speed");
        float JumpHeight /* meters */ = (float)GetMeta("JumpHeight");
        float JumpSpeed /* meters per second */ = -Mathf.Sqrt(JumpHeight * gravity);

        Vector2 velocity /* meters per second */ = Velocity / meter;

        // Player's WASD/ULDR input
        Vector2 direction = Input.GetVector("game_left", "game_right", "game_up", "game_down");

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
        if (Input.IsActionJustPressed("game_jump") && timeSinceLastOnFloor <= softJumpTime)
        {
            velocity.Y = JumpSpeed;
            timeSinceLastOnFloor = 1;
        }

        Velocity /* pixels per second */ = velocity * meter;
    }

    private void CalculateCameraPosition()
    {
        Vector2 mousePos = GetViewport().GetMousePosition() / attachedCamera.GetViewportRect().Size * 2.0f - new Vector2(1.0f, 1.0f);

        attachedCamera.Position = mousePos * meter * 3;
    }

    private void CalculateParallax()
    {
        foreach(var node in layerNodes)
        {
            node.Scale = Vector2.One;
            node.Position = Vector2.Zero;
        }
        
        for(int i = 0; i < layerNodes.Length; i++)
        {
            int z = i - layer;

            GD.Print(modulationScale);
            layerNodes[i].Modulate = Color.FromHsv(0.0f, 0.0f, 1.0f - modulationScale * Math.Abs(z), 1.0f - modulationScale * Math.Clamp(z, 0, int.MaxValue));

            for(int j = 0; j < Math.Abs(z); j++)
            {
                layerNodes[i].Scale += layerNodes[i].Scale * parallaxScale * Math.Sign(z);
            }

            layerNodes[i].Position -= attachedCamera.Position * parallaxScale * z + Position * (layerNodes[i].Scale - Vector2.One);
        }


    }


    public override void _PhysicsProcess(double delta)
    {
        CalculateLayerPosition();

        CalculateVelocity((float)delta);

        CalculateCameraPosition();

		MoveAndSlide();

        CalculateParallax();
	}
}