using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitMaterials
{
    /*
    [SerializeField]
    private List<Renderer> renderers;
    */

    //[SerializeField]
    //private bool transparent;

    //private Color emissionColor; //emission color is the what makes a unit appear frozen, poisoned, or hovered by an ability

    [SerializeField]
    private StatusEffectMaterialSwapping materialSwapper;

    private bool hovered;

/*
    public bool Transparent
    {
        get { return transparent; }
        set { transparent = value; }
    }


    public List<Renderer> Renderers
    {
        get { return renderers; }
    }

    public void Start() {
        emissionColor = new Color(1f,1f,1f,1f);
    }
*/
    public void MakeInvisible()
    {
        /*
        foreach(Renderer renderer in renderers) {
            Material mat = renderer.material;
            mat.SetColor("_Color", new Color(1f,1f,1f,0));
            mat.SetFloat("_Mode", 3);
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.renderQueue = 3000;
        }
        //transparent = true;
        */


    }

    public void MakeTransparent()
    {
        /*
        foreach(Renderer renderer in renderers) {
            Material mat = renderer.material;
            mat.SetColor("_Color", new Color(1f,1f,1f,.65f));
            mat.SetFloat("_Mode", 3);
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.renderQueue = 3000;
        }
        //transparent = true;
        */


    }

    public void MakeOpaque()
    {
        /*
        foreach(Renderer renderer in renderers) {
            Material mat = renderer.material;
            mat.SetColor("_Color", new Color(1f,1f,1f,1f));
            mat.SetFloat("_Mode", 0);
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            mat.DisableKeyword("_ALPHABLEND_ON");
            mat.renderQueue = -1;
        }
        //transparent = false;
        */


    }

    public void TintPurple()
    {
        /*
        emissionColor = new Color(emissionColor.r + .1569f, emissionColor.g, emissionColor.b + .4706f, 0);
        foreach(Renderer renderer in renderers) {
            Material mat = renderer.material;
            if(!hovered)
                mat.SetColor("_EmissionColor", emissionColor);
        }
        */

        if (hovered == false)
        {
            Vector4 statusMaterialHDRColor = materialSwapper.StatusMaterialHDRColor;
            materialSwapper.ChangeHDRColor(new Vector4(statusMaterialHDRColor.x + .1569f, statusMaterialHDRColor.y, statusMaterialHDRColor.z + .4706f, 1));
        }
    }

    public void RemovePurple()
    {
        /*
        emissionColor = new Color(emissionColor.r - .1569f, emissionColor.g, emissionColor.b - .4706f, 0);
        foreach(Renderer renderer in renderers) {
            Material mat = renderer.material;
            if(!hovered)
                mat.SetColor("_EmissionColor", emissionColor);
        }
        */

        if (hovered == false)
        {
            Vector4 statusMaterialHDRColor = materialSwapper.StatusMaterialHDRColor;
            materialSwapper.ChangeHDRColor(new Vector4(statusMaterialHDRColor.x - .1569f, statusMaterialHDRColor.y, statusMaterialHDRColor.z - .4706f, 1));
        }
    }

    public void TintCyan()
    {
        /*
        emissionColor = new Color(emissionColor.r + .0784f, emissionColor.g + .451f, emissionColor.b + .39216f, 0);
        foreach(Renderer renderer in renderers) {
            Material mat = renderer.material;
            if(!hovered)
                mat.SetColor("_EmissionColor", emissionColor);
        }
        */

        if (hovered == false)
        {
            Vector4 statusMaterialHDRColor = materialSwapper.StatusMaterialHDRColor;
            materialSwapper.ChangeHDRColor(new Vector4(statusMaterialHDRColor.x + .0784f, statusMaterialHDRColor.y + .451f, statusMaterialHDRColor.z + .39216f, 1));
        }
    }

    public void RemoveCyan()
    {
        /*
        emissionColor = new Color(emissionColor.r - .0784f, emissionColor.g - .451f, emissionColor.b - .39216f, 0);
        foreach(Renderer renderer in renderers) {
            Material mat = renderer.material;
            if(!hovered)
                mat.SetColor("_EmissionColor", emissionColor);
        }
        */

        if (hovered == false)
        {
            Vector4 statusMaterialHDRColor = materialSwapper.StatusMaterialHDRColor;
            materialSwapper.ChangeHDRColor(new Vector4(statusMaterialHDRColor.x - .0784f, statusMaterialHDRColor.y - .451f, statusMaterialHDRColor.z - .39216f, 1));
        }
    }

    public void AbilityHover()
    {
        /*
        Color color = new Color(.5f, 0, 0, 1f);
        foreach(Renderer renderer in renderers) {
            Material mat = renderer.material;
            mat.SetColor("_EmissionColor", color);
            //mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.None;
            //mat.EnableKeyword("_EMISSION");
        }
        */

        Vector4 statusMaterialHDRColor = materialSwapper.StatusMaterialHDRColor;
        materialSwapper.ChangeHDRColor(new Vector4(.5f, 0, 0, 1));

        hovered = true;
    }

    public void RemoveAbilityHover()
    {
        /*
        foreach(Renderer renderer in renderers) {
            Material mat = renderer.material;
            mat.SetColor("_EmissionColor", emissionColor);
            //mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.None;
            //mat.DisableKeyword("_EMISSION");
        }
        */

        Vector4 statusMaterialHDRColor = materialSwapper.StatusMaterialHDRColor;
        materialSwapper.ChangeHDRColor(new Vector4(0, 0, 0, 1));

        hovered = false;
    }
}
