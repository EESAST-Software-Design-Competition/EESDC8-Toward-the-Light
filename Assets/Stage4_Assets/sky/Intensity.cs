using UnityEngine;

public class Intensity : MonoBehaviour
{
    public LineRenderer line;
    public float intensity = 1.0f;
    

    void Update()
    {
        // ��̬���� Emission ǿ��
        //line.material.SetColor("_EmissionColor", Color.white * intensity);

        // URP ��ʹ�� "_BaseColor" �� "_EmissionColor"
         line.material.SetColor("_BaseColor", Color.yellow * intensity);
    }
}