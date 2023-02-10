using UnityEngine;

public class ImageEffectOnRender : MonoBehaviour {

    public Material material;

    void OnRenderImage(RenderTexture src, RenderTexture dest) {
        Graphics.Blit(src, dest, material);
    }
}
