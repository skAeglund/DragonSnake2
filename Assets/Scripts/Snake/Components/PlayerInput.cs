using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;

public class PlayerInput : MonoBehaviour
{
    public void CheckInputs(out RotationDirection rD, out Ability ability, out bool slowDown)
    {
        rD = CheckRotationInput();
        ability = CheckAbilityInput();
        slowDown = CheckSlowDownInput();
    }

    public RotationDirection CheckRotationInput()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            return RotationDirection.Left;
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            return RotationDirection.Right;
        }
        else return RotationDirection.None;
    }

    public Ability CheckAbilityInput()
    {
        if (!Input.anyKeyDown) return Ability.Null;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Alpha2))
        {
            return Ability.Whirlwind;
        }
        else if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl) || Input.GetKeyDown(KeyCode.Alpha3))
        {
            return Ability.StoneState;
        }
        else if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Alpha1))
        {
            return Ability.Fireball;
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.Alpha4))
        {
            return Ability.Dash;
        }
        return Ability.Null;
    }
    public bool CheckSlowDownInput()
    {
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            return true;

        return false;
    }
}
