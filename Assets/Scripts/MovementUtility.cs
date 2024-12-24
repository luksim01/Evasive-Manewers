using UnityEngine;

public static class MovementUtility
{
    public static void Fall(Rigidbody rigidbody, float riseGravityScale, float fallGravityScale)
    {
        float gravityScale;
        if (rigidbody.velocity.y < 0)
        {
            gravityScale = fallGravityScale;
        }
        else
        {
            gravityScale = riseGravityScale;
        }

        Vector3 gravity = -9.81f * gravityScale * Vector3.up;
        rigidbody.AddForce(gravity, ForceMode.Acceleration);
    }

    public static void Jump(Rigidbody rigidbody, Vector3 direction, float riseGravityScale, float jumpHeight)
    {
        float jumpForce = Mathf.Sqrt(jumpHeight * (Physics.gravity.y * riseGravityScale) * -2) * rigidbody.mass;
        rigidbody.AddForce(direction * jumpForce, ForceMode.Impulse);
    }
}
