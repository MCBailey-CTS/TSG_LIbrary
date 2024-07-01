
namespace TSG_Library.UFuncs.DrainHoleCreator
{
    public interface IDrainHoleModel
    {
        IDrainHoleCreator DrainHoleCreator { get; set; }

        IDrainHoleSettings DrainHoleSettings { get; set; }
    }
}