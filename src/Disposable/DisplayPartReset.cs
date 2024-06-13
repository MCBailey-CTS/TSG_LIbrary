using System;
using NXOpen;
using static TSG_Library.Extensions.__Extensions_;

namespace TSG_Library.Disposable
{
    public class DisplayPartReset : IDisposable
    {
        private readonly Part _originalDisplayPart;

        private readonly Part _originalWorkPart;

        /// <summary>Instantiates the display reset.</summary>
        public DisplayPartReset()
        {
            if (__display_part_ is null)
                throw new Exception("No display part.");

            _originalDisplayPart = __display_part_;

            _originalWorkPart = __work_part_;

            //if()
            //_originalWorkComponent = __work_component_;
        }

        /// <summary>Instantiates the display reset and sets the <paramref name="part" /> to be the current display part.</summary>
        /// <param name="part">The part to set as the display part.</param>
        public DisplayPartReset(Part part) : this()
        {
            __display_part_ = part;
        }

        //private readonly NXOpen.Assemblies.Component _originalWorkComponent;


        public void Dispose()
        {
            __display_part_ = _originalDisplayPart;

            __work_part_ = _originalWorkPart;

            //if (!(_originalWorkComponent is null))

            //    __work_component_ = _originalWorkComponent;
        }
    }
}