using System;
using UnhollowerBaseLib;
using UnhollowerBaseLib.Attributes;
using UnhollowerRuntimeLib;

public class AssetBundle : UnityEngine.Object
{
    static AssetBundle()
    {
        ClassInjector.RegisterTypeInIl2Cpp<AssetBundle>();
    }

    public AssetBundle(IntPtr ptr) : base(ptr) { }

    // LoadFromFile

    internal delegate IntPtr d_LoadFromFile(IntPtr path, uint crc, ulong offset);
    internal static d_LoadFromFile icall_LoadFromFile = IL2CPP.ResolveICall<d_LoadFromFile>("UnityEngine.AssetBundle::LoadFromFile_Internal");

    [HideFromIl2Cpp]
    public static AssetBundle LoadFromFile(string path)
    {
        IntPtr ptr = icall_LoadFromFile(IL2CPP.ManagedStringToIl2Cpp(path), 0, 0);
        return ptr != IntPtr.Zero ? new AssetBundle(ptr) : null;
    }

    // LoadAllAssets()

    internal delegate IntPtr d_LoadAssetWithSubAssets_Internal(IntPtr _this, IntPtr name, IntPtr type);
    internal d_LoadAssetWithSubAssets_Internal icall_LoadAssetsWithSubAssets_Internal =
        IL2CPP.ResolveICall<d_LoadAssetWithSubAssets_Internal>("UnityEngine.AssetBundle::LoadAssetWithSubAssets_Internal");

    [HideFromIl2Cpp]
    public UnityEngine.Object[] LoadAllAssets()
    {

        IntPtr ptr = icall_LoadAssetsWithSubAssets_Internal(
            this.Pointer,
            IL2CPP.ManagedStringToIl2Cpp(""),
            UnhollowerRuntimeLib.Il2CppType.Of<UnityEngine.Object>().Pointer);

        return ptr != IntPtr.Zero
            ? new Il2CppReferenceArray<UnityEngine.Object>(ptr)
            : Array.Empty<UnityEngine.Object>();
    }

    // LoadAsset<T>

    internal delegate IntPtr d_LoadAsset_Internal(IntPtr _this, IntPtr name, IntPtr type);
    internal static d_LoadAsset_Internal icall_LoadAsset_Internal =
        IL2CPP.ResolveICall<d_LoadAsset_Internal>("UnityEngine.AssetBundle::LoadAsset_Internal");

    [HideFromIl2Cpp]
    public T LoadAsset<T>(string name) where T : UnityEngine.Object
    {
        IntPtr ptr = icall_LoadAsset_Internal(
            this.Pointer,
            IL2CPP.ManagedStringToIl2Cpp(name),
            UnhollowerRuntimeLib.Il2CppType.Of<T>().Pointer);

        return ptr != IntPtr.Zero ? new UnityEngine.Object(ptr).TryCast<T>() : null;
    }
}
