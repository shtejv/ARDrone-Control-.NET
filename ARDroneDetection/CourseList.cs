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
