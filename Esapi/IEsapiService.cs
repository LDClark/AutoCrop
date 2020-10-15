using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace AutoCrop
{
    public interface IEsapiService
    {
        Task<StructSet[]> GetStructureSetsAsync();
        Task<Struct[]> GetStructureIdsAsync(string structureSet);
        Task AddRingAsync(string structureSetId, string ptvId, string ringId, double innerMargin, double outerMargin);
        Task CleanUpRingsAsync(string structureSetId, string ptvHighId, string ptvMidId, string ptvLowId, string ptv4Id, string ringHighId, string ringMidId, string ringLowId, string ring4Id);
    }
}