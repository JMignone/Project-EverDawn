using UnityEngine;

public class StatusEffectMaterialSwapping : MonoBehaviour
{
    [SerializeField]
    private Renderer[] renderers;
    [SerializeField]
    private Material statusMaterialSolidColor;

    private bool hasOriginalMaterials = true;
    private Material statusMaterialCopy;
    private Material[][] originalMaterialArrays;
    private Material[][] statusMaterialArrays;
    private MaterialPropertyBlock[] materialPropertyBlocks;

    private Vector4 statusMaterialHDRColor;

    public Vector4 StatusMaterialHDRColor
    {
        get { return statusMaterialHDRColor; }
    }

    public void Awake()
    {
        statusMaterialHDRColor = new Vector4(0, 0, 0, 1);
        statusMaterialCopy = new Material(statusMaterialSolidColor);

        int renderersLength = renderers.Length;

        originalMaterialArrays = new Material[renderersLength][];
        statusMaterialArrays = new Material[renderersLength][];

        for (int i = 0, l = renderersLength; i < l; i++)
        {
            originalMaterialArrays[i] = renderers[i].materials;

            statusMaterialArrays[i] = new Material[renderers[i].materials.Length];
            for (int j = 0, m = statusMaterialArrays[i].Length; j < m; j++)
            {
                statusMaterialArrays[i][j] = statusMaterialCopy;
            }
        }

        materialPropertyBlocks = new MaterialPropertyBlock[renderersLength];

        for (int i = 0, l = materialPropertyBlocks.Length; i < l; i++)
        {
            materialPropertyBlocks[i] = new MaterialPropertyBlock();
        }
    }

    private void SwapToStatusMaterial()
    {
        for (int i = 0, l = renderers.Length; i < l; i++)
        {
            renderers[i].materials = statusMaterialArrays[i];
        }

        hasOriginalMaterials = false;
    }

    private void SwapToOriginalMaterials()
    {
        for (int i = 0, l = renderers.Length; i < l; i++)
        {
            renderers[i].materials = originalMaterialArrays[i];
        }

        hasOriginalMaterials = true;
    }

    public void ChangeHDRColor(Vector4 newColor)
    {
        if (hasOriginalMaterials)
        {
            SwapToStatusMaterial();
        }

        if (newColor == new Vector4(0, 0, 0, 1))
        {
            SwapToOriginalMaterials();
        }
        else
        {
            statusMaterialCopy.SetVector("_EmissionColor", newColor);
        }

        statusMaterialHDRColor = newColor;
    }

    private void OnDestroy()
    {
        Destroy(statusMaterialCopy);
    }
}
