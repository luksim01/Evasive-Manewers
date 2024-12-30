using UnityEngine;
using System.Collections;

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

    public static float Flee(Rigidbody rigidbody, Vector3 startPosition, Vector3 direction, float distance, float time, Vector3 targetPosition, float elapsedTime)
    {
        if (elapsedTime < time)
        {
            // location of next position to move
            float step = (distance / time) * Time.fixedDeltaTime;
            Vector3 nextPosition = rigidbody.position + direction.normalized * step;

            // if distance covered then snap to target position
            if (Vector3.Distance(nextPosition, startPosition) > distance)
            {
                nextPosition = targetPosition;
            }

            // move and keep track of time 
            rigidbody.MovePosition(nextPosition);
            elapsedTime += Time.fixedDeltaTime;
        }

        return elapsedTime;
    }
}
