using UnityEngine;

public class CameraCutout : MonoBehaviour
{
    [SerializeField]
    private Transform cutoutTarget;

    [SerializeField]
    private LayerMask occlusionMask;

    [SerializeField]
    private float cutoutSize = 0.1f;
    [SerializeField]
    private float falloffSize = 0.05f;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = GetComponent<Camera>();
    }

    private void Update()
    {
        Vector2 cutoutPos = mainCamera.WorldToViewportPoint(cutoutTarget.position);
        cutoutPos.y /= (Screen.width / Screen.height);

        Vector3 offset = cutoutTarget.position - transform.position;
        RaycastHit[] hitObjects = Physics.RaycastAll(transform.position, offset, offset.magnitude, occlusionMask);

        for(int i = 0; i < hitObjects.Length; ++i)
        {
            Material[] materials = hitObjects[i].transform.GetComponent<Renderer>().materials;

            for(int m = 0; m < materials.Length; ++m)
            {
                materials[m].SetVector("_CutoutPos", cutoutPos);
                materials[m].SetFloat("_CutoutSize", cutoutSize);
                materials[m].SetFloat("_FalloffSize", falloffSize);
            }
        }
    }
}
