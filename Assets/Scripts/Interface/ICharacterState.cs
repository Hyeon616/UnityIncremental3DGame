using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacterState
{
    void Enter();
    void Execute();
    void Exit();
}
