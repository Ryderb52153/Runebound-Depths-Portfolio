using System.Collections;
using UnityEngine;

public class Player_VFX : Entity_VFX
{
    [Header("Image Echo VFX")]
    [Range(0.01f, .2f)]
    [SerializeField] private float imageEchoInterval = 0.05f;
    [SerializeField] private GameObject imageEchoPrefab;

    private Coroutine imageCo;

    public void CreateEffect(GameObject effect, Transform target)
    {
        ObjectPoolManager.SpawnObject(effect, target.position, Quaternion.identity);
    }

    public void DoImageEchoEffect(float duration)
    {
        if (imageCo != null)
            StopCoroutine(imageCo);

        imageCo = StartCoroutine(ImageEchoEffectCo(duration));
    }

    private IEnumerator ImageEchoEffectCo(float duration)
    {
        float time = 0f;

        while (time < duration)
        {
            CreateImageEcho();
            yield return new WaitForSeconds(imageEchoInterval);
            time += imageEchoInterval;
        }
    }    

    private void CreateImageEcho()
    {
        GameObject echo = ObjectPoolManager.SpawnObject(imageEchoPrefab, transform.position, transform.rotation);
        echo.GetComponentInChildren<SpriteRenderer>().sprite = spriteRenderer.sprite;
    }
}