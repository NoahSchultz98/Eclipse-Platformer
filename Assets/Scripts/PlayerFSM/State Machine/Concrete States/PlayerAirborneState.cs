using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAirborneState : PlayerState
{
    public PlayerAirborneState(Player player, PlayerStateMachine playerFsm) : base(player, playerFsm)
    {
    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();
        if (player.direction.magnitude > 0)
        {
            player.transform.rotation = Quaternion.Lerp(player.transform.rotation, Quaternion.Euler(new Vector3(0, player.targetAngle, 0)), Time.deltaTime * player.turningSpeed);

            player.rb.velocity = new Vector3(player.movedirection.x * player.currentSpeed, player.rb.velocity.y, player.movedirection.z * player.currentSpeed); //affect movement

            //placeholder air movement code, copied from ground movement                                                                                                                                      //throwing ground movement in air for testing
        }

    }

    public override void StateStart()
    {
        base.StateStart();
        player.input.Player.Jump.canceled += cutJump;
        player.AnimationTriggerEvent(PlayerAnims.AnimationTriggers[PlayerAnims.AnimationNames.Jump]);
    }
    public override void StateExit()
    {
        base.StateExit();
        player.input.Player.Jump.canceled -= cutJump;
        player.AnimationFinishedEvent(PlayerAnims.AnimationTriggers[PlayerAnims.AnimationNames.Jump]);
    }

    private void cutJump(InputAction.CallbackContext ctx)
    {
        player.rb.velocity = new Vector2(player.rb.velocity.x, Mathf.Min(player.rb.velocity.y, player.jumpAmount / player.jumpCutMultiplier));
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        player.speedTarget = player.maxSpeed *
                  (Mathf.Abs(player.input.Player.Movement.ReadValue<Vector2>().x)
                 + Mathf.Abs(player.input.Player.Movement.ReadValue<Vector2>().y)); //simple way of speed checking the joystick

        player.currentSpeed = Mathf.Lerp(player.currentSpeed,
                                         player.speedTarget,
                                         Time.deltaTime * player.accelSpeed);


        if (player.CheckGround() && player.rb.velocity.y < 0)
        {
            playerFsm.SwitchState(player.movementState);
            //Debug.Log("Switched");
        }
    }
}
