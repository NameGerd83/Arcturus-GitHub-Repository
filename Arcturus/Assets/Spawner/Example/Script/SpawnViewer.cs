///////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Filename: SpawnViewer.cs
//  
// Author: Garth de Wet <garthofhearts@gmail.com>
// Website: http://corruptedsmilestudio.blogspot.com/
// Date Modified: 04 Feb 2012
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
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

/// <summary>
/// A simple script allowing the changing between the different spawn modes. Via a GUI
/// useful to see how the system works.
/// </summary>
[RequireComponent(typeof(Spawner))]
public class SpawnViewer : MonoBehaviour
{
    private Spawner spawn;

    void Start()
    {
        spawn = gameObject.GetComponent<Spawner>();
    }

    void OnGUI()
    {
        GUILayout.Label("Current unit level: " + spawn.unitLevel.ToString());
        GUILayout.Label("Current mode: " + spawn.spawnType.ToString());
        if (spawn.spawnType == CorruptedSmileStudio.Spawner.SpawnModes.TimeSplitWave || spawn.spawnType == CorruptedSmileStudio.Spawner.SpawnModes.TimedWave && spawn.TimeTillWave != 0.0f)
            GUILayout.Label("Time till next wave: " + spawn.TimeTillWave.ToString("F1"));
        if (GUILayout.Button("Change Mode: Normal"))
        {
            spawn.spawnType = CorruptedSmileStudio.Spawner.SpawnModes.Normal;
            spawn.unitLevel = CorruptedSmileStudio.Spawner.UnitLevels.Easy;
            spawn.Reset();
            spawn.StartSpawn();
        }
        if (GUILayout.Button("Change Mode: Once"))
        {
            spawn.spawnType = CorruptedSmileStudio.Spawner.SpawnModes.Once;
            spawn.unitLevel = CorruptedSmileStudio.Spawner.UnitLevels.Medium;
            spawn.Reset();
            spawn.StartSpawn();
        }
        if (GUILayout.Button("Change Mode: Wave"))
        {
            spawn.spawnType = CorruptedSmileStudio.Spawner.SpawnModes.Wave;
            spawn.unitLevel = CorruptedSmileStudio.Spawner.UnitLevels.Hard;
            spawn.Reset();
            spawn.StartSpawn();
        }
        if (GUILayout.Button("Change Mode: TimedWave"))
        {
            spawn.spawnType = CorruptedSmileStudio.Spawner.SpawnModes.TimedWave;
            spawn.unitLevel = CorruptedSmileStudio.Spawner.UnitLevels.Boss;
            spawn.Reset();
            spawn.StartSpawn();
        }
        if (GUILayout.Button("Change Mode: TimeSplitWave"))
        {
            spawn.spawnType = CorruptedSmileStudio.Spawner.SpawnModes.TimeSplitWave;
            spawn.unitLevel = CorruptedSmileStudio.Spawner.UnitLevels.Easy;
            spawn.Reset();
            spawn.StartSpawn();
        }
    }
}