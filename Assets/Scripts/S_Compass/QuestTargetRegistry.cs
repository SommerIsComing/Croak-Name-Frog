using UnityEngine;
using System.Collections.Generic;

public class QuestTargetRegistry : MonoBehaviour
{
    public static QuestTargetRegistry questTargetRegistry;

    private readonly Dictionary<string, Transform> targetsByID = new Dictionary<string, Transform>();

    private void Awake()
    {
        if (questTargetRegistry == null)
        {
            questTargetRegistry = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterTarget(string id, Transform target)
    {
        if (string.IsNullOrWhiteSpace(id) || target == null)
        {
            return;
        }

        string key = NormalizeID(id);
        targetsByID[key] = target;
    }

    public void UnregisterTarget(string id, Transform target = null)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return;
        }

        string key = NormalizeID(id);
        if (!targetsByID.TryGetValue(key, out Transform current))
        {
            return;
        }

        if (target != null && current != target)
        {
            return;
        }

        targetsByID.Remove(key);
    }

    public bool TryGetTarget(string id, out Transform target)
    {
        target = null;

        if (string.IsNullOrWhiteSpace(id))
        {
            return false;
        }

        string key = NormalizeID(id);

        if (!targetsByID.TryGetValue(key, out target))
        {
            return false;
        }

        if (target == null)
        {
            targetsByID.Remove(key);
            return false;
        }

        return true;
    }

    private static string NormalizeID(string id)
    {
        return id.Trim().ToLowerInvariant();
    }
}
