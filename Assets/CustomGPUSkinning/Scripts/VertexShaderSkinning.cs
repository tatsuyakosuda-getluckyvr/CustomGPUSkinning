using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Runtime.InteropServices;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;

public class VertexShaderSkinning : MonoBehaviour
{

    private struct SVertInSkin
    {
        public float weight0, weight1, weight2, weight3;
        public int index0, index1, index2, index3;
    }

    [BurstCompile]
    private struct CalculateBoneMatrices : IJob
    {
        public NativeArray<Matrix4x4> result;
        public Matrix4x4 worldToLocalMatrix;
        public NativeArray<Matrix4x4> localToWorldMatrix;
        public NativeArray<Matrix4x4> bindposes;

        public void Execute()
        {
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = worldToLocalMatrix * localToWorldMatrix[i] * bindposes[i];
            }
        }
    }

    [SerializeField] private Mesh _mesh;
    [SerializeField] private Material _mat;

    private Transform[] _boneTransforms;
    private ComputeBuffer _bones;
    private NativeArray<Matrix4x4> _boneMatrices;
    private NativeArray<Matrix4x4> _localToWorldMatrix;
    private NativeArray<Matrix4x4> _bindposes;
    private Matrix4x4[] _bindposeMatrices;

    private void Awake()
    {
        Initialize();
    }

    private void OnDestroy()
    {
        _boneMatrices.Dispose();
        _localToWorldMatrix.Dispose();
        _bindposes.Dispose();
    }

    private void Update()
    {
        SetBoneMatrices();
    }

    private void SetBoneMatrices()
    {
        for (int i = 0; i < _boneTransforms.Length; i++)
        {
            _localToWorldMatrix[i] = _boneTransforms[i].localToWorldMatrix;
            _bindposes[i] = _bindposeMatrices[i];
        }

        var myjob = new CalculateBoneMatrices()
        {
            localToWorldMatrix = _localToWorldMatrix,
            worldToLocalMatrix = transform.worldToLocalMatrix,
            bindposes = _bindposes,
            result = _boneMatrices
        };
        var handle = myjob.Schedule();
        handle.Complete();
        _bones.SetData(_boneMatrices);
    }

    private void Initialize()
    {
        var skin = GetComponentInChildren<SkinnedMeshRenderer>();

        if (_mesh == null) { _mesh = skin.sharedMesh; }

        if (_mat == null) { _mat = Resources.Load<Material>("VertexSkinning"); }

        int vertCount = _mesh.vertexCount;
        var inSkins = _mesh.boneWeights.Select(x => new SVertInSkin()
        {
            weight0 = x.weight0,
            weight1 = x.weight1,
            weight2 = x.weight2,
            weight3 = x.weight3,
            index0 = x.boneIndex0,
            index1 = x.boneIndex1,
            index2 = x.boneIndex2,
            index3 = x.boneIndex3,
        }).ToArray();

        // Push these computebuffer to vertex shader
        var sourceSkin = new ComputeBuffer(vertCount, Marshal.SizeOf(typeof(SVertInSkin)));
        sourceSkin.SetData(inSkins);
        _boneTransforms = skin.bones;
        _bones = new ComputeBuffer(_boneTransforms.Length, Marshal.SizeOf(typeof(Matrix4x4)));
        _mat.SetBuffer("_Skin", sourceSkin);
        _mat.SetBuffer("_Bones", _bones);

        // setup job parameters
        _boneMatrices = new NativeArray<Matrix4x4>(_boneTransforms.Length, Allocator.TempJob);
        _localToWorldMatrix = new NativeArray<Matrix4x4>(_boneTransforms.Length, Allocator.TempJob);
        _bindposes = new NativeArray<Matrix4x4>(_mesh.bindposeCount, Allocator.TempJob);
        _bindposeMatrices = _mesh.bindposes;

        // Replace SkinnedMeshRenderer with MeshRenderer
        var obj = new GameObject("AvatarRenderer");
        var meshFilter = obj.AddComponent<MeshFilter>();
        var meshRenderer = obj.AddComponent<MeshRenderer>();
        meshFilter.mesh = skin.sharedMesh;
        meshRenderer.material = _mat;
        obj.transform.SetParent(transform, false);
        Destroy(skin.gameObject);
    }

}
