using System.Windows.Forms;
using NXOpen;
using NXOpen.UF;
using TSG_Library.Attributes;
using TSG_Library.Utilities;
using static TSG_Library.Extensions.__Extensions_;

namespace TSG_Library.UFuncs
{
    [UFunc("data-base-image-exporter")]
    [RevisionEntry("1.0", "2017", "06", "05")]
    [Revision("1.0.1", "Revision Log Created for NX 11.")]
    [RevisionEntry("1.1", "2017", "08", "22")]
    [Revision("1.1.1", "Signed so it will run outside of CTS.")]
    [RevisionEntry("1.2", "2019", "08", "28")]
    [Revision("1.2.1", "GFolder updated to allow old job number under non cts folder.")]
    [RevisionEntry("11.1", "2023", "01", "09")]
    [Revision("11.1.1", "Removed validation")]
    public class DataBaseImageExporter : _UFunc
    {
        public override void execute()
        {
            if(Session.GetSession().Parts.Display is null)
            {
                print_("There is no displayed part loaded");

                return;
            }

            var folder = GFolder.create_or_null(__display_part_);

            if(folder is null)
            {
                print_("The current displayed part does not reside in a GFolder.");

                return;
            }

            session_.SetUndoMark(Session.MarkVisibility.Visible, "");

            var saveFileDialog1 = new SaveFileDialog { Filter = "JPEG (.jpg)|*.jpg" };

            if(!__display_part_.FullPath.Contains("Q:\\"))
            {
                saveFileDialog1.InitialDirectory = "G:\\CTS\\job-info\\pictures\\";

                saveFileDialog1.FileName = folder.customer_number;
            }
            else
            {
                saveFileDialog1.InitialDirectory = folder.dir_job;
            }

            var result = saveFileDialog1.ShowDialog();

            if(result != DialogResult.OK)
                return;

#pragma warning disable CS0618 // Type or member is obsolete
            TheUFSession.Disp.CreateFramedImage(
                saveFileDialog1.FileName,
                UFDisp.ImageFormat.Jpeg,
                UFDisp.BackgroundColor.White,
                new[] { 0, 0 },
                314,
                235);
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }
}