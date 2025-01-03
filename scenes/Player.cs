using Godot;
using System;

public partial class Player : CharacterBody3D
{
    private const float Speed = 5.0f;
    private const float Gravity = 30.0f;
    private const float JumpVelocity = 4.5f;
    private const float Acceleration = 0.5f;
    private const float Sensitivity = 0.01f;

    [Export] public Node3D Head;
    [Export] public Camera3D Camera;

    private Vector3 _velocity = Vector3.Zero;

    public override void _Ready()
    {
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseMotion)
        {
            Head.RotateY(-mouseMotion.Relative.X * Sensitivity);
            Camera.RotateX(-mouseMotion.Relative.Y * Sensitivity);

            var clampedCameraRotation = Camera.Rotation;
            clampedCameraRotation.X = Mathf.Clamp(clampedCameraRotation.X, Mathf.DegToRad(-80f), Mathf.DegToRad(80f));
            Camera.Rotation = clampedCameraRotation;

            Input.MouseMode = Input.MouseModeEnum.Captured;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        var inputDirection = Input.GetVector("WalkLeft", "WalkRight", "WalkForward", "WalkBackward");
        var direction = (Head.GlobalTransform.Basis * new Vector3(inputDirection.X, 0f, inputDirection.Y)).Normalized();

        if (direction != Vector3.Zero)
        {
            _velocity.X = direction.X * Speed;
            _velocity.Z = direction.Z * Speed;
        }
        else
        {
            _velocity.X = Mathf.MoveToward(_velocity.X, 0, Acceleration);
            _velocity.Z = Mathf.MoveToward(_velocity.Z, 0, Acceleration);
        }

        if (IsOnFloor() && Input.IsActionJustPressed("Jump"))
        {
            _velocity.Y = JumpVelocity;
        }
        else if (!IsOnFloor())
        {
            _velocity.Y -= Gravity * (float)delta;
        }

        Velocity = _velocity;
        MoveAndSlide();
    }
}