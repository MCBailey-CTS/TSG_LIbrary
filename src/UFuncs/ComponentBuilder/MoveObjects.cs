using NXOpen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSG_Library.UFuncs
{
    public partial class EditBlockForm
    {

private void NewMethod76(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, double distance)
{
    MoveObjects(movePtsFull.ToArray(), distance, "Y");
    MoveObjects(movePtsHalf.ToArray(), distance / 2, "Y");
}

        private void NewMethod20(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, double xDistance)
        {
            MoveObjects(movePtsFull.ToArray(), xDistance, "X");
            MoveObjects(movePtsHalf.ToArray(), xDistance / 2, "X");

            ShowTemporarySizeText();
        }

        private void NewMethod22(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, double xDistance)
        {
            MoveObjects(movePtsFull.ToArray(), xDistance, "X");
            MoveObjects(movePtsHalf.ToArray(), xDistance / 2, "X");

            ShowTemporarySizeText();
        }
       


        private void NewMethod56(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, double distance)
        {
            MoveObjects(movePtsFull.ToArray(), distance, "Y");
            MoveObjects(movePtsHalf.ToArray(), distance / 2, "Y");
        }


        private void NewMethod54(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, double distance)
        {
            MoveObjects(movePtsFull.ToArray(), distance, "Z");
            MoveObjects(movePtsHalf.ToArray(), distance / 2, "Z");
        }



        private void NewMethod52(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, double distance)
        {
            MoveObjects(movePtsFull.ToArray(), distance, "Z");
            MoveObjects(movePtsHalf.ToArray(), distance / 2, "Z");
        }


        private void NewMethod62(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, double distance)
        {
            MoveObjects(movePtsFull.ToArray(), distance, "Y");
            MoveObjects(movePtsHalf.ToArray(), distance / 2, "Y");
        }

        private void NewMethod60(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, double distance)
        {
            MoveObjects(movePtsFull.ToArray(), distance, "X");
            MoveObjects(movePtsHalf.ToArray(), distance / 2, "X");
        }

        private void NewMethod46(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, double distance)
        {
            MoveObjects(movePtsFull.ToArray(), distance, "Y");
            MoveObjects(movePtsHalf.ToArray(), distance / 2, "Y");
        }

        private void NewMethod48(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, double distance)
        {
            MoveObjects(movePtsFull.ToArray(), distance, "Z");
            MoveObjects(movePtsHalf.ToArray(), distance / 2, "Z");
        }


        private void NewMethod50(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, double distance)
        {
            MoveObjects(movePtsFull.ToArray(), distance, "Z");
            MoveObjects(movePtsHalf.ToArray(), distance / 2, "Z");
        }



        private void NewMethod64(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, double distance)
        {
            MoveObjects(movePtsFull.ToArray(), distance, "Z");
            MoveObjects(movePtsHalf.ToArray(), distance / 2, "Z");
        }


        private void NewMethod44(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, double distance)
        {
            MoveObjects(movePtsFull.ToArray(), distance, "X");
            MoveObjects(movePtsHalf.ToArray(), distance / 2, "X");
        }



        private void NewMethod42(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, double distance)
        {
            MoveObjects(movePtsFull.ToArray(), distance, "X");
            MoveObjects(movePtsHalf.ToArray(), distance / 2, "X");
        }




        private void NewMethod58(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, double distance)
        {
            MoveObjects(movePtsFull.ToArray(), distance, "X");
            MoveObjects(movePtsHalf.ToArray(), distance / 2, "X");
        }




        private void NewMethod40(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, double distance)
        {
            MoveObjects(movePtsFull.ToArray(), distance, "Z");
            MoveObjects(movePtsHalf.ToArray(), distance / 2, "Z");
        }




        private void NewMethod37(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, double distance)
        {
            MoveObjects(movePtsFull.ToArray(), distance, "Z");
            MoveObjects(movePtsHalf.ToArray(), distance / 2, "Z");
        }


        private void NewMethod34(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, double distance)
        {
            MoveObjects(movePtsFull.ToArray(), distance, "Y");
            MoveObjects(movePtsHalf.ToArray(), distance / 2, "Y");
        }



        private void NewMethod29(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, double distance)
        {
            MoveObjects(movePtsFull.ToArray(), distance, "Y");
            MoveObjects(movePtsHalf.ToArray(), distance / 2, "Y");
        }

        private void NewMethod27(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, double distance)
        {
            MoveObjects(movePtsFull.ToArray(), distance, "X");
            MoveObjects(movePtsHalf.ToArray(), distance / 2, "X");
        }



        private void NewMethod24(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, double distance)
        {
            MoveObjects(movePtsFull.ToArray(), distance, "X");
            MoveObjects(movePtsHalf.ToArray(), distance / 2, "X");
        }



        private void NewMethod15(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, double zDistance)
        {
            MoveObjects(movePtsFull.ToArray(), zDistance, "Z");
            MoveObjects(movePtsHalf.ToArray(), zDistance / 2, "Z");

            ShowTemporarySizeText();
        }



        private void NewMethod14(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, double zDistance)
        {
            MoveObjects(movePtsFull.ToArray(), zDistance, "Z");
            MoveObjects(movePtsHalf.ToArray(), zDistance / 2, "Z");

            ShowTemporarySizeText();
        }



        private void NewMethod11(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, double yDistance)
        {
            MoveObjects(movePtsFull.ToArray(), yDistance, "Y");
            MoveObjects(movePtsHalf.ToArray(), yDistance / 2, "Y");

            ShowTemporarySizeText();
        }



        private void NewMethod18(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, double yDistance)
        {
            MoveObjects(movePtsFull.ToArray(), yDistance, "Y");
            MoveObjects(movePtsHalf.ToArray(), yDistance / 2, "Y");

            ShowTemporarySizeText();
        }



        private void NewMethod73(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, double distance)
        {
            MoveObjects(movePtsFull.ToArray(), distance, "Y");
            MoveObjects(movePtsHalf.ToArray(), distance / 2, "Y");
        }



        private void NewMethod71(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, double distance)
        {
            MoveObjects(movePtsFull.ToArray(), distance, "Y");
            MoveObjects(movePtsHalf.ToArray(), distance / 2, "Y");
        }


        private void NewMethod69(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, double distance)
        {
            MoveObjects(movePtsFull.ToArray(), distance, "X");
            MoveObjects(movePtsHalf.ToArray(), distance / 2, "X");
        }


        private void NewMethod67(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, double distance)
        {
            MoveObjects(movePtsFull.ToArray(), distance, "X");
            MoveObjects(movePtsHalf.ToArray(), distance / 2, "X");
        }



        private void NewMethod65(List<NXObject> movePtsHalf, List<NXObject> movePtsFull, double distance)
        {
            MoveObjects(movePtsFull.ToArray(), distance, "Z");
            MoveObjects(movePtsHalf.ToArray(), distance / 2, "Z");
        }




    }
}
