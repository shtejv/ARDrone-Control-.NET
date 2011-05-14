/* ARDrone Control .NET - An application for flying the Parrot AR drone in Windows.
 * Copyright (C) 2010, 2011 Thomas Endres
 * 
 * This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License along with this program; if not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARDrone.Detection
{
    public class CourseList
    {
        private const int maxCountDirection = 15;
        private List<CourseAdvisor.Direction> courseDirections = null;

        public CourseList()
        {
            courseDirections = new List<CourseAdvisor.Direction>();
        }

        public void addDirection(CourseAdvisor.Direction direction)
        {
            courseDirections.Insert(0, direction);

            if (courseDirections.Count > maxCountDirection)
                courseDirections.RemoveRange(maxCountDirection, courseDirections.Count - maxCountDirection);
        }

        public CourseAdvisor.Direction LatestValidDirection
        {
            get
            {
                foreach (CourseAdvisor.Direction direction in courseDirections)
                {
                    if (direction.AdviceGiven)
                        return direction;
                }
                return new CourseAdvisor.Direction(false);
            }
        }

        public CourseAdvisor.Direction LatestDirection
        {
            get
            {
                if (courseDirections.Count > 0)
                    return courseDirections[0];
                else
                    return new CourseAdvisor.Direction(false);
            }
        }
    }
}
