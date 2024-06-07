namespace TSG_Library.Enum
{
    /// <summary>The possible states of a layer</summary>
    /// <remarks>
    ///     <para>
    ///         The actual states of the layers in the Work Part are accessible via the
    ///         <see cref="P:TSG_Library.LayerStates">TSG_Library.LayerStates</see> array.
    ///         Specifically, the array entry TSG_Library.LayerStates[n] gives the status
    ///         of layer number n.
    ///     </para>
    /// </remarks>
    /// <seealso cref="P:TSG_Library.LayerStates">TSG_Library.LayerStates</seealso>
    public enum LayerState
    {
        /// <summary>Work layer. The layer on which all newly created objects are placed.</summary>
        WorkLayer,

        /// <summary>Objects on the layer are selectable</summary>
        Selectable,

        /// <summary>Objects on the layer are visible, but not selectable</summary>
        Visible,

        /// <summary>Objects on the layer are not visible and not selectable</summary>
        Hidden
    }
}