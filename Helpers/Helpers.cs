using System;
using System.Collections.Generic;
using System.Linq;
using VMS.TPS.Common.Model.API;

namespace AutoRing_SIB
{
    public static class Helpers
    {
        public static bool CheckStructure(Structure structure)
        {
            if (structure.ApprovalHistory.FirstOrDefault().ApprovalStatus == VMS.TPS.Common.Model.Types.StructureApprovalStatus.Approved)
                return false;
            else
                return true;
        }

        public static bool CheckStructureSet(Patient patient, StructureSet structureSet)
        {
            bool canModifyStructureSet = false;
            foreach (Course course in patient.Courses)
            {
                foreach (PlanSetup planSetup in course.PlanSetups)
                {
                    if (planSetup.StructureSet == structureSet)
                    {
                        if (course.CompletedDateTime == null) //course is active and can modify ss
                            canModifyStructureSet = true;
                        else  //course is completed and cannot modify ss
                            return false;
                    }
                }
            }
            return canModifyStructureSet;
        }

        public static Course AddCourse(Patient patient, string courseId)
        {
            patient.BeginModifications();
            try
            {
                var res = patient.Courses.Where(c => c.Id == courseId);
                if (res.Any())
                {
                    var oldCourse = res.Single();
                    patient.RemoveCourse(oldCourse);
                }
                var course = patient.AddCourse();
                course.Id = courseId;
                return course;
            }
            catch
            {
                var course = patient.AddCourse();
                course.Id = courseId;
                return course;
            }
        }

        public static ExternalPlanSetup AddNewPlan(Course course, StructureSet structureSet, string planId)
        {
           try
            {
                var oldPlans = course.PlanSetups.Where(x => x.Id == planId);
                if (oldPlans.Any())
                {
                    var plansToBeRemoved = oldPlans.ToArray();
                    foreach (var p in plansToBeRemoved)
                        course.RemovePlanSetup(p);
}
            }
            catch
            {
                var message = string.Format("Could not cleanup old plans.");
                throw new Exception(message);
            }

            ExternalPlanSetup plan = course.AddExternalPlanSetup(structureSet);           
            plan.Id = planId;
            return plan;
        }

        public static ExternalPlanSetup FindPlanSetup(Patient patient, string courseId, string planSetupId)
        {
            var plans = new List<PlanSetup>();
            foreach (var c in patient.Courses)
            {
                if (c.Id == courseId)
                {
                    var temp = c.PlanSetups.Where(p => p.Id == planSetupId);
                    plans.AddRange(temp);
                }
            }
            return plans.First() as ExternalPlanSetup;
        }

        public static void RemoveStructures(StructureSet structureSet, List<string> structureIDs)
        {
            try
            {
                foreach (var id in structureIDs)
                {
                    if (structureSet.Structures.Any(st => st.Id == id))
                    {
                        Structure structure = structureSet.Structures.Single(x => x.Id == id);
                        Structure body = structureSet.Structures.Single(x => x.Id == "BODY");
                        structure.SegmentVolume = structure.Sub(body);
                    }
                }
            }
            catch
            {
            }
        }
    }
}
