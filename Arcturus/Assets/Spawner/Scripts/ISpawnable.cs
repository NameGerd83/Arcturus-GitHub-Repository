///////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Filename: ISpawnable.cs
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
    /// Used to provide the methods that need to be called in order to interact
    /// with the spawner system.
    /// </summary>
    /// <description>
    /// This is an interface that you should extend from if you want to handle the interaction between your GameObject and the Spawner.cs, however it is highly advised that you use SpawnAI.cs or take a look at it in order to understand what you need to do in order for the system to work together.
    /// </description>
    public interface ISpawnable
    {
        /// <summary>
        /// Call this method to remove the unit.
        /// You will need to use tags to send a message to all spawner objects with the ID of the spawner
        /// </summary>
        void Remove();
        /// <summary>
        /// This is called by the Spawner, with the spawner ID passed along with it.
        /// </summary>
        /// <param name="ID">The ID of the Spawner, you will want to assign this to a variable for use in Remove.</param>
        void SetID(int ID);
    }
}