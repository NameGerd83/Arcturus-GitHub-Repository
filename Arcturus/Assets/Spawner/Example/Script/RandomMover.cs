///////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Filename: RandomMover.cs
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
/// This is an example class that shows off how the system works.
/// </summary>
[RequireComponent(typeof(SpawnAI))]
public class RandomMover : MonoBehaviour
{
    /// <summary>
    /// The time to wait before destorying the unit.
    /// </summary>
    private float killTime = 0.0f;
    /// <summary>
    /// Gets the SpawnAI component on this GameObject.
    /// </summary>
    private SpawnAI ai;

    void Start()
    {
        ai = gameObject.GetComponent<SpawnAI>();
        Set();
    }

    void KillUnit()
    {
        ai.Remove();
    }
    /// <summary>
    /// This resets the variables that will be changed throughout the game, this will
    /// allow the object to be reused by Pool Manager (by Path-o-Logical).
    /// </summary>
    void Set()
    {
        // This simply places the GameObject at a random position
        Vector3 pos = new Vector3(Random.Range(0.0f, 10.0f), Random.Range(0.0f, 10.0f), 0);
        transform.position = pos;

        // This simply gives the units a random kill time if it is set to 0
        if (killTime == 0.0f)
        {
            killTime = Random.Range(1.0f, 10.0f);
        }
        Invoke("KillUnit", killTime);
    }
    /// <summary>
    /// This is called by Pool Manager (by Path-o-Logical) when spawning a unit,
    /// this can be used to reinitialise variables and so forth.
    /// </summary>
    void OnSpawned()
    {
        Set();
    }
}