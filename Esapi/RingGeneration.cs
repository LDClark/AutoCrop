using System.Linq;
using VMS.TPS.Common.Model.API;

namespace AutoRing_SIB
{
    public class RingGeneration
    {
        public void CreateRingFromPTV(StructureSet structureSet, string ptvId, string ringId, double innerMargin, double outerMargin)
        {
            structureSet.Patient.BeginModifications();
            Structure ptv = structureSet.Structures.Where(structure => structure.Id == ptvId).FirstOrDefault();
            Structure ring;
            Structure spacerTemp;
            try
            {
                ring = structureSet.AddStructure("CONTROL", ringId);  //doesnt exist yet
            }
            catch
            {               
                ring = structureSet.Structures.FirstOrDefault(x => x.Id == ringId);  //already exists 
                ring.SegmentVolume = ring.Sub(structureSet.Structures.Single(x => x.Id == "BODY"));  //cleanup
            }
            try
            {
                spacerTemp = structureSet.AddStructure("CONTROL", "spacerTemp");  //doesnt exist yet
            }
            catch
            {
                spacerTemp = structureSet.Structures.FirstOrDefault(x => x.Id == "spacerTemp");  //already exists 
                spacerTemp.SegmentVolume = spacerTemp.Sub(structureSet.Structures.Single(x => x.Id == "BODY"));  //cleanup
            }
            //ring.ConvertToHighResolution();
            ring.SegmentVolume = ptv.Margin(outerMargin);            
            spacerTemp.SegmentVolume = ptv.Margin(innerMargin); //3 mm between ring and PTV
            ring.SegmentVolume = ring.Sub(ptv);
            ring.SegmentVolume = ring.Sub(spacerTemp);
            ring.SegmentVolume = ring.And(structureSet.Structures.Single(x => x.Id == "BODY"));  //clear outside of body
            structureSet.RemoveStructure(spacerTemp);
        }

        public void CleanUpRings(StructureSet structureSet, string ptvHighId, string ptvMidId, string ptvLowId, string ptv4Id, string ringHighId, string ringMidId, string ringLowId, string ring4Id)
        {
            Structure ptvHigh = structureSet.Structures.Where(structure => structure.Id == ptvHighId).FirstOrDefault();
            Structure ringHigh = structureSet.Structures.Where(structure => structure.Id == ringHighId).FirstOrDefault();

            if (ptvMidId != null)
            {
                Structure ptvMid = structureSet.Structures.Where(structure => structure.Id == ptvMidId).FirstOrDefault();
                Structure ringMid = structureSet.Structures.Where(structure => structure.Id == ringMidId).FirstOrDefault();
                ringMid.SegmentVolume = ringMid.SegmentVolume.Sub(ringHigh.SegmentVolume);
                ringMid.SegmentVolume = ringMid.SegmentVolume.Sub(ptvHigh.SegmentVolume);

                if (ptvLowId != null)
                {
                    Structure ptvLow = structureSet.Structures.Where(structure => structure.Id == ptvLowId).FirstOrDefault();
                    Structure ringLow = structureSet.Structures.Where(structure => structure.Id == ringLowId).FirstOrDefault();
                    ringLow.SegmentVolume = ringLow.SegmentVolume.Sub(ringMid.SegmentVolume);
                    ringLow.SegmentVolume = ringLow.SegmentVolume.Sub(ptvMid.SegmentVolume);
                    ringLow.SegmentVolume = ringLow.SegmentVolume.Sub(ringHigh.SegmentVolume);
                    ringLow.SegmentVolume = ringLow.SegmentVolume.Sub(ptvHigh.SegmentVolume);

                    if (ptv4Id != null)
                    {
                        Structure ptv4 = structureSet.Structures.Where(structure => structure.Id == ptv4Id).FirstOrDefault();
                        Structure ring4 = structureSet.Structures.Where(structure => structure.Id == ring4Id).FirstOrDefault();
                        ring4.SegmentVolume = ring4.SegmentVolume.Sub(ringLow.SegmentVolume);
                        ring4.SegmentVolume = ring4.SegmentVolume.Sub(ptvLow.SegmentVolume);
                        ring4.SegmentVolume = ring4.SegmentVolume.Sub(ringMid.SegmentVolume);
                        ring4.SegmentVolume = ring4.SegmentVolume.Sub(ptvMid.SegmentVolume);
                        ring4.SegmentVolume = ring4.SegmentVolume.Sub(ringHigh.SegmentVolume);
                        ring4.SegmentVolume = ring4.SegmentVolume.Sub(ptvHigh.SegmentVolume);
                    }
                }
            }
        }
    }
}
