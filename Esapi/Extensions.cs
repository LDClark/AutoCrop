using System.Linq;
using VMS.TPS.Common.Model.API;

public static class Extensions
{
    private static Course GetCourse(Patient patient, string courseId) =>
        patient?.Courses?.FirstOrDefault(x => x.Id == courseId);

    public static Structure GetStructure(PlanningItem plan, string structureId) =>
        plan?.StructureSet?.Structures?.FirstOrDefault(x => x.Id == structureId);
}