using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SamuraiSoccer.SoccerGame
{
    public interface ISlashed 
    {
        /// <summary>
        /// ÕŒ‚”g‚Éa‚ç‚ê‚é‚ÉŒÄ‚Î‚ê‚é
        /// </summary>
        /// <param name="dir">‚Á”ò‚Ô•ûŒü</param>
        public void Slashed(Vector3 dir);
    }
}
