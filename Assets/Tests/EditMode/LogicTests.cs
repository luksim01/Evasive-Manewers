using NUnit.Framework;
using UnityEngine;

public class LogicTests
{
    [Test]
    // using the spawn interval defined in SpawnManager, the test checks that an increment returns false to
    // prevent stray sheep spawning logic from occurring. 
    public void StraySheepSpawnNonTriggerTest()
    {
        // Arrange
        GameObject spawnManager = new();
        spawnManager.AddComponent<SpawnManager>();

        int targetSeconds = spawnManager.GetComponent<SpawnManager>().spawnInterval;
        spawnManager.GetComponent<SpawnManager>().timeSinceLostSheep = 0;
        bool expectedStatus = false;

        // Act
        bool spawnStatus = spawnManager.GetComponent<SpawnManager>().CheckTimeSinceLostSheep(targetSeconds);

        // Assert
        Assert.That(spawnStatus, Is.EqualTo(expectedStatus));
    }

    [Test]
    // using the spawn interval defined in SpawnManager, the test checks that the target time achieved returns true to
    // enable stray sheep spawning logic.
    public void StraySheepSpawnTriggerTest()
    {
        // Arrange
        GameObject spawnManager = new();
        spawnManager.AddComponent<SpawnManager>();

        int targetSeconds = spawnManager.GetComponent<SpawnManager>().spawnInterval;
        spawnManager.GetComponent<SpawnManager>().timeSinceLostSheep = targetSeconds - 1;
        bool expectedStatus = true;
        
        // Act
        bool spawnStatus = spawnManager.GetComponent<SpawnManager>().CheckTimeSinceLostSheep(targetSeconds);

        // Assert
        Assert.That(spawnStatus, Is.EqualTo(expectedStatus));
    }
}
