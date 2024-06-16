using System.Collections.Generic;
using NXOpen;
using NXOpen.Features;
using NXOpen.GeometricUtilities;
using NXOpen.UF;
using System;
using NXOpen.UserDefinedObjects;
using static TSG_Library.Extensions.Extensions;
using static NXOpen.UF.UFConstants;
using NXOpenUI;
using System.Globalization;
using NXOpen.Assemblies;
using System.Windows.Forms;
using TSG_Library.Utilities;
using NXOpen.Utilities;
using NXOpen.Preferences;
using TSG_Library.Properties;

namespace TSG_Library.UFuncs
{
    public partial class EditBlockForm
    {
  private void Exit()
  {
      session_.Parts.RemoveWorkPartChangedHandler(_idWorkPartChanged1);
      Close();
      Settings.Default.udoComponentBuilderWindowLocation = Location;
      Settings.Default.Save();

      using (this)
          new ComponentBuilder().Show();
  }

    }
}
// 2045