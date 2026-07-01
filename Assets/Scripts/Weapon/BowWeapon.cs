using UnityEngine;
using System.Collections.Generic;

public class BowWeapon : MonoBehaviour, IWeapon
{
    [SerializeField] private GameObject bowObject;
    [SerializeField] private Vector3 bowLoadPosition = Vector3.zero;
    [SerializeField] private Vector3 bowLoadRotation = Vector3.zero;
    [SerializeField] private Vector3 bowFirePosition = Vector3.zero;
    [SerializeField] private Vector3 bowFireRotation = Vector3.zero;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Vector3 arrowRestPosition = Vector3.zero;
    [SerializeField] private Vector3 arrowRestRotation = Vector3.zero;
    [SerializeField] private Vector3 arrowDrawPosition = Vector3.zero;
    [SerializeField] private Vector3 arrowDrawRotation = Vector3.zero;
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
    private Animator bowAnimator;

    public bool Reload()
    {
        if (reloadProgress < 1)
        {
            reloadProgress += Time.deltaTime * reloadSpeed;

            if (reloadProgress < 0.5)
            {
                float lowerProgress = reloadProgress * 2;
                bowObject.transform.localPosition = Vector3.Lerp(bowFirePosition, bowLoadPosition, lowerProgress);
                bowObject.transform.localRotation = Quaternion.Lerp(Quaternion.Euler(bowFireRotation), Quaternion.Euler(bowLoadRotation), lowerProgress);
            }
            else
            {
                if (reloadingArrow == null)
                {
                    reloadingArrow = Instantiate(arrowPrefab, bowObject.transform);
                    reloadingArrow.GetComponent<Collider>().enabled = false;
                    reloadingArrow.transform.localPosition = arrowRestPosition;
                    reloadingArrow.transform.localRotation = Quaternion.Euler(arrowRestRotation);
                    SpawnWakeEffect(reloadingArrow.transform.position);
                }

                float raiseProgress = (reloadProgress - 0.5f) * 2;
                bowObject.transform.localPosition = Vector3.Lerp(bowLoadPosition, bowFirePosition, raiseProgress);
                bowObject.transform.localRotation = Quaternion.Lerp(Quaternion.Euler(bowLoadRotation), Quaternion.Euler(bowFireRotation), raiseProgress);
                reloadingArrow.transform.localPosition = Vector3.Lerp(arrowRestPosition, arrowDrawPosition, raiseProgress);
                reloadingArrow.transform.localRotation = Quaternion.Lerp(Quaternion.Euler(arrowRestRotation), Quaternion.Euler(arrowDrawRotation), raiseProgress);

                bowAnimator.speed = 0;
                bowAnimator.Play(bowAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash, -1, raiseProgress * 0.6f);
                bowAnimator.Update(0);
            }

            if (reloadingArrow != null)
            {
                UpdateWakeEffect(reloadingArrow.transform.position, reloadingArrow.transform.position, reloadProgress);
            }

            return false;
        }

        return true;
    }
    public void Fire()
    {
        if (reloadProgress >= 1)
        {
            bowAnimator.speed = 1;

            reloadingArrow.transform.parent = null;
            reloadingArrow.GetComponent<Collider>().enabled = true;
            firingArrows.Add(new ArrowInfo
            {
                Origin = reloadingArrow.transform.position,
                WakeOrigin = reloadingArrow.transform.position,
                Direction = reloadingArrow.transform.up,
                Arrow = reloadingArrow
            });

            reloadingArrow = null;
            reloadProgress = 0;
        }
    }
    void Awake()
    {
        bowAnimator = bowObject.GetComponent<Animator>();
        bowAnimator.speed = 0;

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

            if (arrowInfo.Arrow == null)
            {
                firingArrows.RemoveAt(arrowIndex);
                RemoveWakeEffect(arrowIndex);
                continue;
            }

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
                reloadProgress
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
