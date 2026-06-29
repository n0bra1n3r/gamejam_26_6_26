using UnityEngine;
using System.Collections.Generic;

public class BowWeapon : MonoBehaviour, IWeapon
{
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Vector3 arrowOffset = Vector3.zero;
    [SerializeField] private float arrowSpeed = 10;
    [SerializeField] private float arrowRange = 50;
    [SerializeField] private float reloadSpeed = 1;

    private class ArrowInfo
    {
        public Vector3 Origin;
        public Vector3 WakeOrigin;
        public Vector3 Direction;
        public GameObject Arrow;
    }

    private List<IEffect> effects = new List<IEffect>();
    private List<float> effectRadii = new List<float>();
    private GameObject reloadingArrow;
    private List<ArrowInfo> firingArrows = new List<ArrowInfo>();
    private float reloadProgress = 0;

    public bool Reload()
    {
        if (reloadingArrow != null) return false;

        Vector3 arrowPosition = transform.position + arrowOffset;

        if (reloadProgress == 0) SpawnWakeEffect(arrowPosition);
        UpdateWakeEffect(arrowPosition, arrowPosition, reloadProgress);

        reloadProgress += Time.deltaTime * reloadSpeed;
        if (reloadProgress < 1) return false;

        reloadProgress = 0;
        reloadingArrow = Instantiate(arrowPrefab, arrowPosition, transform.rotation, transform);
        return true;
    }
    public void Fire()
    {
        if (reloadingArrow == null) return;

        reloadingArrow.transform.parent = null;
        firingArrows.Add(new ArrowInfo
        {
            Origin = reloadingArrow.transform.position,
            WakeOrigin = reloadingArrow.transform.position,
            Direction = transform.forward,
            Arrow = reloadingArrow
        });
        reloadingArrow = null;
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
        int arrowIndex = 0;
        while (firingArrows.Count > arrowIndex)
        {
            ArrowInfo arrowInfo = firingArrows[arrowIndex];

            Vector3 arrowPosition = arrowInfo.Arrow.transform.position;
            Vector3 arrowVelocity = arrowInfo.Direction * Time.deltaTime * arrowSpeed;

            if (Vector3.Distance(arrowInfo.Origin, arrowPosition) > arrowRange)
            {
                Vector3 toArrowPosition = Vector3.Normalize(arrowPosition - arrowInfo.Origin);
                Vector3 toWakePosition = Vector3.Normalize(arrowPosition - arrowInfo.WakeOrigin);

                if (Vector3.Dot(toWakePosition, toArrowPosition) > 0)
                {
                    arrowInfo.WakeOrigin += arrowVelocity;
                    UpdateWakeEffect(arrowInfo.WakeOrigin, arrowPosition, 1, arrowIndex);
                    arrowIndex++;
                }
                else
                {
                    firingArrows.RemoveAt(arrowIndex);
                    Destroy(arrowInfo.Arrow);
                    RemoveWakeEffect(arrowIndex);
                }
            }
            else
            {
                arrowPosition += arrowVelocity;
                UpdateWakeEffect(arrowInfo.WakeOrigin, arrowPosition, 1, arrowIndex);
                arrowInfo.Arrow.transform.position = arrowPosition;
                arrowIndex++;
            }
        }
        if (reloadingArrow != null)
        {
            UpdateWakeEffect(
                reloadingArrow.transform.position,
                reloadingArrow.transform.position,
                reloadProgress > 0 ? reloadProgress : 1
            );
        }
    }
    private void SpawnWakeEffect(Vector3 origin)
    {
        foreach (var effect in effects)
        {
            List<WorldSegment> segments = effect.WorldSegments ?? new List<WorldSegment>();
            segments.Add(new WorldSegment{ Start = origin, End = origin, Radius = 0 });
            effect.WorldSegments = segments;
        }
    }
    private void UpdateWakeEffect(Vector3 start, Vector3 end, float radiusFraction, int index = -1)
    {
        for (int i = 0; i < effects.Count; i++)
        {
            IEffect effect = effects[i];
            float effectRadius = effectRadii[i];

            List<WorldSegment> segments = effect.WorldSegments ?? new List<WorldSegment>();
            segments[index >= 0 ? index : segments.Count - 1] = new WorldSegment{
                Start = start,
                End = end,
                Radius = effectRadius * radiusFraction
            };
            effect.WorldSegments = segments;
        }
    }
    private void RemoveWakeEffect(int index)
    {
        foreach (var effect in effects)
        {
            List<WorldSegment> segments = effect.WorldSegments;
            if (segments == null || segments.Count <= index) continue;

            segments.RemoveAt(index);
            effect.WorldSegments = segments;
        }
    }
}
