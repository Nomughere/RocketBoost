using UnityEngine;

public class ScrollingCameraLooper : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 40f;     // Units per second along +Z
    [SerializeField] private float startZ = -970f;  // Loop start (inclusive)
    [SerializeField] private float endZ = 2080f;    // Loop end (exclusive)

    [Header("Behavior")]
    [SerializeField] private bool playOnStart = true;

    // Cached X/Y so only Z is modified
    private float fixedX;
    private float fixedY;

    private void OnEnable()
    {
        // Cache X/Y once on enable
        var p = transform.position;
        fixedX = p.x;
        fixedY = p.y;

        if (playOnStart)
        {
            ResetToStart();
        }
        else
        {
            // Ensure Z is within range without snapping if already moving
            ClampIntoRange();
        }
    }

    private void Update()
    {
        if (speed <= 0f) return;

        Vector3 p = transform.position;
        p.x = fixedX;
        p.y = fixedY;
        p.z += speed * Time.deltaTime;

        // Seamless wrap with overflow carried over
        float range = endZ - startZ;
        if (range <= 0f)
        {
            // Safety: if misconfigured, make sure we don't divide by zero.
            range = 1f;
            endZ = startZ + range;
        }

        if (p.z >= endZ)
        {
            float overflow = (p.z - startZ) % range;
            p.z = startZ + overflow;
        }

        transform.position = p;
    }

    // Public utility to reset to exact start position
    public void ResetToStart()
    {
        transform.position = new Vector3(fixedX, fixedY, startZ);
    }

    // Keeps current Z projected into the [startZ, endZ) interval without a jump
    private void ClampIntoRange()
    {
        float range = endZ - startZ;
        if (range <= 0f) return;

        Vector3 p = transform.position;
        float offset = p.z - startZ;
        // Proper modulo that handles negative values
        offset = (offset % range + range) % range;
        p = new Vector3(fixedX, fixedY, startZ + offset);
        transform.position = p;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // Visualize the path and endpoints in Scene view
        Gizmos.color = Color.cyan;

        // Use current X/Y for markers
        float x = Application.isPlaying ? fixedX : transform.position.x;
        float y = Application.isPlaying ? fixedY : transform.position.y;

        Vector3 a = new Vector3(x, y, startZ);
        Vector3 b = new Vector3(x, y, endZ);

        Gizmos.DrawLine(a, b);
        Gizmos.DrawSphere(a, 5f);
        Gizmos.DrawSphere(b, 5f);
    }
#endif
}