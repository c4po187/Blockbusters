#region Prerequisites

using System;
using System.Collections.Generic;
using BlockBusters.Graphics;
using BlockBusters.Sys;
using EUMD_CS.Graphics.GeometryPrimitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace BlockBusters.UI {

    #region Objects

    /// <summary>
    /// Class that defines a rectangular shaped Selector.
    /// </summary>
    public class Selector {

        #region Properties

        public Oblong Bounds { get; set; }
        public bool Visible { get; set; }

        #endregion
    }

    /// <summary>
    /// Class that defines a simple selective area (Menu Option, etc...).
    /// </summary>
    public class Selective {

        #region Properties

        public Rectangle Container { get; set; }
        public SpriteFont Font { get; set; }
        public Color Colour { get; set; }
        public string Text { get; set; }
        public bool Selected { get; set; }
        public bool Hover { get; set; }
        public Menu Child { get; set; }

        #endregion
    }

    public abstract class Menu {

        #region Declarations

        protected Selector m_selector;
        protected Color m_hoverColour;
        protected List<Selective> m_selectives;
        protected List<Animated> m_animations;
        protected List<Menu> m_children;
        protected Menu m_parent;

        #endregion

        #region Properties

        public Selector MenuSelector {
            get { return m_selector; }
        }

        public List<Selective> Selectives {
            get { return m_selectives; }
        }

        public List<Animated> Animations {
            get { return m_animations; }
        }

        public List<Menu> Children {
            get { return m_children; }
        }

        public Menu Parent {
            get { return m_parent; }
            set { m_parent = value; }
        }

        public bool Active { get; set; }

        #endregion

        #region Functions

        public virtual void update(GameTime gameTime, InputManager inputManager) {
            if (this.Active) {
                // Update all the animations in the menu
                foreach (Animated animation in m_animations)
                    animation.updateAnimation(gameTime);

                /* Check to see if a selective has been selected/hovering and
                 * initiate any sub-menu/options. 
                 */
                foreach (Selective selective in m_selectives) {
                    if (selective.Container.Intersects((Rectangle)m_selector.Bounds) ||
                        (inputManager.MouseLocation.X > selective.Container.Left &&
                        inputManager.MouseLocation.X < selective.Container.Right &&
                        inputManager.MouseLocation.Y > selective.Container.Top &&
                        inputManager.MouseLocation.Y < selective.Container.Bottom)) {
                        selective.Hover = true;
                        if (inputManager.isATapped() || inputManager.isStartTapped() ||
                        inputManager.isKeyTapped(Keys.Enter) || inputManager.isLeftMouseButtonTapped()) {
                            selective.Selected = true;
                            foreach (Menu menu in m_children) {
                                if (Object.ReferenceEquals(selective.Child, menu)) {
                                    menu.Active = true;
                                    menu.Parent = this;
                                }
                            }
                        }
                    } else {
                        selective.Hover = false;
                    }
                }

                // Drop back from this menu
                if (inputManager.isKeyTapped(Keys.Escape) || inputManager.isBTapped() ||
                    inputManager.isBackTapped()) {
                    this.Active = false;
                    if (m_parent != null) {
                        foreach (Selective pSelective in m_parent.Selectives) {
                            pSelective.Selected = false;
                        }
                    }
                }
            }

            // Update child menus
            if (m_children.Count > 0) {
                foreach (Menu menu in m_children) {
                    if (menu.Active)
                        menu.update(gameTime, inputManager);
                }
            }
        }

        public virtual void draw(SpriteBatch spriteBatch) {
            if (this.Active) {
                foreach (Animated animation in m_animations)
                    animation.draw(spriteBatch);
            }

            if (m_children.Count > 0) {
                foreach (Menu menu in m_children) {
                    if (menu.Active)
                        menu.draw(spriteBatch);
                }
            }

            if (m_selectives.Count > 0) {
                foreach (Selective selective in m_selectives) {
                    if (selective.Hover) {
                        spriteBatch.DrawString(selective.Font, selective.Text,
                            new Vector2(selective.Container.X, selective.Container.Y - 10f), m_hoverColour, 
                            0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0f);
                    } else {
                        spriteBatch.DrawString(selective.Font, selective.Text,
                            new Vector2(selective.Container.X, selective.Container.Y), selective.Colour);
                    }

                }
            }

            if (m_selector.Visible)
                m_selector.Bounds.draw(null);
        }

        #endregion
    }

    #endregion
}
