using System;
using UnityEngine;

namespace Game.Scripts.Input
{
    public interface IInputService
    {
        IObservable<Ray> OnTap { get; }
        IObservable<Ray> OnDrag { get; }
        IObservable<Ray> OnRelease { get; }
    }
}