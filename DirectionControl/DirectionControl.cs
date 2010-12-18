using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DirectionControl
{
    public partial class DirectionControl : UserControl
    {
        public enum Direction { None, Up, UpRight, Right, DownRight, Down, DownLeft, Left, UpLeft };

        public Direction currentDirection = Direction.None;

        public DirectionControl()
        {
            InitializeComponent();
            UpdateCurrentDirection();
        }

        public Direction ArrowDirection
        {
            get
            {
                return currentDirection;
            }
            set
            {
                this.currentDirection = value;
                UpdateCurrentDirection();
            }
        }

        private void UpdateCurrentDirection()
        {
            switch (currentDirection)
            {
                case Direction.None:
                    pictureBoxArrow.Image = Properties.Resources.ArrowNoDirection;
                    break;
                case Direction.Up:
                    pictureBoxArrow.Image = Properties.Resources.ArrowUp;
                    break;
                case Direction.UpRight:
                    pictureBoxArrow.Image = Properties.Resources.ArrowUpRight;
                    break;
                case Direction.Right:
                    pictureBoxArrow.Image = Properties.Resources.ArrowRight;
                    break;
                case Direction.DownRight:
                    pictureBoxArrow.Image = Properties.Resources.ArrowDownRight;
                    break;
                case Direction.Down:
                    pictureBoxArrow.Image = Properties.Resources.ArrowDown;
                    break;
                case Direction.DownLeft:
                    pictureBoxArrow.Image = Properties.Resources.ArrowDownLeft;
                    break;
                case Direction.Left:
                    pictureBoxArrow.Image = Properties.Resources.ArrowLeft;
                    break;
                case Direction.UpLeft:
                    pictureBoxArrow.Image = Properties.Resources.ArrowUpLeft;
                    break;
            }
        }
    }
}
