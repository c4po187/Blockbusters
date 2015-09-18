#region Prerequisites

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace BlockBusters.Players {

    #region Objects

    public class Human : Player {

        #region Constructors

        /// <summary>
        /// Instantiates a Human player with a name and gamer picture.
        /// </summary>
        /// <param name="name">
        /// Represents the name of the player.
        /// </param>
        /// <param name="gamerPic">
        /// The players gamer picture / avatar.
        /// </param>
        public Human(string name, Texture2D gamerPic) {
            m_name = name;
            m_gamerPic = gamerPic;
            m_turnSpec = TurnSpecifier.Waiting;
            Score = Consecutive = 0;
        }

        #endregion
    }

    #endregion
}
