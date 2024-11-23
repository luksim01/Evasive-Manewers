using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class MoveBoundaryTests
{
    List<GameObject> tearDownList = new List<GameObject>();

    [SetUp]
    public void SetUp()
    {
        GameObject forestTrailPrefab = Resources.Load<GameObject>("Prefabs/Alpha/Forest Trail");
        Material backgroundMaterial = Resources.Load<Material>("Materials/Background");

        if (forestTrailPrefab == null || backgroundMaterial == null)
        {
            Assert.Fail("forestTrailPrefab or backgroundMaterial couldn't be located and assigned");
        }
        else
        {
            GameObject forestBoundaryLeft = Object.Instantiate(forestTrailPrefab, new Vector3(-9.3f, 0, 0), forestTrailPrefab.transform.rotation);
            forestBoundaryLeft.name = "Forest Boundary Left";
            forestBoundaryLeft.GetComponent<MeshRenderer>().material = backgroundMaterial;
            tearDownList.Add(forestBoundaryLeft);

            GameObject forestTrailL1 = Object.Instantiate(forestTrailPrefab, new Vector3(-6.2f, 0, 0), forestTrailPrefab.transform.rotation);
            forestTrailL1.name = "Forest Trail L1";
            tearDownList.Add(forestTrailL1);

            GameObject forestTrailL2 = Object.Instantiate(forestTrailPrefab, new Vector3(-3.1f, 0, 0), forestTrailPrefab.transform.rotation);
            forestTrailL2.name = "Forest Trail L2";
            tearDownList.Add(forestTrailL2);

            GameObject forestTrailL3 = Object.Instantiate(forestTrailPrefab, new Vector3(0, 0, 0), forestTrailPrefab.transform.rotation);
            forestTrailL3.name = "Forest Trail L3";
            tearDownList.Add(forestTrailL3);

            GameObject forestTrailL4 = Object.Instantiate(forestTrailPrefab, new Vector3(3.1f, 0, 0), forestTrailPrefab.transform.rotation);
            forestTrailL4.name = "Forest Trail L4";
            tearDownList.Add(forestTrailL4);

            GameObject forestTrailL5 = Object.Instantiate(forestTrailPrefab, new Vector3(6.2f, 0, 0), forestTrailPrefab.transform.rotation);
            forestTrailL5.name = "Forest Trail L5";
            tearDownList.Add(forestTrailL5);

            GameObject forestBoundaryRight = Object.Instantiate(forestTrailPrefab, new Vector3(9.3f, 0, 0), forestTrailPrefab.transform.rotation);
            forestBoundaryRight.name = "Forest Boundary Right";
            forestBoundaryRight.GetComponent<MeshRenderer>().material = backgroundMaterial;
            tearDownList.Add(forestBoundaryRight);
        }

        GameObject mainCameraPrefab = Resources.Load<GameObject>("Prefabs/TDD/MainCamera");
        if (mainCameraPrefab == null)
        {
            Assert.Fail("mainCameraPrefab couldn't be located and assigned");
        }
        else
        {
            GameObject mainCamera = Object.Instantiate(mainCameraPrefab, mainCameraPrefab.transform.position, mainCameraPrefab.transform.rotation);
            mainCamera.name = "MainCamera";
            mainCamera.GetComponent<AudioSource>().enabled = false;
            CameraController cameraController = mainCamera.GetComponent<CameraController>();

            MockUIManager mockUIManager = new MockUIManager();
            mockUIManager.IsGameActive = true;

            cameraController.SetDependencies(mockUIManager);
            tearDownList.Add(mainCamera);
        }

        GameObject directionalLightPrefab = Resources.Load<GameObject>("Prefabs/TDD/Directional Light");
        if (directionalLightPrefab == null)
        {
            Assert.Fail("directionalLightPrefab couldn't be located and assigned");
        }
        else
        {
            GameObject directionalLight = Object.Instantiate(directionalLightPrefab, directionalLightPrefab.transform.position, directionalLightPrefab.transform.rotation);
            directionalLight.name = "Directional Light";
            directionalLight.GetComponent<Light>().color = Color.red;
            tearDownList.Add(directionalLight);
        }
    }

    // requirement: test boundaries of player gameobject (sheepdog)
    [UnityTest]
    [TestCase(1f, 1f, 1f, 7f, 15f, ExpectedResult = null)]
    [TestCase(1f, 1f, -1f, -7f, -15f, ExpectedResult = null)]
    public IEnumerator PlayerControllerMoveBoundaryTest(float x, float z, float direction, float xExpected, float zExpected)
    {
        // Arrange
        GameObject sheepdogPrefab = Resources.Load<GameObject>("Prefabs/Final/Sheepdog");
        if (sheepdogPrefab == null)
        {
            Assert.Fail("sheepdogPrefab couldn't be located and assigned");
        }
        
        GameObject sheepdog = Object.Instantiate(sheepdogPrefab, sheepdogPrefab.transform.position, sheepdogPrefab.transform.rotation);
        sheepdog.name = "Sheepdog";
        tearDownList.Add(sheepdog);
        PlayerController playerController = sheepdog.GetComponent<PlayerController>();

        // mock up of dependancies
        MockAudioManager mockAudioManager = new MockAudioManager();
        MockUIManager mockUIManager = new MockUIManager();
        mockUIManager.IsGameActive = true;
        MockSpawnManager mockSpawnManager = new MockSpawnManager();

        playerController.SetDependencies(mockAudioManager, mockUIManager, mockSpawnManager);

        // Act
        for (int frame = 0; frame < 1000; frame++)
        {
            playerController.Move(new Vector3(x, 0, z), direction, 100f);
            yield return null;
        }

        Debug.Log($"(PlayerControllerMoveBoundaryTest) sheepdog position at end of test: x = {sheepdog.transform.position.x}, z = {sheepdog.transform.position.z}");

        // Assert
        Assert.That(sheepdog.transform.position.x, Is.EqualTo(xExpected));
        Assert.That(sheepdog.transform.position.z, Is.EqualTo(zExpected));
    }

    [TearDown]
    public void TearDown()
    {
        foreach (GameObject gameObject in new List<GameObject>(tearDownList))
        {
            if (gameObject != null)
            {
                Object.DestroyImmediate(gameObject);
            }
        }
        tearDownList.Clear();
        Resources.UnloadUnusedAssets();
    }
}
