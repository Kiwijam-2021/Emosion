using UnityEngine;

public class CamBorder : MonoBehaviour
{
    private Camera _camera;

    private void Awake()
    {
        AddCollider();
        _camera = GetComponent<Camera>();
    }

    // https://stackoverflow.com/questions/58941259/how-to-give-the-cameras-edge-collision-in-unity
    private void AddCollider()
    {
        if (_camera == null)
        {
            Debug.LogError("Camera.main not found, failed to create edge colliders");
            return;
        }

        var cam = _camera;
        if (!cam.orthographic)
        {
            Debug.LogError("Camera.main is not Orthographic, failed to create edge colliders");
            return;
        }

        var bottomLeft = (Vector2)cam.ScreenToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
        var topLeft = (Vector2)cam.ScreenToWorldPoint(new Vector3(0, cam.pixelHeight, cam.nearClipPlane));
        var topRight =
            (Vector2)cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, cam.pixelHeight, cam.nearClipPlane));
        var bottomRight = (Vector2)cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, 0, cam.nearClipPlane));

        // add or use existing EdgeCollider2D
        var edge = GetComponent<EdgeCollider2D>() == null
            ? gameObject.AddComponent<EdgeCollider2D>()
            : GetComponent<EdgeCollider2D>();

        var edgePoints = new[] { bottomLeft, topLeft, topRight, bottomRight, bottomLeft };
        edge.points = edgePoints;
    }
}