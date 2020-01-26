///////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Filename: SpawnEnums.cs
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

namespace CorruptedSmileStudio.Spawner
{
    /// <summary>
    /// The different spawn modes supported.
    /// </summary>
    public enum SpawnModes
    {
        /// <summary>
        /// This will continually spawn units until the limit, then spawn whenever a unit dies.
        /// Use this mode with a trigger that disables the spawner.
        /// </summary>
        Normal,
        /// <summary>
        /// This will spawn units till the limit is hit then stop.
        /// </summary>
        Once,
        /// <summary>
        /// This will spawn units in waves. Once all units are dead it will spawn a new wave.
        /// </summary>
        Wave,
        /// <summary>
        /// This will spawn units in waves, once the specified time has passed a new wave will spawn
        /// regardless of whether there was still units left.
        /// </summary>
        TimedWave,
        /// <summary>
        /// This will spawn units in waves, once all units are dead it will after the specified time
        /// spawn a new wave.
        /// </summary>
        TimeSplitWave
    }
    /// <summary>
    /// The unit levels allowed.
    /// </summary>
    public enum UnitLevels
    {
        /// <summary>
        /// Easy unit
        /// </summary>
        Easy = 0,
        /// <summary>
        /// Medium unit
        /// </summary>
        Medium,
        /// <summary>
        /// Hard unit
        /// </summary>
        Hard,
        /// <summary>
        /// Boss unit
        /// </summary>
        Boss
    }
}