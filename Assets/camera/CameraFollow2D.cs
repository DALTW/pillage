using System.Collections.Generic;
using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);
    [SerializeField] private float smoothSpeed = 8f;
    [SerializeField] private float followDelay = 0.2f;
    [SerializeField, Range(0f, 1f)] private float followFractionPerTick = 0.1f;

    private readonly List<TargetSample> targetHistory = new List<TargetSample>();

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        targetHistory.Add(new TargetSample(Time.time, target.position));

        if (!TryGetFollowTarget(out Vector3 delayedTargetPosition))
        {
            return;
        }

        Vector3 targetPosition = delayedTargetPosition + offset;
        float followAmount = followFractionPerTick > 0f
            ? followFractionPerTick
            : smoothSpeed * Time.deltaTime;

        transform.position = Vector3.Lerp(transform.position, targetPosition, followAmount);
    }

    public void SnapToTarget()
    {
        if (target == null)
        {
            return;
        }

        transform.position = target.position + offset;
        ResetTargetHistory();
    }

    public void SetTarget(Transform newTarget, bool snapToTarget)
    {
        target = newTarget;
        ResetTargetHistory();

        if (snapToTarget)
        {
            SnapToTarget();
        }
    }

    private bool TryGetFollowTarget(out Vector3 followTarget)
    {
        if (followDelay <= 0f)
        {
            followTarget = target.position;
            return true;
        }

        float targetTime = Time.time - followDelay;
        if (targetHistory.Count == 0 || targetHistory[0].time > targetTime)
        {
            followTarget = default;
            return false;
        }

        while (targetHistory.Count > 1 && targetHistory[1].time <= targetTime)
        {
            targetHistory.RemoveAt(0);
        }

        if (targetHistory.Count == 1)
        {
            followTarget = targetHistory[0].position;
            return true;
        }

        TargetSample older = targetHistory[0];
        TargetSample newer = targetHistory[1];
        float duration = newer.time - older.time;
        float progress = duration > 0.0001f
            ? Mathf.Clamp01((targetTime - older.time) / duration)
            : 0f;

        followTarget = Vector3.Lerp(older.position, newer.position, progress);
        return true;
    }

    private void ResetTargetHistory()
    {
        targetHistory.Clear();
    }

    private readonly struct TargetSample
    {
        public readonly float time;
        public readonly Vector3 position;

        public TargetSample(float time, Vector3 position)
        {
            this.time = time;
            this.position = position;
        }
    }
}
