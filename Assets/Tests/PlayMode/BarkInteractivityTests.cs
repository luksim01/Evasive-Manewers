using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BarkInteractivityTests
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

    // test: testing functionality of bark interactivity ring (BIR) and interactive scenarios
    // req #1: herd sheep is outlined when within BIR
    // req #2: wolf that is hunting sheep is outlined when within the BIR
    // req #3: stray sheep is outlined when within BIR
    // req #4: wolf that is hunting sheep within BIR will flee BIR when bark is triggered
    // req #5: herd sheep within BIR will move forward when bark is triggered
    // req #6: stray sheep within BIR will become herd sheep when bark is triggered
    // req #7: herd sheep that is being hunted is unaffected by BIR
    // req #8: wolf that is hunting dog is unaffected by BIR


    [UnityTest]
    [TestCase("Sheep", "Sheep", 4f, "Sheep", true, ExpectedResult = null)]
    [TestCase("Wolf", "WolfHuntingSheep", 4f, "WolfHuntingSheep", true, ExpectedResult = null)]
    [TestCase("Sheep", "Stray", 4f, "Sheep", true, ExpectedResult = null)]
    [TestCase("Sheep", "Hunted", 4f, "Hunted", false, ExpectedResult = null)]
    [TestCase("Wolf", "WolfHuntingDog", 4f, "WolfHuntingDog", false, ExpectedResult = null)]
    public IEnumerator PlayerControllerBarkInteractivityRingTest(string characterName, string characterTag, float expectedZ, string expectedCharacterTag, bool expectedInteractivityStatus)
    {
        // Arrange
        Vector3 characterSpawnPosition = new Vector3(4f, 0, -8);

        // mock up of dependancies
        MockAudioManager mockAudioManager = new MockAudioManager();
        MockUIManager mockUIManager = new MockUIManager();
        mockUIManager.IsGameActive = true;
        MockSpawnManager mockSpawnManager = new MockSpawnManager();

        // dog
        GameObject sheepdogPrefab = Resources.Load<GameObject>("Prefabs/Final/Sheepdog");
        if (sheepdogPrefab == null)
        {
            Assert.Fail("sheepdogPrefab couldn't be located and assigned");
        }

        GameObject sheepdog = Object.Instantiate(sheepdogPrefab, sheepdogPrefab.transform.position, sheepdogPrefab.transform.rotation);
        sheepdog.name = "Sheepdog";
        tearDownList.Add(sheepdog);
        PlayerController playerController = sheepdog.GetComponent<PlayerController>();

        mockSpawnManager.StraySheepSpawnPosition = characterSpawnPosition;
        mockSpawnManager.StraySheepTargetPosition = sheepdog.transform.position;

        playerController.SetDependencies(mockAudioManager, mockUIManager, mockSpawnManager);

        GameObject barkInteractionIndicator = playerController.CreateBarkInteractionIndicator();
        tearDownList.Add(barkInteractionIndicator);

        // test sheep
        GameObject sheepPrefab = Resources.Load<GameObject>("Prefabs/Final/Sheep");
        if (sheepPrefab == null)
        {
            Assert.Fail("sheepPrefab couldn't be located and assigned");
        }

        GameObject testSheep = Object.Instantiate(sheepPrefab, new Vector3(characterSpawnPosition.x, 5, characterSpawnPosition.z), sheepPrefab.transform.rotation);
        testSheep.name = "TestSheep";
        testSheep.tag = "Sheep";
        tearDownList.Add(testSheep);
        Rigidbody testSheepRigidbody = testSheep.GetComponent<Rigidbody>();
        testSheepRigidbody.isKinematic = true;
        SheepController testSheepController = testSheep.GetComponent<SheepController>();
        testSheepController.SetDependencies(mockAudioManager, mockUIManager, mockSpawnManager, playerController);

        mockSpawnManager.Herd = GameObject.FindGameObjectsWithTag("Sheep");

        // character
        GameObject characterPrefab = Resources.Load<GameObject>("Prefabs/Final/" + characterName);
        if (characterPrefab == null)
        {
            Assert.Fail($"characterPrefab ({characterName}) couldn't be located and assigned");
        }

        GameObject characterObject = Object.Instantiate(characterPrefab, characterSpawnPosition, characterPrefab.transform.rotation);
        characterObject.name = characterName;
        characterObject.tag = characterTag;
        tearDownList.Add(characterObject);

        BaseController characterController = characterObject.GetComponent<BaseController>();
        characterController.SetDependencies(mockAudioManager, mockUIManager, mockSpawnManager, playerController);

        bool isInteractive = false;

        // Act
        for (int frame = 0; frame < 250; frame++)
        {
            isInteractive = playerController.CastRadius(barkInteractionIndicator);
            
            yield return null;
        }

        // Assert
        Assert.That(characterObject.tag, Is.EqualTo(expectedCharacterTag));
        Assert.That(isInteractive, Is.EqualTo(expectedInteractivityStatus));
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
