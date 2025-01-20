using UnityEngine;
using System.Collections.Generic;

public static class InteractivityUtility
{
    public static GameObject CreateInteractivityIndicator(Transform interactiveCharacterTransform, GameObject interactivityIndicator, Material indicatorMaterial, Vector3 positionOffset, float range)
    {
        Vector3 interactiveCharacterPosition = interactiveCharacterTransform.position;
        Vector3 newInteractivityIndicatorPosition = new Vector3(interactiveCharacterPosition.x, interactivityIndicator.transform.position.y, interactiveCharacterPosition.z) + positionOffset;
        GameObject newInteractivityIndicator = Object.Instantiate(interactivityIndicator, newInteractivityIndicatorPosition, interactivityIndicator.transform.rotation);

        // add material to indicator
        Renderer[] renderers = newInteractivityIndicator.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.material = indicatorMaterial;
        }

        newInteractivityIndicator.transform.localScale = Vector3.one * range * 2;
        newInteractivityIndicator.gameObject.transform.parent = interactiveCharacterTransform;
        return newInteractivityIndicator;
    }

    public static void UpdateInteractivityIndicator(GameObject interactivityIndicator, float range)
    {
        interactivityIndicator.transform.localScale = Vector3.one * range * 2;
    }

    // obstacle avoidance
    public static readonly int includeObstacleLayer = LayerMask.NameToLayer("Obstacle");
    public static readonly int obstacleMask = (1 << includeObstacleLayer);

    // interactivity layer
    public static readonly int includeInteractiveLayer = LayerMask.NameToLayer("Interactive");
    public static readonly int interactiveMask = (1 << includeInteractiveLayer);

    public static List<Collider> CastRadius(Transform selfTransform, Vector3 interactivityIndicatorPosition, List<Collider> trackedCollidedList, List<Collider> removeCollidedList, float range)
    {
        // only look for interactive characters
        Collider[] collidedCharacterArray = Physics.OverlapSphere(interactivityIndicatorPosition, range, interactiveMask);

        foreach (Collider collidedCharacter in collidedCharacterArray)
        {
            // check that the interactive character isn't self and isn't already in tracked list
            if (!trackedCollidedList.Contains(collidedCharacter) && collidedCharacter.transform != selfTransform)
            {
                //Debug.Log($"{collidedCharacter.gameObject.name} is added to tracked list.");
                trackedCollidedList.Add(collidedCharacter);
            }
        }

        foreach (Collider collidedCharacter in trackedCollidedList)
        {
            // check that the interactive character hasn't disappeared from character list
            bool isPresent = false;

            for (int i = 0; i < collidedCharacterArray.Length; i++)
            {
                if (collidedCharacter == collidedCharacterArray[i])
                {
                    isPresent = true;
                    break;
                }
            }

            // if interactive character no longer in interactivity, mark for removal from tracked list
            if (!isPresent)
            {
                removeCollidedList.Add(collidedCharacter);
            }
        }

        // remove interactive characters from tracked list
        if (removeCollidedList.Count > 0)
        {
            foreach (Collider collidedCharacter in removeCollidedList)
            {
                trackedCollidedList.Remove(collidedCharacter);
            }

            removeCollidedList.Clear();
        }

        return trackedCollidedList;
    }

    public static Vector3 GetTowardDirection(Vector3 selfPosition, Vector3 targetPosition)
    {
        float directionX = targetPosition.x - selfPosition.x;
        float directionZ = targetPosition.z - selfPosition.z;
        Vector3 direction = new Vector3(directionX, 0, directionZ);

        return direction;
    }

    public static Vector3 GetAwayDirection(Vector3 selfPosition, Vector3 fleePosition)
    {
        float directionX = selfPosition.x - fleePosition.x;
        float directionZ = selfPosition.z - fleePosition.z;
        Vector3 direction = new Vector3(directionX, 0, directionZ);

        return direction;
    }
}
