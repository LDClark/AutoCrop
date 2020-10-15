using System.Linq;
using System.Threading.Tasks;
using EsapiEssentials.Plugin;
using VMS.TPS.Common.Model.API;
using System;
using VMS.TPS.Common.Model.Types;

namespace AutoCrop
{
    public class EsapiService : EsapiServiceBase<PluginScriptContext>, IEsapiService
    {
        private readonly RingGeneration _planGeneration;

        public EsapiService(PluginScriptContext context) : base(context)
        {
            _planGeneration = new RingGeneration();
        }

        public Task<StructSet[]> GetStructureSetsAsync() =>
            RunAsync(context =>
            {
                return context.Patient.StructureSets?
                .Select(x => new StructSet
                {
                    CreationDate = x.HistoryDateTime,
                    ImageId = x.Image.Id,
                    StructureSetId = x.Id,
                    StructureSetIdWithCreationDate = x.Id + " - " + x.HistoryDateTime.ToString()
                })
                .ToArray();
            });

        public Task<Struct[]> GetStructureIdsAsync(string structureSet) =>
            RunAsync(context =>
            {
                return context.Patient.StructureSets?
                .FirstOrDefault(x => x.Id == structureSet)
                .Structures.Where(x => x.Id.ToUpper().Contains("PTV") == true).Select(x => new Struct
                {
                    StructureId = x.Id,
                    StructureVolume = x.Volume
                })
                .ToArray();
            });

        public Task AddRingAsync(string structureSetId, string ptvId, string ringId, double innerMargin, double outerMargin) =>
            RunAsync(context => AddRing(context.Patient, structureSetId, ptvId, ringId, innerMargin, outerMargin));

        public void AddRing(Patient patient, string structureSetId, string ptvId, string ringId, double innerMargin, double outerMargin)
        {
            StructureSet structureSet = patient.StructureSets.FirstOrDefault(x => x.Id == structureSetId);
            _planGeneration.CreateRingFromPTV(structureSet, ptvId, ringId, innerMargin, outerMargin);           
        }

        public Task CleanUpRingsAsync(string structureSetId, string ptvHighId, string ptvMidId, string ptvLowId, string ptv4Id, string ringHighId, string ringMidId, string ringLowId, string ring4Id) =>
            RunAsync(context => CleanUpRings(context.Patient, structureSetId, ptvHighId, ptvMidId, ptvLowId, ptv4Id, ringHighId, ringMidId, ringLowId, ring4Id));

        public void CleanUpRings(Patient patient, string structureSetId, string ptvHighId, string ptvMidId, string ptvLowId, string ptv4Id, string ringHighId, string ringMidId, string ringLowId, string ring4Id)
        {
            StructureSet structureSet = patient.StructureSets.FirstOrDefault(x => x.Id == structureSetId);
            _planGeneration.CleanUpRings(structureSet, ptvHighId, ptvMidId, ptvLowId, ptv4Id, ringHighId, ringMidId, ringLowId, ring4Id);
        }
    }
}