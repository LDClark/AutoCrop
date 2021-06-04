using System.Linq;
using System.Threading.Tasks;
using EsapiEssentials.Plugin;
using VMS.TPS.Common.Model.API;
using System;

namespace AutoRing_SIB
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
                    StructureSetIdWithCreationDate = x.Id + " - " + x.HistoryDateTime.ToString(),
                    CanModify = Helpers.CheckStructureSet(context.Patient, x)
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
                    StructureVolume = x.Volume,
                    //CanModify = Helpers.CheckStructure(x)
                })
                .ToArray();
            });

        public Task<string> GetEditableRingNameAsync(string structureSetId, string ringId) =>
            RunAsync(context => GetEditableRingName(context.Patient, structureSetId, ringId));

        public string GetEditableRingName(Patient patient, string structureSetId, string ringId)
        {
            StructureSet structureSet = patient.StructureSets.FirstOrDefault(x => x.Id == structureSetId);
            bool ringInStructureSet = false;
            bool possibleInStructureSet = false;
            bool canEditRing = true;
            string possibleStructureId = String.Empty;
            foreach (var structure in structureSet.Structures)
            {
                if (structure.Id == ringId)
                {
                    ringInStructureSet = true;
                    if (Helpers.CheckStructure(structure) == true) //ring is present but editable
                        return structure.Id;
                    else
                        canEditRing = false;
                }           
            }
            if (ringInStructureSet == true && canEditRing == false) //ring is present but not editable
            {
                for (int i = 1; i <= 5; i++)
                {
                    foreach (var structure in structureSet.Structures)
                    {
                        if (structure.Id == ringId + i.ToString())  //possible already present
                        {
                            possibleInStructureSet = true;
                            if (Helpers.CheckStructure(structure) == true) //possible is editable
                                return structure.Id;
                            else
                            {
                                possibleInStructureSet = false;
                                possibleStructureId = ringId + (i + 1).ToString();
                                break;
                            }

                        }
                        else
                            possibleStructureId = ringId + i.ToString();
                    }
                    if (possibleInStructureSet == false) //possible not in structure and can be added
                        return possibleStructureId;                     
                }
            }
            if (ringInStructureSet == false) //ring not in structure set and can be added
                return ringId;
            throw new Exception("Too many uneditable rings in structure set.");
        }

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
