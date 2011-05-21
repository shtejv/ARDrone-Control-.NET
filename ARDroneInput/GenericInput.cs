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
using ARDrone.Input.Utils;

namespace ARDrone.Input
{
    public abstract class GenericInput
    {
        public abstract void InitDevice();
        public abstract void Dispose();

        public virtual void StartRawInput()
        { }
        public abstract String GetCurrentRawInput(out bool isAxis);
        public virtual void EndRawInput()
        { }

        public virtual void StartControlInput()
        { }
        public abstract InputState GetCurrentControlInput();
        public virtual void EndControlInput()
        { }

        public virtual void CancelEvents()
        { }

        public virtual bool Cancellable
        {
            get { return false; }
        }

        public abstract bool IsDevicePresent
        {
            get;
        }

        public virtual String DeviceName
        {
            get { return string.Empty; }
        }

        public virtual String DeviceInstanceId
        {
            get { return string.Empty; }
        }
    }
}