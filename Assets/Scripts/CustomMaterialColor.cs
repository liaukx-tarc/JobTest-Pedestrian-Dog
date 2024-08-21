using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class CustomMaterialColor : MonoBehaviour
{
    [SerializeField] private Material originMaterial;
    [Space][SerializeField] private Color color;

    private void Awake()
    {
        ApplyColor();

        enabled = false;
    }

    public void ApplyColor()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

        Material material = new Material(originMaterial);
        material.SetColor("_Color", color);

        meshRenderer.material = material;
    }
}