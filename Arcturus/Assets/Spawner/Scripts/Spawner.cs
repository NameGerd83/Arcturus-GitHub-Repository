///////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Filename: Spawner.cs
//  
// Author: Garth de Wet <garthofhearts@gmail.com>
// Website: http://corruptedsmilestudio.blogspot.com/
// Date Modified: 09 Feb 2012
//
// Copyright (c) 2012 Garth de Wet
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
///////////////////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;
using CorruptedSmileStudio.Spawner;

/// <summary>
/// Spawns prefabs, either in waves, at once or continually till all enemies are spawned.
/// </summary>
/// <description>
/// Controls the spawning of selected perfabs, useful for making enemy spawn points.<br />
/// It supports a variety of spawn modes, which allows you to bend the system to fit your needs.<br />
/// This class is required for the system to work, you will need to place this class on a GameObject with
/// a tag of Spawner (However, this is changeable within the SpawnAI class).
/// </description>
public class Spawner : MonoBehaviour
{
    #region Variables
    /// <summary>
    /// The unit level to spawn.
    /// </summary>
    public UnitLevels unitLevel = UnitLevels.Easy;

    /// <summary>
    /// The list of enemies, do not go over 4.
    /// </summary>
    public GameObject[] unitList = new GameObject[4];

    /// <summary>
    /// The total number of enemies to spawn on each spawn session.
    /// </summary>
    public int totalUnits = 10;
    /// <summary>
    /// The current number of spawned enemies.
    /// </summary>
    private int numberOfUnits = 0;
    /// <summary>
    /// The total number of spawned enemies. Not just alive.
    /// </summary>
    private int totalSpawnedUnits = 0;

    /// <summary>
    /// The ID of the spawner.
    /// </summary>
    public int spawnID = 0;

    /// <summary>
    /// Used to determine if there is actual spawning to occur.
    /// </summary>
    private bool waveSpawn = true;
    /// <summary>
    /// Determines whether the spawn should spawn.
    /// </summary>
    public bool spawn = true;
    /// <summary>
    /// The type of spawning.
    /// </summary>
    public SpawnModes spawnType = SpawnModes.Normal;
    /// <summary>
    /// The time between each wave when spawn type is set to wave.
    /// </summary>
    public float waveTimer = 30.0f;
    /// <summary>
    /// The time the last wave was spawned
    /// </summary>
    private float lastWaveSpawnTime = 0.0f;
    /// <summary>
    /// The total number of waves to spawn.
    /// </summary>
    public int totalWaves = 5;
    /// <summary>
    /// The number of waves that has spawned.
    /// </summary>
    private int numWaves = 0;

    /// <summary>
    /// Used within the TimeSplitWave.
    /// </summary>
    private bool checkEnemyLevel = false;
    /// <summary>
    /// The time between each spawn of a unit
    /// </summary>
    public float timeBetweenSpawns = 0.5f;
    /// <summary>
    /// The location of where to spawn units.
    /// </summary>
    public Transform spawnLocation;
    #endregion

    void Start()
    {
        if (spawnLocation == null)
        {
            spawnLocation = transform;
        }
        InstanceManager.ReadyPreSpawn(unitList[(int)unitLevel].transform, totalUnits);
        StartCoroutine("DoSpawn");
    }
    /// <summary>
    /// Spawns a unit based on the level set.
    /// </summary>
    private void SpawnUnit()
    {
        if (unitList[(int)unitLevel] != null)
        {
            Transform unit = InstanceManager.Spawn(unitList[(int)unitLevel].transform, spawnLocation.position, Quaternion.identity);
            unit.SendMessage("SetID", spawnID);
            // Increase the total number of enemies spawned and the number of spawned enemies
            numberOfUnits++;
            totalSpawnedUnits++;
        }
        else
        {
            Debug.LogError("Error trying to spawn unit of level " + unitLevel.ToString() + " on spawner " + spawnID + " - No unit set");
            spawn = false;
        }
    }
    /// <summary>
    /// This removes an "unit" in order to allow waves and such that depend on the number of enemies decreasing
    /// to properly start a new spawn.
    /// </summary>
    /// <param name="sID">The spawner ID that created the unit.</param>
    public void KillUnit(int sID)
    {
        // if the unit's spawnID is equal to this spawnersID then remove an unit count
        if (spawnID == sID)
        {
            numberOfUnits--;
        }
    }
    /// <summary>
    /// Enable the spawner by ID
    /// </summary>
    /// <param name="sID">The spawner's ID</param>
    public void EnableSpawner(int sID)
    {
        if (spawnID == sID)
        {
            spawn = true;
            InstanceManager.ReadyPreSpawn(unitList[(int)unitLevel].transform, totalUnits);
            StartCoroutine("DoSpawn");
        }
    }
    /// <summary>
    /// Disable the spawner by ID
    /// </summary>
    /// <param name="sID">The spawner's ID</param>
    public void DisableSpawner(int sID)
    {
        if (spawnID == sID)
        {
            spawn = false;
            StopCoroutine("DoSpawn");
        }
    }
    /// <summary>
    /// The time till the next wave
    /// </summary>
    public float TimeTillWave
    {
        get
        {
            if (numWaves >= totalWaves)
            {
                return 0;
            }
            if (spawnType == SpawnModes.TimeSplitWave && waveSpawn || numberOfUnits > 0)
            {
                return 0;
            }

            float time = (lastWaveSpawnTime + waveTimer) - Time.time;
            if (time >= 0)
                return time;
            else
                return 0;
        }
    }
    /// <summary>
    /// Enables the spawner. Useful for triggers.
    /// </summary>
    public void EnableTrigger()
    {
        spawn = true;
        StartCoroutine("DoSpawn");
    }
    /// <summary>
    /// Draws an icon to show where the spawn point is. Useful if you don't have a object that show the spawn point
    /// </summary>
    void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "SpawnIcon.psd");
    }
    /// <summary>
    /// Resets all the private variables to their original state.
    /// </summary>
    public void Reset()
    {
        waveSpawn = false;
        checkEnemyLevel = true;
        numWaves = 0;
        totalSpawnedUnits = 0;
        lastWaveSpawnTime = Time.time;
    }
    /// <summary>
    /// Returns the number of waves left
    /// </summary>
    public int WavesLeft
    {
        get
        {
            return totalWaves - numWaves;
        }
    }
    /// <summary>
    /// Controls the spawning of units and so forth.
    /// </summary>
    /// <returns></returns>
    private System.Collections.IEnumerator DoSpawn()
    {
        while (spawn)
        {
            switch (spawnType)
            {
                case SpawnModes.Normal:
                    if (numberOfUnits < totalUnits)
                    {
                        yield return new WaitForSeconds(timeBetweenSpawns);
                        SpawnUnit();
                    }
                    break;
                case SpawnModes.Once:
                    if (totalSpawnedUnits >= totalUnits)
                    {
                        spawn = false;
                    }
                    else
                    {
                        yield return new WaitForSeconds(timeBetweenSpawns);
                        SpawnUnit();
                    }
                    break;
                case SpawnModes.Wave:
                    if (numWaves <= totalWaves)
                    {
                        if (waveSpawn)
                        {
                            yield return new WaitForSeconds(timeBetweenSpawns);
                            SpawnUnit();
                            checkEnemyLevel = true;

                            if ((totalSpawnedUnits / (numWaves + 1)) == totalUnits)
                            {
                                waveSpawn = false;
                            }
                        }
                        if (checkEnemyLevel)
                        {
                            if (numberOfUnits <= 0)
                            {
                                checkEnemyLevel = false;
                                waveSpawn = true;
                                numberOfUnits = 0;
                                numWaves++;
                            }
                        }
                    }
                    else
                    {
                        spawn = false;
                    }
                    break;
                case SpawnModes.TimedWave:
                    if (numWaves <= totalWaves)
                    {
                        if (waveSpawn)
                        {
                            yield return new WaitForSeconds(timeBetweenSpawns);
                            SpawnUnit();

                            if ((totalSpawnedUnits / (numWaves + 1)) == totalUnits)
                            {
                                waveSpawn = false;
                            }
                        }
                        else
                        {
                            waveSpawn = true;
                            numWaves++;
                            // A hack to spawn even if there are unit left alive.
                            numberOfUnits = 0;
                            lastWaveSpawnTime = Time.time;
                            yield return new WaitForSeconds(waveTimer);
                        }
                    }
                    else
                    {
                        spawn = false;
                    }
                    break;
                case SpawnModes.TimeSplitWave:
                    if (numWaves <= totalWaves)
                    {
                        if (waveSpawn)
                        {
                            yield return new WaitForSeconds(timeBetweenSpawns);
                            SpawnUnit();
                            if ((totalSpawnedUnits / (numWaves + 1)) == totalUnits)
                            {
                                waveSpawn = false;
                                checkEnemyLevel = true;
                            }
                        }
                        else
                        {
                            if (checkEnemyLevel)
                            {
                                if (numberOfUnits <= 0)
                                {
                                    numWaves++;
                                    checkEnemyLevel = false;
                                    numberOfUnits = 0;
                                    lastWaveSpawnTime = Time.time;
                                    yield return new WaitForSeconds(waveTimer);
                                    waveSpawn = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        spawn = false;
                    }
                    break;
                default:
                    spawn = false;
                    break;
            }
            yield return new WaitForEndOfFrame();
        }
    }
    /// <summary>
    /// Starts spawning, if you want to interact with a spawner from code.
    /// </summary>
    public void StartSpawn()
    {
        EnableTrigger();
    }
}