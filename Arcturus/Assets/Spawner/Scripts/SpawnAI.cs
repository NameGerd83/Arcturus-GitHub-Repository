///////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Filename: SpawnAI.cs
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
using CorruptedSmileStudio.Spawner;

/// <summary>
/// SpawnAI component. Provides the base methods required to interact with the spawner.
/// </summary>
/// <description>
/// This component can be used out of the box on existing GameObjects that get spawned.<br />
/// This class interacts with the Spawner and handles all interaction between the two. It also makes
/// use of InstanceManager in order to work with Pool Manager by Path-o-Logical (if it is installed).<br />
/// It is highly suggested to add this component to GameObjects you wish to spawn, simply interact with this class
/// in your killing method and call Remove() on this component last in the method, you will not need to call Destroy(gameObject) in order to destroy the GameObject as this class handles all of that.
/// </description>
public class SpawnAI : MonoBehaviour, ISpawnable
{
    /// <summary>
    /// This is the tag that has been applied to all Spawner objects.
    /// This is used when "killing" off units in order to find all Spawner objects to send the kill message to.
    /// The Spawners then take over from there.
    /// </summary>
    public string spawnerTag = "Spawner";
    /// <summary>
    /// The ID of the spawner that spawned this unit.
    /// </summary>
    private int spawnID = -1;

    public void Remove()
    {
        if (spawnID != -1)
        {
            // Gets all Spawner GameObjects and sends a KillUnit message to all of them
            // The one that has the matched ID will remove it from the list.
            GameObject[] objects = GameObject.FindGameObjectsWithTag(spawnerTag);
            foreach (GameObject obj in objects)
            {
                obj.SendMessage("KillUnit", spawnID);
            }
            spawnID = -1;
            InstanceManager.Despawn(transform);
        }
    }

    public void SetID(int ID)
    {
        spawnID = ID;
    }
}