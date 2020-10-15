using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using VMS.TPS.Common.Model.API;

namespace AutoCrop
{
    public class Cleanup
    {
        public void CleanupPatient(ScriptContext context)
        {           
            var patient = context.Patient;
            patient.BeginModifications();
            if (patient != null)
            {
                if (patient.Courses.Any())
                {
                    var courses = patient.Courses.ToList();
                    foreach (var course in courses)
                    {
                        if (course.Id == "ESAPI TEST SRS")
                            patient.RemoveCourse(course);
                    }
                }

                const string PatientStructures = "Prostate";
                var removedStructureIds = new List<string> {"PTV", "CTV+margin"};
                if (patient.StructureSets.Any(set => set.Id == PatientStructures))
                {
                    var structureSet = patient.StructureSets.Single(set => set.Id == PatientStructures);
                    foreach (var id in removedStructureIds)
                    {
                        if (structureSet.Structures.Any(st => st.Id == id))
                        {
                            var removedStructure = structureSet.Structures.Single(st => st.Id == id);
                            structureSet.RemoveStructure(removedStructure);
                        }
                    }  
                }
            }
        }
    }
}
