using UnityEngine;
using System.Collections.Generic;

public class BowWeapon : MonoBehaviour, IWeapon
{
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Vector3 arrowOffset = Vector3.zero;
    [SerializeField] private float arrowSpeed = 10;
    [SerializeField] private float arrowRange = 50;
    [SerializeField] private float reloadSpeed = 1;

    private List<IEffect> effects = new List<IEffect>();
    private List<float> effectRadii = new List<float>();
    private Vector3 effectOrigin;
    private Vector3 arrowDirection;
    private GameObject reloadingArrow;
    private GameObject firingArrow;
    private float reloadProgress = 0;

    public bool Reload()
    {
        if (firingArrow != null || reloadingArrow != null) return false;
        Vector3 arrowPosition = transform.position + arrowOffset;
        UpdateEffectRadii(reloadProgress);
        UpdateEffectPositions(arrowPosition, arrowPosition);
        reloadProgress += Time.deltaTime * reloadSpeed;
        if (reloadProgress < 1) return false;
        UpdateEffectRadii(1);
        reloadProgress = 0;
        reloadingArrow = Instantiate(arrowPrefab, arrowPosition, transform.rotation, transform);
        return true;
    }
    public void Fire()
    {
        if (firingArrow != null || reloadingArrow == null) return;
        reloadingArrow.transform.parent = null;
        firingArrow = reloadingArrow;
        reloadingArrow = null;
        effectOrigin = firingArrow.transform.position;
        arrowDirection = transform.forward;
    }
    void Awake()
    {
        foreach (var effect in GetComponents<IEffect>())
        {
            effects.Add(effect);
            effectRadii.Add(effect.Radius);
        }
    }
    void Update()
    {
        if (firingArrow)
        {
            Vector3 arrowPosition = firingArrow.transform.position;
            Vector3 arrowVelocity = arrowDirection * Time.deltaTime * arrowSpeed;

            UpdateEffectPositions(effectOrigin, firingArrow.transform.position);

            if (Vector3.Distance(transform.position, arrowPosition) > arrowRange)
            {
                Vector3 toArrowPosition = Vector3.Normalize(arrowPosition - transform.position);
                Vector3 toEffectOrigin = Vector3.Normalize(arrowPosition - effectOrigin);

                if (Vector3.Dot(toEffectOrigin, toArrowPosition) > 0)
                {
                    effectOrigin += arrowVelocity;
                }
                else
                {
                    Destroy(firingArrow);
                    firingArrow = null;
                }
            }
            else
            {
                firingArrow.transform.position += arrowVelocity;
            }
        }
        else
        {
            if (reloadingArrow)
            {
                UpdateEffectPositions(reloadingArrow.transform.position, reloadingArrow.transform.position);
            }
        }
    }
    private void UpdateEffectPositions(Vector3 start, Vector3 end)
    {
        foreach (var effect in effects)
        {
            effect.StartWorldPos = start;
            effect.EndWorldPos = end;
        }
    }
    private void UpdateEffectRadii(float frac)
    {
        for (int i = 0; i < effects.Count; i++)
        {
            effects[i].Radius = effectRadii[i] * frac;
        }
    }
}
