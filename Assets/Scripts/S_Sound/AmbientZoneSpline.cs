using UnityEngine;
using Unity.Mathematics;
using UnityEngine.Splines;

// ============== INSTRUCTION ==============
// Create empty game object and add "Spline Container" component
// Add knots and draw line using the "Spline Edit Mode" in the Scene Tools
// Create another empty game object and add this script
// Select "Spline" as well as "Player" in the inspector
// Add sound to the object
// Credits: VS Audio Design https://www.youtube.com/watch?v=31CCmSsiRfY

public class AmbientZoneSpline : MonoBehaviour
{
    [Tooltip("Spline Path to follow")]
    public SplineContainer Spline;

    [Tooltip("Character to track")]
    public Transform Player;

    void Update()
    {
        // Convert Player position to spline local space
        Vector3 localPlayerPos = Spline.transform.InverseTransformPoint(Player.position);

        // Find nearest point on spline
        SplineUtility.GetNearestPoint(Spline.Spline, localPlayerPos, out float3 nearestPointLocal, out float normalizedT);

        // Convert back to world space
        Vector3 nearestWorldPos = Spline.transform.TransformPoint(nearestPointLocal);

        // Set object position and rotation
        transform.position = nearestWorldPos;
        Vector3 tangent = Spline.transform.TransformDirection(Spline.Spline.EvaluateTangent(normalizedT));
        if (tangent != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(tangent);

        // Check if Spline is closed
        if (!Spline.Spline.Closed)
            return;

        // Define vectors for the dot product
        Vector3 sub = transform.position - Player.position;
        Vector3 splineRight = Vector3.Cross(Vector3.up, tangent);

        // Dot product to check whether Player is inside or outside
        if (Vector3.Dot(sub, splineRight) < 0f)
        {
            transform.position = Player.position + new Vector3(0f, 1f, 0f);
            transform.rotation = Player.rotation;
        }
    }
}