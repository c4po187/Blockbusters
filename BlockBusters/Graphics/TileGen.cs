#region Prerequisites

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace BlockBusters.Graphics {

    #region Structures

    /// <summary>
    /// Structure represents a single tile.
    /// </summary>
    public struct Tile {

        #region Members

        public Texture2D spriteSheet;
        public int width,
                   height,
                   stepX,
                   stepY,
                   evenRow_offsetX;

        #endregion

        #region Functions

        /// <summary>
        /// Creates a Rectangle used to select the current sprite from
        /// the spritesheet.
        /// </summary>
        /// <param name="tileIndex">
        /// Represents the index of the sprite.
        /// </param>
        /// <returns>
        /// A Rectangle positioned at the location of the desired sprite
        /// on the spritesheet.
        /// </returns>
        public Rectangle getSrcRect(int tileIndex) {
            int tileY = tileIndex / (this.spriteSheet.Width / this.width);
            int tileX = tileIndex % (this.spriteSheet.Width / this.width);

            return new Rectangle((tileX * this.width), (tileY * this.height),
                this.width, this.height);
        }

        #endregion
    }

    #endregion

    #region Objects

    /// <summary>
    /// Class represents a single cell of a terrain or collection.
    /// A cell itself, is usually made up of many tiles.
    /// </summary>
    public class Cell {

        #region Constructor

        /// <summary>
        /// Constructs the Cell.
        /// </summary>
        /// <param name="tileID">
        /// The index of the tile.
        /// </param>
        public Cell(int tileID) {
            TileID = tileID; 
        }

        #endregion

        #region Declarations

        public List<int> BaseTiles = new List<int>();
        public List<int> HighTiles = new List<int>();
        public List<int> TopTiles = new List<int>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets and sets the tileID.
        /// </summary>
        public int TileID {
            get { return BaseTiles.Count > 0 ? BaseTiles[0] : 0; }
            set {
                if (BaseTiles.Count > 0)
                    BaseTiles[0] = value;
                else
                    addBaseTile(value);
            }
        }

        #endregion

        #region Overloads

        /// <summary>
        /// Creates the possibility of casting an integer 
        /// explicitly to an instance of Cell.
        /// </summary>
        /// <param name="tileID">
        /// Represents the index of the tile on the spritesheet.
        /// </param>
        /// <returns>
        /// An instance of Cell.
        /// </returns>
        public static explicit operator Cell(int tileID) {
            return new Cell(tileID);
        }

        /// <summary>
        /// Creates the possiblility of incrementing the tileID
        /// by the instance.
        /// </summary>
        /// <param name="cell">
        /// Represents the instance of Cell to be iterated.
        /// </param>
        /// <returns>
        /// An instance of Cell.
        /// </returns>
        public static Cell operator ++(Cell cell) {
            ++cell.TileID;
            return cell;
        }

        /// <summary>
        /// Creates the possiblility of decrementing the tileID
        /// by the instance.
        /// </summary>
        /// <param name="cell">
        /// Represents the instance of Cell to be iterated.
        /// </param>
        /// <returns>
        /// An instance of Cell.
        /// </returns>
        public static Cell operator --(Cell cell) {
            --cell.TileID;
            return cell;
        }

        #endregion

        #region Functions

        /// <summary>
        /// Adds a tile from the spritesheet to the BaseTile list.
        /// </summary>
        /// <param name="tileID">
        /// The index of the tile.
        /// </param>
        public void addBaseTile(int tileID) {
            BaseTiles.Add(tileID);
        }

        /// <summary>
        /// Adds a tile from the spritesheet to the HighTile list.
        /// </summary>
        /// <param name="tileID">
        /// The index of the tile.
        /// </param>
        public void addHighTile(int tileID) {
            HighTiles.Add(tileID);
        }

        /// <summary>
        /// Adds a tile from the spritesheet to the TopTile list.
        /// </summary>
        /// <param name="tileID">
        /// The index of the tile.
        /// </param>
        public void addTopTile(int tileID) {
            TopTiles.Add(tileID);
        }

        #endregion
    }

    /// <summary>
    /// Class represents a single row of cells.
    /// </summary>
    public class Row {

        #region Declarations

        public List<Cell> Columns = new List<Cell>();

        #endregion
    }

    #endregion
}
