using System.Threading.Tasks;

namespace AutoRing_SIB
{
    public interface IEsapiService
    {
        Task<StructSet[]> GetStructureSetsAsync();
        Task<Struct[]> GetStructureIdsAsync(string structureSet);
        Task<string> GetEditableRingNameAsync(string structureSetId, string ringId);
        Task AddRingAsync(string structureSetId, string ptvId, string ringId, double innerMargin, double outerMargin);
        Task CleanUpRingsAsync(string structureSetId, string ptvHighId, string ptvMidId, string ptvLowId, string ptv4Id, string ringHighId, string ringMidId, string ringLowId, string ring4Id);
    }
}
