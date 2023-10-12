using System;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.Direct3D12;

namespace DX12GameProgramming
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct ObjectConstants
    {
        public Matrix World;
        public Matrix TexTransform;
        public int MaterialIndex;
        public int ObjPad0;
        public int ObjPad1;
        public int ObjPad2;

        public static ObjectConstants Default => new ObjectConstants
        {
            World = Matrix.Identity,
            TexTransform = Matrix.Identity
        };
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct SkinnedConstants
    {
        private const int BoneTransformsCount = 96;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BoneTransformsCount)]
        public Matrix[] BoneTransforms;

        public static SkinnedConstants Default => new SkinnedConstants
        {
            BoneTransforms = new Matrix[BoneTransformsCount]
        };
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct PassConstants
    {
        public Matrix View;
        public Matrix InvView;
        public Matrix Proj;
        public Matrix InvProj;
        public Matrix ViewProj;
        public Matrix InvViewProj;
        public Matrix ViewProjTex;
        public Matrix ShadowTransform;
        public Vector3 EyePosW;
        public float PerObjectPad1;
        public Vector2 RenderTargetSize;
        public Vector2 InvRenderTargetSize;
        public float NearZ;
        public float FarZ;
        public float TotalTime;
        public float DeltaTime;

        public Vector4 AmbientLight;

        // Indices [0, NUM_DIR_LIGHTS) are directional lights;
        // indices [NUM_DIR_LIGHTS, NUM_DIR_LIGHTS+NUM_POINT_LIGHTS) are point lights;
        // indices [NUM_DIR_LIGHTS+NUM_POINT_LIGHTS, NUM_DIR_LIGHTS+NUM_POINT_LIGHT+NUM_SPOT_LIGHTS)
        // are spot lights for a maximum of MaxLights per object.
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Light.MaxLights)]
        public Light[] Lights;

        public static PassConstants Default => new PassConstants
        {
            View = Matrix.Identity,
            InvView = Matrix.Identity,
            Proj = Matrix.Identity,
            InvProj = Matrix.Identity,
            ViewProj = Matrix.Identity,
            InvViewProj = Matrix.Identity,
            ViewProjTex = Matrix.Identity,
            ShadowTransform = Matrix.Identity,
            AmbientLight = Vector4.UnitW,
            Lights = Light.DefaultArray
        };
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct SsaoConstants
    {
        private const int OffsetVectorCount = 14;
        private const int BlurWeightCount = 3;

        public Matrix Proj;
        public Matrix InvProj;
        public Matrix ProjTex;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = OffsetVectorCount)]
        public Vector4[] OffsetVectors;

        // For SsaoBlur.hlsl
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = BlurWeightCount)]
        public Vector4[] BlurWeights;

        public Vector2 InvRenderTargetSize;

        // Coordinates given in view space.
        public float OcclusionRadius;
        public float OcclusionFadeStart;
        public float OcclusionFadeEnd;
        public float SurfaceEpsilon;

        public static SsaoConstants Default => new SsaoConstants
        {
            OcclusionRadius = 0.5f,
            OcclusionFadeStart = 0.2f,
            OcclusionFadeEnd = 2.0f,
            SurfaceEpsilon = 0.05f,
            OffsetVectors = new Vector4[OffsetVectorCount],
            BlurWeights = new Vector4[BlurWeightCount]
        };
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct MaterialData
    {
        public Vector4 DiffuseAlbedo;
        public Vector3 FresnelR0;
        public float Roughness;

        // Used in texture mapping.
        public Matrix MatTransform;

        public int DiffuseMapIndex;
        public int NormalMapIndex;
        public int MaterialPad0;
        public int MaterialPad1;

        public static MaterialData Default => new MaterialData
        {
            DiffuseAlbedo = Vector4.One,
            FresnelR0 = new Vector3(0.01f),
            Roughness = 0.5f,
            MatTransform = Matrix.Identity
        };
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct Vertex
    {
        public Vector3 Pos;
        public Vector3 Normal;
        public Vector2 TexC;
        public Vector3 TangentU;
    }

    internal class FrameResource : IDisposable
    {
        public FrameResource(Device device, int passCount, int objectCount, int skinnedObjectCount, int materialCount)
        {
            CmdListAlloc = device.CreateCommandAllocator(CommandListType.Direct);

            PassCB = new UploadBuffer<PassConstants>(device, passCount, true);
            SsaoCB = new UploadBuffer<SsaoConstants>(device, 1, true);
            MaterialBuffer = new UploadBuffer<MaterialData>(device, materialCount, false);
            ObjectCB = new UploadBuffer<ObjectConstants>(device, objectCount, true);
            SkinnedCB = new UploadBuffer<SkinnedConstants>(device, skinnedObjectCount, true);
        }

        // We cannot reset the allocator until the GPU is done processing the commands.
        // So each frame needs their own allocator.
        public CommandAllocator CmdListAlloc { get; }

        // We cannot update a cbuffer until the GPU is done processing the commands
        // that reference it. So each frame needs their own cbuffers.
        public UploadBuffer<PassConstants> PassCB { get; }
        public UploadBuffer<ObjectConstants> ObjectCB { get; }
        public UploadBuffer<SkinnedConstants> SkinnedCB { get; }
        public UploadBuffer<SsaoConstants> SsaoCB { get; }
        public UploadBuffer<MaterialData> MaterialBuffer { get; }

        // Fence value to mark commands up to this fence point.  This lets us
        // check if these frame resources are still in use by the GPU.
        public long Fence { get; set; }

        public void Dispose()
        {
            MaterialBuffer.Dispose();
            SsaoCB.Dispose();
            SkinnedCB.Dispose();
            ObjectCB.Dispose();
            PassCB.Dispose();
            CmdListAlloc.Dispose();
        }
    }
}
