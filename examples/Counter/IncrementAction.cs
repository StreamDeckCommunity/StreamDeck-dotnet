namespace Counter
{
    /// <summary>
    /// Provides manifest information about the increment action.
    /// </summary>
    [PluginAction(
        name: "Counter",
        uuid: "com.streamdeckcommunity.counter.increment",
        icon: "Images/Action",
        SupportedInMultiActions = false,
        Tooltip = "Increment the count by one.")]
    [PluginActionState(
        image: "Images/Key",
        TitleAlignment = TitleAlignment.Middle,
        FontSize = "18")]
    public class IncrementAction
    {
    }
}
