using UnityEngine;

public class RadialGaussBlur : MonoBehaviour {

    [Range(0, 25)]
    public float radius = 1;

    [Range(1, 10)]
    public int iterations = 1;

    public Shader radialGaussShader;
    private Material radialGaussMaterial;

    private int radiusID = -1;
    private int texelWidthID = -1;
    private int texelHeightID = -1;

    private void Start() {
        radialGaussMaterial = new Material(radialGaussShader);

        //radiusID = Shader.PropertyToID();
        radiusID = Shader.PropertyToID("_radius");
        texelWidthID = Shader.PropertyToID("_texelWidth");
        texelHeightID = Shader.PropertyToID("_texelHeight");
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination) {
        if (!radialGaussMaterial) {
            Graphics.Blit(source, destination);
        }

        RenderTexture temp = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
        radialGaussMaterial.SetFloat(radiusID, 1 / radius);
        radialGaussMaterial.SetFloat(texelWidthID, 1.0f / source.width);
        radialGaussMaterial.SetFloat(texelHeightID, 1.0f / source.height);

        //radialGaussMaterial.SetFloat()
        for (int i = 0; i < iterations; ++i) {
            RenderTexture target = (i == iterations - 1) ? destination : source;

            Graphics.Blit(source, temp, radialGaussMaterial, 0);
            Graphics.Blit(temp, target, radialGaussMaterial, 1);
        }

        RenderTexture.ReleaseTemporary(temp);
    }
}