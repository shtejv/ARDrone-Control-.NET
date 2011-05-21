/* ARDrone Control .NET - An application for flying the Parrot AR drone in Windows.
 * Copyright (C) 2010, 2011 Thomas Endres, Stephen Hobley, Julien Vinel
 * 
 * This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License along with this program; if not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace ARDrone.Input.Utils
{
    public class InputState
    {
        public float Roll  { get; set; }
        public float Pitch { get; set; }
        public float Yaw   { get; set; }
        public float Gaz   { get; set; }

        public bool CameraSwap { get; set; }
        public bool TakeOff    { get; set; }
        public bool Land       { get; set; }
        public bool Hover      { get; set; }
        public bool Emergency  { get; set; }
        public bool FlatTrim   { get; set; }
        public bool SpecialAction { get; set; }

        public InputState()
        {
            Roll = 0.0f; Pitch = 0.0f; Gaz = 0.0f;
            TakeOff = false; Land = false; Emergency = false; FlatTrim = false; SpecialAction = false;
        }

        public InputState(float roll, float pitch, float yaw, float gaz, bool cameraSwapButton, bool takeOffButton, bool landButton, bool hoverButton, bool emergencyButton, bool flatTrimButton, bool specialActionButton)
        {
            Roll = roll; Pitch = pitch; Yaw = yaw; Gaz = gaz;
            CameraSwap = cameraSwapButton;
            TakeOff = takeOffButton; Land = landButton; Hover = hoverButton;
            Emergency = emergencyButton; FlatTrim = flatTrimButton;
            SpecialAction = specialActionButton;
        }

        public override String ToString()
        {
            String value = "Roll: " + Roll.ToString("0.000") + ", Pitch: " + Pitch.ToString("0.000") + ", Yaw: " + Yaw.ToString("0.000") + ", Gaz: " + Gaz.ToString("0.000");
            if (CameraSwap) { value += ", Change Camera"; }
            if (TakeOff) { value += ", Take Off"; }
            if (Land) { value += ", Land"; }
            if (Hover) { value += ", Hover"; }
            if (Emergency) { value += ", Emergency"; }
            if (FlatTrim) { value += ", Flat Trim"; }
            if (SpecialAction) { value += ", Special Action"; }

            return value;
        }
    }
}