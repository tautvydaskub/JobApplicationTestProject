using NUnit.Framework;
using System;
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

public class AssetDatabaseTestScript
{

    [Test]
    [TestCase("Assets/TestAssets/TestPNG.png", "")]
    [TestCase(null, "Cannot move asset.  is not a valid path.")]
    [TestCase("", "Cannot move asset.  is not a valid path.")]
    public void RenameAssetTest(string assetPath, string output)
    {
        var oldName = Path.GetFileName(assetPath);
        var result = AssetDatabase.RenameAsset(assetPath, "hello");

        Assert.AreEqual(output, result);

        if (result == "")
        {
            var assets = AssetDatabase.FindAssets("hello", new[] { "Assets/TestAssets" });
            AssetDatabase.GUIDToAssetPath(assets[0]);
            AssetDatabase.RenameAsset(AssetDatabase.GUIDToAssetPath(assets[0]), oldName);
        }
    }

    private static IEnumerable TestCases()
    {
        yield return new TestCase { asset = new Material(Shader.Find("Transparent/Diffuse")), path = "Assets/TestAssets/testMaterial.mat" }; 
        yield return new TestCase { asset = new Cubemap(1, TextureFormat.ARGB32, false), path = "Assets/TestAssets/newCubemap.cubemap" };
        yield return new TestCase { asset = new AnimationClip(), path = "Assets/TestAssets/newAnimation.anim" };
        yield return new TestCase { asset = ScriptableObject.CreateInstance("TestScriptableObjectClass"), path = "Assets/TestAssets/newScriptableObject.asset" };

    }
    public struct TestCase
    {
        public UnityEngine.Object asset;
        public string path;
    }

    [Test]
    public void CreateAssetTestWithValidInput([ValueSource(nameof(TestCases))] TestCase testCase)
    {
        AssetDatabase.CreateAsset(testCase.asset, testCase.path);
        var newObject = AssetDatabase.FindAssets(testCase.path);
        Assert.True(newObject != null);

        AssetDatabase.DeleteAsset(testCase.path);
    }

    [Test]
    public void CreateAssetTestWithNullAsset()
    {
        Assert.Throws<ArgumentNullException>(() => AssetDatabase.CreateAsset(null, "Assets/TestAssets/testMaterial.mat"));
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    public void CreateAssetTestWithInvalidPath(string path)
    {
        LogAssert.Expect(LogType.Error, "Couldn't create asset file!");
        Assert.Throws<UnityException>(() => AssetDatabase.CreateAsset(new AnimationClip(), path));
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("Assets/TestAssets/AssetToDelete.png")]

    public void DeleteAssetTest(string path)
    {
        AssetDatabase.DeleteAsset(path);
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    [TestCase("Assets/TestAssets/AssetToDelete.png")]
    public void FindAssetsTest(string path)
    {

    }

    [Test]
    [TestCase(null, null)]
    [TestCase("", null)]
    [TestCase("Assets/TestAssets/TestPNG.png", "Assets/TestAssets/CopiedAsset.png")]
    [TestCase("Assets/TestAssets/TestPNG.png", "Assets/TestAssets/TestPNG.png")]
    public void CopyAssetTest(string assetPath, string duplicatePath)
    {
        Assert.IsTrue(AssetDatabase.CopyAsset(assetPath, duplicatePath));

        AssetDatabase.DeleteAsset(duplicatePath);
    }
}
