using Godot;
using System;

namespace Game
{
    public interface IAnimationManager
    {
        uint GetBehavior();
        void SetBehavior(uint state);

        void Update();
    }
}