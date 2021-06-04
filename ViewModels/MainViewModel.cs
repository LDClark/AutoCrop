using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;

namespace AutoRing_SIB
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IEsapiService _esapiService;
        private readonly IDialogService _dialogService;
        public MainViewModel(IEsapiService esapiService, IDialogService dialogService)
        {
            _esapiService = esapiService;
            _dialogService = dialogService;
        }
        private Struct[] _structuresHigh;
        public Struct[] StructuresHigh
        {
            get => _structuresHigh;
            set => Set(ref _structuresHigh, value);
        }
        private Struct[] _structuresMid;
        public Struct[] StructuresMid
        {
            get => _structuresMid;
            set => Set(ref _structuresMid, value);
        }
        private Struct[] _structuresLow;
        public Struct[] StructuresLow
        {
            get => _structuresLow;
            set => Set(ref _structuresLow, value);
        }
        private Struct[] _structures4;
        public Struct[] Structures4
        {
            get => _structures4;
            set => Set(ref _structures4, value);
        }
        private StructSet[] _structureSets;
        public StructSet[] StructureSets
        {
            get => _structureSets;
            set => Set(ref _structureSets, value);
        }
        private StructSet _selectedStructureSet;
        public StructSet SelectedStructureSet
        {
            get => _selectedStructureSet;
            set => Set(ref _selectedStructureSet, value);
        }
        private Struct _selectedStructurePTVHigh;
        public Struct SelectedStructurePTVHigh
        {
            get => _selectedStructurePTVHigh;
            set => Set(ref _selectedStructurePTVHigh, value);
        }
        private Struct _selectedStructurePTVMid;
        public Struct SelectedStructurePTVMid
        {
            get => _selectedStructurePTVMid;
            set => Set(ref _selectedStructurePTVMid, value);
        }
        private Struct _selectedStructurePTVLow;
        public Struct SelectedStructurePTVLow
        {
            get => _selectedStructurePTVLow;
            set => Set(ref _selectedStructurePTVLow, value);
        }
        private Struct _selectedStructurePTV4;
        public Struct SelectedStructurePTV4
        {
            get => _selectedStructurePTV4;
            set => Set(ref _selectedStructurePTV4, value);
        }
        public double InnerMargin { get; set; }
        public double OuterMargin { get; set; }
        public ICommand StartCommand => new RelayCommand(Start);
        public ICommand GetStructuresCommand => new RelayCommand(GetStructures);
        public ICommand GetRingsCommand => new RelayCommand(CreateRings);

        private async void Start()
        {
            StructureSets = await _esapiService.GetStructureSetsAsync();
        }

        private async void GetStructures()
        {
            StructuresHigh = await _esapiService.GetStructureIdsAsync(SelectedStructureSet.StructureSetId);
            StructuresMid = await _esapiService.GetStructureIdsAsync(SelectedStructureSet.StructureSetId);
            StructuresLow = await _esapiService.GetStructureIdsAsync(SelectedStructureSet.StructureSetId);
            Structures4 = await _esapiService.GetStructureIdsAsync(SelectedStructureSet.StructureSetId);
        }

        private async void CreateRings()
        {
            string selectedStructureSetId = SelectedStructureSet?.StructureSetId;
            string ptvHighId = SelectedStructurePTVHigh?.StructureId;
            string ptvMidId = SelectedStructurePTVMid?.StructureId;
            string ptvLowId = SelectedStructurePTVLow?.StructureId;
            string ptv4Id = SelectedStructurePTV4?.StructureId;

            string ringHighId = string.Empty;
            string ringMidId = string.Empty;
            string ringLowId = string.Empty;
            string ring4Id = string.Empty;

            _dialogService.ShowProgressDialog("Getting ring high...",
                async progress =>
                {
                    ringHighId = await _esapiService.GetEditableRingNameAsync(selectedStructureSetId, "RingHigh");
                });

            _dialogService.ShowProgressDialog("Getting ring mid...",
                async progress =>
                {
                    ringMidId = await _esapiService.GetEditableRingNameAsync(selectedStructureSetId, "RingMid");
                });

            _dialogService.ShowProgressDialog("Getting ring low...",
                async progress =>
                {
                    ringLowId = await _esapiService.GetEditableRingNameAsync(selectedStructureSetId, "RingLow");
                });

            _dialogService.ShowProgressDialog("Getting ring4...",
                async progress =>
                {
                    ring4Id = await _esapiService.GetEditableRingNameAsync(selectedStructureSetId, "Ring4");
                });

            _dialogService.ShowProgressDialog("Adding Rings High...",
                async progress =>
                {
                    await _esapiService.AddRingAsync(selectedStructureSetId, ptvHighId, ringHighId, InnerMargin * 10, OuterMargin * 10);
                });

            if (ptvMidId != null)
            {
                _dialogService.ShowProgressDialog("Adding Rings Mid...",
                    async progress =>
                    {
                        await _esapiService.AddRingAsync(selectedStructureSetId, ptvMidId, ringMidId, InnerMargin * 10, OuterMargin * 10);
                    });
            }

            if (ptvLowId != null)
            {
                _dialogService.ShowProgressDialog("Adding Rings Low...",
                async progress =>
                {
                    await _esapiService.AddRingAsync(selectedStructureSetId, ptvLowId, ringLowId, InnerMargin * 10, OuterMargin * 10);
                });
            }

            if (ptv4Id != null)
            {
                _dialogService.ShowProgressDialog("Adding Rings 4...",
                async progress =>
                {
                    await _esapiService.AddRingAsync(selectedStructureSetId, ptv4Id, ring4Id, InnerMargin * 10, OuterMargin * 10);
                });
            }

            _dialogService.ShowProgressDialog("Cleaning up rings...",
                async progress =>
                {
                    await _esapiService.CleanUpRingsAsync(selectedStructureSetId, ptvHighId, ptvMidId, ptvLowId, ptv4Id, ringHighId, ringMidId, ringLowId, ring4Id);
                });
        }
    }
}
