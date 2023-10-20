using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StressTest : MonoBehaviour
{

    private enum GPUSkinningType
    { 
        UNITY,
        CUSTOM
    }

    [SerializeField, Range(1, 64)] private int _cloneCount = 64;
    [SerializeField] private GameObject _avatarPrefab;
    [SerializeField] private GPUSkinningType _type;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        for (int i = 0; i < _cloneCount; i++)
        {
            var obj = Instantiate(_avatarPrefab);
            obj.transform.SetParent(transform, false);
            obj.transform.localPosition = new Vector3(i % 8 * 2, 0, - i / 8 * 2);

            if (_type == GPUSkinningType.CUSTOM)
            {
                var anim = obj.GetComponentInChildren<Animator>();
                anim.gameObject.AddComponent<VertexShaderSkinning>();
            }
        }
    }

}
