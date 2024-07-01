namespace TSG_Library.UFuncs.DrainHoleCreator
{
    public class DrainHoleModel : IDrainHoleModel
    {
        public DrainHoleModel(IDrainHoleCreator drainHoleCreator, IDrainHoleSettings drainHoleSettings)
        {
            DrainHoleCreator = drainHoleCreator;
            DrainHoleSettings = drainHoleSettings;
        }


        // ReSharper disable once NotNullMemberIsNotInitialized
        public DrainHoleModel()
        {
        }

        public IDrainHoleCreator DrainHoleCreator { get; set; }

        public IDrainHoleSettings DrainHoleSettings { get; set; }
    }
}