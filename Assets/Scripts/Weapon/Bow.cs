using UnityEngine;

public class Bow : MonoBehaviour, IWeapon
{
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Vector3 arrowOffset = Vector3.zero;
    [SerializeField] private float arrowSpeed = 10;
    [SerializeField] private float arrowRange = 50;
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
        reloadingArrow.transform.parent = null;
        firingArrow = reloadingArrow;
        reloadingArrow = null;
    }
    void Start()
    {
    }
    void Update()
    {
        if (firingArrow)
        {
            firingArrow.transform.position += transform.forward * Time.deltaTime * arrowSpeed;
            if (Vector3.Distance(transform.position, firingArrow.transform.position) > arrowRange)
            {
                Destroy(firingArrow);
                firingArrow = null;
            }
        }
    }
}
