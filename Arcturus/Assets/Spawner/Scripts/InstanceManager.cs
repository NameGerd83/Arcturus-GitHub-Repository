//#define POOLMANAGER_INSTALLED

///////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Filename: InstanceManager.cs
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
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////

namespace CorruptedSmileStudio.Spawner
{
    using UnityEngine;
    /// <summary>
    /// Contains methods which interface with Pool Manager by Path-o-Logical
    /// This will ensure that there is less Instantiating and Destroying being called which will
    /// increase performance.
    /// </summary>
    public static class InstanceManager
    {
        /// <summary>
        /// The pool name under which to spawn the various units.
        /// </summary>
        public static string poolName = "SpawnedEnemy";

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="prefab">The prefab to spawn an instance from</param>
        /// <param name="pos">The position to spawn the instance</param>
        /// <param name="rot">The rotation of the new instance</param>
        /// <returns></returns>
        public static Transform Spawn(Transform prefab, Vector3 pos, Quaternion rot)
        {
#if POOLMANAGER_INSTALLED
            // If the pool doesn't exist, it will be created before use
            if (!PoolManager.Pools.ContainsKey(poolName))
                (new GameObject(poolName)).AddComponent<SpawnPool>();

            return PoolManager.Pools[poolName].Spawn(prefab, pos, rot);
#else
            return (Transform)Object.Instantiate(prefab, pos, rot);
#endif
        }

        /// <summary>
        /// Despawns an instance.
        /// </summary>
        public static void Despawn(Transform instance)
        {
#if POOLMANAGER_INSTALLED
            PoolManager.Pools[poolName].Despawn(instance);
#else
            Object.Destroy(instance.gameObject);
#endif
        }
        /// <summary>
        /// This is used by Spawner.cs, You should never need to access this.
        /// This is only used when Pool Manager is present.
        /// </summary>
        /// <param name="prefab">The Prefab to start pooling.</param>
        /// <param name="amount">The number of prefabs to pool.</param>
        public static void ReadyPreSpawn(Transform prefab, int amount)
        {
#if POOLMANAGER_INSTALLED
            if (!PoolManager.Pools.ContainsKey(poolName))
                (new GameObject(poolName)).AddComponent<SpawnPool>();

            PrefabPool item = new PrefabPool(prefab);
            item.preloadAmount = amount;

            PoolManager.Pools[poolName].CreatePrefabPool(item);

#endif
        }
    }
}