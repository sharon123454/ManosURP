using System.Collections.Generic;
using UnityEngine;

public class AnimationVFXManager : MonoBehaviour
{
    [SerializeField] private Transform _bulletProjectilePrefab;
    [SerializeField] private Transform _shootPointTransform;

    private OnShootEventArgs _rangeEventArgs;

    public void SetShootVFXData(OnShootEventArgs eventArgs) { _rangeEventArgs = eventArgs; }

    #region Logic fired based on animation events
    //Shooting VFX
    public void ShootingVFX()
    {
        //Finding Target Hit Position
        Vector3 targetPosition = _rangeEventArgs.TargetUnit.GetWorldPosition();//setting with base unit world position as to not be NULL
        List<HitPosition> hitPositionList = _rangeEventArgs.TargetUnit.GetUnitHitPositionList();//gets the units hit positions
        foreach (HitPosition hitPosition in hitPositionList)
        {
            if (hitPosition.Type == _rangeEventArgs.TargetHitPositionType)//if type to hit matches, cache the accurate target position
            {
                targetPosition = hitPosition.HitLocation.position;
            }
        }

        //Creating VFX
        Transform bulletTransform = Instantiate(_bulletProjectilePrefab, _shootPointTransform.position, Quaternion.identity);
        BulletProjectile bulletProjectile = bulletTransform.GetComponent<BulletProjectile>();

        //Sets the VFX target position
        bulletProjectile.SetUp(targetPosition);
    }
    #endregion

}