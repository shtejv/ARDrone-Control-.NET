/****************************************
 * Joystick control
 * Code by Jason Labbé
 * C# 2008 Express
 * 
 * History:
 * v1.0 - Initial script version
 ***************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace maxCustomControl
{
    // Triggered when a target's position is changed
    public class joystickEventArgs : EventArgs
    {
        private double x;
        private double y;
        
        public joystickEventArgs(double xVal, double yVal)
        {
            x = xVal;
            y = yVal;
        }

        public double X
        {
            get { return x; }
        }

        public double Y
        {
            get { return y; }
        }
    }

    public delegate void joystickHandler(object sender, joystickEventArgs e);
    public delegate void spinnerHandler(object sender, EventArgs e);

    // Example form
    public partial class main : Form
    {
        public main()
        {
            InitializeComponent();
            joystickControl myJoystick = new joystickControl();
            this.Controls.Add(myJoystick);
        }
    }
    
    // Custom joystick control
    public class joystickControl : UserControl
    {
        private SplitContainer mainSplitter = new SplitContainer();
        private SplitContainer labelSplitter = new SplitContainer();
        private Panel mainPanel = new Panel();
        private NumericUpDown label_x = new NumericUpDown();
        private NumericUpDown label_y = new NumericUpDown();

        public double version = 1.0;
        public Color bgColor = Color.Lime;
        public Color gridColor = Color.FromArgb(0, 220, 0);
        public Color targetColor = Color.Red;
        public bool showGrid = true;
        public int radius = 20;

        private bool isMouseDown = false;
        private double x_coords = 0;
        private double y_coords = 0;

        public event joystickHandler joystickEvent;
        public event spinnerHandler spinnerEvent;

        // Returns the current position of the target
        public double[] getPosition()
        {
            double[] values = { x_coords, y_coords };
            return values;
        }

        // Sets the target's position
        public void setPosition(double x, double y)
        {
            updateDisplay(x, y, true);
        }

        // Constructor
        public joystickControl()
        {
            this.Size = new Size(200, 200);

            mainSplitter.Dock = DockStyle.Fill;
            mainSplitter.Orientation = Orientation.Horizontal;
            mainSplitter.Panel2.BackColor = Color.DarkGray;
            mainSplitter.Panel2MinSize = 17;
            mainSplitter.SplitterDistance = 100;
            mainSplitter.SplitterWidth = 1;
            mainSplitter.FixedPanel = FixedPanel.Panel2;

            mainPanel.Dock = DockStyle.Fill;
            mainPanel.BackColor = bgColor;

            labelSplitter.Dock = DockStyle.Fill;
            labelSplitter.SplitterWidth = 1;
            labelSplitter.Panel1.BackColor = Color.FromArgb(150,150,150);

            label_x.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label_x.Dock = DockStyle.Fill;
            label_x.Minimum = -1;
            label_x.Maximum = 1;
            label_x.DecimalPlaces = 2;
            label_x.Increment = 0.01m;
            label_x.ContextMenu = new ContextMenu();

            label_y.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));   
            label_y.Dock = DockStyle.Fill;
            label_y.Minimum = -1;
            label_y.Maximum = 1;
            label_y.DecimalPlaces = 2;
            label_y.Increment = 0.01m;
            label_y.ContextMenu = new ContextMenu();

            this.Controls.Add(mainSplitter);
            mainSplitter.Panel1.Controls.Add(mainPanel);
            mainSplitter.Panel2.Controls.Add(labelSplitter);
            labelSplitter.Panel1.Controls.Add(label_x);
            labelSplitter.Panel2.Controls.Add(label_y);
            labelSplitter.SplitterDistance = this.Width / 2;

            // Add events
            mainPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(move_MouseDown);
            mainPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(move_MouseUp);
            mainPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(move_MouseMove);
            this.mainPanel.Paint += new System.Windows.Forms.PaintEventHandler(OnPaint);
            label_x.MouseDown += new System.Windows.Forms.MouseEventHandler(xSpinnerMouseDown);
            label_x.ValueChanged += new System.EventHandler(xSpinnerValueChanged);
            label_x.Enter += new System.EventHandler(spinnerEnter);
            spinnerEvent += new spinnerHandler(spinnerFocus);
            label_y.MouseDown += new System.Windows.Forms.MouseEventHandler(ySpinnerMouseDown);
            label_y.ValueChanged += new System.EventHandler(ySpinnerValueChanged);
            label_y.Enter += new System.EventHandler(spinnerEnter);
            joystickEvent += new joystickHandler(joystickValueChanged);
        }

        public void spinnerEnter(object sender, EventArgs e)
        {
            if (spinnerEvent != null)
            {
                spinnerEvent(sender, new EventArgs());
            }
        }

        public void spinnerFocus(object sender, EventArgs e) { }

        public void xSpinnerMouseDown(object sender, MouseEventArgs e)
        {
            label_x.Focus();
            if (e.Button.ToString() == "Right") label_x.Value = 0;
        }

        public void xSpinnerValueChanged(object sender, EventArgs e)
        {
            if (label_x.Focused)
            {
                updateDisplay((double)label_x.Value, y_coords, true);
                if (joystickEvent != null)
                {
                    joystickEvent(sender, new joystickEventArgs(x_coords, y_coords));
                }
            }
        }

        public void ySpinnerMouseDown(object sender, MouseEventArgs e)
        {
            label_y.Focus();
            if (e.Button.ToString() == "Right") label_y.Value = 0;
        }

        public void ySpinnerValueChanged(object sender, EventArgs e)
        {
            if (label_y.Focused)
            {
                updateDisplay(x_coords, (double)label_y.Value, true);
                if (joystickEvent != null)
                {
                    joystickEvent(sender, new joystickEventArgs(x_coords, y_coords));
                }
            }
        }

        public void joystickValueChanged(object sender, joystickEventArgs e) { }

        public void OnPaint(object sender, PaintEventArgs e)
        {
            updateDisplay(x_coords, y_coords, true);
        }

        // Set values to spinners
        private void setLabels()
        {
            double xVal = (x_coords / (double)mainPanel.Width)-0.5;
            double yVal = (y_coords / (double)mainPanel.Height)-0.5;
            
            xVal = xVal * 2;
            yVal = yVal * 2;

            label_x.Value = (decimal)x_coords;
            label_y.Value = (decimal)y_coords;
        }

        private double convertToValue(int val, int multiplierType)
        {
            int multiplier;
            if (multiplierType == 0) multiplier = mainPanel.Width;
            else multiplier = mainPanel.Height;

            double newValue = ( (val / (double)multiplier) - 0.5) * 2;
            if (multiplierType == 1) newValue *= -1;
            return newValue;
        }

        private void move_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            mainPanel.Focus();
            if (e.Button.ToString() == "Left")
            {
                isMouseDown = true;
                double x = convertToValue(e.X, 0);
                double y = convertToValue(e.Y, 1);
                updateDisplay(x, y, false);
            }
            else if (e.Button.ToString() == "Right")
            {
                isMouseDown = false;
                updateDisplay(0, 0, true);
            }

            if (e.Button.ToString() == "Left" || e.Button.ToString() == "Right")
            {
                if (joystickEvent != null)
                {
                    joystickEvent(sender, new joystickEventArgs(x_coords, y_coords));
                }
            }
        }

        private void move_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            isMouseDown = false;
        }

        private void move_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            double x = convertToValue(e.X, 0);
            double y = convertToValue(e.Y, 1);
            updateDisplay(x, y, false);

            if (isMouseDown & joystickEvent != null)
            {
                joystickEvent(sender, new joystickEventArgs(x_coords, y_coords));
            }
        }

        // Refresh control's view
        public void refreshJoystick()
        {
            updateDisplay(x_coords, y_coords, true);
        }

        private void updateDisplay(double x, double y, bool forceUpdate)
        {
            if (forceUpdate | isMouseDown)
            {
                if (x < -1.0) x = -1.0;
                else if (x > 1.0) x = 1.0;

                if (y < -1.0) y = -1.0;
                else if (y > 1.0) y = 1.0;

                x_coords = x;
                y_coords = y;
                
                // Convert value to actual coordinates
                double xPosAsDouble = ((x / 2.0) + 0.5) * mainPanel.Width;
                double yPosAsDouble = ((-y / 2.0) + 0.5) * mainPanel.Height;
                int xPos = (int)xPosAsDouble;
                int yPos = (int)yPosAsDouble;
                
                Graphics myGraphics = mainPanel.CreateGraphics();
                myGraphics.Clear(bgColor);

                // Draw grid
                if (showGrid)
                {
                    System.Drawing.Pen gridPen;
                    gridPen = new System.Drawing.Pen(gridColor);
                    gridPen.Width = 2;
                    int gridCount = 10;

                    int percent;
                    if (mainPanel.Height >= mainPanel.Width) percent = mainPanel.Height / gridCount;
                    else percent = mainPanel.Width / gridCount;

                    int val = 0;
                    for (int count = 0; count < gridCount + 1; ++count)
                    {
                        myGraphics.DrawLine(gridPen, 0, val, mainPanel.Width, val);
                        val += percent;
                    }

                    val = 0;
                    while (val < mainPanel.Width)
                    {
                        myGraphics.DrawLine(gridPen, val, 0, val, mainPanel.Height);
                        val += percent;
                    }
                }

                // Draw target
                int drawX = xPos - (radius / 2);
                int drawY = yPos - (radius / 2);
                
                System.Drawing.Pen myPen;
                myPen = new System.Drawing.Pen(targetColor);
                myPen.Width = 2;

                myGraphics.DrawLine(myPen, xPos, 0, xPos, mainPanel.Height);
                myGraphics.DrawLine(myPen, 0, yPos, mainPanel.Width, yPos);

                myGraphics.FillEllipse(new SolidBrush(Color.Black), drawX, drawY, radius, radius);
                myGraphics.FillEllipse(new SolidBrush(targetColor), drawX+2, drawY+2, radius-4, radius-4);

                myGraphics.Dispose();
                setLabels();
            }
        }
    }
}