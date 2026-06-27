using UnityEngine;
using System.Collections.Generic;

public class BowWeapon : MonoBehaviour, IWeapon
{
    [SerializeField] private Material wakeEffect;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Vector3 arrowOffset = Vector3.zero;
    [SerializeField] private float arrowSpeed = 10;
    [SerializeField] private float arrowRange = 50;
    private List<IEffect> effects = new List<IEffect>();
    private Vector3 firePosition;
    private GameObject reloadingArrow;
    private GameObject firingArrow;
    public void Reload()
    {
        if (reloadingArrow) return;
        reloadingArrow = Instantiate(arrowPrefab, transform.position + arrowOffset, transform.rotation, transform);
    }
    public void Fire()
    {
        if (firingArrow) return;
        firePosition = transform.position + arrowOffset;
        reloadingArrow.transform.parent = null;
        firingArrow = reloadingArrow;
        reloadingArrow = null;
    }
    void Awake()
    {
        effects.AddRange(GetComponents<IEffect>());
    }
    void Update()
    {
        if (firingArrow)
        {
            firingArrow.transform.position += transform.forward * Time.deltaTime * arrowSpeed;
            UpdateEffects(firePosition, firingArrow.transform.position);
            if (Vector3.Distance(transform.position, firingArrow.transform.position) > arrowRange)
            {
                Destroy(firingArrow);
                firingArrow = null;
            }
        }
        else
        {
            if (reloadingArrow)
            {
                UpdateEffects(reloadingArrow.transform.position, reloadingArrow.transform.position);
            }
        }
    }
    private void UpdateEffects(Vector3 start, Vector3 end)
    {
        foreach (var effect in effects)
        {
            effect.StartWorldPos = start;
            effect.EndWorldPos = end;
        }
    }
}
